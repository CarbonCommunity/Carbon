/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

using System;
using Carbon.Extensions;

namespace Carbon.Components;

public static class CommandLine
{
	internal static readonly char[] _delimiter = new char[] { '|' };

	public static void ExecuteCommands(string @switch, string context, string[] lines = null)
	{
		var arg = CommandLineEx.GetArgumentResult(lines ?? Environment.GetCommandLineArgs(), @switch, string.Empty);
		var commands = arg.Split(_delimiter, StringSplitOptions.RemoveEmptyEntries);

		if (commands.Length > 0) Logger.Log($" Executing {commands.Length:n0} {commands.Length.Plural("command", "commands")} for the '{@switch}' switch ({context}):");

		ExecuteCommands(commands);

		Array.Clear(commands, 0, commands.Length);
		commands = null;
	}

	internal static void ExecuteCommands(string[] commands)
	{
		foreach (var command in commands)
		{
			if (string.IsNullOrEmpty(command)) continue;

			try { ConsoleSystem.Run(ConsoleSystem.Option.Unrestricted, command); } catch (Exception ex) { Logger.Error($"Failed executing '{command}'", ex); }
		}
	}
}
