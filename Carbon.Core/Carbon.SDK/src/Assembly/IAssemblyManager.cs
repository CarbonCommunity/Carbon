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
	public List<string> LoadedModules { get; }
	public List<string> LoadedComponents { get; }
	public List<string> LoadedExtensions { get; }

	public IReadOnlyList<string> References { get; }

	public byte[] Read(string file, string requester);
	public System.Reflection.Assembly LoadComponent(string file, string requester);
	public System.Reflection.Assembly LoadModule(string file, string requester);
	public System.Reflection.Assembly LoadExtension(string file, string requester);
	public System.Reflection.Assembly LoadHook(string file, string requester);
}