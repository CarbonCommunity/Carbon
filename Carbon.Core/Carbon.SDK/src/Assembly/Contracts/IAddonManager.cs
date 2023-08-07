using System;
using System.Collections.Generic;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace API.Assembly;

public interface IAddonManager
{
	public byte[] Read(string file);
	public IReadOnlyList<string> Loaded { get; }
	public IReadOnlyList<Type> LoadedTypes { get; }
	public System.Reflection.Assembly Load(string file, string requester);

	//public bool IsLoaded(string file);
	public void Unload(string file, string requester);
	public void Reload(string requester);
}
