///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
///
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Carbon.Common;

namespace Carbon.Components;

internal sealed class HarmonyLoader : Singleton<HarmonyLoader>, IDisposable
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

		Plugin mod = new Plugin()
		{
			fileExt = Path.GetExtension(assemblyPath),
			fileName = Path.GetFileNameWithoutExtension(assemblyPath),
			filePath = Path.GetDirectoryName(assemblyPath)
		};

		try
		{
			mod.domain = AppDomain.CreateDomain(mod.Identifier);
			Carbon.Utility.Logger.Log($"New domain created '{mod.domain.FriendlyName}'");
		}
		catch (Exception e)
		{
			Carbon.Utility.Logger.Error("wtf", e);
		}

		switch (mod.fileExt)
		{
			case ".dll":
				LoadAssemblyFromFile(ref mod);
				break;

			// case ".drm"
			// 	LoadFromDRM();
			// 	break;

			default:
				throw new ArgumentOutOfRangeException("File extension not supported");
		}

		if (mod.IsLoaded)
		{
			Activate(ref mod);

			// Logger.Log("Applied harmony patches:");
			// foreach (var method in HarmonyLib.Harmony.GetAllPatchedMethods())
			// 	Logger.Log($" - {method.Name} at {method.Module} by " +
			// 		$"{string.Join(",", HarmonyLib.Harmony.GetPatchInfo(method).Owners)}");
		}
	}

	private void LoadAssemblyFromFile(ref Plugin mod)
	{
		Carbon.Utility.Logger.Log($"HarmonyLoader:LoadAssemblyFromFile('{mod}')");

		byte[] raw;

		try
		{
			string dll = Path.Combine(mod.filePath, $"{mod.fileName}{mod.fileExt}");
			if (!File.Exists(dll)) throw new FileNotFoundException();
			raw = File.ReadAllBytes(dll);
		}
		catch (Exception e)
		{
			Utility.Logger.Error($"Error loading assembly from '{mod}'", e);
			return;
		}

		byte[] sym;

		try
		{
			string pdb = Path.Combine(mod.filePath, mod.fileName, ".pdb");
			sym = File.ReadAllBytes(pdb);
		}
		catch
		{
			Utility.Logger.Warn($"No symbol information found for '{mod}'");
			sym = null;
		}

		mod.assembly = Assembly.Load(raw, sym);

		// collect metadata
		mod.types = mod.assembly.GetTypes();
		mod.references = mod.assembly.GetReferencedAssemblies();
	}

	internal void Activate(ref Plugin mod)
	{
		Utility.Logger.Log($"HarmonyLoader:Activate('{mod}')");

		foreach (Type type in mod.types)
		{
			if (typeof(IHarmonyModHooks).IsAssignableFrom(type))
			{
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

		try
		{
			mod.harmonyInstance = new HarmonyLib.Harmony(mod.Identifier);
			mod.harmonyInstance.PatchAll(mod.assembly);
		}
		catch (System.Exception e)
		{
			Utility.Logger.Error($"Failed to apply harmony patches for '{mod}'", e);
			throw;
		}

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

	public void Dispose()
	{

	}
}
