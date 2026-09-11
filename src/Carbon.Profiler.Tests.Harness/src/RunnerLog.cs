using UnityEngine;

namespace Carbon.Profiler.Tests.Harness;

internal static class RunnerLog
{
	public const string Prefix = "[Carbon.Profiler.Tests]";

	public static void Info(string message)
	{
		Debug.LogWarning(Prefix + " " + message);
	}

	public static void Error(string message)
	{
		Debug.LogError(Prefix + " " + message);
	}
}
