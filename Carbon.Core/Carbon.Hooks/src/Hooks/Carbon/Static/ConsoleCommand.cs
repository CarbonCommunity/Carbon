using System;
using Carbon.Extensions;
using Facepunch.Extend;
using Oxide.Core;
using Oxide.Plugins;

/*
 *
 * Copyright (c) 2022 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;

public partial class Category_Static
{
	public partial class Static_ConsoleSystem
	{
		/*
		[CarbonHook.AlwaysPatched]
		[CarbonHook("OnCarbonCommand"), CarbonHook.Category(Hook.Category.Enum.Core)]
		[CarbonHook.Parameter("player", typeof(BasePlayer))]
		[CarbonHook.Parameter("message", typeof(string))]
		[CarbonHook.Info("Called whenever a Carbon server command is called.")]
		[CarbonHook.Patch(typeof(ConsoleSystem), "Run")]
		*/

		public class Static_ConsoleSystem_Run_4be71c5d077949cdb88438ec6dabac24
		{
			public static Metadata metadata = new Metadata("OnCarbonCommand",
				typeof(ConsoleSystem), "Run", new System.Type[] { typeof(ConsoleSystem.Option), typeof(string), typeof(object[]) });

			static Static_ConsoleSystem_Run_4be71c5d077949cdb88438ec6dabac24()
			{
				metadata.SetIdentifier("4be71c5d077949cdb88438ec6dabac24");
				metadata.SetAlwaysPatch(true);
			}

			internal static string[] EmptyArgs = new string[0];

			public static bool Prefix(ConsoleSystem.Option options, string strCommand, object[] args)
			{
				if (Community.Runtime == null) return true;

				try
				{
					var split = strCommand.Split(ConsoleArgEx.CommandSpacing, StringSplitOptions.RemoveEmptyEntries);
					var command = split[0].Trim();
					var args2 = split.Length > 1 ? strCommand.Substring(command.Length + 1).SplitQuotesStrings() : EmptyArgs;
					Facepunch.Pool.Free(ref split);

					var player = options.Connection?.player as BasePlayer;

					foreach (var cmd in Community.Runtime.AllConsoleCommands)
					{
						if (cmd.Command == command)
						{
							if (player != null)
							{
								if (cmd.Permissions != null)
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
										player?.ConsoleMessage($"You don't have any of the required permissions to run this command.");
										continue;
									}
								}

								if (cmd.Groups != null)
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
										player?.ConsoleMessage($"You aren't in any of the required groups to run this command.");
										continue;
									}
								}

								if (cmd.AuthLevel != -1)
								{
									var hasAuth = !ServerUsers.users.ContainsKey(player.userID) ? player.Connection.authLevel >= cmd.AuthLevel : (int)ServerUsers.Get(player.userID).group >= cmd.AuthLevel;

									if (!hasAuth)
									{
										player?.ConsoleMessage($"You don't have the minimum auth level required to execute this command.");
										continue;
									}
								}
							}

							try
							{
								global::Command._fromRcon = false;
								cmd.Callback?.Invoke(player, command, args2);
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

		/*
		[CarbonHook.AlwaysPatched]
		[Hook("OnServerCommand"), Hook.Category(Hook.Category.Enum.Server)]
		[Hook.Parameter("arg", typeof(ConsoleSystem.Arg))]
		[Hook.Info("Useful for intercepting commands before they get to their intended target.")]
		[Hook.Patch(typeof(ConsoleSystem), "Internal", true)]
		*/

		public class Static_ConsoleSystem_ServerConsoleCommand
		{
			public static Metadata metadata = new Metadata("OnServerCommand",
				typeof(ConsoleSystem), "Internal", new System.Type[] { typeof(ConsoleSystem.Arg) });

			static Static_ConsoleSystem_ServerConsoleCommand()
			{
				metadata.SetAlwaysPatch(true);
			}

			public static bool Prefix(ConsoleSystem.Arg arg)
			{
				if (arg.Invalid)
				{
					return false;
				}

				return Interface.CallHook("OnServerCommand", arg) == null;
			}
		}
	}
}