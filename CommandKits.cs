using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using UnityEngine;

namespace DarkKits.Commands
{
    public class CommandKits : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "kits";
        public string Help => "";
        public string Syntax => "/kits";
        public List<string> Aliases => new List<string>() { "ks" };
        public List<string> Permissions => new List<string>() { "dark.kits" };


        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            List<string> kitsCooldown = new List<string>();
            List<string> kitsReady = new List<string>();

            if (DarkKits.GetPlayerKits(player, kitsReady, kitsCooldown))
            {
                string separator = DarkKits.Instance.Translate("command_kits_kits_separator");
                if (kitsCooldown?.Count > 0)
                {
                    UnturnedChat.Say(player, DarkKits.Instance.Translate("command_kits_kits_cooldown", string.Join(separator, kitsCooldown)), Color.yellow);
                }
                if (kitsReady?.Count > 0)
                {
                    UnturnedChat.Say(player, DarkKits.Instance.Translate("command_kits_kits_ready", string.Join(separator, kitsReady)), Color.green);
                }
            }
            else
            {
                UnturnedChat.Say(player, DarkKits.Instance.Translate("command_kits_not_kits"), Color.magenta);
            }
        }
    }
}
