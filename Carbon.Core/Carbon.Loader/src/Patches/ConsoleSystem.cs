using Carbon.LoaderEx.Harmony;
using HarmonyLib;

/*
 *
 * Copyright (c) 2022 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.LoaderEx.Patches;

internal static class __ConsoleSystem
{
	[HarmonyPatch(typeof(ConsoleSystem), methodName: "Run")]
	internal static class __Run
	{
		[HarmonyPriority(int.MaxValue)]
		private static bool Prefix(string strCommand)
		{
			switch (strCommand)
			{
				case "c.boot":
					if (!HarmonyLoaderEx.GetInstance().IsLoaded("Carbon.dll"))
						HarmonyLoaderEx.GetInstance().Load(Context.Directories.CarbonManaged + "/Carbon.dll");
					return false;

				default:
					return true;
			}
		}
	}
}
