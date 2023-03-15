#define DEBUG_VERBOSE

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

internal sealed class AssemblyLoader : IDisposable
{
	private class Item : IAssemblyCache
	{
		public string Name { get; internal set; }
		public byte[] Raw { get; internal set; }
		public Assembly Assembly { get; internal set; }
	}

	private readonly Hash<string, Item> _cache = new();

	private readonly string[] _directoryList =
	{
		Context.CarbonManaged,
		Context.CarbonHooks,
		Context.CarbonExtensions,
	};

	internal IAssemblyCache Load(string file, string requester = "unknown", string[] directories = null)
	{
		// normalize filename
		file = Path.GetFileName(file);

#if DEBUG_VERBOSE
		Logger.Debug($"Load assembly '{file}' requested by '{requester}'");
#endif

		string path = default;
		foreach (string directory in (directories is null) ? _directoryList : directories)
		{
			if (!File.Exists(Path.Combine(directory, file))) continue;
			path = Path.Combine(directory, file);
		}

		if (path.IsNullOrEmpty())
		{
			Logger.Debug($"Unable to load assembly: '{file}'");
			return default;
		}

		byte[] raw = File.ReadAllBytes(path);
		string sha1 = Util.SHA1(raw);

		if (_cache.TryGetValue(sha1, out Item cache))
		{
#if DEBUG_VERBOSE
			Logger.Debug($"Loaded assembly from cache: "
				+ $"'{cache.Assembly.GetName().Name}' v{cache.Assembly.GetName().Version}");
#endif
			return cache;
		}

		Assembly asm = Assembly.Load(raw);
		cache = new Item { Name = file, Raw = raw, Assembly = asm };
		_cache.Add(sha1, cache);

#if DEBUG_VERBOSE
		Logger.Debug($"Loaded assembly: '{asm.GetName().Name}' v{asm.GetName().Version}");
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
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}