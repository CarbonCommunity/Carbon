using API.Events;
using Carbon.Profiler;
using Timer = Oxide.Plugins.Timer;

namespace Carbon.Core;

#pragma warning disable IDE0051

public partial class CorePlugin
{
	public static MonoProfiler.Sample ProfileSample = MonoProfiler.Sample.Create();

	private string _pluginProfile;
	private float _pluginProfileDuration;
	private MonoProfiler.ProfilerArgs _pluginProfileFlags;
	private Timer _pluginProfileTimeout;

	[CommandVar("profilestatus", "Mono profiling status.")]
	[AuthLevel(2)]
	private bool IsProfiling
	{
		get { return MonoProfiler.IsRecording; }
		set { }
	}

	[ConsoleCommand("profile", "Toggles recording status of the Carbon native Mono-profiling. Syntax: c.profile [duration] [-cm] [-am] [-t] [-c] [-gc]")]
	[AuthLevel(2)]
	private void Profile(ConsoleSystem.Arg arg)
	{
		var duration = arg.GetFloat(0);
		var flags = GetProfilerFlags(arg);

		if (MonoProfiler.IsRecording)
		{
			Analytics.profiler_ended(flags, MonoProfiler.CurrentDurationTime.TotalSeconds, false);
			MonoProfiler.ToggleProfiling(flags);
			ProfileSample.Resample();
			MonoProfiler.Clear();
			return;
		}

		if (duration <= 0)
		{
			MonoProfiler.ToggleProfiling(flags);
			Analytics.profiler_started(flags, false);
		}
		else
		{
			MonoProfiler.ToggleProfilingTimed(duration, flags, args =>
			{
				Analytics.profiler_ended(flags, duration, true);
				ProfileSample.Resample();
				MonoProfiler.Clear();
			});
			Analytics.profiler_started(flags, true);
		}
	}

	[ConsoleCommand("profileabort", "Aborts recording of the Carbon native Mono-profiling if it was recording.")]
	[AuthLevel(2)]
	private void ProfileAbort(ConsoleSystem.Arg arg)
	{
		if (!MonoProfiler.IsRecording && _pluginProfile == null)
		{
			arg.ReplyWith("No profiling process active");
			return;
		}

		if (_pluginProfile != null)
		{
			arg.ReplyWith($"Cancelled profiling '{_pluginProfile}'");
		}

		if (MonoProfiler.IsRecording)
		{
			MonoProfiler.ToggleProfiling(MonoProfiler.ProfilerArgs.Abort);
			ProfileSample.Clear();
		}

		EndPluginProfile();
	}

	[ConsoleCommand("profile.plugin", "Profiles one plugin: recompiles it with profiler hooks, records for the duration, then recompiles it without them. Syntax: c.profile.plugin <name> <duration> [-cm] [-am] [-t] [-c] [-gc] [-sw]")]
	[AuthLevel(2)]
	private void ProfilePlugin(ConsoleSystem.Arg arg)
	{
		var name = arg.GetString(0);
		var duration = arg.GetFloat(1);

		if (string.IsNullOrEmpty(name) || duration < 1f)
		{
			arg.ReplyWith("Syntax: c.profile.plugin <name> <duration> [-cm] [-am] [-t] [-c] [-gc] [-sw]");
			return;
		}

		if (MonoProfiler.IsRecording || _pluginProfile != null)
		{
			arg.ReplyWith("Profiler is actively recording");
			return;
		}

		if (!Community.Runtime.MonoProfilerConfig.TrackCalls)
		{
			arg.ReplyWith("Call tracking is off. Set TrackCalls to true in config.profiler.json and restart the server.");
			return;
		}

		var plugin = ModLoader.FindPlugin(name);

		if (plugin == null || plugin.IsPrecompiled)
		{
			arg.ReplyWith($"Couldn't find a script plugin named '{name}'");
			return;
		}

		if (!MonoProfiler.Start())
		{
			arg.ReplyWith("Mono profiler couldn't start. Check the server log.");
			return;
		}

		_pluginProfile = plugin.Name;
		_pluginProfileDuration = duration;
		_pluginProfileFlags = GetProfilerFlags(arg);

		if (Assemblies.Plugins.Get(plugin.Name) is { IsProfiledAssembly: true })
		{
			arg.ReplyWith(StartPluginProfile()
				? $"Profiling '{plugin.Name}' for {duration}s"
				: "Couldn't start recording. Check the server log.");
			return;
		}

		Community.Runtime.MonoProfilerConfig.TemporaryPlugins.Add(plugin.Name);
		Community.Runtime.Events.Subscribe(CarbonEvent.PluginLoaded, OnProfiledPluginLoaded);
		_pluginProfileTimeout = timer.In(60f, () =>
		{
			Logger.Warn($"'{_pluginProfile}' didn't load within 60s, cancelled profiling it");
			EndPluginProfile();
		});

		plugin.ProcessorProcess.MarkDirty();
		arg.ReplyWith($"Recompiling '{plugin.Name}' with profiler hooks; profiling starts once it has loaded");
	}

