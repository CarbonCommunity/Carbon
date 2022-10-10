///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
///

using UnityEngine;
using Object = UnityEngine.Object;

[Hook.AlwaysPatched, Hook.Hidden]
[Hook("IDebugLogWarning"), Hook.Category(Hook.Category.Enum.Core)]
[Hook.Patch(typeof(Debug), "LogWarning", true, typeof(object), typeof(Object))]
public class Debug_LogWarning
{
	public static bool Prefix(object message, Object context)
	{
		try
		{
			var log = message.ToString();

			if (log.Contains("failed to sample navmesh at position") ||
				log.Contains("not close enough to the NavMesh"))
			{
				return false;
			}
		}
		catch { }

		return true;
	}
}
