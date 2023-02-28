using System.Linq;
using Carbon;
using Carbon.Core;
using Facepunch;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Oxide.Plugins;

public class Plugins
{
	public bool Exists(string name)
	{
		return CommunityCommon.CommonRuntime.Plugins.Plugins.Any(x => x.Name == name);
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
