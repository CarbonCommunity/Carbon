using System;
using System.Collections.Generic;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace API.Assembly;

public interface ITypeManager
{
	public IReadOnlyList<string> Loaded { get; }
	public IReadOnlyList<Type> LoadedTypes { get; }

	//public bool IsLoaded(string file);
	//public void UnLoad(string file, string requester);

	public byte[] Read(string file);
	public System.Reflection.Assembly Load(string file, string requester);

}