	private void OnProfiledPluginLoaded(EventArgs args)
	{
		if (args is not CarbonEventArgs { Payload: RustPlugin plugin } || plugin.Name != _pluginProfile)
		{
			return;
		}

		Community.Runtime.Events.Unsubscribe(CarbonEvent.PluginLoaded, OnProfiledPluginLoaded);
		_pluginProfileTimeout?.Destroy();
		_pluginProfileTimeout = null;
		StartPluginProfile();
	}

	private bool StartPluginProfile()
	{
		if (MonoProfiler.IsRecording)
		{
			Logger.Warn($"Another recording started while '{_pluginProfile}' was recompiling, cancelled profiling it");
			EndPluginProfile();
			return false;
		}

		var duration = _pluginProfileDuration;
		var flags = _pluginProfileFlags;

		MonoProfiler.ToggleProfilingTimed(duration, flags, _ =>
		{
			Analytics.profiler_ended(flags, duration, true);
			ProfileSample.Resample();
			MonoProfiler.Clear();
		});

		if (!MonoProfiler.IsRecording)
		{
			EndPluginProfile();
			return false;
		}

		MonoProfiler.RecordingStopped += EndPluginProfile;
		Analytics.profiler_started(flags, true);
		return true;
	}

	private void EndPluginProfile()
	{
		if (_pluginProfile == null)
		{
			return;
		}

		var name = _pluginProfile;
		_pluginProfile = null;
		MonoProfiler.RecordingStopped -= EndPluginProfile;
		Community.Runtime.Events.Unsubscribe(CarbonEvent.PluginLoaded, OnProfiledPluginLoaded);
		_pluginProfileTimeout?.Destroy();
		_pluginProfileTimeout = null;

		if (Community.Runtime.MonoProfilerConfig.TemporaryPlugins.Remove(name))
		{
			ModLoader.FindPlugin(name)?.ProcessorProcess?.MarkDirty();
		}
	}

	private static MonoProfiler.ProfilerArgs GetProfilerFlags(ConsoleSystem.Arg arg)
	{
		var flags = MonoProfiler.ProfilerArgs.None;

		if (arg.HasArg("-cm")) flags |= MonoProfiler.ProfilerArgs.CallMemory;
		if (arg.HasArg("-am")) flags |= MonoProfiler.ProfilerArgs.AdvancedMemory;
		if (arg.HasArg("-t")) flags |= MonoProfiler.ProfilerArgs.Timings;
		if (arg.HasArg("-c")) flags |= MonoProfiler.ProfilerArgs.Calls;
		if (arg.HasArg("-gc")) flags |= MonoProfiler.ProfilerArgs.GCEvents;
		if (arg.HasArg("-sw")) flags |= MonoProfiler.ProfilerArgs.StackWalkAllocations;

		return flags == MonoProfiler.ProfilerArgs.None ? MonoProfiler.AllFlags : flags;
	}

	[ConsoleCommand("profiler.print", "If any parsed data available, it'll print basic and advanced information. (-c=CSV, -j=JSON, -t=Table, -p=ProtoBuf [default])")]
	[AuthLevel(2)]
	private void ProfilerPrint(ConsoleSystem.Arg arg)
	{
		if (MonoProfiler.IsRecording)
		{
			arg.ReplyWith("Profiler is actively recording");
			return;
		}

		var mode = arg.GetString(0);

		switch (mode)
		{
			case "-c":
				arg.ReplyWith(WriteFileString("csv", ProfileSample.ToCSV()));
				break;

			case "-j":
				arg.ReplyWith(WriteFileString("json", ProfileSample.ToJson(true)));
				break;

			case "-t":
				arg.ReplyWith(WriteFileString("txt", ProfileSample.ToTable()));
				break;

			default:
			case "-p":
				arg.ReplyWith(WriteFileBytes(MonoProfiler.ProfileExtension, ProfileSample.ToProto()));
				break;

		}

		static string WriteFileString(string extension, string data)
		{
			var date = DateTime.Now;
			var file = Path.Combine(Defines.GetProfilesFolder(), $"profile-{date.Year}_{date.Month:00}_{date.Day:00}_{date.Hour:00}{date.Minute:00}{date.Second:00}.{extension}");
			OsEx.File.Create(file, data);

			return $"Exported profile output at '{file}'";
		}
		static string WriteFileBytes(string extension, byte[] data)
		{
			var date = DateTime.Now;
			var file = Path.Combine(Defines.GetProfilesFolder(), $"profile-{date.Year}_{date.Month:00}_{date.Day:00}_{date.Hour:00}{date.Minute:00}{date.Second:00}.{extension}");
			OsEx.File.Create(file, data);

			return $"Exported profile output at '{file}'";
		}
	}

