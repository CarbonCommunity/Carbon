using System.Collections.Generic;
using System.Linq;
using Carbon;
using Carbon.Core;
using Oxide.Core.Plugins;
using Oxide.Plugins;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

public class PluginManager
{
	public string ConfigPath => Defines.GetConfigsFolder();

	public bool AddPlugin(RustPlugin plugin)
	{
		if (!Community.Runtime.Plugins.Plugins.Any(x => x == plugin))
		{
			Community.Runtime.Plugins.Plugins.Add(plugin);
			return true;
		}

		return false;
	}
	public bool RemovePlugin(RustPlugin plugin)
	{
		if(Community.Runtime.Plugins.Plugins.Any(x => x == plugin))
		{
			Community.Runtime.Plugins.Plugins.Remove(plugin);
			return true;
		}

		return false;
	}

	public Plugin GetPlugin(string name)
	{
		if (name == "RustCore") return Community.Runtime.CorePlugin;

		return Community.Runtime.Plugins.Plugins.FirstOrDefault(x => x.Name == name);
	}
	public IEnumerable<Plugin> GetPlugins()
	{
		return Community.Runtime.Plugins.Plugins.AsEnumerable();
	}
}
