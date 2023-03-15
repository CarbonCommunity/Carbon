//#define DEBUG_VERBOSE

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using API.Contracts;
using Utility;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Components.Loaders;

internal sealed class LibraryLoader : IDisposable
{
	private class Item : IAssemblyCache
	{
		public string Name { get; internal set; }
		public byte[] Raw { get; internal set; }
		public Assembly Assembly { get; internal set; }
	}

	private AppDomain _domain;
	private readonly Hash<string, Item> _cache = new();

	private readonly string[] _directoryList =
	{
		Context.CarbonLib,
		Context.GameManaged
	};

	internal AppDomain GetDomain()
		=> _domain;

	internal void RegisterDomain(AppDomain domain)
	{
		_domain = domain;
		_domain.AssemblyResolve += ResolveAssembly;
		Logger.Log($"Library resolver attached to '{_domain.FriendlyName}'");
	}

	internal void UnregisterDomain()
	{
		_domain = null;
		_domain.AssemblyResolve -= ResolveAssembly;
		Logger.Log($"Library resolver detached from '{_domain.FriendlyName}'");
	}

	internal Assembly ResolveAssembly(object sender, ResolveEventArgs args)
	{
		AssemblyName assemblyName = new AssemblyName(args.Name);
		string requester = args.RequestingAssembly?.GetName().Name ?? "unknown";
		return ResolveAssembly(assemblyName.Name, requester).Assembly;
	}

	internal IAssemblyCache ResolveAssembly(string name, string requester)
	{
		string path = default;

#if DEBUG_VERBOSE
		Logger.Debug($"Resolve library '{name}' requested by '{requester}'");
#endif

		foreach (string directory in _directoryList)
		{
			if (!File.Exists(Path.Combine(directory, $"{name}.dll"))) continue;
			path = Path.Combine(directory, $"{name}.dll");
		}

		if (path.IsNullOrEmpty())
		{
			Logger.Debug($"Unresolved library: '{name}'");
			return default;
		}

		byte[] raw = File.ReadAllBytes(path);
		string sha1 = Util.SHA1(raw);

		if (_cache.TryGetValue(sha1, out Item cache))
		{
#if DEBUG_VERBOSE
			Logger.Debug($"Resolved library from cache: "
				+ $"'{cache.Assembly.GetName().Name}' v{cache.Assembly.GetName().Version}");
#endif
			return cache;
		}

		Assembly asm = Assembly.LoadFile(path);
		cache = new Item { Name = name, Raw = raw, Assembly = asm };
		_cache.Add(sha1, cache);

#if DEBUG_VERBOSE
		Logger.Debug($"Resolved library: '{asm.GetName().Name}' v{asm.GetName().Version}");
#endif

		return cache;
	}

	internal IAssemblyCache ReadFromCache(string name)
	{
		Item item = _cache.Select(x => x.Value).Last(x => x.Name == name);
		return item ?? default;
	}

	private bool disposedValue;

	private void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
				_cache.Clear();
			disposedValue = true;

			_domain.AssemblyResolve -= ResolveAssembly;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}