using Facepunch;
using HarmonyLib;

/*
 *
 * Copyright (c) 2022 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.LoaderEx.Patches;

internal static class __RCon
{
	[HarmonyPatch(typeof(RCon), methodName: "OnCommand")]
	internal static class __OnCommand
	{
		[HarmonyPriority(int.MaxValue)]
		private static bool Prefix(RCon.Command cmd)
		{
			switch (cmd.Name)
			{
				case "c.boot":
					if (!Supervisor.Core.IsStarted) Supervisor.Core.Start();
					return false;

				default:
					return true;
			}
		}
	}
}
