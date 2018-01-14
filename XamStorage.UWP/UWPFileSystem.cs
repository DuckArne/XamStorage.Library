using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

namespace XamStorage.UWP

{
    /// <summary>
    /// UWPFileSystem
    /// </summary>
    public class UWPFileSystem : IFileSystem
    {
        /// <summary>
        /// A folder representing storage which is local to the current device
        /// </summary>
        public IFolder LocalStorage {
            get {
                var applicationData = ApplicationData.Current;
                var localAppData = applicationData.LocalFolder.Path;
                return new UWPFileSystemFolder(localAppData, applicationData.LocalFolder);
            }
        }

        /// <summary>
        /// A folder representing storage which may be synced with other devices for the same user For UWP and Forms Otherwise returns null
        /// </summary>
        public IFolder RoamingStorage {
            get {

                var applicationData = ApplicationData.Current;
                var roamingAppData = applicationData.RoamingFolder.Path;
                return new UWPFileSystemFolder(roamingAppData, applicationData.RoamingFolder);
            }
        }

        /// <summary>
        /// A folder representing UWP = Applicationdata.LocalFolder,  iOS and Android SpecialFolder.Personal. If you embed files in project root you can get the path with this method.
        /// </summary>
        public IFolder PersonalStorage {
            get {
                return LocalStorage;
            }
        }

        /// <summary>
        /// A public folder representing storage which contains Music.
        /// </summary>
        async public Task<IFolder> MusicFolderAsync()
        {
            var musicLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Music);
            return new UWPFileSystemFolder(musicLibrary.SaveFolder.Path, musicLibrary.SaveFolder);
        }

        /// <summary>
        /// A public folder representing storage which contains Pictures.
        /// </summary>
        async public Task<IFolder> PicturesFolderAsync()
        {
            var picturesLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Pictures);
            return new UWPFileSystemFolder(picturesLibrary.SaveFolder.Path, picturesLibrary.SaveFolder);
        }

        /// <summary>
        /// A public folder representing storage which contains Videos.
        /// </summary>
        async public Task<IFolder> VideosFolderAsync()
        {
            var videosLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Videos);
            return new UWPFileSystemFolder(videosLibrary.SaveFolder.Path, videosLibrary.SaveFolder);
        }

        /// <summary>
        /// A public folder representing storage which contains Videos.
        /// </summary>
        async public Task<IFolder> DocumentsFolderAsync()
        {
            var documentsLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Documents);
            return new UWPFileSystemFolder(documentsLibrary.SaveFolder.Path, documentsLibrary.SaveFolder);
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

            StorageFile file = null;
            try
            {
                file = await StorageFile.GetFileFromPathAsync(path);
            }
            catch (UnauthorizedAccessException)
            {

                Trace.WriteLine("UnauthorizedAccessException Was captured by XamStorage.FileSystem GetFileFromPathAsync");
                return null;
            }
            catch (FileNotFoundException)
            {
                return null;
            }
            catch (ArgumentException)
            {
                Trace.WriteLine("ArgumentException Was captured by XamStorage.FileSystem GetFileFromPathAsync, The path cannot be a relative path or a Uri. Check the value of path.");
                return null;
            }

            return new UWPFileSystemFile(file.Path, file);

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

            return new UWPFileSystemFolder(path, await StorageFolder.GetFolderFromPathAsync(path));

        }

        ///// <summary>
        ///// Gets a file, given its Uri.  Returns null if the file does not exist.
        ///// </summary>
        ///// <param name="uri">The uri </param>
        ///// <returns>A file for the given uri, or null if it does not exist.</returns>
        //async public Task<IFile> GetFileFromUri(Uri uri)
        //{
        //    var file = await StorageFile.GetFileFromApplicationUriAsync(uri);

        //    return new FileSystemFile(file?.Path);
        //}
    }
}
