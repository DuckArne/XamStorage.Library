using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

namespace XamStorage.UWP
{
    /// <summary>
    /// Represents a folder in the <see cref="UWPFileSystemFolder"/>
    /// </summary>
    [DebuggerDisplay("Path = {_path}")]
    public class UWPFileSystemFolder : IFolder
    {
        readonly string _name;
        readonly string _path;
        readonly bool _canDelete;
        internal StorageFolder Folder { get; set; }

        /// <summary>
        /// Creates a new <see cref="UWPFileSystemFolder" /> corresponding to a specified path
        /// </summary>
        /// <param name="path">The folder path</param>
        /// <param name="canDelete">Specifies whether the folder can be deleted (via <see cref="DeleteAsync"/>)</param>
        /// <param name="folder">StorageFolder to work from</param>
        public UWPFileSystemFolder(string path, bool canDelete, StorageFolder folder)
        {
            _name = System.IO.Path.GetFileName(path);
            _path = path;
            _canDelete = canDelete;
            Folder = folder;

            if (Folder == null)
            {
                throw new ArgumentNullException("folder", "StorageFolder param can't be null when initializing new IFolder (UWPFileSystemFolder)");
            }
        }

        /// <summary>
        /// Creates a new <see cref="UWPFileSystemFolder" /> corresponding to a specified path
        /// </summary>
        /// <param name="path">The folder path</param>
        /// <param name="folder">StorageFolder to work from</param>
        /// <remarks>A folder created with this constructor cannot be deleted via <see cref="DeleteAsync"/></remarks>
        public UWPFileSystemFolder(string path, StorageFolder folder)
            : this(path, false, folder)
        {
        }

        /// <summary>
        /// The name of the folder
        /// </summary>
        public string Name {
            get { return _name; }
        }

        /// <summary>
        /// The "full path" of the folder, which should uniquely identify it within a given <see cref="IFileSystem"/>
        /// </summary>
        public string Path {
            get { return _path; }
        }

        /// <summary>
        /// Creates a file in this folder
        /// </summary>
        /// <param name="desiredName">The name of the file to create</param>
        /// <param name="option">Specifies how to behave if the specified file already exists</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The newly created file</returns>
        public async Task<IFile> CreateFileAsync(string desiredName, CreationCollisionOption option, CancellationToken cancellationToken)
        {
            StorageFile newFile = null;
            Requires.NotNullOrEmpty(desiredName, "desiredName");
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            string nameToUse = desiredName;

            if (await Folder.TryGetItemAsync(nameToUse) != null)
            {
                if (option == CreationCollisionOption.GenerateUniqueName)
                {
                    string desiredRoot = System.IO.Path.GetFileNameWithoutExtension(desiredName);
                    string desiredExtension = System.IO.Path.GetExtension(desiredName);
                    for (int num = 2; await Folder.TryGetItemAsync(nameToUse) != null; num++)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        nameToUse = desiredRoot + "(" + num + ")" + desiredExtension;
                    }
                    newFile = await Folder.CreateFileAsync(nameToUse);
                }

                else if (option == CreationCollisionOption.ReplaceExisting)
                {
                    var oldFile = await Folder.GetFileAsync(nameToUse);
                    await oldFile.DeleteAsync();
                    newFile = await Folder.CreateFileAsync(nameToUse);
                }

                else if (option == CreationCollisionOption.FailIfExists)
                {
                    throw new IOException("File already exists: " + System.IO.Path.Combine(Path, nameToUse));
                }

                else if (option == CreationCollisionOption.OpenIfExists)
                {
                    newFile = await Folder.GetFileAsync(desiredName);
                }

                else
                {
                    throw new ArgumentException("Unrecognized CreationCollisionOption: " + option);
                }
            }

            else
            {
                newFile = await Folder.CreateFileAsync(nameToUse);
            }

            var ret = new UWPFileSystemFile(newFile.Path, newFile);
            return ret;
        }


