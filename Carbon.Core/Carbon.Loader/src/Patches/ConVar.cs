///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 
using HarmonyLib;

namespace Carbon.LoaderEx.Patches;

internal static class __ConVar
{
	internal static class __Harmony
	{
		[HarmonyPatch(typeof(ConVar.Harmony), methodName: "Load")]
		internal static class __Load
		{
			[HarmonyPriority(int.MaxValue)]
			private static bool Prefix()
				=> false;
		}

		[HarmonyPatch(typeof(ConVar.Harmony), methodName: "Unload")]
		internal static class __Unload
		{
			[HarmonyPriority(int.MaxValue)]
			private static bool Prefix()
				=> false;
		}
	}
}
