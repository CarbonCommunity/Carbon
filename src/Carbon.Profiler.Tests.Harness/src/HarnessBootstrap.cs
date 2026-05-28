using UnityEngine;
using Object = UnityEngine.Object;

namespace Carbon.Profiler.Tests.Harness;

internal static class HarnessBootstrap
{
	private static bool _started;

	public static void Start()
	{
		if (_started)
		{
			return;
		}

		_started = true;
		var go = new GameObject("Carbon Profiler Test Harness");
		Object.DontDestroyOnLoad(go);
		go.AddComponent<Runner>();
	}
}
