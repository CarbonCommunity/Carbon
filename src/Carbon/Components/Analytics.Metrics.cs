using Facepunch;

namespace Carbon.Components;

public partial struct Analytics
{
	/// <summary>
	/// Metric executed whenever the server started up. Sends anonymous information about Carbon and Rust versions.
	/// </summary>
	public static void on_server_startup()
	{
		if (!Enabled)
		{
			return;
		}

		Singleton.
			Include("carbon", $"{Community.Runtime.Analytics.Version}/{Community.Runtime.Analytics.Platform}/{Community.Runtime.Analytics.Protocol}").
			Include("carbon_informational", Community.Runtime.Analytics.InformationalVersion).
			Include("carbon_build", $"{Build.Git.Author} on {Build.Git.Branch} [{Build.Git.HashLong}]").
			Include("rust", $"{BuildInfo.Current.Build.Number}/{Rust.Protocol.printable}").
			Submit("on_server_startup");

	}

	/// <summary>
	/// Metric executed when the server is fully initialized. Sends anonymous information about overall plugin, extension, module and hook count and initial plugin hook memory and time usage.
	/// </summary>
	public static void on_server_initialized()
	{
		if (!Enabled)
		{
			return;
		}

		Singleton.
			Include("plugin_count", ModLoader.Packages.Sum(x => x.Plugins.Count)).
			Include("plugins_totalmemoryused", $"{ByteEx.Format(ModLoader.Packages.Sum(x => x.Plugins.Sum(y => y.TotalMemoryUsed)), valueFormat: "0", stringFormat: "{0}{1}").ToLower()}").
			Include("plugins_totalhooktime", $"{ModLoader.Packages.Sum(x => x.Plugins.Sum(y => y.TotalHookTime.TotalMilliseconds)).RoundUpToNearestCount(100):0}ms").
			Include("extension_count", Community.Runtime.AssemblyEx.Extensions.Loaded.Count).
			Include("module_count", Community.Runtime.AssemblyEx.Modules.Loaded.Count).
			Include("hook_count", Community.Runtime.HookManager.LoadedDynamicHooks.Count(x => x.IsInstalled) + Community.Runtime.HookManager.LoadedStaticHooks.Count(x => x.IsInstalled)).
			Submit("on_server_initialized");
	}

	/// <summary>
	/// Metric executed whenever a plugin constructor fails. A rare error but we anonymously track it to identify potential issues.
	/// </summary>
	public static void plugin_constructor_failure(RustPlugin plugin)
	{
		if (!Enabled)
		{
			return;
		}

		Singleton.
			Include("plugin", $"{plugin.Name} v{plugin.Version} by {plugin.Author}")
			.Submit("plugin_constructor_failure");
	}

	/// <summary>
	/// Metric executed when a plugin loading batch finalized, giving us anonymous statistics about base plugin types used overall (RustPlugin, CovalencePlugin and CarbonPlugin).
	/// </summary>
	public static void batch_plugin_types()
	{
		if (!Enabled)
		{
			return;
		}

		var rustPluginCount = 0;
		var covalencePluginCount = 0;
		var carbonPluginCount = 0;

		foreach (var plugin in ModLoader.Packages.SelectMany(package => package.Plugins))
		{
			if (plugin.HookableType.BaseType == typeof(CovalencePlugin)) covalencePluginCount++;
			else if (plugin.HookableType.BaseType == typeof(RustPlugin)) rustPluginCount++;
			else if (plugin.HookableType.BaseType == typeof(CarbonPlugin)) carbonPluginCount++;
		}

		Singleton.
			Include("rustplugin", $"{rustPluginCount:n0}").
			Include("covalenceplugin", $"{covalencePluginCount:n0}").
			Include("carbonplugin", $"{carbonPluginCount:n0}").
			Submit("batch_plugin_types");
	}

	/// <summary>
	/// Metric executed when a major time warning caused by a plugin occurs.
	/// </summary>
	public static void plugin_time_warn(string readableHook, Plugin basePlugin, double afterHookTime, double totalMemory, bool lagSpike, BaseHookable.CachedHook cachedHook, BaseHookable hookable)
	{
		if (!Enabled)
		{
			return;
		}

		Singleton.
			Include("name", $"{readableHook} ({basePlugin.ToPrettyString()})").
			Include("time", $"{afterHookTime.RoundUpToNearestCount(50)}ms").
			Include("memory", $"{ByteEx.Format(totalMemory, shortName: true).ToLower()}").
			Include("fires", $"{cachedHook?.TimesFired}").
			Include("hasgc", hookable.HasGCCollected).
			Include("lagspike", lagSpike).
			Submit("plugin_time_warn");
	}

	/// <summary>
	/// Metric executed whenever a Carbon-specific error occurred in the compilation process.
	/// </summary>
	public static void plugin_native_compile_fail(ISource initialSource, Exception ex)
	{
		if (!Enabled)
		{
			return;
		}

		Singleton.
			Include("file", $"{initialSource.ContextFilePath}").
			Include("stacktrace", $"({ex.Message}) {ex.StackTrace}").
			Submit("plugin_native_compile_fail");
	}

