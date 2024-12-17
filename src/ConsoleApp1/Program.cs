using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Text.Json;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;

namespace ConsoleApp1;

public class Program
{
    public static void Main(string[] args)
    {
        // 使用 Connect 将 listA 和 listB 双向绑定
        //.Do(change => Console.WriteLine($"Change detected: {change}"))
        var obj = new
        {
            listA = new ObservableCollectionExtended<int>(),
            listB = new ObservableCollectionExtended<int>()
        };
        obj.listA.ObserveCollectionChanges()
            .BindTo(obj, x=>x.listB);
        //listB.ObserveCollectionChanges().BindTo(listA, x => x);
        // 添加测试数据
        obj.listA.AddRange([1, 2, 3]);

        // 打印 listB 的内容以验证同步
        Console.WriteLine("ListB contents:");
        foreach (var item in obj.listB)
        {
            Console.WriteLine(item);
        }

        //修改 listB 的内容以验证双向同步
        obj.listB.Add(4);
        obj.listB.Remove(1);

        // 打印 listA 的内容以验证同步
        Console.WriteLine("ListA contents:");
        foreach (var item in obj.listA)
        {
            Console.WriteLine(item);
        }
    }
}
