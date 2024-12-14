namespace BetterClip.Extension;

public static class NumberExtension
{
    //public static string ToBase64String(Decimal)
    public static int Mod(this int a, int m) => (a % m + m) % m;

    /// <summary>
    /// 将数值限制在一个范围内
    /// </summary>
    /// <param name="a"></param>
    /// <param name="low">下界</param>
    /// <param name="high">上界</param>
    /// <returns></returns>
    public static int Limit(this int a, int low, int high) => a < low ? low : a > high ? high : a;
}
