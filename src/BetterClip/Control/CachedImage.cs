using System.IO;
using System.Net.Http;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using BetterClip.Core.Config;

namespace BetterClip.Control;

public class CachedImage : Image
{
    static CachedImage()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(CachedImage),
                 new FrameworkPropertyMetadata(typeof(CachedImage)));
    }

    public readonly static DependencyProperty ImageUrlProperty = DependencyProperty.Register(
      "ImageUrl", typeof(string), typeof(CachedImage),
      new PropertyMetadata("", ImageUrlPropertyChanged));

    public string ImageUrl
    {
        get
        {
            return (string)GetValue(ImageUrlProperty);
        }
        set
        {
            SetValue(ImageUrlProperty, value);
        }
    }

    private static readonly object SafeCopy = new();

    private static readonly HttpClient HttpClient = new();

    private static async void ImageUrlPropertyChanged(DependencyObject obj,
            DependencyPropertyChangedEventArgs e)
    {
        var url = (string)e.NewValue;
        if (string.IsNullOrEmpty(url))
            return;

        var uri = new Uri(url);
        var cache_folder = Global.Absolute(".cache");
        Directory.CreateDirectory(cache_folder);
        var localFile = string.Format(Path.Combine
                       (cache_folder, uri.Segments[uri.Segments.Length - 1]));
        var tempFile = string.Format(Path.Combine(cache_folder, Guid.NewGuid().ToString()));

        if (File.Exists(localFile))
        {
            SetSource((CachedImage)obj, localFile);
        }
        else
        {
            try
            {
                var response = await HttpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                await using var fs = new FileStream(tempFile, FileMode.CreateNew);
                await response.Content.CopyToAsync(fs);
                fs.Close();

                lock (SafeCopy)
                {
                    if (!File.Exists(localFile))
                    {
                        File.Move(tempFile, localFile);
                    }
                }
                SetSource((CachedImage)obj, localFile);
            }
            catch
            {
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        }
    }

    private static void SetSource(Image inst, String path)
    {
        inst.Source = new BitmapImage(new Uri(path))
        {
            CacheOption = BitmapCacheOption.OnLoad,
            CreateOptions = BitmapCreateOptions.None,
        };
    }
}
