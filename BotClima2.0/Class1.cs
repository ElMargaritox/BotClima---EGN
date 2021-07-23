using Rocket.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDG.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using UnityEngine;
using Rocket.Unturned.Chat;
using Rocket.API.Collections;
using Rocket.Core;
using Rocket.API;
using Rocket.Unturned;
using System.Timers;
using Rocket.Core.Utils;
using Logger = Rocket.Core.Logging.Logger;
using System.IO;
using Newtonsoft.Json;

namespace BotClima2._0
{
    public class Class1 : RocketPlugin<Configuration>
    {
        public static Class1 Instance;
        public List<UnturnedPlayer> personas = new List<UnturnedPlayer>();
        public bool activo;
        public Timer Reloj { get; set; }
        public int contador;
        public string texto;
        public static string myip;
        public int votos;
        public int margaritadios;
        public string tipovotacion;
        protected override void Load()
        {



            base.Load();
            Instance = this;
            activo = false;
            Reloj = new Timer(Class1.Instance.Configuration.Instance.interval * 1000);
            Reloj.Elapsed += Reloj_Elapsed;
            if (Provider.getServerWorkshopFileIDs().Contains(2387083742) & Instance.Configuration.Instance.ui == true)
            {
                Logger.LogWarning("ESTA INSTALADO EL MOD Y EL UI FUNCIONA CORRECTAMENTE");
            }
            else if (!Provider.getServerWorkshopFileIDs().Contains(2387083742) & Instance.Configuration.Instance.ui == true)
            {
                Logger.LogError("NO ESTA INSTALADO EL MOD: 2387083742");
            }


            Logger.LogWarning("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");
            Logger.Log("Plugin Creado Por @Margarita#8172", ConsoleColor.Green);
            Logger.Log($"Version Del Plugin: {Assembly.GetName().Version}", ConsoleColor.Cyan);
            Logger.Log($"Nombre Del Plugin: {Assembly.GetName().Name}", ConsoleColor.Cyan);
            Logger.LogWarning("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-"); System.Threading.Thread.Sleep(500);

 
                Reloj.AutoReset = false;
                UnturnedPlayerEvents.OnPlayerChatted += PlayerChatted;
                U.Events.OnPlayerConnected += PlayerConnected;
                U.Events.OnPlayerDisconnected += PlayerDisconnected;
                Logger.Log("El Plugin Ha Cargado Correctamente", ConsoleColor.Yellow);

               
            


        }


        public void apagadoforzoso()
        {
            Instance.Unload();
            Instance.UnloadPlugin();
            base.Unload();
            base.UnloadPlugin();
            Provider.shutdown();
        }
        private void Reloj_Elapsed(object sender, ElapsedEventArgs e)
        {
            TaskDispatcher.QueueOnMainThread(() =>
            {
                Comprobar();
                Reloj.Stop();
            });
        }

        public void Comprobar()
        {
            if (activo)
            {
                ChatManager.serverSendMessage(Instance.Translate("finalizo_sin_tiempo").Replace('(', '<').Replace(')', '>'), Color.white, null, null, EChatMode.SAY, Instance.Configuration.Instance.icon, true);
                limpieza(); return;
            }
        }

        #region joinyleave
        private void PlayerDisconnected(UnturnedPlayer player)
        {
            personas.Remove(player);
        }

        private void PlayerConnected(UnturnedPlayer player)
        {
            personas.Add(player);
        }
        #endregion

        protected override void Unload()
        {
            activo = false;
            contador = 0;
            limpieza();
            tipovotacion = "";
            UnturnedPlayerEvents.OnPlayerChatted -= PlayerChatted;
            U.Events.OnPlayerConnected -= PlayerConnected;
            U.Events.OnPlayerDisconnected -= PlayerDisconnected;


            // BORRE DATOS GUARDADOS
            try
            {
                UnturnedPlayer p = UnturnedPlayer.FromPlayer(Player.player);
                Savedata valor = p.GetComponent<Savedata>();
                valor.ya_esta_activo = false;
            }
            catch (Exception)
            {

            }
            


            // LLAMAR MENSAJES

            // ChatManager.serverSendMessage(Class1.Instance.Translate("winner", player.CharacterName, "3 ROCKET", tipollave).Replace('(', '<').Replace(')', '>'), Color.white, null, null, EChatMode.SAY, Class1.Instance.Configuration.Instance.icon, true);

        }

