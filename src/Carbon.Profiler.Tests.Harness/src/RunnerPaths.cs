using System.IO;
using System.Reflection;

namespace Carbon.Profiler.Tests.Harness;

internal static class RunnerPaths
{
	public static readonly string ModPath = GetModPath();

	private static string GetModPath()
	{
		var harmonyLoader = typeof(HarmonyLoader);
		var modPath = harmonyLoader.GetField("modPath", BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(null) as string;
		if (!string.IsNullOrWhiteSpace(modPath))
		{
			return modPath;
		}

		return Path.Combine(Directory.GetCurrentDirectory(), "HarmonyMods");
	}
}
