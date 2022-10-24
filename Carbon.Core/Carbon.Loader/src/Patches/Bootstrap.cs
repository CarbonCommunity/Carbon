///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 
using System.Diagnostics;
using Carbon.Utility;
using Harmony;

namespace Carbon.Patches;

internal static class __Bootstrap
{
	[HarmonyPatch(typeof(Bootstrap), methodName: "StartupShared")]
	internal static class __StartupShared
	{
		public static void Prefix()
		{

			if (Hijacker.DoUnload() || Hijacker.DoMove())
			{
				Logger.Warn("Application will now exit");
				Process.GetCurrentProcess().Kill();
			}

			Hijacker.DoHijack();
			Components.HarmonyLoader.GetInstance().Load("Carbon.dll");
		}
	}
}
