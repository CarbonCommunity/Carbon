///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
///
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Carbon.Common;

internal class Plugin
{
	internal AppDomain domain;

	private Guid guid;
	internal string filePath, fileName, fileExt;

	internal string Identifier
	{
		get => guid.ToString();
	}

	internal bool IsLoaded
	{
		get => (assembly != null);
	}

	internal Assembly assembly;
	internal Type[] types;
	internal AssemblyName[] references;

	internal List<IHarmonyModHooks> hooks;
	internal HarmonyLib.Harmony harmonyInstance;

	public Plugin()
	{
		guid = Guid.NewGuid();
		hooks = new List<IHarmonyModHooks>();
	}

	public override string ToString()
		=> $"{fileName}{fileExt}";
}