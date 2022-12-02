///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Harmony;

namespace Carbon.LoaderEx.Utility;

internal sealed class Hijacker
{
	internal static bool DoUnload()
	{
		BindingFlags bindingFlags = (BindingFlags)62;

		Type HarmonyLoader
			= AccessTools.TypeByName("HarmonyLoader") ?? null;

		FieldInfo field
			= HarmonyLoader?.GetField("loadedMods", bindingFlags) ?? null;

		Type HarmonyMod
			= HarmonyLoader?.GetNestedType("HarmonyMod", bindingFlags) ?? null;

		int count = 0;
		foreach (object mod in field.GetValue(null) as IEnumerable)
		{
			string Name = HarmonyMod.GetProperty("Name").GetValue(mod) as string;

			// TODO: better validation
			if (Regex.IsMatch(Name, Context.Patterns.carbonNamePattern)) continue;
			Logger.Warn($"Found loaded plugin '{Name}'");

			try
			{
				HarmonyInstance harmonyInstance = HarmonyMod.GetProperty("Harmony").GetValue(mod) as HarmonyInstance;
				harmonyInstance.UnpatchAll(Name);
				harmonyInstance = default;

				HarmonyMod.GetProperty("Assembly").SetValue(mod, default);
			}
			catch (Exception e)
			{
				Logger.Error($"Error unloading plugin '{Name}'", e);
			}

			count++;
		}
		field.SetValue(null, null);
		return (count > 0);
	}

	internal static bool DoMove()
	{
		try
		{
			string source = Context.Directory.GameHarmony;
			if (!Directory.Exists(source)) throw new Exception("Unable to find the HarmonyMods folder");

			string target = Context.Directory.CarbonHarmony;
			if (!Directory.Exists(target)) Directory.CreateDirectory(target);

			int count = 0;
			foreach (string file in Directory.EnumerateFiles(source, "*.dll"))
			{
				string name = Path.GetFileName(file);
				if (string.IsNullOrEmpty(file) || Regex.IsMatch(name, Context.Patterns.carbonFileNamePattern)) continue;
				File.Copy(file, Path.Combine(target, name), true);
				File.Delete(file);

				Logger.Warn($"Moved '{name}'");
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

	internal static bool DoHijack()
	{
		try
		{
			Logger.Log("Patching Facepunch's harmony loader");
			Program.GetInstance().Harmony.PatchAll();
		}
		catch (System.Exception e)
		{
			Logger.Error("Error patching Facepunch's harmony loader", e);
			return false;
		}

		try
		{
			Logger.Log("Cleaning AssemblyResolve event handler");

			EventInfo eventInfo = typeof(AppDomain).GetEvent("AssemblyResolve",
				BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

			FieldInfo fieldInfo = typeof(AppDomain).GetField(eventInfo.Name,
				BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

			Delegate eventDelegate = (Delegate)fieldInfo.GetValue(AppDomain.CurrentDomain) ?? null;

			MethodInfo nfo;
			foreach (Delegate evh in eventDelegate?.GetInvocationList())
			{
				nfo = evh.GetMethodInfo();

				// TODO: stop being lazy and do this in a proper way
				if (nfo.Module.Name.Contains("Rust.Harmony.dll"))
				{
					eventInfo.RemoveEventHandler(AppDomain.CurrentDomain, evh);
					Logger.Warn($" - Removed {nfo.Name} [{nfo.Module}]");
				}
			}
		}
		catch (System.Exception e)
		{
			Logger.Error("Error cleaning event handler", e);
			return false;
		}

		return true;
	}
}
