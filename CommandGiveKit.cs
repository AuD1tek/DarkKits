using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DarkKits.Commands
{
    public class CommandGiveKit : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "givekit";
        public string Help => "";
        public string Syntax => "/givekit (kit_name) (player) [amount]";
        public List<string> Aliases => new List<string>() { "givek", "kitg" };
        public List<string> Permissions => new List<string>() { "dark.givekit" };


        public void Execute(IRocketPlayer caller, string[] command) // /kitc (kitname) (player/self) [amount]
        {
            bool callerIsPlayer = caller is UnturnedPlayer;
            UnturnedPlayer player = callerIsPlayer ? (UnturnedPlayer)caller : null;

            if (command.Length > 3)
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
                string text = DarkKits.Instance.Translate("command_givekit_syntax", Syntax);
                if (callerIsPlayer)
                {
                    UnturnedChat.Say(player, DarkKits.Instance.Translate("command_givekit_syntax", Syntax), Color.yellow);
                }
                else
                {
                    Rocket.Core.Logging.Logger.Log(text, ConsoleColor.Yellow);
                }
                return;
            }

            string nameKit = command[0];
            UnturnedPlayer toPlayer = player;
            uint amount = 0;

            if (command.Length >= 2 && command[1].ToLower() != "self")
            {
                if (callerIsPlayer && !player.HasPermission("dark.givekit.other") && !player.IsAdmin)
                {
                    UnturnedChat.Say(player, DarkKits.Instance.Translate("command_givekit_no_permissions_other"), Color.red);
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
            if (!DarkKits.IsKitExists(nameKit))
            {
                string text = DarkKits.Instance.Translate("command_givekit_not_found", nameKit);
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
            if (command.Length == 3 && !uint.TryParse(command[2], out amount))
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
            if (amount == 0)
            {
                if ((amount = DarkKits.DefaultGiveKitAmount) == 0)
                {
                    string text = DarkKits.Instance.Translate("command_givekit_invalide_amount");
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

            string key = toPlayer.CSteamID.ToString() + nameKit;
            TemporaryOwner owner;

            if (DarkKits.CanUseTemporaryKit(key, out owner))
            {
                owner.Amount += amount;
                DarkKits.Instance.Configuration.Save();

                string text = DarkKits.Instance.Translate("command_givekit_successful_add", nameKit, amount, toPlayer.DisplayName, owner.Amount);
                if (callerIsPlayer)
                {
                    UnturnedChat.Say(player, text, Color.green);
                }
                else
                {
                    Rocket.Core.Logging.Logger.Log(text, ConsoleColor.Green);
                }
                string fromConsole = DarkKits.Instance.Translate("command_givekit_console_name");
                UnturnedChat.Say(toPlayer, DarkKits.Instance.Translate("command_givekit_successful_add_toplayer", nameKit, amount, callerIsPlayer ? player.DisplayName : fromConsole), Color.green);
            }
            else
            {
                owner = new TemporaryOwner(key, amount);
                DarkKits.Instance.Configuration.Instance.TemporaryOwners.Add(owner);
                DarkKits.Instance.Configuration.Save();

                string text = DarkKits.Instance.Translate("command_givekit_successful_new", nameKit, toPlayer.DisplayName, amount);
                if (callerIsPlayer)
                {
                    UnturnedChat.Say(player, text, Color.green);
                }
                else
                {
                    Rocket.Core.Logging.Logger.Log(text, ConsoleColor.Green);
                }
                string fromConsole = DarkKits.Instance.Translate("command_givekit_console_name");
                UnturnedChat.Say(toPlayer, DarkKits.Instance.Translate("command_givekit_successful_new_toplayer", nameKit, amount, callerIsPlayer ? player.DisplayName : fromConsole), Color.green);
            }
        }
    }
}
