#define DEBUG_VERBOSE

using System;
using System.IO;
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
		public byte[] raw { get; internal set; }
		public Assembly assembly { get; internal set; }
	}

	private readonly Hash<string, Item> _cache = new();

	private readonly string[] _directoryList =
	{
		Context.CarbonManaged,
		Context.CarbonHooks,
		Context.CarbonExtensions,
	};

	internal IAssemblyCache Load(string file, string requester = "unknown")
	{
		// normalize filename
		file = Path.GetFileName(file);

#if DEBUG_VERBOSE
		Logger.Debug($"Resolve assembly '{file}' requested by '{requester}'");
#endif

		string path = default;
		foreach (string directory in _directoryList)
		{
			if (!File.Exists(Path.Combine(directory, file))) continue;
			path = Path.Combine(directory, file);
		}

		if (path.IsNullOrEmpty())
		{
			Logger.Debug($"Unresolved assembly: '{file}'");
			return default;
		}

		byte[] raw = File.ReadAllBytes(path);
		string sha1 = Util.SHA1(raw);

		if (_cache.TryGetValue(sha1, out Item cache))
		{
#if DEBUG_VERBOSE
			Logger.Debug($"Resolved assembly from cache: "
				+ $"'{cache.assembly.GetName().Name}' v{cache.assembly.GetName().Version}");
#endif
			return cache;
		}

		Assembly asm = Assembly.Load(raw);
		cache = new Item { raw = raw, assembly = asm };
		_cache.Add(sha1, cache);

#if DEBUG_VERBOSE
		Logger.Debug($"Resolved assembly: '{asm.GetName().Name}' v{asm.GetName().Version}");
#endif

		return cache;
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