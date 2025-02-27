using Carbon.Profiler;

namespace Carbon.Core;

#pragma warning disable IDE0051

public partial class CorePlugin
{
	public static MonoProfiler.Sample ProfileSample = MonoProfiler.Sample.Create();

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
		if (!MonoProfiler.Enabled)
		{
			arg.ReplyWith("Mono profiler is disabled. Enable it in the 'carbon/config.profiler.json' config file. Must restart the server for changes to apply.");
			return;
		}

		var duration = arg.GetFloat(0);
		var flags = MonoProfiler.ProfilerArgs.None;

		if (arg.HasArg("-cm")) flags |= MonoProfiler.ProfilerArgs.CallMemory;
		if (arg.HasArg("-am")) flags |= MonoProfiler.ProfilerArgs.AdvancedMemory;
		if (arg.HasArg("-t")) flags |= MonoProfiler.ProfilerArgs.Timings;
		if (arg.HasArg("-c")) flags |= MonoProfiler.ProfilerArgs.Calls;
		if (arg.HasArg("-gc")) flags |= MonoProfiler.ProfilerArgs.GCEvents;

		if (flags == MonoProfiler.ProfilerArgs.None) flags = MonoProfiler.AllFlags;

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
		if (!MonoProfiler.IsRecording)
		{
			arg.ReplyWith("No profiling process active");
			return;
		}

		MonoProfiler.ToggleProfiling(MonoProfiler.ProfilerArgs.Abort);
		ProfileSample.Clear();
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
			var file = Path.Combine(Defines.GetProfilesFolder(), $"profile-{date.Year}_{date.Month}_{date.Day}_{date.Hour}{date.Minute}{date.Second}.{extension}");
			OsEx.File.Create(file, data);

			return $"Exported profile output at '{file}'";
		}
		static string WriteFileBytes(string extension, byte[] data)
		{
			var date = DateTime.Now;
			var file = Path.Combine(Defines.GetProfilesFolder(), $"profile-{date.Year}_{date.Month}_{date.Day}_{date.Hour}{date.Minute}{date.Second}.{extension}");
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

		var returnVal = type switch
		{
			"assembly" => Community.Runtime.MonoProfilerConfig.AppendProfile(
				returnType = MonoProfilerConfig.ProfileTypes.Assembly, value),
			"plugin" => Community.Runtime.MonoProfilerConfig.AppendProfile(
				returnType = MonoProfilerConfig.ProfileTypes.Plugin, value),
			"module" => Community.Runtime.MonoProfilerConfig.AppendProfile(
				returnType = MonoProfilerConfig.ProfileTypes.Module, value),
			"ext" => Community.Runtime.MonoProfilerConfig.AppendProfile(
				returnType = MonoProfilerConfig.ProfileTypes.Extension, value),
			_ => InvalidReturn(arg)
		};

		arg.ReplyWith(returnVal
			? $" Added {returnType} object '{value}' to tracking"
			: $" Couldn't add {returnType} object '{value}' for tracking");

		if (returnVal) Community.Runtime.SaveMonoProfilerConfig();

		static bool InvalidReturn(ConsoleSystem.Arg arg)
		{
			arg.ReplyWith("Syntax: c.profiler.track (assembly|plugin|module|ext) value");
			return false;
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

		var returnVal = type switch
		{
			"assembly" => Community.Runtime.MonoProfilerConfig.RemoveProfile(
				returnType = MonoProfilerConfig.ProfileTypes.Assembly, value),
			"plugin" => Community.Runtime.MonoProfilerConfig.RemoveProfile(
				returnType = MonoProfilerConfig.ProfileTypes.Plugin, value),
			"module" => Community.Runtime.MonoProfilerConfig.RemoveProfile(
				returnType = MonoProfilerConfig.ProfileTypes.Module, value),
			"ext" => Community.Runtime.MonoProfilerConfig.RemoveProfile(
				returnType = MonoProfilerConfig.ProfileTypes.Extension, value),
			_ => InvalidReturn(arg)
		};

		arg.ReplyWith(returnVal
			? $" Removed {returnType} object '{value}' from tracking"
			: $" Couldn't remove {returnType} object '{value}' for tracking");

		if (returnVal) Community.Runtime.SaveMonoProfilerConfig();

		static bool InvalidReturn(ConsoleSystem.Arg arg)
		{
			arg.ReplyWith("Syntax: c.profiler.untrack (assembly|plugin|module|ext) value");
			return false;
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
