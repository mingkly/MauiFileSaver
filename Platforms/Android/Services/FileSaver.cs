using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using Application = Android.App.Application;
using Environment = Android.OS.Environment;
using OS = Android.OS;
using Uri = Android.Net.Uri;

namespace MauiFileSaver.Platforms.Android.Services
{
    public class FileSaver
    {
        internal static FileSaveResult? Save(int folderType, string childPath)
        {
            if (Build.VERSION.SdkInt > BuildVersionCodes.Q)
            {
                return SaveAboveAPI29(folderType, childPath);
            }
            else
            {
                return SaveUnderAPI29( childPath);
            }
        }
        internal static Stream? Open(string platformPath, string fileOpenMode)
        {
            return OpenFile(platformPath,fileOpenMode);
        }
        static FileSaveResult? SaveUnderAPI29(string childPath)
        {
            var status = Permissions.CheckStatusAsync<Permissions.StorageRead>().Result;
            if (status != PermissionStatus.Granted)
            {
                status = Permissions.RequestAsync<Permissions.StorageRead>().Result;
                if (status != PermissionStatus.Granted)
                {
                    return null;
                }
                _ =Permissions.RequestAsync<Permissions.StorageWrite>().Result;
            }
            var folder =OS.Environment.ExternalStorageDirectory;
            var fileName= System.IO.Path.GetFileName(childPath);
            var baseFolder = childPath.Replace(fileName, "");
            var folderFile = new Java.IO.File(folder, baseFolder);
            if (!string.IsNullOrEmpty(baseFolder))
            {
                if (!folderFile.Exists())
                {
                    if(!folderFile.Mkdir())
                    {
                        throw new Exception();
                    }
                }
            }
            var file = new Java.IO.File(folderFile,fileName);
            return new FileSaveResult(fileName, file.AbsolutePath, file.AbsolutePath);
        }
        static FileSaveResult? SaveAboveAPI29(int folderType, string childPath)
        {
            try
            {
                Uri? downloadUri;
                SaveFolderType saveFolderType;
                try
                {
                    saveFolderType = (SaveFolderType)folderType;
                }
                catch
                {
                    saveFolderType = SaveFolderType.Other;
                }
                downloadUri = saveFolderType switch
                {
                    SaveFolderType.Video => MediaStore.Video.Media.ExternalContentUri,
                    SaveFolderType.Image => MediaStore.Images.Media.ExternalContentUri,
                    SaveFolderType.Audio => MediaStore.Audio.Media.ExternalContentUri,
                    _ => MediaStore.Files.GetContentUri(MediaStore.VolumeExternal),
                };
                if (downloadUri == null)
                {
                    return null;
                }
                var fileName = System.IO.Path.GetFileName(childPath);
                var baseFolder = childPath.Replace(fileName, "");
                ContentValues file = new();
                file.Put("_display_name", $"{fileName}");
                file.Put("relative_path", $"{baseFolder}");
                var fileUri = Application.Context.ContentResolver?.Insert(downloadUri, file);
                if (fileUri == null)
                {
                    return null;
                }
                return new FileSaveResult(fileName, PermissionWrapper.GetAbsolutePath(fileUri), fileUri.ToString());
            }
            catch
            {
                throw new NotSupportedException($"{(SaveFolderType)folderType} cant create child path :{childPath}");
            }

        }
        static Stream? OpenFile(string platformPath, string fileOpenMode)
        {
            if (platformPath.StartsWith("content"))
            {
                var uri = Uri.Parse(platformPath);
                if(uri == null)
                {
                    return null;
                }
                var context= Application.Context;
                if (fileOpenMode == "r")
                {
                    return context.ContentResolver?.OpenInputStream(uri);
                }
                else if(fileOpenMode == "w")
                {
                    return context?.ContentResolver?.OpenOutputStream(uri,fileOpenMode);
                }
                else
                {
                    var fd = Application.Context.ContentResolver?.OpenFileDescriptor(uri, "rw", null);
                    if (fd == null)
                    {
                        return null;
                    }
                    return new JavaStreamWrapper(fd);
                }
            }
            else
            {
                if (CanUseDirectFileApi()|| RequestUseDirerctFileApi().Result)
                {
                    return MauiFileSaver.FileSaver.OpenUseFileApi(platformPath, fileOpenMode);
                }
                return null;
            }
        }

        public static bool CanUseDirectFileApi()
        {
            var context=Application.Context;
            if (Build.VERSION.SdkInt > BuildVersionCodes.Q)
            {
#pragma warning disable CA1416 // Validate platform compatibility
                return Environment.IsExternalStorageManager;
#pragma warning restore CA1416 // Validate platform compatibility
            }
            else
            {
                var permission = AndroidX.Core.Content.ContextCompat.CheckSelfPermission(context,Manifest.Permission.ReadExternalStorage);
                return permission == Permission.Granted;
            }
        }
        public static async Task<bool> RequestUseDirerctFileApi()
        {
            if (Build.VERSION.SdkInt > BuildVersionCodes.Q)
            {
                var intent = new Intent(Application.Context, typeof(PermissionWrapper));
                intent.AddFlags(ActivityFlags.NewTask);
                PermissionWrapper.TaskCompletionSource = new TaskCompletionSource<bool>();
                Application.Context.StartActivity(intent);
                return await PermissionWrapper.TaskCompletionSource.Task;
            }
            else
            {
                var permission = await Permissions.RequestAsync<Permissions.StorageRead>();
                if (permission == PermissionStatus.Granted)
                {
                    await Permissions.RequestAsync<Permissions.StorageWrite>();
                    return true;
                }
                return false;
            }
        }
    }
}
