using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using API.Abstracts;
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

internal abstract class AddonManager : CarbonBehaviour, IAddonManager
{
	internal class Item
	{
		public ICarbonAddon Addon { get; internal set; }
		public IReadOnlyList<Type> Types { get; internal set; }
		public IReadOnlyList<Type> Shared { get; internal set; }
		public string File { get; internal set; }
	}

	internal readonly AssemblyLoader _loader = new();

	internal IAssemblyManager AssemblyManager
	{ get => GetComponentInParent<IAssemblyManager>(); }

	internal List<Item> _loaded
	{ get; set; } = new();

	public IReadOnlyDictionary<Type, string> Loaded
	{
		get
		{
			var dictionary = new Dictionary<Type, string>();
			foreach (var item in _loaded)
			{
				foreach (var type in item.Types)
				{
					dictionary.Add(type, item.File);
				}
			}

			return dictionary;
		}
	}
	public IReadOnlyDictionary<Type, string> Shared
	{
		get
		{
			var dictionary = new Dictionary<Type, string>();
			foreach (var item in _loaded)
			{
				foreach (var type in item.Shared)
				{
					dictionary.Add(type, item.File);
				}
			}

			return dictionary;
		}
	}

	public byte[] Read(string file)
		=> _loader.ReadFromCache(file).Raw;

	public abstract Assembly Load(string file, string requester);
	public abstract void Unload(string file, string requester);
	public abstract void Reload(string requester);

	internal virtual void Hydrate(Assembly assembly, ICarbonAddon addon)
	{
		// BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		// foreach (Type type in assembly.GetTypes())
		// {
		// 	foreach (MethodInfo method in type.GetMethods(flags))
		// 	{
		// 		// Community.Runtime.HookManager.IsHookLoaded(method.Name)
		// 		// Community.Runtime.HookManager.Subscribe(method.Name, Name);
		// 	}
		// }
	}
}
