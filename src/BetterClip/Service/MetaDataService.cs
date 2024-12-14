using BetterClip.Core.Config;
using BetterClip.Model;
using System.Text.Json.Nodes;
using System.Text.Json;
using BetterClip.Model.Metadata;
using BetterClip.Service.Interface;
using System.IO;

namespace BetterClip.Service;

public class MetaDataService : IMetadataService
{
    private IList<Avatar>? cachedAvatars = null; // Cache field to store avatars

    public Avatar GetAvatar(int id) => GetAvatars().First(avatar => avatar.Id == id);

    public Avatar GetAvatar(string name) => GetAvatars().First(avatar => avatar.Name == name);

    public IList<Avatar> GetAvatars()
    {
        if (cachedAvatars != null)
        {
            // Return cached result if available
            return cachedAvatars;
        }

        // Otherwise, read and process the data
        var avatar_file_path = Global.Absolute("Resources", "SimplyAvatar.json");
        var avatar_json = JsonNode.Parse(File.ReadAllText(avatar_file_path));

        if (avatar_json is not JsonArray avatarArray)
        {
            throw new JsonException("Invalid JSON format");
        }

        var avatar_list = new List<Avatar>();
        var name_en_dict = GetAvatarNameENDict();
        foreach (JsonNode? item in avatarArray)
        {
            if (item != null)
            {
                var avatar = JsonSerializer.Deserialize<Avatar>(item.ToString()) ?? throw new JsonException("Invalid JSON format");
                avatar.NameEN = name_en_dict.GetValueOrDefault(avatar.Name, avatar.Icon.Split("_")[2]);
                avatar_list.Add(avatar);
            }
        }

        avatar_list.Reverse();

        // Cache the result for future calls
        cachedAvatars = avatar_list;
        return cachedAvatars;
    }

    private Dictionary<string, string> GetAvatarNameENDict()
    {
        return JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(Global.Absolute("Resources", "AvatarNameEN.json"))) ?? new Dictionary<string, string>();
    }
}

