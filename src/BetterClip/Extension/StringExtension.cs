using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BetterClip.Extension
{
    internal static class StringExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Uri ToUri(this string value)
        {
            return new(value);
        }
    }
}
