using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DarkKits.Commands
{
    public class CommandCreateKit : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "createkit";
        public string Help => "";
        public string Syntax => "/ckit (kit_name) (cooldown) [XP] [vehicle_id]";
        public List<string> Aliases => new List<string>() { "ckit" };
        public List<string> Permissions => new List<string>() { "dark.createkit" };


        public void Execute(IRocketPlayer caller, string[] command) // /ckit (name) (cooldown) [XP] [vehicle_id]
        {
            try
            {
                UnturnedPlayer player = (UnturnedPlayer)caller;
                if (command.Length > 4)
                {
                    UnturnedChat.Say(player, DarkKits.Instance.Translate("command_invalide"), Color.red);
                    return;
                }
                else if (command.Length == 0 || command.Length == 1)
                {
                    UnturnedChat.Say(player, DarkKits.Instance.Translate("command_createkit_syntax", Syntax), Color.yellow);
                    return;
                }

                string nameKit = command[0];
                int cooldown = 0;
                uint xp = 0;
                ushort vehicle = 0;


                if (DarkKits.IsKitExists(nameKit))
                {
                    UnturnedChat.Say(player, DarkKits.Instance.Translate("command_createkit_name_exist", nameKit), Color.red);
                    return;
                }
                if (!int.TryParse(command[1], out cooldown))
                {
                    UnturnedChat.Say(player, DarkKits.Instance.Translate("command_invalide"), Color.red);
                    return;
                }
                if (command.Length > 2 && !uint.TryParse(command[2], out xp))
                {
                    UnturnedChat.Say(player, DarkKits.Instance.Translate("command_invalide"), Color.red);
                    return;
                }
                if (command.Length > 3 && !ushort.TryParse(command[3], out vehicle))
                {
                    UnturnedChat.Say(player, DarkKits.Instance.Translate("command_invalide"), Color.red);
                    return;
                }


                List<KitItem> kitItems = new List<KitItem>();
                if (DarkKits.GetPlayerKitItems(player.Player, kitItems))
                {
                    DarkKits.Instance.Configuration.Instance.Kits.Add(new Kit()
                    {
                        Name = nameKit,
                        Cooldown = cooldown,
                        XP = xp == 0 ? new uint?(0) : xp,
                        Vehicle = vehicle == 0 ? new ushort?(0) : vehicle,
                        Items = kitItems
                    });
                    DarkKits.Instance.Configuration.Save();
                    UnturnedChat.Say(player, DarkKits.Instance.Translate("command_createkit_successful", nameKit, cooldown, xp, vehicle), Color.yellow);
                }
                else
                {
                    UnturnedChat.Say(player, DarkKits.Instance.Translate("command_createkit_failed"), Color.red);
                    return;
                }
            }
            catch (Exception ex)
            {
                UnturnedChat.Say(caller, DarkKits.Instance.Translate("command_exception"), Color.red);
                Console.Write(ex);
            }
        }
    }
}
