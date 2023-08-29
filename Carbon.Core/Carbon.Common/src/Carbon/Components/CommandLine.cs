/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Components;

public static class CommandLine
{
	internal static readonly char[] _delimiter = new char[] { '|' };

	public static void ExecuteCommands(string @switch, string context, string[] lines = null, bool carbonRegisteredCmdsOnly = false)
	{
		var arg = CommandLineEx.GetArgumentResult(lines ?? Environment.GetCommandLineArgs(), @switch, string.Empty);
		var commands = arg.Split(_delimiter, StringSplitOptions.RemoveEmptyEntries);

		if (commands.Length > 0) Logger.Log($" Executing {commands.Length:n0} {commands.Length.Plural("command", "commands")} for the '{@switch}' switch ({context}):");

		ExecuteCommands(commands, carbonRegisteredCmdsOnly);

		Array.Clear(commands, 0, commands.Length);
		commands = null;
	}

	public static void ExecuteCommands(string[] commands, bool apiRegisteredCmdsOnly)
	{
		foreach (var command in commands)
		{
			if (string.IsNullOrEmpty(command)) continue;

			if (apiRegisteredCmdsOnly)
			{
				var split = command.Split(' ');
				var name = split[0];
				Array.Clear(split, 0, split.Length);
				split = null;

				if (!Community.Runtime.CommandManager.Contains(Community.Runtime.CommandManager.RCon, name, out _))
				{
					continue;
				}
			}

			try
			{
				ConsoleSystem.Run(ConsoleSystem.Option.Server, command);
			}
			catch (Exception ex)
			{
				Logger.Error($"Failed executing '{command}'", ex);
			}
		}
	}
}
