using System;
using System.Threading.Tasks;
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
				.Trigger(API.Events.CarbonEvent.StartupShared, EventArgs.Empty);
		}

		public static void Postfix()
		{
			Loader.GetInstance().Events
				.Trigger(API.Events.CarbonEvent.StartupSharedComplete, EventArgs.Empty);



			// example mockup -----
			bool ArePluginsReady = false;
			Loader.GetInstance().Events.Subscribe(API.Events.CarbonEvent.CarbonPluginsReady, x => ArePluginsReady = true);

			Task WaitForPlugins = Task.Run(async delegate
			{
				Utility.Logger.Debug("Waiting for event CarbonPluginsReady");
				while (!ArePluginsReady) await Task.Delay(1000);
				return;
			});

			// remove comment to start waiting for the event
			//WaitForPlugins.Wait();
		}
	}
}
