///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using UnityEngine;

[Hook.AlwaysPatched, Hook.Hidden]
[Hook("IServerConsoleLog"), Hook.Category(Hook.Category.Enum.Core)]
[Hook.Patch(typeof(ServerConsole), "HandleLog")]
public class ServerConsoleLog
{
	public static bool Prefix(string message, string stackTrace, LogType type)
	{
		if (message.StartsWith("Trying to load assembly: DynamicAssembly")) return false;

		return true;
	}
}
