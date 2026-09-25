using System;
using Carbon.Events;
using HarmonyLib;

namespace Patches;

[HarmonyPatch(typeof(Bootstrap), methodName: nameof(Bootstrap.StartupShared))]
internal static class __StartupShared
{
	public static void Prefix()
	{
		Carbon.Managers.Services.Events.Trigger(CarbonEvent.StartupShared, EventArgs.Empty);
	}

	public static void Postfix()
	{
		Carbon.Managers.Services.Events.Trigger(CarbonEvent.StartupSharedComplete, EventArgs.Empty);
	}
}
