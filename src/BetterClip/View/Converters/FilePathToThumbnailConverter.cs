using BetterClip.Helpers;
using BetterClip.View.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace BetterClip.View.Converters;

internal sealed class FilePathToThumbnailConverter : ValueConverter<string, BitmapSource?>
{
    public override BitmapSource? Convert(string from)
    {
        try
        {
            return ImageHelper.GetThumbnailImage(from);
        }
        catch
        {
            return null;
        }
    }
}
