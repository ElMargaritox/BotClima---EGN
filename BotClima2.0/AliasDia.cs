using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rocket.API;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using UnityEngine;

namespace BotClima2._0
{
    class AliasDia : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "dia";

        public string Help => "Votacion Para Hacer De Dia";

        public string Syntax => string.Empty;

        public List<string> Aliases => new List<string> { "dias" };

        public List<string> Permissions => new List<string> { "clima.voto" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (Class1.Instance.activo)
            {
                UnturnedChat.Say(caller, "Ya Se Ha Iniciado Una Votacion. Espera A Que Finalize");
                return;
            }


            if (caller.HasPermission("clima.voto"))
            {
                Class1.Instance.tipovotacion = "DIA";
                Class1.Instance.Reloj.Start();
                Class1.Instance.texto = "DE DIA";
                Class1.Instance.activo = true;

                if (Class1.Instance.Configuration.Instance.ui)
                {
                    EffectManager.sendUIEffect(12761, 12760, false, "[" + Class1.Instance.votos + "/" + Class1.Instance.Configuration.Instance.votos_minimos + "]"); return;
                }

                ChatManager.serverSendMessage(Class1.Instance.Translate("votacion_count", Class1.Instance.tipovotacion, Class1.Instance.texto, Class1.Instance.votos, Class1.Instance.Configuration.Instance.votos_minimos).Replace('(', '<').Replace(')', '>'), Color.white, null, null, EChatMode.SAY, Class1.Instance.Configuration.Instance.icon, true); return;
            }
        }
    }
}
