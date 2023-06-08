using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace MauiFileSaver
{
    public class DefaultFolderProvider : IDefaultFolderProvider
    {
        public readonly static string AndroidImageDCIMPath = "DCIM";
        public readonly static string AndroidImagePicturesPath = "Pictures";

        public readonly static string AndroidVideoDCIMPath = "DCIM";
        public readonly static string AndroidVideoPicturesPath = "Pictures";
        public readonly static string AndroidVideoMoviesPath = "Movies";

        public readonly static string AndroidAudioAlarmsPath = "Alarms";
        public readonly static string AndroidAudioAudiobooksPath = "Audiobooks";
        public readonly static string AndroidAudioMusicPath = "Music";
        public readonly static string AndroidAudioNotificationsPath = "Notifications";
        public readonly static string AndroidAudioPodcastsPath = "Podcasts";
        [SupportedOSPlatform("android30.0")]
        public readonly static string AndroidAudioRingtonesPath = "Ringtones";

        public readonly static string AndroidOtherDownloadPath = "Download";

        public virtual string GetDefaultFolder(int folderType)
        {
#if ANDROID
            return GetDefaultFolder(DevicePlatform.Android, folderType);
#elif WINDOWS
            return GetDefaultFolder(DevicePlatform.WinUI, folderType);
#else
            return GetDefaultFolder(DevicePlatform.iOS, folderType);
#endif
        }
        protected virtual string GetDefaultFolder(DevicePlatform platform, int folderType)
        {
            string folder;
            SaveFolderType folderEnumType;
            try
            {
                folderEnumType = (SaveFolderType)folderType;
            }
            catch
            {
                folderEnumType = SaveFolderType.Other;
            }
            if (platform == DevicePlatform.Android)
            {
                folder = folderEnumType switch
                {
                    SaveFolderType.Video => AndroidVideoMoviesPath,
                    SaveFolderType.Audio => AndroidAudioMusicPath,
                    SaveFolderType.Image => AndroidImagePicturesPath,
                    _ => AndroidOtherDownloadPath,
                };
            }
            else if (platform == DevicePlatform.WinUI)
            {
                folder = folderEnumType switch
                {
                    SaveFolderType.Video => Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
                    SaveFolderType.Audio => Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
                    SaveFolderType.Image => Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                    _ => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                };
            }
            else
            {
                folder = FileSystem.AppDataDirectory;
            }
            return folder;
        }

    }
}
