using System;
using System.Collections.Generic;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace API.Assembly;

public interface IAssemblyManager
{
	public IAddonManager Components { get; }
	public IAddonManager Extensions { get; }
	public IAddonManager Hooks { get; }
	public IAddonManager Modules { get; }

#if EXPERIMENTAL
	public IAddonManager Plugins { get; }
#endif

	public byte[] Read(string file);
	public IReadOnlyList<string> RefBlacklist { get; }
	public IReadOnlyList<string> RefWhitelist { get; }
	public bool IsType<T>(System.Reflection.Assembly assembly, out IEnumerable<Type> output);
}
