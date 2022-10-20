///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
///
using System;
using System.IO;
using Carbon.Common;

namespace Carbon.Components;

internal sealed class HarmonyLoader : Singleton<HarmonyLoader>
{
	static HarmonyLoader() { }

	/// <summary>
	/// Loads assembly into AppDomain.<br/>
	/// The process will automatically detect the type of file being loaded
	/// and use the appropriate loading method to deal with it.
	/// </summary>
	///
	/// <param name="assemblyPath">Full path to the assembly file</param>
	internal void Load(string assemblyPath)
	{
		Carbon.Utility.Logger.Log($"HarmonyLoader:Load('{assemblyPath})");

		if (assemblyPath == Path.GetFileName(assemblyPath))
			assemblyPath = Path.Combine(Context.Directory.Carbon, "managed", assemblyPath);

		HarmonyPlugin mod = new HarmonyPlugin()
		{
			name = Path.GetFileNameWithoutExtension(assemblyPath),
			identifier = Guid.NewGuid().ToString(),
			fullPath = assemblyPath,
		};

		switch (mod.Extension)
		{
			case ".dll":
				mod.LoadFromFile(assemblyPath);
				PrepareHooks(ref mod);
				ApplyHarmonyPatches(ref mod);
				TriggerHooks(ref mod);
				break;

			// case ".drm"
			// 	LoadFromDRM();
			// 	break;

			default:
				throw new ArgumentOutOfRangeException("File extension not supported");
		}
	}

	private void PrepareHooks(ref HarmonyPlugin mod)
	{
		Utility.Logger.Log($"HarmonyLoader:PrepareHooks('{mod}')");

		foreach (Type type in mod.types)
		{
			if (!typeof(IHarmonyModHooks).IsAssignableFrom(type)) continue;

			try
			{
				IHarmonyModHooks hook = Activator.CreateInstance(type) as IHarmonyModHooks;
				if (hook != null) mod.hooks.Add(hook);
				else throw new NullReferenceException();
				Utility.Logger.Log($"Instance of '{hook}' created");
			}
			catch (System.Exception e)
			{
				Utility.Logger.Error("Failed to instantiate hook (null)", e);
				throw;
			}
		}
	}

	private void ApplyHarmonyPatches(ref HarmonyPlugin mod)
	{
		Utility.Logger.Log($"HarmonyLoader:ApplyHarmonyPatches('{mod}')");

		try
		{
			mod.harmonyInstance = new HarmonyLib.Harmony(mod.identifier);
			mod.harmonyInstance.PatchAll(mod.assembly);
		}
		catch (System.Exception e)
		{
			Utility.Logger.Error($"Failed to apply harmony patches for '{mod}'", e);
			throw;
		}

		// foreach (var method in HarmonyLib.Harmony.GetAllPatchedMethods())
		// 	Logger.Log($" > {method.Name} at {method.Module} by " +
		// 		$"{string.Join(",", HarmonyLib.Harmony.GetPatchInfo(method).Owners)}");
	}

	private void TriggerHooks(ref HarmonyPlugin mod)
	{
		Utility.Logger.Log($"HarmonyLoader:TriggerHooks('{mod}')");

		foreach (IHarmonyModHooks hook in mod.hooks)
		{
			try
			{
				Utility.Logger.Log($"Trigger '{hook}' event");
				hook.OnLoaded(args: new OnHarmonyModLoadedArgs());
			}
			catch (System.Exception e)
			{
				Utility.Logger.Error($"Failed to trigger 'OnLoaded' hook for '{mod}'", e);
			}
		}
	}
}
