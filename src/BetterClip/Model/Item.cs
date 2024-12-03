using BetterClip.Model.Intrinsic;

namespace BetterClip.Model
{
    public class Item : INameIcon
    {
        /// <summary>
        /// 物品名称
        /// </summary>
        public string Name { get; set; } = default!;

        /// <summary>
        /// 主图标
        /// </summary>
        public Uri Icon { get; set; } = default!;

        /// <summary>
        /// 小图标
        /// </summary>
        public Uri Badge { get; set; } = default!;

        public QualityType Quality { get; set; }

    }
}