	[ConsoleCommand("profiler.tracks", "All tracking lists present in the config which are used by the Mono profiler for tracking.")]
	[AuthLevel(2)]
	private void ProfilerTracked(ConsoleSystem.Arg arg)
	{
		arg.ReplyWith($"Tracked Assemblies ({Community.Runtime.MonoProfilerConfig.Assemblies.Count:n0}):\n" +
		              $"{Community.Runtime.MonoProfilerConfig.Assemblies.Select(x => $"- {x}").ToString("\n")}\n" +
		              $"Tracked Plugins ({Community.Runtime.MonoProfilerConfig.Plugins.Count:n0}):\n" +
		              $"{Community.Runtime.MonoProfilerConfig.Plugins.Select(x => $"- {x}").ToString("\n")}\n" +
		              $"Tracked Modules ({Community.Runtime.MonoProfilerConfig.Modules.Count:n0}):\n" +
		              $"{Community.Runtime.MonoProfilerConfig.Modules.Select(x => $"- {x}").ToString("\n")}\n" +
		              $"Tracked Extensions ({Community.Runtime.MonoProfilerConfig.Extensions.Count:n0}):\n" +
		              $"{Community.Runtime.MonoProfilerConfig.Extensions.Select(x => $"- {x}").ToString("\n")}\n" +
		              $"Use wildcard (*) to include all.");
	}

	[ConsoleCommand("profiler.track", "Adds an object to be tracked. Reloading the plugin will start tracking. Restarting required for assemblies, modules and extensions.")]
	[AuthLevel(2)]
	private void ProfilerTrackPlugin(ConsoleSystem.Arg arg)
	{
		if (!arg.HasArgs(2))
		{
			InvalidReturn(arg);
			return;
		}

		var type = arg.GetString(0);
		var value = arg.GetString(1);
		MonoProfilerConfig.ProfileTypes returnType = default;

		bool? returnVal = type switch
		{
			"assembly" => Community.Runtime.MonoProfilerConfig.AppendProfile(
				returnType = MonoProfilerConfig.ProfileTypes.Assembly, value),
			"plugin" => Community.Runtime.MonoProfilerConfig.AppendProfile(
				returnType = MonoProfilerConfig.ProfileTypes.Plugin, value),
			"module" => Community.Runtime.MonoProfilerConfig.AppendProfile(
				returnType = MonoProfilerConfig.ProfileTypes.Module, value),
			"ext" => Community.Runtime.MonoProfilerConfig.AppendProfile(
				returnType = MonoProfilerConfig.ProfileTypes.Extension, value),
			_ => null
		};

		if (returnVal == null)
		{
			InvalidReturn(arg);
			return;
		}

		arg.ReplyWith(returnVal.Value
			? $" Added {returnType} object '{value}' to tracking"
			: $" Couldn't add {returnType} object '{value}' for tracking");

		if (returnVal.Value) Community.Runtime.SaveMonoProfilerConfig();

		static void InvalidReturn(ConsoleSystem.Arg arg)
		{
			arg.ReplyWith("Syntax: c.profiler.track (assembly|plugin|module|ext) value");
		}
	}

	[ConsoleCommand("profiler.untrack", "Removes a plugin from being tracked. Reloading the plugin will remove it from being tracked. Restarting required for assemblies, modules and extensions.")]
	[AuthLevel(2)]
	private void ProfilerRemovePlugin(ConsoleSystem.Arg arg)
	{
		if (!arg.HasArgs(2))
		{
			InvalidReturn(arg);
			return;
		}

		var type = arg.GetString(0);
		var value = arg.GetString(1);
		MonoProfilerConfig.ProfileTypes returnType = default;

		bool? returnVal = type switch
		{
			"assembly" => Community.Runtime.MonoProfilerConfig.RemoveProfile(
				returnType = MonoProfilerConfig.ProfileTypes.Assembly, value),
			"plugin" => Community.Runtime.MonoProfilerConfig.RemoveProfile(
				returnType = MonoProfilerConfig.ProfileTypes.Plugin, value),
			"module" => Community.Runtime.MonoProfilerConfig.RemoveProfile(
				returnType = MonoProfilerConfig.ProfileTypes.Module, value),
			"ext" => Community.Runtime.MonoProfilerConfig.RemoveProfile(
				returnType = MonoProfilerConfig.ProfileTypes.Extension, value),
			_ => null
		};

		if (returnVal == null)
		{
			InvalidReturn(arg);
			return;
		}

		arg.ReplyWith(returnVal.Value
			? $" Removed {returnType} object '{value}' from tracking"
			: $" Couldn't remove {returnType} object '{value}' for tracking");

		if (returnVal.Value) Community.Runtime.SaveMonoProfilerConfig();

		static void InvalidReturn(ConsoleSystem.Arg arg)
		{
			arg.ReplyWith("Syntax: c.profiler.untrack (assembly|plugin|module|ext) value");
		}
	}

	[CommandVar("profiler.recwarns", "It should or should not print a reminding warning every 5 minutes when profiling for an un-set amount of time.")]
	[AuthLevel(2)]
	private bool RecordingWarnings
	{
		get { return Community.Runtime.Config.Profiler.RecordingWarnings; }
		set { Community.Runtime.Config.Profiler.RecordingWarnings = value; }
	}
}
