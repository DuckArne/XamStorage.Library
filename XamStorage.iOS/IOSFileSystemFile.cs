using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace XamStorage.iOS
{
    /// <summary>
    /// Represents a file in the <see cref="IOSFileSystemFile"/>
    /// </summary>
    [DebuggerDisplay("Name = {_name}")]
    public class IOSFileSystemFile : IFile
    {
        string _name;
        string _path;

        /// <summary>
        /// Creates a new <see cref="IOSFileSystemFile"/> corresponding to the specified path
        /// </summary>
        /// <param name="path">The file path</param>
        public IOSFileSystemFile(string path)
        {
            _name = System.IO.Path.GetFileName(path);
            _path = path;
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
                return File.OpenRead(Path);
            }
            else if (fileAccess == FileAccess.ReadAndWrite)
            {
                return File.Open(Path, FileMode.Open, System.IO.FileAccess.ReadWrite);
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

            if (!File.Exists(Path))
            {
                throw new Exceptions.FileNotFoundException("File does not exist: " + Path);
            }

            File.Delete(Path);
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

            await MoveAsync(PortablePath.Combine(System.IO.Path.GetDirectoryName(_path), newName),null, collisionOption, cancellationToken);
        }

        /// <summary>
        /// Moves a file.
        /// </summary>
        /// <param name="newPath">The new full path of the file.</param>
        /// <param name="toFolder">Folder to move to</param>
        /// <param name="collisionOption">How to deal with collisions with existing files.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task which will complete after the file is moved.
        /// </returns>
        public async Task MoveAsync(string newPath,IFolder toFolder, NameCollisionOption collisionOption, CancellationToken cancellationToken)
        {
            Requires.NotNullOrEmpty(newPath, "newPath");

            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            string newDirectory = System.IO.Path.GetDirectoryName(newPath);
            string newName = System.IO.Path.GetFileName(newPath);

            for (int counter = 1; ; counter++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                string candidateName = newName;
                if (counter > 1)
                {
                    candidateName = String.Format(
                        CultureInfo.InvariantCulture,
                        "{0} ({1}){2}",
                        System.IO.Path.GetFileNameWithoutExtension(newName),
                        counter,
                        System.IO.Path.GetExtension(newName));
                }

                string candidatePath = PortablePath.Combine(newDirectory, candidateName);

                if (File.Exists(candidatePath))
                {
                    switch (collisionOption)
                    {
                        case NameCollisionOption.FailIfExists:
                            throw new IOException("File already exists.");
                        case NameCollisionOption.GenerateUniqueName:
                            continue; // try again with a new name.
                        case NameCollisionOption.ReplaceExisting:
                            File.Delete(candidatePath);
                            break;
                    }
                }

                File.Move(_path, candidatePath);
                _path = candidatePath;
                _name = candidateName;
                return;
            }
        }

        /// <summary>
        /// Writes data to a binary file.
        /// </summary>
        /// <param name="buffer">The buffer to write data from.</param>
        /// <param name="offset">The zero-based byte offset in buffer from which to begin copying bytes to the stream.</param>
        /// <param name="count">The maximum number of bytes to write.</param>
        /// <returns></returns>
        async public Task WriteAsync(byte[] buffer,int offset,int count)
        {
            using (var s = File.Open(Path, FileMode.Open, System.IO.FileAccess.ReadWrite))
            {
                await s.WriteAsync(buffer, offset, count);
            }

        }

        /// <summary>
        /// Reads the contents of a file as a string
        /// </summary>
        /// <returns>The contents of the file</returns>
        async public Task<string> ReadAllTextAsync()
        {
            using (var stream = await OpenAsync(FileAccess.Read, default(CancellationToken)).ConfigureAwait(false))
            {
                using (var sr = new StreamReader(stream))
                {
                    string text = await sr.ReadToEndAsync().ConfigureAwait(false);
                    return text;
                }
            }
        }

        /// <summary>
        /// Writes text to a file, overwriting any existing data
        /// </summary>
        /// <param name="contents">The content to write to the file</param>
        /// <returns>A task which completes when the write operation finishes</returns>
        async public Task WriteAllTextAsync(string contents)
        {
            using (var stream = await OpenAsync(FileAccess.ReadAndWrite, default(CancellationToken)).ConfigureAwait(false))
            {
                stream.SetLength(0);
                using (var sw = new StreamWriter(stream))
                {
                    await sw.WriteAsync(contents).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        ///  Reads the contents of the file into a byte array, and then closes the file.
        /// </summary>
        /// <returns>A byte array containing the contents of the file.</returns>
        async public Task<byte[]> ReadAllBytesAsync()
        {
            byte[] result = null;
            using (var stream = await OpenAsync(FileAccess.Read, default(CancellationToken)).ConfigureAwait(false))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    await stream.CopyToAsync(ms);
                    result = ms.ToArray();
                }
            }
            return result;
        }
    }
}