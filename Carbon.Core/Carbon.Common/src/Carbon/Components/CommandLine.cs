/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

using API.Commands;
using static API.Commands.Command;

namespace Carbon.Components;

public static class CommandLine
{
	internal static readonly string[] _emptyArgs = new string[0];
	internal static readonly char[] _delimiter = new char[] { '|' };

	public static void ExecuteCommands(string @switch, string context, string[] lines = null)
	{
		var arg = CommandLineEx.GetArgumentResult(lines ?? Environment.GetCommandLineArgs(), @switch, string.Empty);
		var commands = arg.Split(_delimiter, StringSplitOptions.RemoveEmptyEntries);

		if (commands.Length > 0)
		{
			Logger.Log($" Executing {commands.Length:n0} {commands.Length.Plural("command", "commands")} for the '{@switch}' switch ({context}):");
		}

		ExecuteCommands(commands);

		Array.Clear(commands, 0, commands.Length);
		commands = null;
	}

	public static void ExecuteCommands(string[] commands)
	{
		foreach (var command in commands)
		{
			if (string.IsNullOrEmpty(command))
			{
				continue;
			}

			var split = command.Split(' ');
			var name = split[0];

			if (Community.Runtime.CommandManager.Contains(Community.Runtime.CommandManager.RCon, name, out var cmd))
			{
				try
				{
					var commandArgs = Facepunch.Pool.Get<PlayerArgs>();
					commandArgs.Type = cmd.Type;
					commandArgs.Arguments = split?.Skip(1)?.ToArray() ?? _emptyArgs;
					commandArgs.PrintOutput = true;
					commandArgs.IsServer = true;
					commandArgs.IsRCon = true;

					Community.Runtime.CommandManager.Execute(cmd, commandArgs);

					commandArgs.Dispose();
					Facepunch.Pool.Free(ref commandArgs);
				}
				catch (Exception ex)
				{
					Logger.Error($"Failed executing Carbon command '{command}'", ex);
				}
			}
			else
			{
				try
				{
					if (ConsoleSystem.Index.All.Any(x => x.FullName == name.Replace("+", string.Empty).Replace("-", string.Empty)))
					{
						ConsoleSystem.Run(ConsoleSystem.Option.Server, command);
					}
				}
				catch (Exception ex)
				{
					Logger.Error($"Failed executing native command '{command}'", ex);
				}
			}

			Array.Clear(split, 0, split.Length);
			split = null;
		}
	}
}
