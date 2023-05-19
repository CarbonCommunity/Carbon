/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

using System.Collections.Generic;
using Oxide.Core.Plugins;

namespace Oxide.Core.Extensions;

public class ExtensionManager
{
	private List<PluginLoader> pluginloaders = new();

	public void RegisterPluginLoader(Oxide.Core.Plugins.PluginLoader loader)
	{
		pluginloaders.Add(loader);
	}
}
