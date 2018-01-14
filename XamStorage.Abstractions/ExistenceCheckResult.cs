namespace XamStorage
{
    /// <summary>
    /// Describes the result of a file or folder existence check.
    /// </summary>
    public enum ExistenceCheckResult
    {
        /// <summary>
        /// No file system entity was found at the given path.
        /// </summary>
        NotFound,

        /// <summary>
        /// A file was found at the given path.
        /// </summary>
        FileExists,

        /// <summary>
        /// A folder was found at the given path.
        /// </summary>
        FolderExists,
        /// <summary>
        /// Some other type? UWP special.
        /// </summary>
        None
    }
}