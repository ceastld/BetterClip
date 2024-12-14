using BetterClip.View.Controls;
using System.IO;

namespace BetterClip.View.Converters;

internal sealed class FilePathToFileNameConverter : ValueConverter<string, string>
{
    public override string Convert(string from)
    {
        return Path.GetFileName(from);
    }
}
