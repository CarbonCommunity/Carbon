using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Carbon.LoaderEx.ASM;
using Carbon.LoaderEx.Common;
using Carbon.LoaderEx.Utility;

/*
 *
 * Copyright (c) 2022 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.LoaderEx.Harmony;

internal sealed class HarmonyLoaderEx : Singleton<HarmonyLoaderEx>
{
	private HashSet<HarmonyPlugin> _loadedPlugins;

	static HarmonyLoaderEx() { }

	internal HarmonyLoaderEx()
	{
		_loadedPlugins = new HashSet<HarmonyPlugin>();
	}

	/// <summary>
	/// Loads assembly into AppDomain by always reading it back from disk.<br/>
	/// The process will automatically detect the type of file being loaded
	/// and use the appropriate loading method to deal with it.
	/// </summary>
	///
	/// <param name="fileName">Assembly name with extension i.e. the file name</param>
	/// <param name="forced">Forces the assembly to be re-read from disk</param>
	internal Assembly Load(string fileName)
	{
		Utility.Logger.Log($"Loading harmony plug-in '{fileName}'..");

		string name = Path.GetFileNameWithoutExtension(fileName);
		string extension = Path.GetExtension(fileName);

		Match match = Regex.Match(name, Context.Patterns.CarbonManagedFile);

		string location = (match.Success)
			? Context.Directories.CarbonManaged
			: Context.Directories.CarbonHarmony;

		try
		{
			switch (extension)
			{
				case ".dll":
					HarmonyPlugin mod = new HarmonyPlugin(fileName, location);
					mod.Awake();
					mod.OnLoaded();
					_loadedPlugins.Add(mod);

					foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies().Where(x => x.GetName().Name.StartsWith("Carbon")))
						Logger.Debug($"---> {assembly.GetName().Name} {assembly.GetName().Version}");

					return mod.Assembly;

				// case ".drm"
				// 	LoadFromDRM();
				// 	break;

				default:
					throw new ArgumentOutOfRangeException("File extension not supported");
			}
		}
		catch (System.Exception e)
		{
			Utility.Logger.Error($"Failed loading '{fileName}'", e);
			throw;
		}
	}



	/// <summary>
	/// Calls the assembly OnUnloaded() and Dispose() methods to "unload".<br/>
	/// Due to the limitations of using only one AppDomain, the assembly will
	/// still be present on the loaded modules.
	/// </summary>
	///
	/// <param name="fileName">Assembly name with extension i.e. the file name</param>
	/// <param name="reload">Loads back the assembly</param>
	internal void Unload(string fileName, bool reload = false)
	{
		Utility.Logger.Log($"Unloading harmony plug-in '{fileName}'..");
		string name = Path.GetFileNameWithoutExtension(fileName);

		try
		{
			HarmonyPlugin mod = _loadedPlugins.SingleOrDefault(x => x.FileName == fileName);
			if (mod?.Assembly == null) throw new Exception($"Assembly '{name}' not loaded");
			_loadedPlugins.Remove(mod);

			mod.OnUnloaded();
			mod.Dispose();
			mod = default;
		}
		catch (System.Exception e)
		{
			Utility.Logger.Error($"Failed to unload '{fileName}'", e);
			throw;
		}

		if (reload)
		{
			Load(fileName);
		}
	}

	internal bool IsLoaded(string fileName)
		=> ((_loadedPlugins.SingleOrDefault(x => x.FileName == fileName) ?? null) != null);

	internal bool IsEnabled(string fileName)
		=> _loadedPlugins.SingleOrDefault(x => x.FileName == fileName).Enabled;
}
