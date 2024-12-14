using System.Windows.Forms;
using BetterClip.Core.Config;

namespace BetterClip.Service.Interface
{
    public interface IConfigService
    {
        AllConfig Get();

        void Save();

        AllConfig Read();

        void Write(AllConfig config);
    }
}
