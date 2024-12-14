using BetterClip.View.Controls;
using System.IO;

namespace BetterClip.View.Converters;

internal sealed class FilePathToExistsConverter : ValueConverter<string, bool>
{
    public override bool Convert(string from)
    {
        return File.Exists(from) || Directory.Exists(from);
    }
}
