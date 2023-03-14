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
	public List<string> LoadedExtensions { get; }

	public byte[] Read(string file, object sender);
	public Assembly LoadComponent(string file, object sender);
	public Assembly LoadModule(string file, object sender);
	public Assembly LoadExtension(string file, object sender);
	public Assembly LoadHook(string file, object sender);
}