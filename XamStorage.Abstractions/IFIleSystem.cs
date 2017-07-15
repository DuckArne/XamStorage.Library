using System.Threading;
using System.Threading.Tasks;

namespace XamStorage
{
    /// <summary>
    /// Represents a file system.
    /// </summary>
    public interface IFileSystem
    {
        /// <summary>
        /// A folder representing storage which is local to the current device
        /// </summary>
        IFolder LocalStorage { get; }

        /// <summary>
        /// A folder representing storage which may be synced with other devices for the same user. UWP only.
        /// </summary>
        IFolder RoamingStorage { get; }

        /// <summary>
        /// A public folder representing storage which contains Documents 
        /// </summary>
        Task<IFolder> DocumentsFolderAsync(); 
        
        /// <summary>
        /// A public folder representing storage which contains Music
        /// </summary>
        Task<IFolder> MusicFolderAsync();

        /// <summary>
        /// A public folder representing storage which contains Pictures
        /// </summary>
        Task<IFolder> PicturesFolderAsync();
      
        /// <summary>
        /// A public folder representing storage which contains Videos
        /// </summary>
        Task<IFolder> VideosFolderAsync();

        /// <summary>
        /// Gets a file, given its path.  Returns null if the file does not exist.
        /// </summary>
        /// <param name="path">The path to a file, as returned from the <see cref="IFile.Path"/> property.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A file for the given path, or null if it does not exist.</returns>
        Task<IFile> GetFileFromPathAsync(string path, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a folder, given its path.  Returns null if the folder does not exist.
        /// </summary>
        /// <param name="path">The path to a folder, as returned from the <see cref="IFolder.Path"/> property.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A folder for the specified path, or null if it does not exist.</returns>
        Task<IFolder> GetFolderFromPathAsync(string path, CancellationToken cancellationToken = default(CancellationToken));
    }
}
