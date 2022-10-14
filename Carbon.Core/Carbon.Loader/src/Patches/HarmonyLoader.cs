///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;

namespace Carbon.Loader.Patches
{
	public static partial class HarmonyLoader
	{
		// HarmonyLoader
		private static bool Prefix_LoadHarmonyMods()
		{
			Console.WriteLine("Prefix_LoadHarmonyMods");
			return true;
		}

		private static bool Prefix_TryLoadMod()
		{
			Console.WriteLine("Prefix_TryLoadMod");
			return true;
		}

		private static bool Prefix_TryUnloadMod()
		{
			Console.WriteLine("Prefix_TryUnloadMod");
			return true;
		}
	}
}