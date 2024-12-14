using BetterClip.View.Controls;
using System.IO;

namespace BetterClip.View.Converters;

internal sealed class FilePathToFileSizeConverter : ValueConverter<string, string>
{
    public override string Convert(string from)
    {
        return File.Exists(from) ? new FileInfo(from).Length.ToFileSize() : string.Empty;
    }
}
