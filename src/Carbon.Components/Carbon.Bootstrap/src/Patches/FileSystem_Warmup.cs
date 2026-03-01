using System;
using System.Threading;
using API.Events;
using HarmonyLib;

namespace Patches;

internal static class __FileSystem_Warmup
{
	[HarmonyPatch(typeof(FileSystem_Warmup), methodName: nameof(FileSystem_Warmup.Run), typeof(Action<string>), typeof(string), typeof(CancellationToken))]
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
