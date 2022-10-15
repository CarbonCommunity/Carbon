///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
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
		private static string CarbonRegexPattern
			= @"(?i)^carbon([\.-](doorstop|loader|unix))?(.dll)?$";

		public static bool Prefix()
		{
			bool needRestart = false;
			Type HarmonyLoader = AccessTools.TypeByName("HarmonyLoader") ?? null;
			FieldInfo field = HarmonyLoader?.GetField("loadedMods", BindingFlags.NonPublic | BindingFlags.Static) ?? null;
			Type HarmonyMod = HarmonyLoader?.GetNestedType("HarmonyMod", BindingFlags.NonPublic | BindingFlags.Static) ?? null;

			try
			{
				foreach (object mod in field.GetValue(null) as IEnumerable)
				{
					string Name = HarmonyMod.GetProperty("Name").GetValue(mod) as string;

					// TODO: better validation
					if (Regex.IsMatch(Name, CarbonRegexPattern)) continue;

					needRestart = true;
					Logger.Warn($"Found loaded plugin '{Name}'");
					((HarmonyInstance)HarmonyMod.GetProperty("Harmony").GetValue(mod)).UnpatchAll(Name);
					HarmonyMod.GetProperty("Harmony").SetValue(mod, default);
				}

				Logger.Log("Patching Facepunch's harmony loader");
				Loader.GetInstance().Harmony.PatchAll();
				field.SetValue(null, null);
			}
			catch (Exception e)
			{
				Logger.Error("Error unloading plugins", e);
			}

			try
			{
				string sourcePath = Path.Combine(Context.GameDirectory, "HarmonyMods");
				if (!Directory.Exists(sourcePath)) throw new Exception("Unable to find the HarmonyMods folder");

				string targetPath = Path.Combine(Context.CarbonDirectory, "harmony");
				if (!Directory.Exists(targetPath)) Directory.CreateDirectory(targetPath);

				foreach (string file in Directory.EnumerateFiles(sourcePath, "*.*"))
				{
					string fileName = Path.GetFileName(file);
					if (string.IsNullOrEmpty(file) || Regex.IsMatch(fileName, CarbonRegexPattern)) continue;
					File.Copy(file, Path.Combine(targetPath, fileName), true);
					File.Delete(file);

					needRestart = true;
					Logger.Warn($"Moved '{fileName}'");
				}
			}
			catch (Exception e)
			{
				Logger.Error("Error while moving files", e);
			}

			if (needRestart)
			{
				Logger.Warn("Application will now exit");
				Process.GetCurrentProcess().Kill();
			}

			return true;
		}
	}
}
