using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Carbon.Components;
using Carbon.Profiler;

namespace Carbon.Managers;

/// <summary>
/// Reads, converts and loads addon assemblies, caching them by content hash.
/// </summary>
public class AssemblyLoader : IDisposable
{
	private readonly Dictionary<string, CachedAssembly> _cache = new();

	private readonly string[] _directoryList =
	{
		Defines.GetManagedFolder(),
		Defines.GetHooksFolder(),
		Defines.GetExtensionsFolder(),
	};

	internal byte[] _checksumBuffer = new byte[20];
	internal IReadOnlyList<byte> _needleBuffer = [0x01, 0xdc, 0x7f, 0x01];

	internal void ResetChechsum()
	{
		for (int i = 0; i < _checksumBuffer.Length; i++)
		{
			_checksumBuffer[i] = default;
		}
	}

	internal CachedAssembly Load(string file, string requester, string[] directories, AddonType type = AddonType.Default)
	{
		file = Path.GetFileName(file);

		string path = default;
		foreach (string directory in (directories is null) ? _directoryList : directories)
		{
			if (!File.Exists(Path.Combine(directory, file))) continue;
			path = Path.Combine(directory, file);
		}

		if (string.IsNullOrEmpty(path))
		{
			return null;
		}

		byte[] raw = File.ReadAllBytes(path);

		if (!Services.Assemblies.Convert(file, type, ref raw))
		{
			return default;
		}

		string sha1 = Util.sha1(raw);

		if (_cache.TryGetValue(sha1, out var cache))
		{
			return cache;
		}

		Assembly result;

		if (IndexOf(raw, _needleBuffer) == 0)
		{
			ResetChechsum();
			Buffer.BlockCopy(raw, 4, _checksumBuffer, 0, 20);
			result = Assembly.Load(Package(_checksumBuffer, raw, 24));
		}
		else
		{
			result = Assembly.Load(raw);
		}

		switch (type)
		{
			case AddonType.Extension:
			{
				MonoProfiler.TryStartProfileFor(MonoProfilerConfig.ProfileTypes.Extension, result, Path.GetFileNameWithoutExtension(file));
				Assemblies.Extensions.Update(Path.GetFileNameWithoutExtension(file), result, file);
				break;
			}
		}

		cache = new CachedAssembly { Name = file, Raw = raw, Assembly = result };
		_cache.Add(sha1, cache);
		return cache;
	}

	internal CachedAssembly ReadFromCache(string name)
	{
		return _cache.Select(x => x.Value).LastOrDefault(x => x.Name == name);
	}

	internal static byte[] Package(IReadOnlyList<byte> a, IReadOnlyList<byte> b, int c = 0)
	{
		var buffer = new byte[b.Count - c];

		for (int i = c; i < b.Count; i++)
		{
			buffer[i - c] = (byte)(b[i] ^ a[(i - c) % a.Count]);
		}

		return buffer;
	}

	internal static int IndexOf(IReadOnlyList<byte> haystack, IReadOnlyList<byte> needle)
	{
		int len = needle.Count;
		int limit = haystack.Count - len;

		for (int i = 0; i <= limit; i++)
		{
			int k = 0;
			for (; k < len; k++)
			{
				if (needle[k] != haystack[i + k])
				{
					break;
				}
			}

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
