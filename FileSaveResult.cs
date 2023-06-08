using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiFileSaver
{
    public class FileSaveResult
    {
        public string? FileName { get;}
        public string? FullPath { get; }
        public string? PlatformPath { get; }
        public FileSaveResult(string? fileName, string? fullPath, string? platformPath)
        {
            FileName = fileName;
            FullPath = fullPath;
            PlatformPath = platformPath;
        }
    }
}
