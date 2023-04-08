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
	public static void ExecuteCommands(string @switch)
	{
		var arg = CommandLineEx.GetArgumentResult(@switch, string.Empty);
		var commands = arg.Split('|');

		Logger.Log($" Executing {commands.Length:n0} {commands.Length.Plural("command", "commands")} for the '{@switch}' switch:");

		ExecuteCommands(commands);

		Array.Clear(commands, 0, commands.Length);
		commands = null;
	}

	internal static void ExecuteCommands(string[] commands)
	{
		foreach(var command in commands)
		{
			ConsoleSystem.Run(ConsoleSystem.Option.Unrestricted, command);
		}
	}
}
