using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;

namespace Carbon.Utility;

public static class HookStringPool
{
	public static readonly ConcurrentDictionary<string, uint> HookNamePoolString = new();
	public static readonly ConcurrentDictionary<uint, string> HookNamePoolInt = new();

	public static uint GetOrAdd(string name)
	{
		var hash = HookNamePoolString.GetOrAdd(name, ManifestHash);
		HookNamePoolInt.TryAdd(hash, name);
		return hash;
	}


	public static string GetOrAdd(uint name)
	{
		return HookNamePoolInt.TryGetValue(name, out var hash) ? hash : string.Empty;
	}

	private static uint ManifestHash(string str)
	{
		return string.IsNullOrEmpty(str)
			? 0
			: BitConverter.ToUInt32(MD5.HashData(Encoding.UTF8.GetBytes(str)), 0);
	}
}
