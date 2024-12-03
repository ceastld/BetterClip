using BetterClip.Model.Intrinsic;
using DependencyPropertyGenerator;


namespace BetterClip.View.Control;

/// <summary>
/// ItemIcon.xaml 的交互逻辑
/// </summary>
[DependencyProperty("Quality", typeof(QualityType), DefaultValue = QualityType.QUALITY_NONE)]
[DependencyProperty("Icon", typeof(Uri))]
[DependencyProperty("Badge", typeof(Uri))]
public partial class ItemIcon : System.Windows.Controls.UserControl
{
    public ItemIcon()
    {
        InitializeComponent();
    }
}
