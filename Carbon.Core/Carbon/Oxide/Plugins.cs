///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;
using Facepunch;

namespace Oxide.Plugins
{
	public class Plugins
	{
		public Plugin Find(string name)
		{
			name = name.Replace(" ", "");

			foreach (var mod in CarbonLoader._loadedMods)
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
			foreach (var mod in CarbonLoader._loadedMods)
			{
				list.AddRange(mod.Plugins);
			}

			var result = list.ToArray();
			Pool.FreeList(ref list);
			return result;
		}
	}
}