        private void PlayerChatted(UnturnedPlayer player, ref Color color, string message, EChatMode chatMode, ref bool cancel)
        {
            if (!activo) { return; }


            switch (tipovotacion)
            {
                case "DIA":
                    margaritadios = 1;
                    break;
                case "LLUVIA":
                    margaritadios = 2;
                    break;
            }
            Savedata valor = player.GetComponent<Savedata>();
            message.ToLower();

            if (message.StartsWith("dia") & margaritadios == 1)
            {
                if (valor.ya_esta_activo)
                {
                    cancel = true;
                    SteamPlayer jugador; jugador = PlayerTool.getSteamPlayer(player.CSteamID);
                    ChatManager.serverSendMessage(Instance.Translate("ya_votaste").Replace('(', '<').Replace(')', '>'), Color.white, null, jugador, EChatMode.SAY, Instance.Configuration.Instance.icon, true); return;
                }

                contador = Configuration.Instance.votos_minimos; contador--;

                if (votos == contador || votos >= contador)
                {
                    foreach (UnturnedPlayer item in personas)
                    {
                        Savedata todos = item.GetComponent<Savedata>();
                        todos.ya_esta_activo = false;
                        EffectManager.askEffectClearByID(12760, todos.Player.CSteamID);
                    }

                    R.Commands.Execute(new ConsolePlayer(), string.Join(" ", "day"));


                    ChatManager.serverSendMessage(Instance.Translate("finalizo_votacion").Replace('(', '<').Replace(')', '>'), Color.white, null, null, EChatMode.SAY, Instance.Configuration.Instance.icon, true); limpieza();  return;
                }
                else
                {
                    valor.ya_esta_activo = true;
                    cancel = true;
                    votos++;

                    if (Instance.Configuration.Instance.ui)
                    {
                        EffectManager.sendUIEffect(12761, 12760, false, "[" + votos + "/" + Instance.Configuration.Instance.votos_minimos + "]"); 
                    }

                    ChatManager.serverSendMessage(Instance.Translate("votacion_count", Instance.tipovotacion, Instance.texto, Instance.votos, Instance.Configuration.Instance.votos_minimos).Replace('(', '<').Replace(')', '>'), Color.white, null, null, EChatMode.SAY, Instance.Configuration.Instance.icon, true);  return;

                }

            }
            else if(message.StartsWith("lluvia") & margaritadios == 2)
            {
                if (valor.ya_esta_activo)
                {
                    cancel = true;
                    SteamPlayer jugador; jugador = PlayerTool.getSteamPlayer(player.CSteamID);
                    ChatManager.serverSendMessage(Instance.Translate("ya_votaste").Replace('(', '<').Replace(')', '>'), Color.white, null, jugador, EChatMode.SAY, Instance.Configuration.Instance.icon, true); return;
                }

                contador = Configuration.Instance.votos_minimos; contador--;

                if (votos == contador || votos >= contador)
                {
                    foreach (UnturnedPlayer item in personas)
                    {
                        Savedata todos = item.GetComponent<Savedata>();
                        todos.ya_esta_activo = false;
                        EffectManager.askEffectClearByID(12760, todos.Player.CSteamID);
                    }

                    R.Commands.Execute(new ConsolePlayer(), string.Join(" ", "weather none"));
                    ChatManager.serverSendMessage(Instance.Translate("finalizo_votacion").Replace('(', '<').Replace(')', '>'), Color.white, null, null, EChatMode.SAY, Instance.Configuration.Instance.icon, true); limpieza();  return;
                }
                else
                {
                    valor.ya_esta_activo = true;
                    cancel = true;
                    votos++;

                    if (Instance.Configuration.Instance.ui)
                    {
                        EffectManager.sendUIEffect(12762, 12760, false, "[" + votos + "/" + Instance.Configuration.Instance.votos_minimos + "]");
                    }
                    ChatManager.serverSendMessage(Instance.Translate("votacion_count", Instance.tipovotacion, Instance.texto, Instance.votos, Instance.Configuration.Instance.votos_minimos).Replace('(', '<').Replace(')', '>'), Color.white, null, null, EChatMode.SAY, Instance.Configuration.Instance.icon, true); return;

                }
            }

        }

        public void LlamarAdmin(UnturnedPlayer caller)
        {
            Savedata test = caller.GetComponent<Savedata>();
            test.ya_esta_activo = false; return;
        }

        public void limpieza()
        {
            foreach (UnturnedPlayer item in personas)
            {
                Savedata todos = item.GetComponent<Savedata>();
                todos.ya_esta_activo = false;

                if (Instance.Configuration.Instance.ui)
                {
                    EffectManager.askEffectClearByID(12760, todos.Player.CSteamID);
                }
            }

            votos = 0;
            contador = Configuration.Instance.votos_minimos;
            activo = false;
            Reloj.Stop();
            texto = string.Empty;
            margaritadios = 0;
            tipovotacion = string.Empty;
    }


        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList
                {

                    {"votacion_count", "(size=15)(color=#F70D1A)「TIO CLIMA」(/color)(color=#5EFB6E)ESCRIBAN EN EL CHAT(/color) (size=20){0}(/size) (color=#5EFB6E)PARA HACER {1}(/color) [{2}/{3}]"},
                    {"ya_votaste", "(color=red)Ya has votado. Espera a que termine la votacion!(/color)" },
                    {"finalizo_votacion", "(size=15)(color=#F70D1A)「TIO CLIMA」(/color) (color=#5EFB6E)HA FINALIZADO LA VOTACION. GRACIAS POR PARTICIPAR(/color)" },
                    {"finalizo_sin_tiempo", "(size=15)(color=#F70D1A)「TIO CLIMA」(/color) (color=red)LA VOTACION HA FALLADO. NO HAN VOTADO A TIEMPO(/color)" }
                };
            }

        }
    }
}