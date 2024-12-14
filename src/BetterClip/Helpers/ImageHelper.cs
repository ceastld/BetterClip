using Microsoft.WindowsAPICodePack.Shell;
using System.Windows.Media.Imaging;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;

namespace BetterClip.Helpers
{
    public static class ImageHelper
    {
        public static BitmapImage ConvertToImageSource(this Image img)
        {
            BitmapImage bitmapImage = new();
            using MemoryStream memoryStream = new();
            img.Save(memoryStream, ImageFormat.Bmp);
            memoryStream.Position = 0L;
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.UriSource = null;
            bitmapImage.StreamSource = memoryStream;
            bitmapImage.EndInit();
            if (bitmapImage.CanFreeze)
                bitmapImage.Freeze();
            return bitmapImage;
        }
        public static void SaveImageToFile(BitmapSource image, string filePath)
        {
            BitmapEncoder encoder = GetBitmapEncoder(filePath);
            encoder.Frames.Add(BitmapFrame.Create(image));
            using var stream = new FileStream(filePath, FileMode.Create);
            encoder.Save(stream);
        }
        private static BitmapEncoder GetBitmapEncoder(string filePath)
        {
            var extName = Path.GetExtension(filePath).ToLower();
            if (extName.Equals(".png"))
            {
                return new PngBitmapEncoder();
            }
            else
            {
                return new JpegBitmapEncoder();
            }
        }

        public static BitmapSource? GetThumbnailImage(string path)
        {
            if (!File.Exists(path) && !Directory.Exists(path)) return null;

            using ShellObject shellObj = ShellObject.FromParsingName(path);

            // 强制刷新缩略图
            shellObj.Thumbnail.FormatOption = ShellThumbnailFormatOption.Default;
            shellObj.Thumbnail.RetrievalOption = ShellThumbnailRetrievalOption.Default;

            // 尝试获取缩略图，如果仍为空，调用刷新逻辑
            var thumbnail = shellObj.Thumbnail.BitmapSource;

            if (thumbnail == null)
            {
                RefreshThumbnailCache(path);
                thumbnail = shellObj.Thumbnail.BitmapSource;
            }

            return thumbnail;
        }

        /// <summary>
        /// 强制刷新系统生成缩略图
        /// </summary>
        /// <param name="path">文件路径</param>
        private static void RefreshThumbnailCache(string path)
        {
            try
            {
                // 使用 Shell 的 COM 接口操作缩略图缓存
                SHChangeNotify(SHCNE_UPDATEITEM, SHCNF_PATH, path, IntPtr.Zero);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to refresh thumbnail cache: {ex.Message}");
            }
        }

        #region PInvoke 声明
        private const uint SHCNE_UPDATEITEM = 0x00002000; // 更新文件条目
        private const uint SHCNF_PATH = 0x0005;          // 文件路径格式

        [System.Runtime.InteropServices.DllImport("shell32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
        private static extern void SHChangeNotify(uint wEventId, uint uFlags, string dwItem1, IntPtr dwItem2);
        #endregion
    }
}
