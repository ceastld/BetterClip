using BetterClip.Core.Config;
using BetterClip.Model.Metadata;

namespace BetterClip.Service.Interface
{
    public interface IConfigService
    {
        AllConfig Get();

        void Save();

        AllConfig Read();

        void Write(AllConfig config);
    }

    public interface IMetadataService
    {
        IList<Avatar> GetAvatars();
    }
}
