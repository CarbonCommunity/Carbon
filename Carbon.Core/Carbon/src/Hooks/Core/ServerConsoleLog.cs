///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using UnityEngine;

namespace Carbon.Hooks
{
	[CarbonHook.AlwaysPatched, CarbonHook.Hidden]
	[CarbonHook("IServerConsoleLog"), CarbonHook.Category(Hook.Category.Enum.Core)]
	[CarbonHook.Patch(typeof(ServerConsole), "HandleLog")]
	public class ServerConsoleLog
	{
		public static bool Prefix(string message, string stackTrace, LogType type)
		{
			if (message.StartsWith("Trying to load assembly: DynamicAssembly")) return false;

			return true;
		}
	}
}
