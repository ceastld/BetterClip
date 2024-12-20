using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.RegularExpressions;
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

        public static string ToJson(this object obj, JsonSerializerOptions? options = null)
        {
            return JsonSerializer.Serialize(obj, options);
        }

        /// <summary>
        /// 比较字符串与给定的一列字符串中的任意一个是否相等。
        /// </summary>
        /// <param name="source">要比较的字符串。</param>
        /// <param name="ignoreCase">指示是否忽略大小写进行比较。</param>
        /// <param name="targets">给定的一列字符串。</param>
        /// <returns>如果字符串与给定的一列字符串中的任意一个相等，则返回 true；否则返回 false。</returns>
        public static bool EqualsAny(this string source, bool ignoreCase, params string[] targets)
        {
            StringComparison comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            return targets.Any(t => string.Equals(source, t, comparison));
        }


        /// <summary>
        /// 尝试获取指定索引位置的字符，如果获取失败则返回 '\0'。
        /// </summary>
        /// <param name="source">源字符串。</param>
        /// <param name="index">要获取字符的索引。</param>
        /// <returns>获取的字符。如果索引超出范围或发生异常，则返回 '\0'。</returns>
        public static char TryGetChar(this string source, int index)
        {
            if (index >= 0 && index < source.Length)
            {
                return source[index];
            }
            return '\0';
        }


        /// <summary>
        /// 从源字符串中移除指定的起始字符串。
        /// </summary>
        /// <param name="source">源字符串。</param>
        /// <param name="start">要移除的起始字符串。</param>
        /// <returns>移除起始字符串后的字符串。如果源字符串不以起始字符串开头，则返回源字符串。</returns>
        public static string CutStart(this string source, string start)
        {
            if (source.StartsWith(start))
                return source.Substring(start.Length);
            return source;
        }

        /// <summary>
        /// 检查字符串是否包含内容（不为 null 或空字符串）。
        /// </summary>
        /// <param name="str">要检查的字符串。</param>
        /// <returns>如果字符串包含内容，则返回 true；否则返回 false。</returns>
        public static bool HasContent(this string str) => !string.IsNullOrEmpty(str);


        /// <summary>
        /// 判断一个字符串是否在指定的一组字符串中出现。
        /// </summary>
        /// <param name="filter">要判断的字符串。</param>
        /// <param name="values">一组字符串。</param>
        /// <returns>如果字符串出现在指定的一组字符串中，则返回 true；否则返回 false。</returns>
        public static bool ContainedInAnyStrings(this string filter, IEnumerable<string> values)
        {
            if (string.IsNullOrEmpty(filter))
                return true;

            if (values == null)
                return false;

            StringComparison comparison = StringComparison.OrdinalIgnoreCase;
            return values.Any(str => !string.IsNullOrEmpty(str) && str.IndexOf(filter, comparison) >= 0);
        }



        /// <summary>
        /// 判断一个字符串是否在在指定的一组字符串中出现
        /// </summary>
        /// <param name="filter">一个字符串</param>
        /// <param name="values">可用逗哈分隔</param>
        /// <returns>是否出现</returns>
        public static bool ContainedInAny(this string filter, params string[] values) => filter.ContainedInAnyStrings(values);
        public static bool ContainsAny(this string source, params string[] values) => values.Any(source.Contains);
        public static string UrlEncode(this string text) => HttpUtility.UrlEncode(text);

        #region split

        private static readonly Regex LineBreakRegex = new(@"\r?\n|\r", RegexOptions.Compiled);

        /// <summary>
        /// 将字符串命令拆分为命令部分和剩余部分。
        /// </summary>
        /// <param name="cmd">要拆分的字符串命令。</param>
        /// <param name="rest">拆分后的剩余部分。</param>
        /// <returns>拆分后的命令部分。</returns>
        public static string SplitCmd(this string cmd, out string rest)
        {
            var parts = cmd.Split(new[] { ' ' }, 2);
            rest = parts.Length > 1 ? parts[1] : "";
            return parts[0];
        }

        /// <summary>
        /// 将字符串命令拆分为命令部分和注释部分。
        /// </summary>
        /// <param name="cmd">要拆分的字符串命令。</param>
        /// <param name="comments">拆分后的注释部分。</param>
        /// <returns>拆分后的命令部分。</returns>
        public static string SplitComments(this string cmd, out string comments)
        {
            var parts = cmd.Split(new[] { "//" }, StringSplitOptions.RemoveEmptyEntries);
            comments = parts.Length > 1 ? parts[1] : "";
            return parts[0];
        }

        /// <summary>
        /// 使用指定的字符串作为分割符，将文本拆分为字符串数组。
        /// </summary>
        /// <param name="text">要拆分的文本。</param>
        /// <param name="splitor">分割符字符串。</param>
        /// <returns>拆分后的字符串数组。</returns>
        public static string[] SplitWithString(this string text, params string[] splitor) => text.Split(splitor, StringSplitOptions.RemoveEmptyEntries);

        /// <summary>
        /// 将字符串按行分割为字符串数组。
        /// </summary>
        /// <param name="str">要拆分的字符串。</param>
        /// <returns>按行拆分后的字符串数组。</returns>
        public static string[] SplitWithLineBreak(this string str) => str.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

        /// <summary>
        /// 使用指定的分隔符将字符串拆分为字符串数组。
        /// </summary>
        /// <param name="str">要拆分的字符串。</param>
        /// <param name="splitter">分隔符字符串。</param>
        /// <returns>拆分后的字符串数组。</returns>
        public static string[] SplitToList(this string str, params string[] splitter)
        {
            if (str == null) return new string[0];
            return str.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// 将字符串按行分割为字符串数组。
        /// </summary>
        /// <param name="str">要拆分的字符串。</param>
        /// <returns>按行拆分后的字符串数组。</returns>
        public static string[] SplitToList(this string str) => str.SplitToList("\r\n", "\r", "\n");

        /// <summary>
        /// 使用正则表达式按行分割字符串为字符串数组。
        /// </summary>
        /// <param name="str">要拆分的字符串。</param>
        /// <returns>按行拆分后的字符串数组。</returns>
        public static string[] SplitWithLineBreakRegex(this string str)
        {
            if (str == null) return new string[0];
            return LineBreakRegex.Split(str);
        }

        /// <summary>
        /// 使用指定的分隔符将字符串拆分为两个部分。
        /// </summary>
        /// <param name="str">要拆分的字符串。</param>
        /// <param name="splitter">分隔符字符串。</param>
        /// <returns>包含拆分后的两个部分的元组。</returns>
        public static (string str1, string str2) SplitWithFirst(this string str, string splitter)
        {
            var parts = str.Split(new[] { splitter }, 2, StringSplitOptions.None);
            return (parts[0], parts.Length > 1 ? parts[1] : "");
        }

        #endregion


        /// <summary>
        /// 在源字符串中查找多个目标字符串，并返回目标字符串在源字符串中的总出现次数。
        /// </summary>
        /// <param name="source">源字符串。</param>
        /// <param name="targets">要查找的目标字符串数组。</param>
        /// <returns>目标字符串在源字符串中的总出现次数。</returns>
        public static int FindCount(this string source, params string[] targets) => targets.Sum(source.FindCount);

        /// <summary>
        /// 在源字符串中查找目标字符串，并返回目标字符串在源字符串中的出现次数。
        /// </summary>
        /// <param name="source">源字符串。</param>
        /// <param name="target">要查找的目标字符串。</param>
        /// <returns>目标字符串在源字符串中的出现次数。</returns>
        public static int FindCount(this string source, string target) => Regex.Matches(source, Regex.Escape(target)).Count;

        /// <summary>
        /// 计算字符串中的行数。
        /// </summary>
        /// <param name="source">源字符串。</param>
        /// <returns>字符串中的行数。</returns>
        public static int CountLines(this string source) => source.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).Length;

        /// <summary>
        /// 从源字符串中取出指定长度的子字符串。
        /// </summary>
        /// <param name="source">源字符串。</param>
        /// <param name="length">要取出的子字符串长度。</param>
        /// <returns>取出的子字符串。如果源字符串长度不足，则返回整个源字符串。</returns>
        public static string TakeLength(this string source, int length) => source.Length > length ? source.Substring(0, length) : source;

        /// <summary>
        /// 从源字符串中获取指定行数的文本。
        /// </summary>
        /// <param name="source">源字符串。</param>
        /// <param name="lineCount">要获取的行数。</param>
        /// <returns>获取的文本。如果源字符串为 null，则返回 null。</returns>
        public static string TakeLine(this string source, int lineCount)
        {
            if (source == null) return null;
            using StringReader sr = new(source);
            StringBuilder sb = new(lineCount * 50); // 假设平均每行50个字符
            for (int i = 0; i < lineCount; i++)
            {
                var line = sr.ReadLine();
                if (line == null) break;
                if (sb.Length > 0)
                {
                    sb.AppendLine();
                }
                sb.Append(line);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 将换行符和制表符替换为可视化的象形字符。
        /// </summary>
        /// <param name="source">源字符串。</param>
        /// <returns>替换后的字符串。</returns>
        public static string ReplaceSpace(this string source)
        {
            return source.Replace(Environment.NewLine, "⇩").Replace("\t", "⇥");
        }

        private static readonly Regex NewLineRegex = new Regex(@"\r?\n", RegexOptions.Compiled);

        /// <summary>
        /// 将字符串转换为指定长度的简短字符串。
        /// </summary>
        /// <param name="source">源字符串。</param>
        /// <param name="length">要转换的字符串的最大长度。</param>
        /// <returns>转换后的简短字符串。如果源字符串为 null，则返回 null。</returns>
        public static string ToShortString(this string source, int length)
        {
            if (source == null) return null;
            var text = NewLineRegex.Replace(source, "⏎");
            return text.Length <= length ? text : text.Substring(0, length) + new string('.', Math.Min(text.Length - length, 3));
        }

        /// <summary>
        /// 检查字符串是否适合在指定的矩形框内显示。
        /// </summary>
        /// <param name="source">要检查的字符串。</param>
        /// <param name="lineCount">矩形框的行数。</param>
        /// <param name="width">矩形框的宽度。</param>
        /// <returns>如果字符串适合在指定的矩形框内显示，则为 true；否则为 false。</returns>
        public static bool IsInRect(this string source, int lineCount, int width)
        {
            if (source.CountLines() > lineCount) return false;
            return source.Split(new[] { Environment.NewLine }, StringSplitOptions.None).All(x => x.Length < width);
        }

        /// <summary>
        /// 检查字符串是否不为 null 且不为空。
        /// </summary>
        /// <param name="source">要检查的字符串。</param>
        /// <returns>如果字符串不为 null 且不为空，则为 true；否则为 false。</returns>
        public static bool HasData(this string source) => !string.IsNullOrEmpty(source);


        /// <summary>
        /// 将字符串转换为 Base64 字符串。
        /// </summary>
        /// <param name="text">要转换的字符串。</param>
        /// <returns>转换后的 Base64 字符串。</returns>
        public static string ToBase64String(this string text) => Convert.ToBase64String(Encoding.ASCII.GetBytes(text));

        /// <summary>
        /// 将字符串重复指定的倍数。
        /// </summary>
        /// <param name="source">原始字符串。</param>
        /// <param name="multiplier">重复的倍数。</param>
        /// <returns>重复后的字符串。</returns>
        public static string Multiply(this string source, int multiplier)
        {
            var repeated = new StringBuilder(source.Length * multiplier);
            for (int i = 0; i < multiplier; i++)
            {
                repeated.Append(source);
            }
            return repeated.ToString();
        }


        /// <summary>
        /// 将字符串的首字母转换为小写。
        /// </summary>
        /// <param name="word">要转换的字符串。</param>
        /// <returns>转换后的字符串。</returns>
        public static string LowerFirstLetter(this string word)
        {
            if (string.IsNullOrEmpty(word)) return string.Empty;
            return word[0].ToString().ToLower() + word.Substring(1);
        }

        /// <summary>
        /// 将字符串的首字母转换为大写。
        /// </summary>
        /// <param name="word">要转换的字符串。</param>
        /// <returns>转换后的字符串。</returns>
        public static string UpperFirstLetter(this string word)
        {
            if (string.IsNullOrEmpty(word)) return string.Empty;
            return word[0].ToString().ToUpper() + word.Substring(1);
        }

        /// <summary>
        /// 将查询字符串转换为字典。
        /// </summary>
        /// <param name="queryString">要转换的查询字符串。</param>
        /// <returns>转换后的字典。</returns>
        public static Dictionary<string, string> QueryStringToDict(this string queryString)
        {
            var nvc = HttpUtility.ParseQueryString(queryString);
            return nvc.AllKeys.ToDictionary(k => k, k => nvc[k]);
        }

        /// <summary>
        /// 检查字符串是否是 quicker 表达式（$=）或插值表达式（$$）。
        /// </summary>
        /// <param name="code">要检查的字符串。</param>
        /// <returns>如果字符串以 "$=" 或 "$$" 开头，则为 true；否则为 false。</returns>
        public static bool IsExpressionOrInterpolation(this string code) => code.StartsWith("$=") || code.StartsWith("$$");

        /// <summary>
        /// 将字符串视为 URL，并获取其域名（Authority）部分。
        /// </summary>
        /// <param name="url">要获取域名的字符串。</param>
        /// <returns>URL 的域名。如果无法解析 URL 或 URL 的方案不是 HTTP 或 HTTPS，则返回 null。</returns>
        public static string GetAuthority(this string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
            {
                return uriResult.Authority;
            }
            return null;
        }
    }

}
