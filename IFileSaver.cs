using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiFileSaver
{
    public interface IFileSaver
    {
        /// <summary>
        /// provider default foder depend on folderType
        /// </summary>
        IDefaultFolderProvider DefaultFolderProvider { get; set; }
        /// <summary>
        /// save file in default folder withFolderType,
        /// if u target android29 lower,declare readExternalPermission and writeExternalPermission
        /// </summary>
        /// <param name="folderType">folderTypes are depend on enum SaveFolderType</param>
        /// <param name="childPath">etc "file.txt","zip/file.txt"</param>
        /// <returns>use method Open to open stream</returns>
        FileSaveResult? Save(int folderType, string childPath);
        /// <summary>
        /// open file you saved
        /// </summary>
        /// <param name="platformPath">the path from file save result</param>
        /// <param name="fileOpenMode">"r","w","rw"</param>
        /// <returns></returns>
        Stream? Open(string platformPath,string fileOpenMode);
    }
}
