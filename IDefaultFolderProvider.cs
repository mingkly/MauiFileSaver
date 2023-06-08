using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiFileSaver
{
    public interface IDefaultFolderProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="folderType">enum SaveFolderType or you own type</param>
        /// <returns>folder path,In windows its absolute path,
        /// In Android its relative path to storage/emulate/0/</returns>
        string GetDefaultFolder(int folderType);
    }
}
