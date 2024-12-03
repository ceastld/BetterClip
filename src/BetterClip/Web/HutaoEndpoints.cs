using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterClip.Web
{
    internal static class HutaoEndpoints
    {
        /// <summary>
        /// 图片资源
        /// </summary>
        /// <param name="category">分类</param>
        /// <param name="fileName">文件名称 包括后缀</param>
        /// <returns>路径</returns>
        public static string StaticRaw(string category, string fileName)
        {
            return $"{ApiSnapGenshinStaticRaw}/{category}/{fileName}";
        }


        private const string ApiSnapGenshin = "https://api.snapgenshin.com";
        private const string ApiSnapGenshinMetadata = $"{ApiSnapGenshin}/metadata";
        private const string ApiSnapGenshinPatch = $"{ApiSnapGenshin}/patch";
        private const string ApiSnapGenshinStaticRaw = $"{ApiSnapGenshin}/static/raw";
        private const string ApiSnapGenshinStaticZip = $"{ApiSnapGenshin}/static/zip";
        private const string ApiSnapGenshinEnka = $"{ApiSnapGenshin}/enka";

    }
}
