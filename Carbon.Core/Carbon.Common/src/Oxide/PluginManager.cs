using System.Collections.Generic;
using System.Linq;
using Carbon;
using Carbon.Core;
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
		if (!CommunityCommon.CommonRuntime.Plugins.Plugins.Any(x => x == plugin))
		{
			CommunityCommon.CommonRuntime.Plugins.Plugins.Add(plugin);
			return true;
		}

		return false;
	}
	public bool RemovePlugin(RustPlugin plugin)
	{
		if(CommunityCommon.CommonRuntime.Plugins.Plugins.Any(x => x == plugin))
		{
			CommunityCommon.CommonRuntime.Plugins.Plugins.Remove(plugin);
			return true;
		}

		return false;
	}

	public Plugin GetPlugin(string name)
	{
		if (name == "RustCore") return CommunityCommon.CommonRuntime.CorePlugin;

		return CommunityCommon.CommonRuntime.Plugins.Plugins.FirstOrDefault(x => x.Name == name);
	}
	public IEnumerable<Plugin> GetPlugins()
	{
		return CommunityCommon.CommonRuntime.Plugins.Plugins.AsEnumerable();
	}
}
