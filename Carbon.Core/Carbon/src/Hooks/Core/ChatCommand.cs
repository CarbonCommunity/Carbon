///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using Carbon.Extensions;
using ConVar;
using Facepunch.Extend;
using Oxide.Core;
using Oxide.Plugins;

namespace Carbon.Hooks
{
	[Hook.AlwaysPatched]
	[Hook("OnPlayerCommand"), Hook.Category(Hook.Category.Enum.Player)]
	[Hook.Parameter("player", typeof(BasePlayer))]
	[Hook.Parameter("message", typeof(string))]
	[Hook.Info("Useful for intercepting players' commands before their handling.")]
	[Hook.Patch(typeof(Chat), "sayAs")]
	public class Chat_SayAs
	{
		internal static string[] EmptyArgs = new string[0];

		public static bool Prefix(Chat.ChatChannel targetChannel, ulong userId, string username, string message, BasePlayer player = null)
		{
			if (Community.Runtime == null) return true;

			try
			{
				var fullString = message.Substring(1);
				var split = fullString.Split(ConsoleArgEx.CommandSpacing, StringSplitOptions.RemoveEmptyEntries);
				var command = split[0].Trim();
				var args = split.Length > 1 ? fullString.Substring(command.Length + 1).SplitQuotesStrings() : EmptyArgs;
				Facepunch.Pool.Free(ref split);

				if (Interface.CallHook("OnPlayerCommand", BasePlayer.FindByID(userId), command, args) != null)
				{
					return false;
				}

				foreach (var cmd in Community.Runtime?.AllChatCommands)
				{
					if (cmd.Command == command)
					{
						if (cmd.Permissions != null && player != null)
						{
							var hasPerm = false;
							foreach (var permission in cmd.Permissions)
							{
								if (cmd.Plugin is RustPlugin rust && rust.permission.UserHasPermission(player.UserIDString, permission))
								{
									hasPerm = true;
									break;
								}
							}

							if (!hasPerm)
							{
								player?.ChatMessage($"You don't have any of the required permissions to run this command.");
								continue;
							}
						}

						if (cmd.Groups != null && player != null)
						{
							var hasGroup = false;
							foreach (var group in cmd.Groups)
							{
								if (cmd.Plugin is RustPlugin rust && rust.permission.UserHasGroup(player.UserIDString, group))
								{
									hasGroup = true;
									break;
								}
							}

							if (!hasGroup)
							{
								player?.ChatMessage($"You aren't in any of the required groups to run this command.");
								continue;
							}
						}

						try
						{
							cmd.Callback?.Invoke(player, command, args);
						}
						catch (Exception ex)
						{
							Carbon.Logger.Error("ConsoleSystem_Run", ex);
						}

						return false;
					}
				}
			}
			catch { }

			return true;
		}
	}
}
