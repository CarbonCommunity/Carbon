using System.Collections.Generic;
using UnityEngine;

namespace Carbon.Pooling;

public class HookStringPool
{
	public static Dictionary<string, uint> HookNamePoolString = new();
	public static Dictionary<uint, string> HookNamePoolInt = new();

	public static uint GetOrAdd(string name)
	{
		if (HookNamePoolString.TryGetValue(name, out var hash))
		{
			return hash;
		}

		hash = name.ManifestHash();
		HookNamePoolString[name] = hash;
		HookNamePoolInt[hash] = name;
		return hash;
	}

	public static string GetOrAdd(uint name)
	{
		if (HookNamePoolInt.TryGetValue(name, out var hash))
		{
			return hash;
		}

		return string.Empty;
	}
}
