using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Wpf.Ui.Controls;
using MenuItem = System.Windows.Controls.MenuItem;

namespace BetterClip.Core.Menu
{
    public static class MenuFactory
    {
        public const int DefaultIconSize = 16;

        public static IconElement ParseIcon(string icon)
        {
            if (string.IsNullOrWhiteSpace(icon))
            {
                throw new ArgumentException("Icon string cannot be null or empty.", nameof(icon));
            }

            if (icon.StartsWith("sb:", StringComparison.OrdinalIgnoreCase))
            {
                var symbolName = icon["sb:".Length..];
                if (Enum.TryParse(typeof(SymbolRegular), symbolName, out var symbolValue))
                {
                    return new SymbolIcon { Symbol = (SymbolRegular)symbolValue };
                }
                else
                {
                    throw new ArgumentException($"Invalid SymbolRegular value: {symbolName}", nameof(icon));
                }
            }
            else
            {
                try
                {
                    var bitmapImage = new BitmapImage(new Uri(icon, UriKind.RelativeOrAbsolute));
                    return new ImageIcon { Source = bitmapImage, };
                }
                catch (Exception ex)
                {
                    throw new ArgumentException($"Invalid image URI: {icon}", ex);
                }
            }
        }
        public static IconElement? TryParseIcon(string icon, int size = DefaultIconSize)
        {
            try
            {
                var ie = ParseIcon(icon);
                ie.Width = size;
                ie.Height = size;
                return ie;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string? GetDescription(string? description) => string.IsNullOrEmpty(description) ? null : description;

        public static Separator CreateSeparator() => new();
        public static MenuItem CreateMenuItem(SymbolRegular symbol, string title, Action? action) => CreateMenuItem(symbol, title, null, action);

        public static MenuItem CreateMenuItem(SymbolRegular symbol, string title, string? des = null, Action? action = null)
        {
            return CreateMenuItem($"sb:{symbol}", title, des, (s, e) => action?.Invoke());
        }
        public static MenuItem CreateMenuItem(
            string icon,
            string title,
            string? description = null,
            RoutedEventHandler? clickHandler = null,
            Action<MenuItem>? configureAction = null,
            IEnumerable<Control>? children = null)
        {
            var menuItem = new MenuItem
            {
                Header = title,
                ToolTip = GetDescription(description),
                Icon = TryParseIcon(icon)
            };

            configureAction?.Invoke(menuItem);

            if (clickHandler != null)
            {
                menuItem.Click += clickHandler;
            }

            if (children != null)
            {
                AddChildren(menuItem, children);
            }

            return menuItem;
        }

        public static MenuItem CreateMenuItemWithChildren(
            string icon,
            string title,
            string? description,
            IEnumerable<Control> children,
            Action<MenuItem>? configureAction = null)
        {
            return CreateMenuItem(icon, title, description, null, configureAction, children);
        }

        public static void ResetChildren(this ItemsControl parent, IEnumerable<Control> children)
        {
            parent.Items.Clear();
            AddChildren(parent, children);
        }

        public static void AddChildren(this ItemsControl parent, IEnumerable<Control> children)
        {
            foreach (var child in children)
            {
                parent.Items.Add(child);
            }
        }
    }
}
