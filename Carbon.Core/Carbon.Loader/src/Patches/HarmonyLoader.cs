///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 
using HarmonyLib;

namespace Carbon.LoaderEx.Patches;

internal static class __HarmonyLoader
{
	[HarmonyPatch(typeof(HarmonyLoader), methodName: "LoadHarmonyMods")]
	internal static class __LoadHarmonyMods
	{
		[HarmonyPriority(int.MaxValue)]
		private static bool Prefix()
			=> false;
	}

	[HarmonyPatch(typeof(HarmonyLoader), methodName: "TryLoadMod")]
	internal static class __TryLoadMod
	{
		[HarmonyPriority(int.MaxValue)]
		private static bool Prefix()
			=> false;
	}

	[HarmonyPatch(typeof(HarmonyLoader), methodName: "TryUnloadMod")]
	internal static class __TryUnloadMod
	{
		[HarmonyPriority(int.MaxValue)]
		private static bool Prefix()
			=> false;
	}
}
