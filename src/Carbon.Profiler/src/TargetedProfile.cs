using System;
using System.IO;
using System.Linq;
using Carbon.Components;
using Carbon.Profiler;
using UnityEngine;

namespace Carbon;

/// <summary>
/// Profiles a single HarmonyMod or Oxide plugin for one recording: it is reloaded with profiler hooks,
/// recorded, then reloaded without them.
/// </summary>
public static class TargetedProfile
{
	private const string SelfName = "Carbon.Profiler";
	private const float PluginLoadTimeout = 60f;

	private static string _name;
	private static MonoProfilerConfig.ProfileTypes _type;
	private static float _duration;
	private static MonoProfiler.ProfilerArgs _flags;
	private static bool _modWasLoaded;
	private static Action _pluginLoadTimeout;

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

		Begin(name, MonoProfilerConfig.ProfileTypes.Harmony, duration, flags);
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

	public static string StartPlugin(string name, float duration, MonoProfiler.ProfilerArgs flags)
	{
		if (!HarmonyProfiler.IsOxideInstalled)
		{
			return "carbon.profile_plugin needs Oxide; use carbon.profile_mod for HarmonyMods";
		}

		var fileName = OxidePlugins.FindFileName(name);

		if (fileName == null)
		{
			return $"Couldn't find an Oxide plugin named '{name}'";
		}

		if (!MonoProfiler.Start())
		{
			return "Mono profiler couldn't start. Check the server log.";
		}

		Begin(fileName, MonoProfilerConfig.ProfileTypes.Plugin, duration, flags);
		MonoProfilerConfig.Instance.TemporaryPlugins.Add(fileName);
		_pluginLoadTimeout = () =>
		{
			Debug.LogWarning($"'{fileName}' didn't load within {PluginLoadTimeout}s, cancelled profiling it");
			End();
		};
		HarmonyProfiler.Runner.Invoke(_pluginLoadTimeout, PluginLoadTimeout);

		if (!OxidePlugins.Recompile(fileName))
		{
			End();
			return $"Oxide couldn't reload '{fileName}'";
		}

		return $"Recompiling '{fileName}' with profiler hooks; profiling starts once it has loaded";
	}

	public static void OnPluginLoaded(string fileName)
	{
		if (_type != MonoProfilerConfig.ProfileTypes.Plugin || _name != fileName || _pluginLoadTimeout == null)
		{
			return;
		}

		CancelPluginLoadTimeout();
		Record();
	}

	public static void End()
	{
		if (_name == null)
		{
			return;
		}

		var name = _name;
		_name = null;
		CancelPluginLoadTimeout();

		if (_type == MonoProfilerConfig.ProfileTypes.Plugin)
		{
			MonoProfilerConfig.Instance.TemporaryPlugins.Remove(name);
			OxidePlugins.Recompile(name);
			return;
		}

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

	private static void Begin(string name, MonoProfilerConfig.ProfileTypes type, float duration, MonoProfiler.ProfilerArgs flags)
	{
		_name = name;
		_type = type;
		_duration = duration;
		_flags = flags;
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

	private static void CancelPluginLoadTimeout()
	{
		if (_pluginLoadTimeout == null)
		{
			return;
		}

		HarmonyProfiler.Runner.CancelInvoke(_pluginLoadTimeout);
		_pluginLoadTimeout = null;
	}
}
