///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
///
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Carbon.LoaderEx.Common;

namespace Carbon.LoaderEx.Components;

internal sealed class HarmonyLoader : Singleton<HarmonyLoader>
{
	private HashSet<HarmonyPlugin> loadedAssembly
		= new HashSet<HarmonyPlugin>();

	static HarmonyLoader() { }

	/// <summary>
	/// Loads assembly into AppDomain.<br/>
	/// The process will automatically detect the type of file being loaded
	/// and use the appropriate loading method to deal with it.
	/// </summary>
	///
	/// <param name="assemblyFile">Full path to the assembly file</param>
	internal void Load(string assemblyFile)
	{
		Carbon.LoaderEx.Utility.Logger.Log($"Loading '{assemblyFile}'..");

		if (assemblyFile == Path.GetFileName(assemblyFile))
			assemblyFile = Path.Combine(Context.Directory.Carbon, "managed", assemblyFile);

		HarmonyPlugin mod = new HarmonyPlugin()
		{
			name = Path.GetFileNameWithoutExtension(assemblyFile),
			identifier = Guid.NewGuid().ToString(),
			location = assemblyFile,
		};

		switch (mod.Extension)
		{
			case ".dll":
				mod.LoadFromFile(assemblyFile);
				PrepareHooks(ref mod);
				ApplyPatches(ref mod);
				OnLoaded(ref mod);
				break;

			// case ".drm"
			// 	LoadFromDRM();
			// 	break;

			default:
				throw new ArgumentOutOfRangeException("File extension not supported");
		}

		loadedAssembly.Add(mod);
	}

	internal void Unload(string assemblyFile)
	{
		Carbon.LoaderEx.Utility.Logger.Log($"Unloading '{assemblyFile}'..");
		string name = Path.GetFileNameWithoutExtension(assemblyFile);

		try
		{
			HarmonyPlugin mod = loadedAssembly.FirstOrDefault(x => x.name == name);
			if (mod.name == null) throw new Exception($"Assembly '{mod}' not found");
			loadedAssembly.Remove(mod);
			OnUnloaded(ref mod);
			mod.Dispose();
		}
		catch (System.Exception e)
		{
			Utility.Logger.Error($"Failed unload '{assemblyFile}'", e);
			throw;
		}
	}

	internal bool IsLoaded(string assemblyFile)
		=> ((loadedAssembly.FirstOrDefault(x => x.FileName == assemblyFile) ?? null) != null);

	private void PrepareHooks(ref HarmonyPlugin mod)
	{
		Utility.Logger.Debug($" Preparing '{mod}' hooks");

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

	private void ApplyPatches(ref HarmonyPlugin mod)
	{
		Utility.Logger.Debug($" Apply '{mod}' harmony patches");

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

	private void OnLoaded(ref HarmonyPlugin mod)
	{
		Utility.Logger.Log($" Trigger '{mod}' OnLoaded hook");

		foreach (IHarmonyModHooks hook in mod.hooks)
		{
			try
			{
				hook.OnLoaded(args: new OnHarmonyModLoadedArgs());
			}
			catch (System.Exception e)
			{
				Utility.Logger.Error($"Failed to trigger 'OnLoaded' hook for '{mod}'", e);
			}
		}
	}

	private void OnUnloaded(ref HarmonyPlugin mod)
	{
		Utility.Logger.Log($" Trigger '{mod}' OnUnloaded hook");

		foreach (IHarmonyModHooks hook in mod.hooks)
		{
			try
			{
				hook.OnUnloaded(args: new OnHarmonyModUnloadedArgs());
			}
			catch (System.Exception e)
			{
				Utility.Logger.Error($"Failed to trigger 'OnUnloaded' hook for '{mod}'", e);
			}
		}
	}
}
