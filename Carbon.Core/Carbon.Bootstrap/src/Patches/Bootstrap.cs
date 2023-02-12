using System;
using HarmonyLib;
using Legacy;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Patches;

internal static class __Bootstrap
{
	[HarmonyPatch(typeof(Bootstrap), methodName: "StartupShared")]
	internal static class __StartupShared
	{
		public static void Prefix()
		{
			Loader.GetInstance().Events
				.Trigger(API.Events.CarbonEvent.GameStartup, EventArgs.Empty);
		}

		public static void Postfix()
		{
			Loader.GetInstance().Events
				.Trigger(API.Events.CarbonEvent.GameStartupComplete, EventArgs.Empty);
		}
	}
}
