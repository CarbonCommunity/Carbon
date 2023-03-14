using System;
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
	public byte[] Read(string file, object sender);
	public Assembly Load(string file, object sender);
}