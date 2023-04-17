using System;
using API.Commands;
using API.Hooks;
using Carbon.Extensions;
using Facepunch.Extend;

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

			public static bool Prefix(ConsoleSystem.Option options, string strCommand, object[] args)
			{
				if (Community.Runtime == null) return true;

				try
				{
					var split = strCommand.Split(ConsoleArgEx.CommandSpacing, StringSplitOptions.RemoveEmptyEntries);
					var command = split.Length == 0 ? string.Empty : split[0].Trim();
					var args2 = split.Length > 1 ? strCommand.Substring(command.Length + 1).SplitQuotesStrings() : EmptyArgs;
					Array.Clear(split, 0, split.Length);

					if (Community.Runtime.Config.oCommandChecks && command.StartsWith("o.") || command.StartsWith("oxide."))
					{
						Logger.Warn($"Oxide commands (o.* or oxide.*) don't work in Carbon. Please use 'c.find c.' to list all available Carbon commands.");
						return false;
					}

					var player = options.Connection?.player as BasePlayer;

					var commandArgs = Facepunch.Pool.Get<PlayerArgs>();
					commandArgs.Arguments = args2;
					commandArgs.Player = player;

					if (Community.Runtime.CommandManager.Contains(Community.Runtime.CommandManager.Console, command, out var cmd))
					{
						Command.FromRcon = false;
						Community.Runtime.CommandManager.Execute(cmd, commandArgs);
						Facepunch.Pool.Free(ref commandArgs);
						return false;
					}
				}
				catch (Exception exception) { Logger.Error($"Failed ConsoleSystem.Run [{strCommand}] [{string.Join(" ", args)}]", exception); }

				return true;
			}
		}
	}
}
