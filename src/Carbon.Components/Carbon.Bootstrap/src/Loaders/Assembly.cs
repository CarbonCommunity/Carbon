using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using API.Assembly;
using Carbon;
using Carbon.Components;
using Carbon.Profiler;
using Utility;
using Logger = Utility.Logger;

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

	internal byte[] _checksumBuffer = new byte[20];
	internal IReadOnlyList<byte> _needleBuffer = [0x01, 0xdc, 0x7f, 0x01];

	internal void ResetChechsum()
	{
		for (int i = 0; i < _checksumBuffer.Length; i++)
		{
			_checksumBuffer[i] = default;
		}
	}

	internal IAssemblyCache Load(string file, string requester, string[] directories, IExtensionManager.ExtensionTypes extensionType = IExtensionManager.ExtensionTypes.Default)
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

		switch (extensionType)
		{
			case IExtensionManager.ExtensionTypes.Extension:
				ConversionResult oxideConvert = Community.Runtime.Compat.AttemptOxideConvert(ref raw);

				switch (oxideConvert)
				{
					case ConversionResult.Fail:
						Logger.Warn($" >> Failed Oxide extension conversion for '{file}'");
						return default;
				}
				break;

			case IExtensionManager.ExtensionTypes.HarmonyMod:
				Community.Runtime.Compat.ConvertHarmonyMod(ref raw);

				if (raw == null)
				{
					Logger.Warn($" >> Failed HarmonyMod conversion for '{file}'");
					return default;
				}
				break;
		}

		string sha1 = Util.sha1(raw);

		if (_cache.TryGetValue(sha1, out Item cache))
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

		switch (extensionType)
		{
			case IExtensionManager.ExtensionTypes.Extension:
			{
				MonoProfiler.TryStartProfileFor(MonoProfilerConfig.ProfileTypes.Extension, result, Path.GetFileNameWithoutExtension(file));
				Assemblies.Extensions.Update(Path.GetFileNameWithoutExtension(file), result, file);
				break;
			}
		}

		cache = new Item { Name = file, Raw = raw, Assembly = result };
		_cache.Add(sha1, cache);
		return cache;
	}

	internal IAssemblyCache ReadFromCache(string name)
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
