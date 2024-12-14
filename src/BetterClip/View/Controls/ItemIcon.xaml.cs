using BetterClip.Model.Intrinsic;
using DependencyPropertyGenerator;


namespace BetterClip.View.Controls;

/// <summary>
/// ItemIcon.xaml 的交互逻辑
/// </summary>
[DependencyProperty("Quality", typeof(QualityType), DefaultValue = QualityType.QUALITY_NONE)]
[DependencyProperty("Icon", typeof(Uri))]
[DependencyProperty("Badge", typeof(Uri))]
[DependencyProperty("CornerRadius", typeof(CornerRadius))]
public partial class ItemIcon : System.Windows.Controls.UserControl
{
    public ItemIcon()
    {
        InitializeComponent();
    }
}
