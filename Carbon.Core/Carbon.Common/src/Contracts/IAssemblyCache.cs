using System;
using System.Reflection;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace API.Contracts;

public interface IAssemblyCache
{
	public string Name { get; }
	public byte[] Raw { get; }
	public Assembly Assembly { get; }
}