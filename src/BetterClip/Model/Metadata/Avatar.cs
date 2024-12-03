using BetterClip.Model.Intrinsic;
using BetterClip.Model.Metadata.Converter;

namespace BetterClip.Model.Metadata
{
    public class Avatar
    {
        public int Id { get; set; }
        public string Icon { get; set; } = default!;
        public string Name { get; set; } = default!;
        public QualityType Quality { get; set; }
        /// <summary>
        /// 转换为基础物品
        /// </summary>
        /// <returns>基础物品</returns>
        public Model.Item ToItem()
        {
            return new()
            {
                Name = Name,
                Icon = AvatarIconConverter.IconNameToUri(Icon),
                Quality = Quality,
            };
        }
    }
}
