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
	public byte[] raw { get; }
	public Assembly assembly { get; }
}