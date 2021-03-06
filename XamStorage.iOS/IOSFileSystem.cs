﻿using System.Threading;
using System.Threading.Tasks;
using System;
using System.IO;
using UIKit;
using Foundation;

namespace XamStorage.iOS
{
    public class IOSFileSystem : IFileSystem
    {
        /// <summary>
        /// A folder representing storage which is local to the current device. User can not see these files on iOS and Android.
        /// </summary>
        public IFolder LocalStorage {
            get {          
                var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var localAppData = Path.Combine(documents, "..", "Library");
                return new IOSFileSystemFolder(localAppData,Storage.Local);
            }
        }

        /// <summary>
        /// A folder representing storage which may be synced with other devices for the same user. For UWP only Otherwise returns null
        /// </summary>
        public IFolder RoamingStorage {
            get {
                return null;
            }
        }

        /// <summary>
        /// A folder representing UWP = Applicationdata.LocalFolder, Android SpecialFolder.Personal, iOS DocumentsFolder.  If you enable  UIFileSharingEnabled in info.plist on ios app you can share through iTunes.
        /// </summary>
        public IFolder PersonalStorage {
            get {
                var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);              
                return new IOSFileSystemFolder(localAppData,Storage.Personal);
            }
        }

        /// <summary>
        /// A public folder representing storage which contains Documents. If you enable  UIFileSharingEnabled in info.plist on ios app you can share through iTunes.
        /// </summary>
        public Task<IFolder> DocumentsFolderAsync()
        {
            var folderPath = IosFolderPathDocuments(NSSearchPathDirectory.DocumentDirectory, Environment.SpecialFolder.MyDocuments);
            return Task.FromResult((IFolder)new IOSFileSystemFolder(folderPath, Storage.Documents));
        }

       

        /// <summary>
        /// A public folder representing storage which contains Music. On iOS this Folder is  the Documents directory.
        /// </summary>
        public Task<IFolder> MusicFolderAsync()
        {
            var folderPath = IosFolderPathDocuments(NSSearchPathDirectory.DocumentDirectory, Environment.SpecialFolder.MyDocuments);
            return Task.FromResult((IFolder)new IOSFileSystemFolder(folderPath, Storage.Music));
        }

        /// <summary>
        /// A public folder representing storage which contains Pictures. On iOS this Folder is  the Documents directory.
        /// </summary>
        public Task<IFolder> PicturesFolderAsync()
        {
            var folderPath = IosFolderPathDocuments(NSSearchPathDirectory.DocumentDirectory, Environment.SpecialFolder.MyDocuments);
            return Task.FromResult((IFolder)new IOSFileSystemFolder(folderPath,Storage.Pictures));
        }

        /// <summary>
        /// A public folder representing storage which contains Videos. On iOS this Folder is  the Documents directory.
        /// </summary>
        public Task<IFolder> VideosFolderAsync()
        {
            var folderPath = IosFolderPathDocuments(NSSearchPathDirectory.DocumentDirectory, Environment.SpecialFolder.MyDocuments);
            return Task.FromResult((IFolder)new IOSFileSystemFolder(folderPath, Storage.Videos));
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
                return new IOSFileSystemFile(path);
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
                return new IOSFileSystemFolder(path, true,Storage.Path);
            }
            
            return null;
        }


        private static string IosFolderPathDocuments(NSSearchPathDirectory nsSearchpath, Environment.SpecialFolder path)
        {
            string folderPath;
            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                folderPath = NSFileManager.DefaultManager.GetUrls(nsSearchpath, NSSearchPathDomain.User)[0].Path;
            }
            else
            {
                folderPath = Environment.GetFolderPath(path);
            }

            return folderPath;
        }



    }
}
