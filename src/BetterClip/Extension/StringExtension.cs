using System.Runtime.CompilerServices;
using System.Text.Json;
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
        public static string ToJson(this object obj, JsonSerializerOptions options)
        {
            return JsonSerializer.Serialize(obj, options);
        }
    }

}
