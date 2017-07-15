using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace XamStorage
{
    /// <summary>
    /// Provides portable versions of APIs such as Path.Combine
    /// </summary>
    public static class PortablePath
    {
        /// <summary>
        /// The character used to separate elements in a file system path
        /// </summary>
        public static char DirectorySeparatorChar {
            get {
#if UWP
				return '\\';
#elif NETSTANDARD
                throw FileSystem.NotImplementedInReferenceAssembly();
#else
                return Path.DirectorySeparatorChar;
#endif
            }
        }

        /// <summary>
        /// Combines multiple strings into a path
        /// </summary>
        /// <param name="paths">Path elements to combine</param>
        /// <returns>A combined path</returns>
        public static string Combine(params string[] paths)
        {
#if NETSTANDARD
            throw FileSystem.NotImplementedInReferenceAssembly();
#else
            return Path.Combine(paths);
#endif
        }
    }
}
