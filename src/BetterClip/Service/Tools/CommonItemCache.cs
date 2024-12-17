using BetterClip.Model.ClipData;

namespace BetterClip.Service;

public class CommonItemCache(string dir, int delayms = 200) : JsonFileSourceCache<CommonItem>(dir, delayms)
{
    protected override CommonItem? Json2Object(string file) => CommonItemHelper.FromJson(file);

    protected override string Object2Json(CommonItem obj) => obj.ToJson();

    protected override string Object2Key(CommonItem obj) => obj.Id;

    protected override DateTime Object2UpdateTime(CommonItem obj) => obj.UpdateTime;
}