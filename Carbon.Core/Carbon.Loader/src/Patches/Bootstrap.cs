///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 
using System;
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;
using Carbon.Utility;
using Harmony;

namespace Carbon.Patches;

internal static class __Bootstrap
{
	[HarmonyPatch(typeof(Bootstrap), "StartupShared")]
	internal static class __StartupShared
	{
		public static bool Prefix()
		{
			Type HarmonyLoader = AccessTools.TypeByName("HarmonyLoader") ?? null;
			FieldInfo field = HarmonyLoader?.GetField("loadedMods", BindingFlags.NonPublic | BindingFlags.Static) ?? null;
			Type HarmonyMod = HarmonyLoader?.GetNestedType("HarmonyMod", BindingFlags.NonPublic | BindingFlags.Static) ?? null;

			try
			{
				foreach (object mod in field.GetValue(null) as IEnumerable)
				{
					string Name = HarmonyMod.GetProperty("Name").GetValue(mod) as string;

					// TODO: better validation
					if (Regex.IsMatch(Name, @"(?i)^(carbon.loader)$"))
						continue;

					Logger.Log($"Found loaded plugin '{Name}'");

					HarmonyInstance Instance = HarmonyMod.GetProperty("Harmony").GetValue(mod) as HarmonyInstance;
					Logger.Log($"  - Got harmony instance {Instance.Id}");

					Instance.UnpatchAll(Name);
					HarmonyMod.GetProperty("Harmony").SetValue(mod, default);
					Logger.Log($"  - Removed all active assembly patches");
				}

				Logger.Log("Patching Facepunch's harmony loader");
				Loader.GetInstance().Harmony.PatchAll();
				field.SetValue(null, null);
			}
			catch (Exception e)
			{
				Logger.Error("Error while dealing with the loaded plugins", e);
			}

			return true;
		}
	}
}