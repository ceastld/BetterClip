using BetterClip.View.Controls;

namespace BetterClip.View.Converters;

internal class TextMinimizeIndentConverter : ValueConverter<string, string>
{
    public override string Convert(string value)
    {
        if (value is string input)
        {
            var lines = input.Split(["\r\n", "\r", "\n"], StringSplitOptions.RemoveEmptyEntries);

            //列表中需要至少包含一个元素
            if (lines.Length == 0) return "";

            int minIndent = lines.Min(line => line.Length - line.TrimStart().Length);

            var modifiedLines = lines.Select(line => line[minIndent..]);
            return string.Join(Environment.NewLine, modifiedLines);
        }

        return value;
    }
}
