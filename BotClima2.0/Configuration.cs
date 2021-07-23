using System.Text;
using System.Threading.Tasks;
using Rocket.API;
using Rocket.Core;
using Rocket.Unturned;

namespace BotClima2._0
{
    public class Configuration : IRocketPluginConfiguration
    {

        public string icon;
        public int votos_minimos;
        public bool ui;
        public int interval;
        public void LoadDefaults()
        {
            ui = false;
            votos_minimos = 5;
            interval = 5;
            icon = "https://i.imgur.com/NaOVAWZ.png";
        }
    }
}
