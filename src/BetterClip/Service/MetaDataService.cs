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
        foreach (JsonNode? item in avatarArray)
        {
            if (item != null)
            {
                var avatar = JsonSerializer.Deserialize<Avatar>(item.ToString()) ?? throw new JsonException("Invalid JSON format");
                avatar_list.Add(avatar);
            }
        }

        avatar_list.Reverse();

        // Cache the result for future calls
        cachedAvatars = avatar_list;
        return cachedAvatars;
    }
}

