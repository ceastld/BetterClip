using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DetectLanguage;
using Windows.Media.Protection.PlayReady;

namespace BetterClip.Service
{
    public class LanguageService
    {
        private readonly DetectLanguageClient client;


        public LanguageService()
        {
            client = new DetectLanguageClient("f342329aa2cc6a143fb98b41565ea83c");
        }

        public async Task<string> DetectLanguage(string text)
        {
            DetectResult[] results = await client.DetectAsync("Buenos dias señor");

        }
    }
}
