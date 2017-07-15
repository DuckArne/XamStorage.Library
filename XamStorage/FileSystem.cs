using System;
using System.IO;
#if UWP
using XamStorage.UWP;
#elif IOS
using XamStorage.iOS;
#elif ANDROID
using XamStorage.Android;
#endif

namespace XamStorage
{
    public static class FileSystem
    {
        static Lazy<IFileSystem> _fileSystem = new Lazy<IFileSystem>(() => CreateFileSystem(), System.Threading.LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// The implementation of <see cref="IFileSystem"/> for the current platform
        /// </summary>
        public static IFileSystem Current {
            get {
                IFileSystem ret = _fileSystem.Value;
                if (ret == null)
                {
                    throw NotImplementedInReferenceAssembly();
                }               
                return ret;
            }
        }

      static IFileSystem CreateFileSystem()
        {
#if UWP
            return new UWPFileSystem();
#elif IOS
            return new IOSFileSystem();
#elif ANDROID
            return new AndroidFileSystem();
#else
            return null;
#endif
        }

        internal static Exception NotImplementedInReferenceAssembly()
        {
            return new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the XamStorage NuGet package from your main application project in order to reference the platform-specific implementation.");
        }
    }
}

