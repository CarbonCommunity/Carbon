using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Carbon.Managers;
#pragma warning disable IDE0051

/// <summary>
/// Base of the addon loaders: loads assemblies of one addon kind and keeps track of their types.
/// </summary>
public abstract class AddonManager : FacepunchBehaviour
{
	internal class Item
	{
		public byte[] PostProcessedRaw { get; internal set; }
		public ICarbonAddon Addon { get; internal set; }
		public IReadOnlyList<Type> Types { get; internal set; }
		public IReadOnlyList<Type> Shared { get; internal set; }
		public string File { get; internal set; }
	}

	internal readonly AssemblyLoader _loader = new();

	internal AssemblyManager AssemblyManager => Services.Assemblies;

	internal List<Item> _loaded { get; set; } = new();

	public WatchFolder Watcher { get; internal set; }

	public IReadOnlyDictionary<Type, KeyValuePair<string, byte[]>> Loaded
	{
		get
		{
			var dictionary = new Dictionary<Type, KeyValuePair<string, byte[]>>();
			foreach (var item in _loaded)
			{
				foreach (var type in item.Types)
				{
					if (!dictionary.ContainsKey(type))
					{
						dictionary.Add(type, new KeyValuePair<string, byte[]>(item.File, item.PostProcessedRaw));
					}
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
					if (!dictionary.ContainsKey(type))
					{
						dictionary.Add(type, item.File);
					}
				}
			}

			return dictionary;
		}
	}

	public byte[] Read(string file)
		=> _loader.ReadFromCache(file).Raw;

	public abstract Assembly Load(string file, string requester);
	public abstract void Unload(string file, string requester);
}
