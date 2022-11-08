///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 
using System.Diagnostics;
using Carbon.LoaderEx.Utility;
using Harmony;

namespace Carbon.LoaderEx.Patches;

internal static class __Bootstrap
{
	[HarmonyPatch(typeof(Bootstrap), methodName: "Init_Tier0")]
	internal static class __Init_Tier0
	{
		public static void Prefix()
		{
			bool r1 = Hijacker.DoUnload();
			bool r2 = Hijacker.DoMove();

			if (r1 || r2)
			{
				Logger.Warn("Application will now exit");
				Process.GetCurrentProcess().Kill();
			}
			else
			{
				Hijacker.DoHijack();
			}
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
			=> Components.HarmonyLoader.GetInstance().Load("Carbon.dll");
	}
}
