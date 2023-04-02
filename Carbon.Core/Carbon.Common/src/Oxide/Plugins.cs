using System.Linq;
using Carbon;
using Carbon.Core;
using Facepunch;
using Oxide.Core.Plugins;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Oxide.Core.Libraries;

public class Plugins : Library
{
	public PluginManager PluginManager { get; private set; } = new();

	public bool IsGlobal => true;

	public Plugins(PluginManager pluginmanager)
	{
		PluginManager = pluginmanager ?? new();
	}

	public bool Exists(string name)
	{
		return Community.Runtime.Plugins.Plugins.Any(x => x.Name == name);
	}

	public Plugin Find(string name)
	{
		name = name.Replace(" ", "");

		foreach (var mod in Loader.LoadedMods)
		{
			foreach (var plugin in mod.Plugins)
			{
				if (plugin.Name.Replace(" ", "").Replace(".", "") == name) return plugin;
			}
		}

		return null;
	}

	public Plugin[] GetAll()
	{
		var list = Pool.GetList<Plugin>();
		foreach (var mod in Loader.LoadedMods)
		{
			list.AddRange(mod.Plugins);
		}

		var result = list.ToArray();
		Pool.FreeList(ref list);
		return result;
	}
}
