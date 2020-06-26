using Rocket.API;
using Rocket.Core;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DarkKits.Commands
{
    public class CommandKitHelp : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "kithelp";
        public string Help => "";
        public string Syntax => "/kithelp";
        public List<string> Aliases => new List<string>() { "kithelp" };
        public List<string> Permissions => new List<string>() { "dark.kithelp" };


        public void Execute(IRocketPlayer caller, string[] command) // /kithelp
        {
            bool callerIsPlayer = caller is UnturnedPlayer;
            UnturnedPlayer player = callerIsPlayer ? (UnturnedPlayer)caller : null;

            if (command.Length != 0)
            {
                string text = DarkKits.Instance.Translate("command_kithelp_syntax", Syntax);
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
            
            List<string> info = new List<string>();
            
            if (callerIsPlayer)
            {
                if (!player.HasPermission("dark.command.kithelp.*"))
                {
                    GetInfo(player, info, true);
                }
                else
                {
                    GetInfo(player, info, false);
                }
            }
            else
            {
                GetInfo(caller, info, true);
            }

            foreach (string str in info)
            {
                string text = DarkKits.Instance.Translate("command_kithelp_info_entry", str);
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

        public static void GetInfo(IRocketPlayer player, List<string> info, bool allInfo)
        {
            if (info == null) { info = new List<string>(); }
            string syntax = string.Empty;
            if (player?.HasPermission("dark.kithelp.kit") == true || allInfo)
            {
                syntax = DarkKits.Instance.Translate("command_kit_syntax", R.Commands.GetCommand("kit").Syntax);
                info.Add(DarkKits.Instance.Translate("command_kithelp_info_command_kit", syntax));
            }
            if (player?.HasPermission("dark.kithelp.kits") == true || allInfo)
            {
                info.Add(DarkKits.Instance.Translate("command_kithelp_info_command_kits"));
            }
            if (player?.HasPermission("dark.kithelp.createkit") == true || allInfo)
            {
                syntax = DarkKits.Instance.Translate("command_createkit_syntax", R.Commands.GetCommand("createkit").Syntax);
                info.Add(DarkKits.Instance.Translate("command_kithelp_info_command_createkit", syntax));
            }
            if (player?.HasPermission("dark.kithelp.givekit") == true || allInfo)
            {
                syntax = DarkKits.Instance.Translate("command_givekit_syntax", R.Commands.GetCommand("givekit").Syntax);
                info.Add(DarkKits.Instance.Translate("command_kithelp_info_command_givekit", syntax));
            }
            if (player?.HasPermission("dark.kithelp.kitclearkd") == true || allInfo)
            {
                syntax = DarkKits.Instance.Translate("command_kitclearkd_syntax", R.Commands.GetCommand("kitclearkd").Syntax);
                info.Add(DarkKits.Instance.Translate("command_kithelp_info_command_kitclearkd", syntax));
            }
        }
    }
}
