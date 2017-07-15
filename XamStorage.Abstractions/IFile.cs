using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace XamStorage
{
    /// <summary>
    /// Represents a file
    /// </summary>
    public interface IFile
    {
        /// <summary>
        /// The name of the file
        /// </summary>
        string Name { get; }
        /// <summary>
        /// The "full path" of the file, which should uniquely identify it within a given <see cref="IFileSystem"/>
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Opens the file
        /// </summary>
        /// <param name="fileAccess">Specifies whether the file should be opened in read-only or read/write mode</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Stream"/> which can be used to read from or write to the file</returns>
        Task<Stream> OpenAsync(FileAccess fileAccess, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deletes the file
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task which will complete after the file is deleted.
        /// </returns>
        Task DeleteAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Renames a file without changing its location.
        /// </summary>
        /// <param name="newName">The new leaf name of the file.</param>
        /// <param name="collisionOption">How to deal with collisions with existing files.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task which will complete after the file is renamed.
        /// </returns>
        Task RenameAsync(string newName, NameCollisionOption collisionOption = NameCollisionOption.FailIfExists, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Moves a file.
        /// </summary>
        /// <param name="newPath">The new full path of the file.</param>
        /// <param name="collisionOption">How to deal with collisions with existing files.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task which will complete after the file is moved.</returns>
        Task MoveAsync(string newPath, NameCollisionOption collisionOption = NameCollisionOption.ReplaceExisting, CancellationToken cancellationToken = default(CancellationToken));


        /// <summary>
        /// Writes data to a binary file.
        /// </summary>
        /// <param name="buffer">The buffer to write data from.</param>
        /// <param name="offset">The zero-based byte offset in buffer from which to begin copying bytes to the stream.</param>
        /// <param name="count">The maximum number of bytes to write.</param>
        /// <returns></returns>
        Task WriteAsync(byte[] buffer,int offset, int count);
      

    }
}
