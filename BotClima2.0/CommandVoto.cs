using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BotClima2._0
{
    class CommandVoto : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "votar";

        public string Help => "/votar [dia|lluvia]";

        public string Syntax => string.Empty;

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string> { "clima.voto" };

        
        public void Execute(IRocketPlayer caller, string[] command)
        {
            
            string tipovoto = command[0].ToString(); tipovoto.ToLower();


            if (command.Length != 1)
            {
                UnturnedChat.Say(caller, "[VOTOS] /votar {dia|lluvia}"); return;
            }

            if (tipovoto == "admin" & caller.IsAdmin) {

                UnturnedPlayer administrador = (UnturnedPlayer)caller;
                
                Class1.Instance.LlamarAdmin(administrador);  return;
            
            }

            if (Class1.Instance.activo)
            {
                UnturnedChat.Say(caller, "Ya Se Ha Iniciado Una Votacion. Espera A Que Finalize");
                return;
            }

            if (caller.HasPermission("clima.voto"))
            {
                
                switch (tipovoto)
                {
                    case "dia":
                        Class1.Instance.tipovotacion = "DIA";
                        Class1.Instance.Reloj.Start();
                        Class1.Instance.texto = "DE DIA";
                        Class1.Instance.activo = true;

                        if (Class1.Instance.Configuration.Instance.ui)
                        {
                            EffectManager.sendUIEffect(12761, 12760, false, "[" + Class1.Instance.votos + "/" + Class1.Instance.Configuration.Instance.votos_minimos + "]"); return;
                        }

                        ChatManager.serverSendMessage(Class1.Instance.Translate("votacion_count", Class1.Instance.tipovotacion, Class1.Instance.texto, Class1.Instance.votos, Class1.Instance.Configuration.Instance.votos_minimos).Replace('(', '<').Replace(')', '>'), Color.white, null, null, EChatMode.SAY, Class1.Instance.Configuration.Instance.icon, true); return;
                    case "lluvia":
                        Class1.Instance.tipovotacion = "LLUVIA";
                        Class1.Instance.texto = "EL SOL";
                        Class1.Instance.Reloj.Start();
                        Class1.Instance.activo = true;

                        if (Class1.Instance.Configuration.Instance.ui)
                        {
                            EffectManager.sendUIEffect(12762, 12760, false, "[" + Class1.Instance.votos + "/" + Class1.Instance.Configuration.Instance.votos_minimos + "]"); return;
                        }

                        ChatManager.serverSendMessage(Class1.Instance.Translate("votacion_count", Class1.Instance.tipovotacion, Class1.Instance.texto, Class1.Instance.votos, Class1.Instance.Configuration.Instance.votos_minimos).Replace('(', '<').Replace(')', '>'), Color.white, null, null, EChatMode.SAY, Class1.Instance.Configuration.Instance.icon, true); return;

                }

                
            }
            else
            {
                UnturnedChat.Say(caller, "No Tienes Permisos Suficientes"); return;
            }
           

        }
    }
}
