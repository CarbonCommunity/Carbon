using System;
using System.Collections.Generic;
using System.Reflection;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace API.Contracts;

public interface IAssemblyManager
{
	public List<string> LoadedModules { get; }
	public List<string> LoadedComponents { get; }
	public List<string> LoadedExtensions { get; }

	public string[] References { get; }

	public byte[] Read(string file, string requester);
	public Assembly LoadComponent(string file, string requester);
	public Assembly LoadModule(string file, string requester);
	public Assembly LoadExtension(string file, string requester);
	public Assembly LoadHook(string file, string requester);
}