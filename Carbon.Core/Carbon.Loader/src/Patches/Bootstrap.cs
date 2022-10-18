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
	[HarmonyPatch(typeof(Bootstrap), "StartupShared")]
	internal static class __StartupShared
	{
		public static void Prefix()
		{
			Loader.GetInstance().Initialize();

			if (Hijacker.DoUnload() || Hijacker.DoMove())
			{
				Logger.Warn("Application will now exit");
				Process.GetCurrentProcess().Kill();
			}
		}
	}
}
