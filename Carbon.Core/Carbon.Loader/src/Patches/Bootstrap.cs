///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using System.Collections;
using System.Reflection;
using Carbon.Utility;
using Harmony;

namespace Carbon.Loader.Patches
{
	//[HarmonyPatch(typeof(Bootstrap), "StartupShared")]
	public static partial class Bootstrap
	{
		private static bool Prefix_StartupShared()
		{
			FieldInfo field = typeof(HarmonyLoader)
				.GetField("loadedMods", BindingFlags.NonPublic | BindingFlags.Static);

			Type HarmonyLoader = typeof(HarmonyLoader)
				.GetNestedType("HarmonyMod", BindingFlags.NonPublic | BindingFlags.Static);

			foreach (object mod in field.GetValue(null) as IEnumerable)
			{
				string Name = HarmonyLoader.GetProperty("Name")
					.GetValue(mod) as string;

				Logger.Log($"Found loaded plugin '{Name}'");

				HarmonyInstance Instance = HarmonyLoader.GetProperty("Harmony")
					.GetValue(mod) as HarmonyInstance;
				Logger.Log($"  - Got harmony instance {Instance.Id}");

				Instance.UnpatchAll(Name);
				Logger.Log($"  - Removed all active assembly patches");

				HarmonyLoader.GetProperty("Harmony").SetValue(mod, default);
				Logger.Log($"  - Set the harmony instance to null");
			}

			try
			{
				Logger.Log("1");
				field.SetValue(null, null);
				Logger.Log("2");
			}
			catch (Exception e)
			{
				Logger.Error("Nope", e);
			}

			return true;
		}
	}
}