using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace XamStorage.Exceptions
{
    /// <exclude/>
    public class FileNotFoundException
#if NETSTANDARD
 : IOException
#else
        : System.IO.FileNotFoundException
#endif
    {
        /// <exclude/>
        public FileNotFoundException(string message)
            : base(message)
        {

        }

        /// <exclude/>
        public FileNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }

    /// <exclude/>
    public class DirectoryNotFoundException
#if NETSTANDARD
        : IOException
#elif UWP
        : System.IO.FileNotFoundException
#else
        : System.IO.DirectoryNotFoundException
#endif
    {
        /// <exclude/>
        public DirectoryNotFoundException(string message)
            : base(message)
        {

        }

        /// <exclude/>
        public DirectoryNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
