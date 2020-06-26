using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DarkKits.Commands
{
    public class CommandKit : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "kit";
        public string Help => "";
        public string Syntax => "/kit (kit_name)";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string>() { "dark.kit" };

        public void Execute(IRocketPlayer caller, string[] command) // /kit (name)
        {
            try
            {
                UnturnedPlayer player = (UnturnedPlayer)caller;

                if (command.Length != 1)
                {
                    UnturnedChat.Say(player, DarkKits.Instance.Translate("command_kit_syntax", Syntax), Color.yellow);
                    return;
                }

                Kit kit = null;

                if (!DarkKits.TryGetKit(command[0], out kit))
                {
                    UnturnedChat.Say(player, DarkKits.Instance.Translate("command_kit_not_found", command[0]), Color.red);
                    return;
                }
                string key = player.CSteamID.ToString() + kit.Name;
                TemporaryOwner temporaryOwner = null;
                if (!(player.HasPermission("kit.*") | player.HasPermission("kit." + kit.Name.ToLower())))
                {
                    if (!DarkKits.CanUseTemporaryKit(key, out temporaryOwner))
                    {
                        UnturnedChat.Say(player, DarkKits.Instance.Translate("command_kit_no_permissions"), Color.red);
                        return;
                    }
                }
                if (DarkKits.IsKitCooldown(key, out CooldownItem cooldown))
                {
                    string stringCooldown = DarkKits.GetTimeFromCooldown(cooldown);
                    UnturnedChat.Say(player, DarkKits.Instance.Translate("command_kit_cooldown", stringCooldown));
                    return;
                }
                DarkKits.UseTemporaryKit(temporaryOwner);

                if (DarkKits.GivePlayerKit(player, kit))
                {
                    DarkKits.Instance.Configuration.Instance.Cooldowns.Add(new CooldownItem(key, kit.Cooldown));
                    DarkKits.Instance.Configuration.Save();

                    if (temporaryOwner != null)
                    {
                        UnturnedChat.Say(player, DarkKits.Instance.Translate("command_kit_successful_temporary", kit.Name, temporaryOwner.Amount), Color.yellow);
                    }
                    else
                    {
                        UnturnedChat.Say(player, DarkKits.Instance.Translate("command_kit_successful", kit.Name), Color.yellow);
                    }
                }
                else
                {
                    UnturnedChat.Say(player, DarkKits.Instance.Translate("command_exception"), Color.red);
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
