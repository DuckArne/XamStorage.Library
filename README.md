# XamStorage.Library
A  .NetStandard 1.1 rewrite of dsplaisted [PCLStorage](https://github.com/dsplaisted/PCLStorage).


# Added public directorys to FileSystem

```C#
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
```

# Write or read from public directorys

On Android:
In your manifest you need to declare permission READ_EXTERNAL_STORAGE and WRITE_EXTERNAL_STORAGE.

On iOS:
In order to view and download files from iTunes you need to set UIFileSharing to true (yes).

info.plist
```xml
 <key>UIFileSharingEnabled</key>
  <true/>
```
	
