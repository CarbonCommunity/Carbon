///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 
using Facepunch;
using HarmonyLib;

namespace Carbon.LoaderEx.Patches;

internal static class __RCon
{
	[HarmonyPatch(typeof(RCon), methodName: "OnCommand")]
	internal static class __OnCommand
	{
		[HarmonyPriority(int.MaxValue)]
		private static bool Prefix(RCon.Command cmd)
		{
			if (cmd.Name != "c.boot" || Components.Supervisor.Core.IsStarted) return true;
			Components.Supervisor.Core.Start();
			return false;
		}
	}
}
