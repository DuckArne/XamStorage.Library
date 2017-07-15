using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
#if UWP
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
#endif
namespace XamStorage.UWP

{
    /// <summary>
    /// UWPFileSystem
    /// </summary>
    public class UWPFileSystem:IFileSystem
    {
        /// <summary>
        /// A folder representing storage which is local to the current device
        /// </summary>
        public IFolder LocalStorage {
            get {           
                var applicationData = ApplicationData.Current;
                var localAppData = applicationData.LocalFolder.Path;
                return new FileSystemFolder(localAppData);
            }
        }

        /// <summary>
        /// A folder representing storage which may be synced with other devices for the same user For UWP and Forms Otherwise returns null
        /// </summary>
        public IFolder RoamingStorage {
            get {

                var applicationData = ApplicationData.Current;
                var roamingAppData = applicationData.RoamingFolder.Path;
                return new FileSystemFolder(roamingAppData);      
            }
        }

        /// <summary>
        /// A public folder representing storage which contains Music.
        /// </summary>
        async public Task<IFolder> MusicFolderAsync()
        {
            var musicLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Music);
            return new FileSystemFolder(musicLibrary.SaveFolder.Path);
        }

        /// <summary>
        /// A public folder representing storage which contains Pictures.
        /// </summary>
        async public Task<IFolder> PicturesFolderAsync()
        {
            var picturesLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Pictures);
            return new FileSystemFolder(picturesLibrary.SaveFolder.Path);
        }

        /// <summary>
        /// A public folder representing storage which contains Videos.
        /// </summary>
        async public Task<IFolder> VideosFolderAsync()
        {
            var videosLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Videos);
            return new FileSystemFolder(videosLibrary.SaveFolder.Path);
        }

        /// <summary>
        /// A public folder representing storage which contains Videos.
        /// </summary>
        async public Task<IFolder> DocumentsFolderAsync()
        {
            var documentsLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Documents);
            return new FileSystemFolder(documentsLibrary.SaveFolder.Path);
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
                return new FileSystemFile(path);
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
                return new FileSystemFolder(path, true);
            }

            return null;
        }

       

       

      
    }
}
