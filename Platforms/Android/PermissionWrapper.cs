using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AndroidN = Android;

namespace MauiFileSaver.Platforms.Android
{
[Activity(MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    internal class PermissionWrapper:Activity
    {
        public static int ManageAllFileId = 1;
        public static TaskCompletionSource<bool>? TaskCompletionSource { get; set; }
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.SetContentView(new AndroidN.Widget.LinearLayout(this));
            try
            {
                var intent = new Intent(AndroidN.Provider.Settings.ActionManageAllFilesAccessPermission);
                StartActivityForResult(intent, ManageAllFileId);
            }
            catch
            {         
                TaskCompletionSource?.TrySetResult(false);
                Finish();
            }
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent? data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if(requestCode== ManageAllFileId)
            {
#pragma warning disable CA1416 // Validate platform compatibility
                TaskCompletionSource?.TrySetResult(AndroidN.OS.Environment.IsExternalStorageManager);
#pragma warning restore CA1416 // Validate platform compatibility
                Finish();
            }
        }
        public static string? GetAbsolutePath(AndroidN.Net.Uri uri)
        {
            using var cusor = AndroidN.App.Application.Context.ContentResolver?.Query(uri,
                new string[] { MediaStore.IMediaColumns.Data }, null, null, null);
            if (cusor != null && cusor.MoveToNext())
            {
                var dataCol = cusor.GetColumnIndex(MediaStore.IMediaColumns.Data);
                return cusor.GetString(dataCol);
            }
            return uri.Path;
        }
    }
}
