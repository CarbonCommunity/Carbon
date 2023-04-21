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
	public ITypeManager Components { get; }
	public ITypeManager Extensions { get; }
	public ITypeManager Hooks { get; }
	public ITypeManager Modules { get; }

#if EXPERIMENTAL
	public IAssemblyTypeManager Plugins { get; }
#endif

	public byte[] Read(string file);
	public IReadOnlyList<string> References { get; }
	public bool IsType<T>(System.Reflection.Assembly assembly, out IEnumerable<Type> output);
}
