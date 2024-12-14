using BetterClip.Model.Metadata;

namespace BetterClip.Service.Interface
{
    public interface IMetadataService
    {
        IList<Avatar> GetAvatars();

        Avatar GetAvatar(int id);

        Avatar GetAvatar(string name);
    }
}
