///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
///

using Harmony;
using UnityEngine;
using Object = UnityEngine.Object;

[HarmonyPatch(typeof(Debug), "LogWarning", typeof(object), typeof(Object))]
public class Debug_LogWarning
{
	public static bool Prefix(object message, Object context)
	{
		try
		{
			if (message.ToString().Contains("failed to sample navmesh at position") || message.ToString().Contains("not close enough to the NavMesh"))
			{
				return false;
			}
		}
		catch { }

		return true;
	}
}
