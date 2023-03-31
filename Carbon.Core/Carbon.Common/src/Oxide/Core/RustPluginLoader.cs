using System;
using Carbon.Core;
using Oxide.Game.Rust;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Oxide.Core.Plugins
{
	public class RustPluginLoader : PluginLoader
	{
		public override Type[] CorePlugins
		{
			get
			{
				return new Type[]
				{
					typeof(CorePlugin)
				};
			}
		}
	}
}
