using System;
using API.Events;
using HarmonyLib;

namespace Patches;

[HarmonyPatch(typeof(Bootstrap), methodName: nameof(Bootstrap.StartupShared))]
internal static class __StartupShared
{
	public static void Prefix()
	{
		Carbon.Bootstrap.Events.Trigger(CarbonEvent.StartupShared, EventArgs.Empty);
	}

	public static void Postfix()
	{
		Carbon.Bootstrap.Events.Trigger(CarbonEvent.StartupSharedComplete, EventArgs.Empty);
	}
}
