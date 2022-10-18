///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Carbon.Utility;
using Harmony;

namespace Carbon;

internal sealed class Hijacker
{
	private static string CarbonRegexPattern
			= @"(?i)^carbon([\.-](doorstop|loader|unix))?(.dll)?$";

	internal static bool DoUnload()
	{
		Type HarmonyLoader = AccessTools.TypeByName("HarmonyLoader") ?? null;
		FieldInfo field = HarmonyLoader?.GetField("loadedMods", BindingFlags.NonPublic | BindingFlags.Static) ?? null;
		Type HarmonyMod = HarmonyLoader?.GetNestedType("HarmonyMod", BindingFlags.NonPublic | BindingFlags.Static) ?? null;

		try
		{
			int count = 0;
			foreach (object mod in field.GetValue(null) as IEnumerable)
			{
				string Name = HarmonyMod.GetProperty("Name").GetValue(mod) as string;

				// TODO: better validation
				if (Regex.IsMatch(Name, CarbonRegexPattern)) continue;

				Logger.Warn($"Found loaded plugin '{Name}'");
				((HarmonyInstance)HarmonyMod.GetProperty("Harmony").GetValue(mod)).UnpatchAll(Name);
				HarmonyMod.GetProperty("Harmony").SetValue(mod, default);
				count++;
			}

			Logger.Log("Patching Facepunch's harmony loader");
			Loader.GetInstance().Harmony.PatchAll();
			field.SetValue(null, null);
			return (count > 0);
		}
		catch (Exception e)
		{
			Logger.Error("Error unloading plugins", e);
		}
		return false;
	}

	internal static bool DoMove()
	{
		try
		{
			string sourcePath = Path.Combine(Context.GameDirectory, "HarmonyMods");
			if (!Directory.Exists(sourcePath)) throw new Exception("Unable to find the HarmonyMods folder");

			string targetPath = Path.Combine(Context.CarbonDirectory, "harmony");
			if (!Directory.Exists(targetPath)) Directory.CreateDirectory(targetPath);

			int count = 0;
			foreach (string file in Directory.EnumerateFiles(sourcePath, "*.*"))
			{
				string fileName = Path.GetFileName(file);
				if (string.IsNullOrEmpty(file) || Regex.IsMatch(fileName, CarbonRegexPattern)) continue;
				File.Copy(file, Path.Combine(targetPath, fileName), true);
				File.Delete(file);

				Logger.Warn($"Moved '{fileName}'");
				count++;
			}
			return (count > 0);
		}
		catch (Exception e)
		{
			Logger.Error("Error while moving files", e);
		}
		return false;
	}
}