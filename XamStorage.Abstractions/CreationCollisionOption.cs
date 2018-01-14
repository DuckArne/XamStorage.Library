namespace XamStorage
{
    /// <summary>
    /// Specifies what should happen when trying to create a file or folder that already exists.
    /// </summary>
    public enum CreationCollisionOption
    {
        /// <summary>
        /// Creates a new file with a unique name of the form "name (2).txt"
        /// </summary>
        GenerateUniqueName = 0,
        /// <summary>
        /// Replaces any existing file with a new (empty) one. UWP will throw exception if there are any files in folder.
        /// </summary>
        ReplaceExisting = 1,
        /// <summary>
        /// Throws an exception if the file exists
        /// </summary>
        FailIfExists = 2,
        /// <summary>
        /// Opens the existing file, if any
        /// </summary>
        OpenIfExists = 3,
    }
}
