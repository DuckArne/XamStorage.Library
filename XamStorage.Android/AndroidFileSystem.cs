using Android.Content;
using Android.Database;
using Android.Provider;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AndroidEnvironment = Android.OS.Environment;

namespace XamStorage.Android
{
    /// <summary>
    /// AndroidFileSystem
    /// </summary>
    public class AndroidFileSystem: IFileSystem
    {
        /// <summary>
        /// A folder representing storage which is local to the current device
        /// </summary>
        public IFolder LocalStorage {
            get {             
                var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);             
                return new DroidFileSystemFolder(localAppData,Storage.Local);
            }
        }

        /// <summary>
        /// A folder representing storage which may be synced with other devices for the same user. For UWP only otherwise returns null
        /// </summary>
        public IFolder RoamingStorage {
            get {
                return null;
            }
        }

        /// <summary>
        /// A folder representing UWP = Applicationdata.LocalFolder,  iOS and Android SpecialFolder.Personal. If you embed files in project root you can get the path with this method.
        /// </summary>
        public IFolder PersonalStorage {
            get {
                var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                return new DroidFileSystemFolder(localAppData,Storage.Personal);
            }
        }

        /// <summary>
        /// A public folder representing storage which contains Documents.
        /// </summary>
        public Task<IFolder> DocumentsFolderAsync()
        {
            var folderPath = "";
            if (AndroidHelpers.IsExternalStorageWritable())
            {
                var f = AndroidEnvironment.GetExternalStoragePublicDirectory(AndroidEnvironment.DirectoryDocuments);
                folderPath = f.Path;
            }
            else
            {
                return null;
            }

            return Task.FromResult((IFolder)new DroidFileSystemFolder(folderPath, Storage.Documents));
        }

        /// <summary>
        /// A public folder representing storage which contains Music.
        /// </summary>
        public Task<IFolder> MusicFolderAsync()
        {
            var folderPath = "";
            if (AndroidHelpers.IsExternalStorageWritable())
            {
                var f = AndroidEnvironment.GetExternalStoragePublicDirectory(AndroidEnvironment.DirectoryMusic);
                folderPath = f.Path;
            }
            else
            {
                return null;
            }

            return Task.FromResult((IFolder)new DroidFileSystemFolder(folderPath,Storage.Music));
        }

        /// <summary>
        /// A public folder representing storage which contains Pictures.
        /// </summary>
        public Task<IFolder> PicturesFolderAsync()
        {
            var folderPath = "";
            if (AndroidHelpers.IsExternalStorageWritable())
            {
                var f = AndroidEnvironment.GetExternalStoragePublicDirectory(AndroidEnvironment.DirectoryPictures);
                folderPath = f.Path;
            }
            else
            {
                return null;
            }

            return Task.FromResult((IFolder)new DroidFileSystemFolder(folderPath,Storage.Pictures));
        }

        /// <summary>
        /// A public folder representing storage which contains Videos. In UWP this is the SharedLocalFolder
        /// </summary>
        public Task<IFolder> VideosFolderAsync()
        {
            var folderPath = "";
            if (AndroidHelpers.IsExternalStorageWritable())
            {
                var f = AndroidEnvironment.GetExternalStoragePublicDirectory(AndroidEnvironment.DirectoryMovies);
                folderPath = f.Path;
            }
            else
            {
                return null;
            }

            return Task.FromResult((IFolder)new DroidFileSystemFolder(folderPath,Storage.Videos));
        }
 
        /// <summary>
        /// Gets a file, given its path.  Returns null if the file does not exist.
        /// </summary>
        /// <param name="path">The path to a file, as returned from the <see cref="IFile.Path"/> property.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A file for the given path, or null if it does not exist.</returns>
        public async Task<IFile> GetFileFromPathAsync(string path, CancellationToken cancellationToken)
        {
            Requires.NotNullOrEmpty(path, "path");

            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            if (File.Exists(path))
            {
                return new DroidFileSystemFile(path);
            }

            return null;
        }

        /// <summary>
        /// Gets a folder, given its path.  Returns null if the folder does not exist.
        /// </summary>
        /// <param name="path">The path to a folder, as returned from the <see cref="IFolder.Path"/> property.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A folder for the specified path, or null if it does not exist.</returns>
        public async Task<IFolder> GetFolderFromPathAsync(string path, CancellationToken cancellationToken)
        {
            Requires.NotNullOrEmpty(path, "path");

            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);
            if (Directory.Exists(path))
            {
                return new DroidFileSystemFolder(path, true, Storage.Path);
            }

            return null;
        }       
       
    }
}