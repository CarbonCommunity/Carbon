using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using API.Assembly;
using Utility;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Loaders;

internal sealed class AssemblyLoader : IDisposable
{
	private class Item : IAssemblyCache
	{
		public string Name { get; internal set; }
		public byte[] Raw { get; internal set; }
		public Assembly Assembly { get; internal set; }
	}

	private readonly Dictionary<string, Item> _cache = new();

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

		if (String.IsNullOrEmpty(path))
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

		Assembly asm;

		if (IndexOf(raw, new byte[4] { 0x01, 0xdc, 0x7f, 0x01 }) == 0)
		{
			byte[] checksum = new byte[20];
			Buffer.BlockCopy(raw, 4, checksum, 0, 20);
			asm = Assembly.Load(Package(checksum, raw, 24));
		}
		else
		{
			asm = Assembly.Load(raw);
		}

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

	private static byte[] Package(IReadOnlyList<byte> a, IReadOnlyList<byte> b, int c = 0)
	{
		byte[] retvar = new byte[b.Count - c];
		for (int i = c; i < b.Count; i++)
			retvar[i - c] = (byte)(b[i] ^ a[(i - c) % a.Count]);
		return retvar;
	}

	private static int IndexOf(IReadOnlyList<byte> haystack, IReadOnlyList<byte> needle)
	{
		int len = needle.Count;
		int limit = haystack.Count - len;

		for (int i = 0; i <= limit; i++)
		{
			int k = 0;
			for (; k < len; k++)
				if (needle[k] != haystack[i + k]) break;
			if (k == len) return i;
		}
		return -1;
	}

	private bool _disposing;

	private void Dispose(bool disposing)
	{
		if (!_disposing)
		{
			if (disposing)
				_cache.Clear();
			_disposing = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}