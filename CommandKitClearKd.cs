using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DarkKits.Commands
{
    public class CommandKitClearKd : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "kitclearkd";
        public string Help => "";
        public string Syntax => "/kitc (kit_name/all) (player/self)";
        public List<string> Aliases => new List<string>() { "kitc" };
        public List<string> Permissions => new List<string>() { "dark.kitclearkd" };


        public void Execute(IRocketPlayer caller, string[] command) // /kitc (kitname) [playerName]
        {
            bool callerIsPlayer = caller is UnturnedPlayer;
            UnturnedPlayer player = callerIsPlayer ? (UnturnedPlayer)caller : null;

            if (command.Length > 2)
            {
                string text = DarkKits.Instance.Translate("command_invalide");
                if (callerIsPlayer)
                {
                    UnturnedChat.Say(player, text, Color.red);
                }
                else
                {
                    Rocket.Core.Logging.Logger.Log(text, ConsoleColor.Red);
                }
                return;
            }
            if (command.Length == 0 || command.Length == 1)
            {
                string text = DarkKits.Instance.Translate("command_kitclearkd_syntax", Syntax);
                if (callerIsPlayer)
                {
                    UnturnedChat.Say(player, text, Color.yellow);
                }
                else
                {
                    Rocket.Core.Logging.Logger.Log(text, ConsoleColor.Yellow);
                }
                return;
            }


            string nameKit = command[0];
            UnturnedPlayer toPlayer = player;

            if (command.Length == 2 && command[1].ToLower() != "self")
            {
                if (callerIsPlayer && !player.HasPermission("dark.kitclearkd.other") && !player.IsAdmin)
                {
                    UnturnedChat.Say(player, DarkKits.Instance.Translate("command_kitclearkd_no_permissions_other"), Color.red);
                    return;
                }
                toPlayer = UnturnedPlayer.FromName(command[1]);
            }
            if (toPlayer == null)
            {
                string text = DarkKits.Instance.Translate("command_player_not_found", command[1]);
                if (callerIsPlayer)
                {
                    UnturnedChat.Say(player, text, Color.red);
                }
                else
                {
                    Rocket.Core.Logging.Logger.Log(text, ConsoleColor.Red);
                }
                return;
            }
            if (callerIsPlayer)
            {
                if (nameKit.ToLower() != "all" && (!player.HasPermission("dark.kitclearkd.all") && !player.IsAdmin))
                {
                    if (!DarkKits.IsKitExists(nameKit))
                    {
                        string text = DarkKits.Instance.Translate("command_kitclearkd_not_found", nameKit);
                        if (callerIsPlayer)
                        {
                            UnturnedChat.Say(player, text, Color.red);
                        }
                        else
                        {
                            Rocket.Core.Logging.Logger.Log(text, ConsoleColor.Red);
                        }
                        return;
                    }
                }
            }
            else
            {
                if (nameKit.ToLower() != "all")
                {
                    if (!DarkKits.IsKitExists(nameKit))
                    {
                        string text = DarkKits.Instance.Translate("command_kitclearkd_not_found", nameKit);
                        if (callerIsPlayer)
                        {
                            UnturnedChat.Say(player, text, Color.red);
                        }
                        else
                        {
                            Rocket.Core.Logging.Logger.Log(text, ConsoleColor.Red);
                        }
                        return;
                    }
                }
            }

            string key = toPlayer.CSteamID.ToString() + nameKit;
            List<(string, string)> keys = new List<(string, string)>();
            bool typeAll = false;

            if (nameKit.ToLower() == "all")
            {
                typeAll = true;
                List<string> kits = new List<string>();
                DarkKits.GetPlayerKits(player, null, null, null, kits);
                if (kits.Count > 0)
                {
                    foreach (string kit in kits)
                    {
                        keys.Add((toPlayer.CSteamID.ToString(), kit));
                    }
                }
            }

            if (typeAll)
            {
                if (keys.Count > 0)
                {
                    List<string> clearedKits = new List<string>();

                    foreach ((string, string) item in keys)
                    {
                        string k = item.Item1 + item.Item2;
                        if (DarkKits.IsKitCooldown(k, out CooldownItem cooldown))
                        {
                            DarkKits.Instance.Configuration.Instance.Cooldowns.Remove(cooldown);
                            DarkKits.Instance.Configuration.Save();

                            string stringCooldown = DarkKits.GetTimeFromCooldown(cooldown);
                            string entry = DarkKits.Instance.Translate("command_kitclearkd_cleared_entry", item.Item2, stringCooldown);

                            clearedKits.Add(entry);
                        }
                    }

                    string separator = DarkKits.Instance.Translate("command_kitclearkd_separator");
                    string text = DarkKits.Instance.Translate("command_kitclearkd_cleared", string.Join(separator, clearedKits));

                    if (callerIsPlayer)
                    {
                        UnturnedChat.Say(player, text, Color.green);
                    }
                    else
                    {
                        Rocket.Core.Logging.Logger.Log(text, ConsoleColor.Green);
                    }

                }
            }
            else
            {
                if (DarkKits.IsKitCooldown(key, out CooldownItem cooldown))
                {
                    DarkKits.Instance.Configuration.Instance.Cooldowns.Remove(cooldown);
                    DarkKits.Instance.Configuration.Save();

                    string stringCooldown = DarkKits.GetTimeFromCooldown(cooldown);
                    string text = DarkKits.Instance.Translate("command_kitclearkd_ready", nameKit, stringCooldown);
                    if (callerIsPlayer)
                    {
                        UnturnedChat.Say(player, text, Color.green);
                    }
                    else
                    {
                        Rocket.Core.Logging.Logger.Log(text, ConsoleColor.Green);
                    }
                }
                else
                {
                    string text = DarkKits.Instance.Translate("command_kitclearkd_no_cooldown", nameKit);
                    if (callerIsPlayer)
                    {
                        UnturnedChat.Say(player, text, Color.green);
                    }
                    else
                    {
                        Rocket.Core.Logging.Logger.Log(text, ConsoleColor.Green);
                    }
                }
            }
        }
    }
}
