using BetterClip.Extension;
using BetterClip.View.Controls;

namespace BetterClip.View.Converters;

internal sealed class AvatarIconConverter : ValueConverter<string, Uri>
{
    /// <summary>
    /// 名称转Uri
    /// </summary>
    /// <param name="name">名称</param>
    /// <returns>链接</returns>
    public static Uri IconNameToUri(string name)
    {
        return Web.HutaoEndpoints.StaticRaw("AvatarIcon", $"{name}.png").ToUri();
    }


    /// <inheritdoc/>
    public override Uri Convert(string from)
    {
        return IconNameToUri(from);
    }
}
