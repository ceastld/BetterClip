using System.Text;
using System.Runtime.InteropServices;
using System.Numerics;
using System.Security.Cryptography;

namespace BetterClip.Helpers;

public static class TextUtil
{
    public static int FindCount(string? mainString, string searchString)
    {
        if (mainString == null)
            return 0;

        int count = 0;
        int index = 0;

        while ((index = mainString.IndexOf(searchString, index)) != -1)
        {
            count++;
            index += searchString.Length;
        }

        return count;
    }

    public static IEnumerable<int> FindIndices(string? mainString, string searchString)
    {
        if (mainString == null)
            yield break;

        int index = 0;

        while ((index = mainString.IndexOf(searchString, index)) != -1)
        {
            yield return index;
            index += searchString.Length;
        }
    }

    /// <summary>
    /// 计算两个字符串的相似度。
    /// </summary>
    /// <param name="s1">第一个字符串。</param>
    /// <param name="s2">第二个字符串。</param>
    /// <returns>字符串的相似度，值介于0和1之间</returns>
    public static double Sim(this string s1, string s2)
    {
        double maxlen = Math.Max(s1.Length, s2.Length);
        return maxlen != 0 ? 1 - Levenshtein(s1, s2) / maxlen : 1;
    }
    public static double Sim(this string[] s1, string s2)
    {
        return 0;
    }

    /// <summary>
    /// 计算字符串s1和s2之间的Levenshtein编辑距离。
    /// </summary>
    /// <param name="s1">第一个字符串。</param>
    /// <param name="s2">第二个字符串。</param>
    /// <returns>返回s1和s2之间的Levenshtein编辑距离。</returns>
    public static int Levenshtein(string s1, string s2)
    {
        int l1 = s1.Length;
        int l2 = s2.Length;
        if (l1 == 0) return l1 == 0 ? 1 : 0;

        int[] c = new int[l2 + 1];
        int[] p = new int[l2 + 1];

        for (int a = 0; a <= l2; a++)
        {
            p[a] = a;
        }

        for (int i = 1; i <= l1; i++)
        {
            c[0] = i;
            for (int j = 1; j <= l2; j++)
            {
                int cost = s1[i - 1] == s2[j - 1] ? 0 : 1;
                c[j] = Math.Min(Math.Min(p[j - 1] + cost, c[j - 1] + 1), p[j] + 1);
            }
            Array.Copy(c, p, l2 + 1);
        }
        return c[l2];
    }

    public static void LongestCommonSubsequence<T>(IList<T> arr1, IList<T> arr2)
    {
        var len1 = arr1.Count;
        var len2 = arr2.Count;
        var dp = new int[len1 + 1, len2 + 1];

        for (int i = 1; i <= len1; i++)
        {
            for (int j = 1; j <= len2; j++)
            {
                dp[i, j] = Equals(arr1[i - 1], arr2[j - 1])
                    ? dp[i - 1, j - 1] + 1
                    : Math.Max(dp[i - 1, j], dp[i, j - 1]);
            }
        }

        Console.WriteLine($"最长公共子序列长度: {dp[len1, len2]}");
        Console.WriteLine("最长公共子序列为:");

        //回溯，输出数组
        void PrintLCS(int i, int j)
        {

            if (i > 0 && dp[i, j] == dp[i - 1, j])
            {
                PrintLCS(i - 1, j);
                Console.WriteLine(" - " + arr1[i - 1]);
            }
            else if (j > 0 && dp[i, j] == dp[i, j - 1])
            {
                PrintLCS(i, j - 1);
                Console.WriteLine(" + " + arr2[j - 1]);
            }
            else if (i > 0 && j > 0)
            {
                PrintLCS(i - 1, j - 1);
                Console.WriteLine("   " + arr1[i - 1]);
            }
        }

        PrintLCS(len1, len2);
    }

    /// <summary>
    /// 获取表示相对时间的短字符串。
    /// </summary>
    /// <param name="time">要表示的时间。</param>
    /// <returns>短字符串，表示相对时间。</returns>
    public static string GetRelativeTimeString(DateTime time, DateTime? target = null)
    {
        var time0 = target ?? DateTime.Now;
        var date = time0.Date;

        if (time >= date)
        {
            var sub = time0 - time;
            if (sub.TotalSeconds < 60)
                return "刚刚";
            if (sub.TotalMinutes < 60)
                return $"{sub.Minutes}分钟前";
            return $"{(int)sub.TotalHours}小时前";
        }

        if (time >= date - TimeSpan.FromDays(1))
            return time.ToString("昨天 HH:mm");

        if (time >= date - TimeSpan.FromDays(3))
            return $"{(time0 - time).Days}天前";
        if (time.Year == date.Year)
            return time.ToString("MM月 dd日");
        return time.ToString("yyyy.MM.dd");
    }

    public static string GetToNowTimeString(DateTime time)
    {
        var now = DateTime.Now;
        var sub = now - time;
        if (sub.TotalDays > 100)
            return time.ToShortDateString();
        else if (sub.TotalDays > 30)
            return sub.Days + "天前";
        else if (sub.TotalHours > 24)
            return $"{sub.Days}天{sub.Hours}小时前";
        else if (sub.TotalMinutes > 60)
            return $"{sub.Hours}小时{sub.Minutes}分钟前";
        else if (sub.TotalSeconds > 60)
            return $"{sub.Minutes}分钟前";
        else
            return "刚刚";
    }

    [DllImport("shell32.dll", SetLastError = true)]
    static extern nint CommandLineToArgvW([MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine, out int pNumArgs);

    public static string[] CommandLineToArgs(string commandLine)
    {
        var argv = CommandLineToArgvW(commandLine, out int argc);
        if (argv == nint.Zero)
            throw new System.ComponentModel.Win32Exception();
        try
        {
            var args = new string[argc];
            for (var i = 0; i < args.Length; i++)
            {
                var p = Marshal.ReadIntPtr(argv, i * nint.Size);
                args[i] = Marshal.PtrToStringUni(p);
            }

            return args;
        }
        finally
        {
            Marshal.FreeHGlobal(argv);
        }
    }

    #region Encryption
    public static string ToHexString(this byte[] bytes)
    {
        return string.Concat(bytes.Select(b => b.ToString("x2")));
    }

    /// <summary>
    /// 计算 MD5 值
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string ToMD5(this object obj, bool base62 = false)
    {
        byte[] data = obj switch
        {
            string str => Encoding.UTF8.GetBytes(str),
            byte[] bytes => bytes,
            _ => throw new ArgumentException("Invalid object type")
        };
        byte[] hash = MD5.HashData(data);
        return base62 ? hash.ToBase62() : hash.ToHexString();
    }

    public static string ToBase62(this Guid guid) => new BigInteger(guid.ToByteArray()).ToBase62();
    public static string ToBase62(this byte[] @byte) => new BigInteger(@byte).ToBase62();


    public static string GetBase62Guid()
    {
        var guid = Guid.NewGuid();
        var bigInt = new BigInteger(guid.ToByteArray());
        return bigInt.ToBase62();
    }

    private static string ToBase62(this BigInteger bigInt)
    {
        const string base62 = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        StringBuilder sb = new();
        while (bigInt != 0)
        {
            var mod = (int)(bigInt % 62);
            if (mod < 0) mod += 62;
            bigInt /= 62;
            sb.Insert(0, base62[mod]);
        }
        return sb.ToString();
    }
    #endregion
}
