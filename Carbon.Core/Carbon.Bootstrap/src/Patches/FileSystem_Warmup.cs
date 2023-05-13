using System;
using API.Events;
using HarmonyLib;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Patches;

internal static class __FileSystem_Warmup
{
	[HarmonyPatch(typeof(FileSystem_Warmup), methodName: "Run", new Type[] { typeof(string[]), typeof(Action<string>), typeof(string), typeof(int) })]
	internal static class __Run
	{
		public static void Prefix()
		{
			Carbon.Bootstrap.Events
				.Trigger(CarbonEvent.FileSystemWarmup, EventArgs.Empty);
		}

		public static void Postfix()
		{
			Carbon.Bootstrap.Events
				.Trigger(CarbonEvent.FileSystemWarmupComplete, EventArgs.Empty);
		}
	}
}
