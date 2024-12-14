using System.Collections;

namespace BetterClip.Extension;

public static class ListExtension
{
    /// <summary>
    /// 列表不为空，并且项数大于零
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    public static bool HasContent(this IList list) => list != null && list.Count > 0;

    //public static string MergeToShortString<T>(this IEnumerable<T> list,int count, string splitter = "\r\n")

    /// <summary>
    /// 获取可以在列表中插入的位置，处理了溢出的情况
    /// </summary>
    /// <param name="list"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static int GetInsetIndex(this IList list, int index)
    {
        var count = list.Count;
        if (count == 0 || index < -1) return 0;
        if (index == -1 || index > count) return count;
        return index;
    }

    /// <summary>
    /// 获取有效的的索引位置
    /// </summary>
    /// <param name="list"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static int GetEffectiveIndex(this ICollection list, int index)
    {
        return index.Limit(0, list.Count - 1);
    }

    /// <summary>
    /// 排除列表 index 溢出的危险，返回相应的项
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static T? SafetyElementAt<T>(this IList<T> list, int index)
    {
        if (list.Count == 0)
            return default;
        return list[index.Limit(0, list.Count - 1)];
    }

    public static int GetCycleIndex(this ICollection list, int index)
    {
        var count = list.Count;
        return count == 0 ? 0 : index.Mod(count);
    }
    public static bool MoreThan<T>(this IEnumerable<T> items, int count) => items.Skip(count).Any();
    public static bool GreatEqual<T>(this IEnumerable<T> items, int count) => items.Skip(count - 1).Any();
    public static bool LessThan<T>(this IEnumerable<T> items, int count) => !items.GreatEqual(count);
    public static bool LessEqual<T>(this IEnumerable<T> items, int count) => !items.MoreThan(count);

    /// <summary>
    /// 查找满足条件的第一个元素，并确保只有一个满足条件的元素存在。
    /// </summary>
    /// <typeparam name="T">集合元素的类型。</typeparam>
    /// <param name="items">要操作的集合。</param>
    /// <param name="predicate">用于匹配元素的条件。</param>
    /// <returns>满足条件的第一个元素；如果不存在满足条件的元素或存在多个满足条件的元素，则为默认值。</returns>
    public static T FirstOnlyOrDefault<T>(this IEnumerable<T> items, Func<T, bool> predicate)
    {
        T result = default;
        bool found = false;
        foreach (var item in items)
        {
            if (predicate(item))
            {
                if (found)
                {
                    return default;
                }
                result = item;
                found = true;
            }
        }
        return found ? result : default;
    }


    /// <summary>
    /// 查找满足条件的第一个元素的索引。
    /// </summary>
    /// <typeparam name="T">列表元素的类型。</typeparam>
    /// <param name="list">要操作的列表。</param>
    /// <param name="predicate">用于匹配元素的条件。</param>
    /// <returns>满足条件的第一个元素的索引；如果找不到满足条件的元素，则为 -1。</returns>
    public static int FirstIndexOf<T>(this IList<T> list, Func<T, bool> predicate)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (predicate(list[i]))
            {
                return i;
            }
        }
        return -1;
    }

    public static int FirstIndexOf(this IList list, Func<object, bool> predicate)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (predicate(list[i]))
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// 查找满足条件的最后一个元素的索引。
    /// </summary>
    /// <typeparam name="T">列表元素的类型。</typeparam>
    /// <param name="list">要操作的列表。</param>
    /// <param name="predicate">用于匹配元素的条件。</param>
    /// <returns>满足条件的最后一个元素的索引；如果找不到满足条件的元素，则为 -1。</returns>
    public static int LastIndexOf<T>(this IList<T> list, Func<T, bool> predicate)
    {
        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (predicate(list[i]))
            {
                return i;
            }
        }
        return -1;
    }


    /// <summary>
    /// 从列表中移除满足条件的第一个元素。
    /// </summary>
    /// <typeparam name="T">列表元素的类型。</typeparam>
    /// <param name="list">要操作的列表。</param>
    /// <param name="predicate">用于匹配元素的条件。</param>
    /// <returns>如果成功移除元素，则为 true；否则为 false。</returns>
    public static bool RemoveFirst<T>(this IList<T> list, Func<T, bool> predicate)
    {
        int index = list.FirstIndexOf(predicate);
        if (index != -1)
        {
            list.RemoveAt(index);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 从列表中移除最后一个元素。
    /// </summary>
    /// <typeparam name="T">列表元素的类型。</typeparam>
    /// <param name="list">要操作的列表。</param>
    public static void RemoveLast<T>(this IList<T> list)
    {
        int lastIndex = list.Count - 1;
        if (lastIndex >= 0)
        {
            list.RemoveAt(lastIndex);
        }
    }
}
