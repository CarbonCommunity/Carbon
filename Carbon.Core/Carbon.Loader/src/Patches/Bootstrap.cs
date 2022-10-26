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
	[HarmonyPatch(typeof(Bootstrap), methodName: "StartupShared")]
	internal static class __StartupShared
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
				Components.HarmonyLoader.GetInstance().Load("Carbon.dll");
			}
		}
	}
}
