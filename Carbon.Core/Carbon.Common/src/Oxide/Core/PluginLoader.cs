using System;
using Carbon.Core;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Oxide.Game.Rust
{
	public class PluginLoader
	{
		public virtual Type[] CorePlugins { get; }
	}
}
