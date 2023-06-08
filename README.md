<p>save file to default folder in maui</p>
<p>在maui上保存文件到默认文件夹的功能</p>
<p>Save a text file</p>
<p>保存文本文件</p>
<code>
            var res = MKFileSaver.Save((int)SaveFolderType.Video, "Test/Mytext.txt");
            using var stream = MKFileSaver.Open(res.PlatformPath, "w");
            using var sw = new StreamWriter(stream);
            sw.Write("测试文字");
  </code>
  <p>In android,it will save in storage/emulate/0/download/Test/Mytext.txt</p>
  <p>In windows,it will save in c:user/*/document/Test/MyText.txt</p>
  <p>if you targets android api29 lower,you need declare android.permission.READ_EXTERNAL_STORAGE and android.permission.WRITE_EXTERNAL_STORAGE</p>
  <p>如果安卓目标在api29以下，需要声明读写外部存储权限</p>
  <p>if you grant android.permission.MANAGE_EXTERNAL_STORAGE or under android api29 or windows,you can use fileApi or input directPath</p>
  <p>如果安卓有管理所有文件权限或者在android api29以下或者windows，可以使用fileapi或者给直接路径</p>
  <code>
            using var stream = MKFileSaver.Open(res.FullPath, "w");
            using var stream = File.OpenWrite(res.FullPath, "w");
  </code>
  <p>if you app running on android api 29 higher,you can only save a mp4(or other video format) file to saveFolderType.Video or saveFodlerType.Other,
otherwise,it will throw an exception.it's same for audio and images</p>
<p>在安卓11以上，只能在视频类型里或洽谈类型里存视频文件，否则抛出异常，音频和图像同理</p>
<p>change default folder:</p>
<p>更改默认文件夹:</p>
<code>
    public class FolderProvider : DefaultFolderProvider
    {
        protected override string GetDefaultFolder(DevicePlatform platform, int folderType)
        {
            SaveFolderType saveFolderType;
            try
            {
                saveFolderType = (SaveFolderType)folderType;
            }
            catch
            {
                saveFolderType = SaveFolderType.Other;
            }
            if (platform == DevicePlatform.Android)
            {
                return saveFolderType switch
                {
                    SaveFolderType.Video =>AndroidVideoDCIMPath,
                    SaveFolderType.Audio => AndroidAudioMusicPath,
                    SaveFolderType.Image => AndroidImagePicturesPath,
                    _ => AndroidOtherDownloadPath,
                };
            }
            else
            {
                return saveFolderType switch
                {
                    SaveFolderType.Video => Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
                    SaveFolderType.Audio => Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
                    SaveFolderType.Image => Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                    _ => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                };
            }
        }
    }
</code>
<code>
  MKFileSaver.Default.DefaultFolderProvider = new FolderProvider();
  </code>
  <p>In windows,you need return an absloute path,no restrict</p>
  <p>In Android api 29 higher,you can only return path define in defaultFolderProvider starts with AndroidImage**** for iamge file type,same as audio,video</p>
  <p>In android api 29 lower,you can return path relative to storage/emulate/0 ,no restrict</p>
  <p>windows上，你可以返回任意绝对路径
android api 29以下，可以返回任意相对于storage/emulate/0的路径，
android api 29以上，只能返回defaultFolderProvider里定义的路径，如图像只能返回DefaultFolderProvider.AndroidImagexxxx,视频音频同理</p>
  
