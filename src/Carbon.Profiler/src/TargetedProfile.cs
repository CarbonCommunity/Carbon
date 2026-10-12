using System;
using System.IO;
using System.Linq;
using Carbon.Components;
using Carbon.Profiler;

namespace Carbon;

/// <summary>
/// Profiles a single HarmonyMod for one recording: it is reloaded with profiler hooks,
/// recorded, then reloaded without them.
/// </summary>
public static class TargetedProfile
{
	private const string SelfName = "Carbon.Profiler";

	private static string _name;
	private static float _duration;
	private static MonoProfiler.ProfilerArgs _flags;
	private static bool _modWasLoaded;

	public static bool IsActive => _name != null;

	public static string StartMod(string name, float duration, MonoProfiler.ProfilerArgs flags)
	{
		if (name.Equals(SelfName, StringComparison.OrdinalIgnoreCase))
		{
			return $"{SelfName} can't profile itself";
		}

		if (!File.Exists(Path.Combine(HarmonyLoader.modPath, $"{name}.dll")))
		{
			return $"Couldn't find '{name}.dll' in '{HarmonyLoader.modPath}'";
		}

		if (!MonoProfiler.Start())
		{
			return "Mono profiler couldn't start. Check the server log.";
		}

		_name = name;
		_duration = duration;
		_flags = flags;
		_modWasLoaded = HarmonyLoader.GetHarmonyMods().Any(mod => mod.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
		MonoProfilerConfig.Instance.TemporaryHarmonyMods.Add(name);

		if (!HarmonyLoader.TryLoadMod(name))
		{
			End();
			return $"Couldn't load '{name}', see the HarmonyLoader log";
		}

		Record();
		return $"Reloaded '{name}' with profiler hooks, profiling it for {duration}s";
	}

	public static void End()
	{
		if (_name == null)
		{
			return;
		}

		var name = _name;
		_name = null;

		MonoProfilerConfig.Instance.TemporaryHarmonyMods.Remove(name);

		if (_modWasLoaded)
		{
			HarmonyLoader.TryLoadMod(name);
		}
		else
		{
			HarmonyLoader.TryUnloadMod(name);
		}
	}

	private static void Record()
	{
		MonoProfiler.ToggleProfilingTimed(_duration, _flags, _ =>
		{
			HarmonyProfiler.ProfileSample.Resample();
			MonoProfiler.Clear();
			End();
		});
	}
}
