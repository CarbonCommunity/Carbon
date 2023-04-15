using System;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace API.Assembly;

public interface IAssemblyCache
{
	public string Name { get; }
	public byte[] Raw { get; }
	public System.Reflection.Assembly Assembly { get; }
}
