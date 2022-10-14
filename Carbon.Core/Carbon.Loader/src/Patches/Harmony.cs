///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;

namespace Carbon.Loader.Patches
{
	public static partial class Harmony
	{
		// ConVar.Harmony
		private static bool Prefix_Load()
		{
			Console.WriteLine("Prefix_Load");
			return true;

		}

		private static bool Prefix_Unload()
		{
			Console.WriteLine("Prefix_Unload");
			return true;
		}
	}
}