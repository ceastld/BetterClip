using BetterClip.Core.Config;
using BetterClip.View.Controls;

namespace BetterClip.View.Converters;

internal sealed class AbsulutePathConverter : ValueConverter<string, string>
{
    public override string Convert(string from)
    {
        return Global.Absolute(from);
    }
}