using System;
using System.Runtime.Serialization;
using API.Commands;
using API.Hooks;
using Carbon.Extensions;
using Facepunch.Extend;
using static ConsoleSystem;
using Command = API.Commands.Command;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;
#pragma warning disable IDE0051
#pragma warning disable IDE0060

public partial class Category_Static
{
	public partial class Static_ConsoleSystem
	{
		[HookAttribute.Patch("OnConsoleCommand", "OnConsoleCommand", typeof(ConsoleSystem), "Run", new System.Type[] { typeof(ConsoleSystem.Option), typeof(string), typeof(object[]) })]
		[HookAttribute.Identifier("4be71c5d077949cdb88438ec6dabac24")]
		[HookAttribute.Options(HookFlags.Static | HookFlags.IgnoreChecksum)]

		// Called whenever a Carbon server command is called.

		public class Static_ConsoleSystem_4be71c5d077949cdb88438ec6dabac24 : Patch
		{
			internal static string[] EmptyArgs = new string[0];
			internal const string Space = " ";

			public static bool Prefix(ConsoleSystem.Option options, string strCommand, object[] args)
			{
				if (Community.Runtime == null) return true;

				try
				{
					var split = strCommand.Split(ConsoleArgEx.CommandSpacing, StringSplitOptions.RemoveEmptyEntries);
					var command = split.Length == 0 ? string.Empty : split[0].Trim();
					var args2 = split.Length > 1 ? strCommand.Substring(command.Length + 1).SplitQuotesStrings() : EmptyArgs;
					var fullString = split.ToString(Space);
					Array.Clear(split, 0, split.Length);

					if (Community.Runtime.Config.oCommandChecks && command.StartsWith("o.") || command.StartsWith("oxide."))
					{
						Logger.Warn($"Oxide commands (o.* or oxide.*) don't work in Carbon. Please use 'c.find c.' to list all available Carbon commands.");
						return false;
					}

					if (!Command.FromRcon)
					{
						var player = options.Connection?.player as BasePlayer;
						var commands = player == null ? Community.Runtime.CommandManager.RCon : Community.Runtime.CommandManager.ClientConsole;

						if (Community.Runtime.CommandManager.Contains(commands, command, out var cmd))
						{
							var arg = FormatterServices.GetUninitializedObject(typeof(Arg)) as Arg;
							arg.Option = options;
							arg.FullString = fullString;
							arg.Args = args2;
							arg.cmd = cmd.RustCommand;

							var commandArgs = Facepunch.Pool.Get<PlayerArgs>();
							commandArgs.Token = arg;
							commandArgs.Type = cmd.Type;
							commandArgs.Arguments = args2;
							commandArgs.Player = player;
							commandArgs.IsServer = player == null;
							commandArgs.PrintOutput = options.PrintOutput || player != null;

							Command.FromRcon = false;
							Community.Runtime.CommandManager.Execute(cmd, commandArgs);

							commandArgs.Dispose();
							Facepunch.Pool.Free(ref commandArgs);
							arg = null;

							return false;
						}
					}
				}
				catch (Exception exception) { Logger.Error($"Failed ConsoleSystem.Run [{strCommand}] [{string.Join(" ", args)}]", exception); }

				return true;
			}
		}
	}
}
