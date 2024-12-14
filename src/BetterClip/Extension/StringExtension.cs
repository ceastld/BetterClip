using System.Runtime.CompilerServices;
using System.Web;

namespace BetterClip.Extension
{

    internal static class StringExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Uri ToUri(this string value)
        {
            return new(value);
        }

        public static string UrlEncode(this string value)
        {
            return HttpUtility.UrlEncode(value);
        }
    }

}
