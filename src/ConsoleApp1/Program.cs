using System.Text.RegularExpressions;

namespace ConsoleApp1;

public class Program
{
    public static string DetectLanguage(string code)
    {
        // 定义一个字典，将语言与正则表达式进行匹配
        var patterns = new List<Tuple<string, string>>()
        {
            // JavaScript 正则
            new("JavaScript", @"\b(function|let|var|const|console\.log|=>|class)\b"),
            // C# 正则
            new("Csharp", @"\b(public\s+class|using|void|namespace)\b"),
            // Python 正则
            new("Python", @"\b(def|import|print|class|self)\b"),
        };

        // 使用 foreach 循环进行匹配
        foreach (var pattern in patterns)
        {
            // 如果匹配成功，则返回对应的语言
            if (Regex.IsMatch(code, pattern.Item2))
            {
                return pattern.Item1;
            }
        }

        // 如果没有匹配，返回 Unknown
        return "Unknown";
    }

    public static void Main()
    {
        string jsCode = "function sayHello() { console.log('Hello, world!'); }";
        string csharpCode = "public class Program { public static void Main() { } }";
        string pythonCode = "def greet():\n    print('Hello, world!')";
        string cppCode = "#include <iostream>\nclass MyClass { public: void greet() {} };";
        string rubyCode = "def greet\n  puts 'Hello, world!'\nend";
        string javaCode = "public class Main { public static void main(String[] args) {} }";

        Console.WriteLine("Detected Language (JS): " + DetectLanguage(jsCode));   // Output: JavaScript
        Console.WriteLine("Detected Language (C#): " + DetectLanguage(csharpCode)); // Output: C#
        Console.WriteLine("Detected Language (Python): " + DetectLanguage(pythonCode)); // Output: Python
        Console.WriteLine("Detected Language (C++): " + DetectLanguage(cppCode)); // Output: C++
        Console.WriteLine("Detected Language (Ruby): " + DetectLanguage(rubyCode)); // Output: Ruby
        Console.WriteLine("Detected Language (Java): " + DetectLanguage(javaCode)); // Output: Java
    }
}
