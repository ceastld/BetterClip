namespace BetterClip.Model
{
    public interface INameIcon
    {
        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 图标
        /// </summary>
        Uri Icon { get; }
    }
}
