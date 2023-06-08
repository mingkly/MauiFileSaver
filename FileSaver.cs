using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiFileSaver
{
    public class FileSaver : IFileSaver
    {
        public IDefaultFolderProvider DefaultFolderProvider { get; set; }
        public FileSaver(IDefaultFolderProvider defaultFolderProvider)
        {
            DefaultFolderProvider = defaultFolderProvider;
        }

        public Stream? Open(string platformPath, string fileOpenMode)
        {
#if ANDROID
            return Platforms.Android.Services.FileSaver.Open(platformPath,fileOpenMode);
#else
            return OpenUseFileApi(platformPath,fileOpenMode);
#endif
        }
        public static Stream? OpenUseFileApi(string platformPath, string fileOpenMode)
        {
            if (fileOpenMode == "r")
            {
                return File.OpenRead(platformPath);
            }
            else if (fileOpenMode == "w")
            {
                return File.OpenWrite(platformPath);
            }
            else
            {
                return File.Open(platformPath, FileMode.OpenOrCreate);
            }
        }
        public FileSaveResult? Save(int folderType, string childPath)
        {
            var folder=DefaultFolderProvider.GetDefaultFolder(folderType);
            if (folder == null)
            {
                return null;
            }
#if ANDROID
            return Platforms.Android.Services.FileSaver.Save(folderType, Path.Combine(folder,childPath));
#else
            var fullPath=Path.Combine(folder,childPath);
            var fileName=Path.GetFileName(fullPath);
            var basePath=fullPath.Replace(fileName,"");
            if(!Directory.Exists(basePath))
            {
               Directory.CreateDirectory(basePath);
            }
            if(!File.Exists(fullPath))
            {
               using(File.Create(fullPath)){}
            }
            return new FileSaveResult(fileName,fullPath,fullPath);
#endif
        }
    }
}
