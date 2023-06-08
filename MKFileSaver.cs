using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiFileSaver
{
    public static class MKFileSaver
    {
        static IFileSaver? fileSaver;
        public static IFileSaver Default
        {
            get
            {
                fileSaver ??= new FileSaver(new DefaultFolderProvider());
                return fileSaver;
            }
        }
        /// <summary>
        /// open file you saved
        /// </summary>
        /// <param name="platformPath">the path from file save result</param>
        /// <param name="fileOpenMode">"r","w","rw"</param>
        /// <returns></returns>
        public static Stream? Open(string platformPath,string fileOpenMode)
        {
            return Default.Open(platformPath,fileOpenMode);
        }

        /// <summary>
        /// save file in default folder withFolderType,
        /// if u target android29 lower,declare readExternalPermission and writeExternalPermission
        /// </summary>
        /// <param name="folderType">folderTypes are depend on enum SaveFolderType</param>
        /// <param name="childPath">etc "file.txt","zip/file.txt"</param>
        /// <returns>use method Open to open stream</returns>
        public static FileSaveResult? Save(int folderType, string childPath)
        {
            return Default.Save(folderType, childPath);
        }
    }
}
