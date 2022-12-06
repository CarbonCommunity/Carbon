using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Carbon.LoaderEx.Common;

/*
 *
 * Copyright (c) 2022 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.LoaderEx;

internal sealed class HarmonyLoaderEx : Singleton<HarmonyLoaderEx>
{
	private HashSet<HarmonyPlugin> loadedAssembly
		= new HashSet<HarmonyPlugin>();

	static HarmonyLoaderEx() { }
	internal HarmonyLoaderEx() { }

	/// <summary>
	/// Loads assembly into AppDomain.<br/>
	/// The process will automatically detect the type of file being loaded
	/// and use the appropriate loading method to deal with it.
	/// </summary>
	///
	/// <param name="fileNameWithExtension">Full path to the assembly file</param>
	internal void Load(string fileNameWithExtension)
	{
		Carbon.LoaderEx.Utility.Logger.Log($"Loading '{fileNameWithExtension}'..");

		if (fileNameWithExtension == Path.GetFileName(fileNameWithExtension))
			fileNameWithExtension = Path.Combine(Context.Directories.Carbon, "managed", fileNameWithExtension);

		HarmonyPlugin mod = new HarmonyPlugin()
		{
			name = Path.GetFileNameWithoutExtension(fileNameWithExtension),
			identifier = $"{Guid.NewGuid():N}",
			location = fileNameWithExtension,
		};

		try
		{
			switch (mod.Extension)
			{
				case ".dll":
					if (mod.LoadFromFile(fileNameWithExtension) == null)
						throw new Exception("Assembly is null");

					PrepareHooks(ref mod);
					ApplyPatches(ref mod);
					OnLoaded(ref mod);

					loadedAssembly.Add(mod);
					break;

				// case ".drm"
				// 	LoadFromDRM();
				// 	break;

				default:
					throw new ArgumentOutOfRangeException("File extension not supported");
			}
		}
		catch (System.Exception e)
		{
			Utility.Logger.Error($"Failed loading '{fileNameWithExtension}'", e);
			throw;
		}
	}

	internal void Unload(string assemblyFile, bool reload = false)
	{
		Carbon.LoaderEx.Utility.Logger.Log($"Unloading '{assemblyFile}'..");
		string name = Path.GetFileNameWithoutExtension(assemblyFile);

		try
		{
			HarmonyPlugin mod = loadedAssembly.FirstOrDefault(x => x.name == name);
			if (mod?.name == null) throw new Exception($"Assembly '{name}' not loaded");
			loadedAssembly.Remove(mod);
			OnUnloaded(ref mod);
			mod.Dispose();
		}
		catch (System.Exception e)
		{
			Utility.Logger.Error($"Failed unload '{assemblyFile}'", e);
			throw;
		}

		if (reload) Load(assemblyFile);
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