        /// <summary>
        /// Gets a file in this folder
        /// </summary>
        /// <param name="name">The name of the file to get</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The requested file, or null if it does not exist</returns>
        public async Task<IFile> GetFileAsync(string name, CancellationToken cancellationToken)
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            string path = System.IO.Path.Combine(Path, name);
            if (Folder.TryGetItemAsync(name) == null)
            {
                throw new Exceptions.FileNotFoundException("File does not exist: " + path);
            }
            var ret = new UWPFileSystemFile(path, await Folder.GetFileAsync(name));
            return ret;
        }

        /// <summary>
        /// Gets a list of the files in this folder
        /// </summary>
        /// <returns>A list of the files in the folder</returns>
        public async Task<IList<IFile>> GetFilesAsync(CancellationToken cancellationToken)
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            var files = await Folder.GetFilesAsync();
            IList<IFile> iFiles = new List<IFile>();
            foreach (var sf in files)
            {
                iFiles.Add(new UWPFileSystemFile(sf.Path, sf));
            }
            return iFiles;
        }

        /// <summary>
        /// Creates a subfolder in this folder
        /// </summary>
        /// <param name="desiredName">The name of the folder to create</param>
        /// <param name="option">Specifies how to behave if the specified folder already exists</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The newly created folder</returns>
        public async Task<IFolder> CreateFolderAsync(string desiredName, CreationCollisionOption option, CancellationToken cancellationToken)
        {
            StorageFolder newFolder = null;
            Requires.NotNullOrEmpty(desiredName, "desiredName");
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            string nameToUse = desiredName;
            string newPath = System.IO.Path.Combine(Path, nameToUse);
            if (await Folder.TryGetItemAsync(nameToUse) != null)
            {
                if (option == CreationCollisionOption.GenerateUniqueName)
                {
                    for (int num = 2; await Folder.TryGetItemAsync(nameToUse) != null; num++)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        nameToUse = desiredName + "(" + num + ")";
                        newPath = System.IO.Path.Combine(Path, nameToUse);
                    }
                    newFolder = await Folder.CreateFolderAsync(nameToUse);
                }
                else if (option == CreationCollisionOption.ReplaceExisting)
                {
                    var existingFolder = await Folder.GetFolderAsync(nameToUse);
                    await existingFolder.DeleteAsync();
                    newFolder = await Folder.CreateFolderAsync(nameToUse);
                }
                else if (option == CreationCollisionOption.FailIfExists)
                {
                    throw new IOException("Directory already exists: " + newPath);
                }
                else if (option == CreationCollisionOption.OpenIfExists)
                {
                    newFolder = await Folder.GetFolderAsync(nameToUse);
                }
                else
                {
                    throw new ArgumentException("Unrecognized CreationCollisionOption: " + option);
                }
            }
            else
            {
                newFolder = await Folder.CreateFolderAsync(nameToUse);
            }

            var ret = new UWPFileSystemFolder(newFolder.Path, true, newFolder);
            return ret;
        }


        /// <summary>
        /// Gets a subfolder in this folder
        /// </summary>
        /// <param name="name">The name of the folder to get</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The requested folder, or null if it does not exist</returns>
        public async Task<IFolder> GetFolderAsync(string name, CancellationToken cancellationToken)
        {
            Requires.NotNullOrEmpty(name, "name");

            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);
            string path = System.IO.Path.Combine(Path, name);
            if (await Folder.TryGetItemAsync(name) == null)
            {
                throw new Exceptions.DirectoryNotFoundException("Directory does not exist: " + path);
            }
            var ret = new UWPFileSystemFolder(path, true, await Folder.GetFolderAsync(name));
            return ret;
        }

        /// <summary>
        /// Gets a list of subfolders in this folder
        /// </summary>
        /// <returns>A list of subfolders in the folder</returns>
        public async Task<IList<IFolder>> GetFoldersAsync(CancellationToken cancellationToken)
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);
            var folders = await Folder.GetFoldersAsync();

            IList<IFolder> ret = new List<IFolder>();
            foreach (var sf in folders)
            {
                ret.Add(new UWPFileSystemFolder(sf.Path, sf));
            }
            return ret;
        }

        /// <summary>
        /// Checks whether a folder or file exists at the given location.
        /// </summary>
        /// <param name="name">The name of the file or folder to check for.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task whose result is the result of the existence check.
        /// </returns>
        public async Task<ExistenceCheckResult> CheckExistsAsync(string name, CancellationToken cancellationToken)
        {
            Requires.NotNullOrEmpty(name, "name");
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);
            string checkPath = PortablePath.Combine(this.Path, name);
            var result = await Folder.TryGetItemAsync(name);
            if (result == null)
            {
                return ExistenceCheckResult.NotFound;
            }
            else if (result.IsOfType(StorageItemTypes.Folder))
            {
                return ExistenceCheckResult.FolderExists;
            }
            else if (result.IsOfType(StorageItemTypes.File))
            {
                return ExistenceCheckResult.FileExists;
            }
            else if (result.IsOfType(StorageItemTypes.None))
            {
                return ExistenceCheckResult.None;
            }

            throw new Exception("Fatal Error Not");

        }

        /// <summary>
        /// Deletes this folder and all of its contents
        /// </summary>
        /// <returns>A task which will complete after the folder is deleted</returns>
        public async Task DeleteAsync(CancellationToken cancellationToken)
        {
            if (!_canDelete)
            {
                throw new IOException("Cannot delete root storage folder.");
            }
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            await Folder.DeleteAsync();
        }

    }
}