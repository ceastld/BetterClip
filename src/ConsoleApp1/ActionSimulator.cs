using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

public class ActionSimulator
{
    public static async Task SimulateAsync()
    {
        var actionSubject = new Subject<Action>();

        // 设置节流时间为 150ms
        actionSubject
            .Throttle(TimeSpan.FromMilliseconds(50)) // 150ms 内只保留最后一个事件
            .Subscribe(action =>
            {
                try
                {
                    // 执行最终的 action
                    action.Invoke();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            },
            () => Console.WriteLine("Completed")); // 订阅完成通知

        // 异步模拟多次调用
        await SimulateAsyncCalls(actionSubject);

        // 通知完成
        actionSubject.OnCompleted();

        // 保持程序运行以观察输出
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
    private static Random random = new();
    private static async Task SimulateAsyncCalls(Subject<Action> actionSubject)
    {
        for (int i = 0; i < 1; i++)
        {
            int callIndex = i; // 捕获当前索引
            actionSubject.OnNext(() => Console.WriteLine($"Action executed: {callIndex}"));

            // 添加延迟以模拟调用间隔
            await Task.Delay(random.Next(100));
        }

        // 最后的调用
        actionSubject.OnNext(() => Console.WriteLine("Final Action: aaa"));
    }
}
