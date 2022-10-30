///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 
using HarmonyLib;

namespace Carbon.LoaderEx.Patches;

internal static class __ConsoleSystem
{
	[HarmonyPatch(typeof(ConsoleSystem), methodName: "Run")]
	internal static class __Run
	{
		[HarmonyPriority(int.MaxValue)]
		private static bool Prefix(string strCommand)
		{
			if (strCommand != "c.boot" || Components.Supervisor.Core.IsStarted) return true;
			Components.Supervisor.Core.Start();
			return false;
		}
	}
}