	/// <summary>
	/// Metric executed whenever an administrator is skipping the AdminModule splash screen panel.
	/// </summary>
	public static void admin_module_wizard(WizardProgress progress)
	{
		if (!Enabled)
		{
			return;
		}

		Singleton.
			Include("walkthrough", progress == WizardProgress.Walkthrough).
			Include("skipped", progress == WizardProgress.Skipped).
			Submit("admin_module_wizard");
	}

	/// <summary>
	/// Metric executed when the Carbon mono-profiler gets started.
	/// </summary>
	public static void profiler_started(MonoProfiler.ProfilerArgs args, bool timed)
	{
		if (!Enabled)
		{
			return;
		}

		Singleton.
			Include("settings", $"{Community.Runtime.MonoProfilerConfig.TrackCalls}tc " +
			                    $"{Community.Runtime.MonoProfilerConfig.Assemblies.Count}a " +
			                    $"{Community.Runtime.MonoProfilerConfig.Plugins.Count}p " +
			                    $"{Community.Runtime.MonoProfilerConfig.Modules.Count}m " +
			                    $"{Community.Runtime.MonoProfilerConfig.Extensions.Count}e " +
			                    $"{Community.Runtime.MonoProfilerConfig.Harmony.Count}h").
			Include("args", $"{args} {timed}t").
			Submit("profiler_started");
	}

	/// <summary>
	/// Metric executed whenever the Carbon mono-profiler get stopped.
	/// </summary>
	public static void profiler_ended(MonoProfiler.ProfilerArgs args, double duration, bool timed)
	{
		if (!Enabled)
		{
			return;
		}

		Singleton.
			Include("settings", $"{Community.Runtime.MonoProfilerConfig.TrackCalls}tc " +
			                    $"{Community.Runtime.MonoProfilerConfig.Assemblies.Count}a " +
			                    $"{Community.Runtime.MonoProfilerConfig.Plugins.Count}p " +
			                    $"{Community.Runtime.MonoProfilerConfig.Modules.Count}m " +
			                    $"{Community.Runtime.MonoProfilerConfig.Extensions.Count}e " +
			                    $"{Community.Runtime.MonoProfilerConfig.Harmony.Count}h").
			Include("args", $"{args} {timed}t").
			Include("duration", $"{TimeEx.Format(duration).ToLower()}").
			Submit("profiler_started");
	}

	/// <summary>
	/// Metric executed whenever the Carbon mono-profiler timeline started.
	/// </summary>
	public static void profiler_tl_started(MonoProfiler.ProfilerArgs args)
	{
		if (!Enabled)
		{
			return;
		}

		Singleton.
			Include("settings", $"{Community.Runtime.MonoProfilerConfig.TrackCalls}tc " +
			                    $"{Community.Runtime.MonoProfilerConfig.Assemblies.Count}a " +
			                    $"{Community.Runtime.MonoProfilerConfig.Plugins.Count}p " +
			                    $"{Community.Runtime.MonoProfilerConfig.Modules.Count}m " +
			                    $"{Community.Runtime.MonoProfilerConfig.Extensions.Count}e " +
			                    $"{Community.Runtime.MonoProfilerConfig.Harmony.Count}h").
			Include("args", $"{args}").
			Submit("profiler_tl_started");
	}

	/// <summary>
	/// Metric executed whenever the Carbon mono-profiler timeline stopped.
	/// </summary>
	public static void profiler_tl_ended(MonoProfiler.ProfilerArgs args, double duration, MonoProfiler.TimelineRecording.StatusTypes status)
	{
		if (!Enabled)
		{
			return;
		}

		Singleton.
			Include("settings", $"{Community.Runtime.MonoProfilerConfig.TrackCalls}tc " +
			                    $"{Community.Runtime.MonoProfilerConfig.Assemblies.Count}a " +
			                    $"{Community.Runtime.MonoProfilerConfig.Plugins.Count}p " +
			                    $"{Community.Runtime.MonoProfilerConfig.Modules.Count}m " +
			                    $"{Community.Runtime.MonoProfilerConfig.Extensions.Count}e " +
			                    $"{Community.Runtime.MonoProfilerConfig.Harmony.Count}h" +
			                    $"{(int)status}st").
			Include("args", $"{args}").
			Include("duration", $"{TimeEx.Format(duration).ToLower()}").
			Submit("profiler_tl_ended");
	}

	/// <summary>
	/// Metric executed when an admin logs into Codefling through the Admin module.
	/// </summary>
	public static void codefling_login()
	{
		if (!Enabled)
		{
			return;
		}

		Singleton.
			Submit("codefling_login");
	}

	public enum WizardProgress
	{
		Walkthrough,
		Skipped
	}
}
