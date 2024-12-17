using BetterClip.Model.ClipData;
using Microsoft.Extensions.Logging;
using Clipboard = System.Windows.Forms.Clipboard;

namespace BetterClip.Service.Tools;

public static class Formats
{
    public const string CanIncludeInClipboardHistory = "CanIncludeInClipboardHistory";
    public const string CanUseClipboardHistory = "CanUseClipboardHistory";
    public const string CanUploadToCloudClipboard = "CanUploadToCloudClipboard";
    public const string QuickerActionSteps = "quicker-action-steps";
    public const string QuickerActionItem = "quicker-action-item";
    public const string QuickerActionDragItem = "quicker-action-drag-item";
    public const string QuickerFAICON = "FA_ICON";
    public const string QuickerCircleMenuDragItem = "circle_menu_drag_item";
    public const string QuickerActionProPage = "Quicker.Common.ActionProfile";
    public const string QuickerActionProFile = "Quicker.Common.ActionProfile";
    public const string QQFormat = "QQ_Unicode_RichEdit_Format";
}

public class ClipboardHelper
{
    private static readonly ILogger logger = App.GetLogger<ClipboardHelper>();
    public static System.Drawing.Image? GetImage()
    {
        return Clipboard.ContainsImage() ? Clipboard.GetImage() : default;
    }
    public static string? GetText()
    {
        return Clipboard.ContainsText() ? Clipboard.GetText() : default;
    }
    public static bool CanUseClipboardHistory()
    {
        logger.LogInformation("CanUseClipboardHistory {0}", Clipboard.GetData(Formats.CanUseClipboardHistory));
        return !Clipboard.ContainsData(Formats.CanUseClipboardHistory);
    }
    public static bool CanIncludeInClipboardHistory()
    {
        logger.LogInformation("CanIncludeInClipboardHistory {0}", Clipboard.GetData(Formats.CanIncludeInClipboardHistory));
        return !Clipboard.ContainsData(Formats.CanIncludeInClipboardHistory);
    }
    public static IList<string>? GetFileDropList()
    {
        return Clipboard.ContainsFileDropList() ? Clipboard.GetFileDropList().Cast<string>().ToList() : default;
    }
}