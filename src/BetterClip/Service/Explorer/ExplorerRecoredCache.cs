using System.Text.Json;

namespace BetterClip.Service.Explorer;

public partial class ExplorerRecoredCache(string dir) : JsonFileSourceCache<ExplorerRecord>(dir)
{
    protected override ExplorerRecord? Json2Object(string json) => JsonSerializer.Deserialize<ExplorerRecord>(json);
    protected override string Object2Json(ExplorerRecord obj) => JsonSerializer.Serialize(obj);
    protected override string Object2Key(ExplorerRecord obj) => obj.Id;
    protected override DateTime Object2UpdateTime(ExplorerRecord obj) => obj.LastUseTime;
}
