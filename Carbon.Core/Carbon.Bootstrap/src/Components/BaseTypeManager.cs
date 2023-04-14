using System.Collections.Generic;
using System.Reflection;
using API.Assembly;
using Loaders;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Components;
#pragma warning disable IDE0051

internal abstract class BaseTypeManager : BaseMonoBehaviour, IAssemblyTypeManager
{
	internal readonly AssemblyLoader _loader = new();

	internal IAssemblyManager AssemblyManager
	{ get => GetComponentInParent<IAssemblyManager>(); }

	public List<string> Loaded
	{ get; private set; } = new();

	public byte[] Read(string file)
		=> _loader.ReadFromCache(file).Raw;

	public abstract Assembly Load(string file, string requester);
}
