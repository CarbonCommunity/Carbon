using System;
using System.Diagnostics;
using Carbon.LoaderEx.ASM;
using Carbon.LoaderEx.Context;
using Carbon.LoaderEx.Harmony;
using Carbon.LoaderEx.Utility;
using HarmonyLib;

/*
 *
 * Copyright (c) 2022 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.LoaderEx.Patches;

internal static class __Bootstrap
{
	[HarmonyPatch(typeof(Bootstrap), methodName: "Init_Tier0")]
	internal static class __Init_Tier0
	{
		public static void Prefix()
		{
			using (Sandbox<Renamer> isolated = new Sandbox<Renamer>())
			{
				Logger.Debug(">>>> " + Directories.CarbonManaged);
				isolated.Do.SetAssemblyName("Carbon.dll",
					Directories.CarbonManaged, "foobar");


				Console.WriteLine($">>> new name is {isolated.Do.GetAssemblyName("Carbon.dll", Directories.CarbonManaged)}");
			}

			Process.GetCurrentProcess().Kill();
		}

#if USE_DEBUGGER
		public static void Postfix()
		{
			Logger.Warn("Waiting for a debugger connection..");
			var t = Task.Run(async delegate
			{
				while (!Debugger.IsAttached)
					await Task.Delay(1000);
				return;
			});

			t.Wait();
			Debugger.Break();
		}
#endif
	}

	[HarmonyPatch(typeof(Bootstrap), methodName: "StartupShared")]
	internal static class __StartupShared
	{
		public static void Prefix()
			=> HarmonyLoaderEx.GetInstance().Load("Carbon.dll");
	}
}
