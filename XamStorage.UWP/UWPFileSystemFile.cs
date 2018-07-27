using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

namespace XamStorage.UWP
{
    /// <summary>
    /// Represents a file in the <see cref="UWPFileSystem"/>
    /// </summary>
    [DebuggerDisplay("Name = {_name}")]
    public class UWPFileSystemFile : IFile
    {
        string _name;
        string _path;

        public StorageFile File { get; private set; }

        /// <summary>
        /// Creates a new <see cref="UWPFileSystemFile"/> corresponding to the specified path
        /// </summary>
        /// <param name="path">The file path</param>
        /// <param name="file"> StorageFile to work from</param>
        public UWPFileSystemFile(string path, StorageFile file)
        {
            _name = System.IO.Path.GetFileName(path);
            _path = path;
            File = file;
            if (File == null)
            {
                throw new ArgumentNullException("file", "Must set StorageFile on initialization");
            }
        }

        /// <summary>
        /// The name of the file
        /// </summary>
        public string Name {
            get { return _name; }
        }

        /// <summary>
        /// The "full path" of the file, which should uniquely identify it within a given <see cref="IFileSystem"/>
        /// </summary>
        public string Path {
            get { return _path; }
        }

        /// <summary>
        /// Opens the file
        /// </summary>
        /// <param name="fileAccess">Specifies whether the file should be opened in read-only or read/write mode</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Stream"/> which can be used to read from or write to the file</returns>
        public async Task<Stream> OpenAsync(FileAccess fileAccess, CancellationToken cancellationToken)
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            if (fileAccess == FileAccess.Read)
            {
                return await File.OpenStreamForReadAsync();
            }
            else if (fileAccess == FileAccess.ReadAndWrite)
            {
                return await File.OpenStreamForWriteAsync();
            }
            else
            {
                throw new ArgumentException("Unrecognized FileAccess value: " + fileAccess);
            }
        }

        /// <summary>
        /// Deletes the file
        /// </summary>
        /// <returns>A task which will complete after the file is deleted.</returns>
        public async Task DeleteAsync(CancellationToken cancellationToken)
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            await File.DeleteAsync();
        }

        /// <summary>
        /// Renames a file without changing its location.
        /// </summary>
        /// <param name="newName">The new leaf name of the file.</param>
        /// <param name="collisionOption">How to deal with collisions with existing files.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task which will complete after the file is renamed.
        /// </returns>
        public async Task RenameAsync(string newName, NameCollisionOption collisionOption, CancellationToken cancellationToken)
        {
            Requires.NotNullOrEmpty(newName, "newName");

            switch (collisionOption)
            {
                case NameCollisionOption.ReplaceExisting:
                    await File.RenameAsync(newName, Windows.Storage.NameCollisionOption.ReplaceExisting);
                    break;
                case NameCollisionOption.GenerateUniqueName:
                    await File.RenameAsync(newName, Windows.Storage.NameCollisionOption.GenerateUniqueName);
                    break;
                case NameCollisionOption.FailIfExists:
                    await File.RenameAsync(newName, Windows.Storage.NameCollisionOption.FailIfExists);
                    break;
            }

        }

        /// <summary>
        /// Moves a file.
        /// </summary>
        /// <param name="newPath">Not in use</param>
        /// <param name="toFolder">Folder to move to</param>
        /// <param name="collisionOption">How to deal with collisions with existing files.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task which will complete after the file is moved.
        /// </returns>
        public async Task MoveAsync(string newPath, IFolder toFolder, NameCollisionOption collisionOption, CancellationToken cancellationToken)
        {
            var f = toFolder as UWPFileSystemFolder;

            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);


            if (await f.Folder.TryGetItemAsync(File.Name) != null)
            {
                switch (collisionOption)
                {
                    case NameCollisionOption.FailIfExists:
                        throw new IOException("File already exists at path: " + Path);
                    case NameCollisionOption.GenerateUniqueName:
                        await File.MoveAsync(f.Folder, Name, Windows.Storage.NameCollisionOption.GenerateUniqueName);
                        break;
                    case NameCollisionOption.ReplaceExisting:
                        await File.MoveAsync(f.Folder, Name, Windows.Storage.NameCollisionOption.ReplaceExisting);
                        break;
                }
            }


            _path = File.Path;
            _name = File.Name;
            return;
        }
        /// <summary>
        /// Writes data to a binary file.
        /// </summary>
        /// <param name="buffer">The buffer to write data from.</param>
        /// <param name="offset">The zero-based byte offset in buffer from which to begin copying bytes to the stream.</param>
        /// <param name="count">The maximum number of bytes to write.</param>
        /// <returns></returns>
        async public Task WriteAsync(byte[] buffer, int offset, int count)
        {
            await FileIO.WriteBytesAsync(File, buffer);
        }

        /// <summary>
        /// Reads the contents of a file as a string
        /// </summary>
        /// <returns>The contents of the file</returns>
        async public Task<string> ReadAllTextAsync()
        {
            return await FileIO.ReadTextAsync(File);
        }

        /// <summary>
        /// Writes text to file, overwriting any existing data
        /// </summary>
        /// <param name="contents">The content to write to the file</param>
        /// <returns>A task which completes when the write operation finishes</returns>
        async public Task WriteAllTextAsync(string contents)
        {
            await FileIO.WriteTextAsync(File, contents);
        }

        /// <summary>
        ///  Reads the contents of the file into a byte array, and then closes the file.
        /// </summary>
        /// <returns>A byte array containing the contents of the file.</returns>
        async public Task<byte[]> ReadAllBytesAsync()
        {
            var buffer = await FileIO.ReadBufferAsync(File);
            return WindowsRuntimeBufferExtensions.ToArray(buffer);
        }
    }
}