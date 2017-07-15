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

On UWP:
In your appxmanifest  Capabilities, tick the Librarys you are using.
You can´t tick Documents Library there, you have to add that manually together with Filetype Assosiations.
```xml
 </uap:VisualElements>
 <!-- This -->
      <Extensions>
        <uap:Extension Category="windows.fileTypeAssociation">
          <uap:FileTypeAssociation Name="xls" DesiredView="default">
            <uap:DisplayName>MyExcelViewer</uap:DisplayName>
            <uap:SupportedFileTypes>
              <uap:FileType>.xls</uap:FileType>
              <uap:FileType>.pdf</uap:FileType>
              <uap:FileType>.html</uap:FileType>
            </uap:SupportedFileTypes>
          </uap:FileTypeAssociation>
        </uap:Extension>
      </Extensions>
      
    </Application>
    
    
    <!-- And in Capabilities -->
    <uap:Capability Name="musicLibrary" />
    <uap:Capability Name="picturesLibrary" />
    <uap:Capability Name="videosLibrary" />
    <uap:Capability Name="documentsLibrary" />
    
```

# Usage
WriteFile in Documents directory Example.
```C#
            IFolder rootFolder =await  FileSystem.Current.DocumentsFolderAsync();
            IFolder folder  = await rootFolder.CreateFolderAsync(SaveFolderName, CreationCollisionOption.OpenIfExists);

            IFile file = await folder.CreateFileAsync((Sheets[0].FirstRow.Name.Replace(" ", "") + "_" + Sheets[0].FirstRow.Year + "_V" +                 Sheets[0].FirstRow.Week + ".xls").ToSafeFileName(),
                CreationCollisionOption.ReplaceExisting);
                 
                var memoryStream = new MemoryStream();
                //Workbook is an ExcelLibrary that saves in the memoryStream
                Workbook.SaveAs(memoryStream);
                
                var bytes = memoryStream.ToArray();
                await file.WriteAsync(bytes, 0, bytes.Length);
```
