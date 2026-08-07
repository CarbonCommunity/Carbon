using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace Carbon.Profiler.Tests.Harness;

internal sealed partial class Runner
{
	private readonly string _profilesPath = Path.Combine(RunnerPaths.ModPath, "profiles");

	private void ValidateProfilerState()
	{
		var monoProfiler = GetMonoProfilerType();
		var fEnabled = (bool)monoProfiler.GetProperty("Enabled", BindingFlags.Public | BindingFlags.Static)!.GetValue(null, null);
		var fCrashed = (bool)monoProfiler.GetProperty("Crashed", BindingFlags.Public | BindingFlags.Static)!.GetValue(null, null);

		if (!fEnabled)
		{
			throw new InvalidOperationException("MonoProfiler.Enabled is false");
		}

		if (fCrashed)
		{
			throw new InvalidOperationException("MonoProfiler.Crashed is true");
		}
	}

	private static bool IsRecording()
	{
		return (bool)GetMonoProfilerType().GetProperty("IsRecording", BindingFlags.Public | BindingFlags.Static)!.GetValue(null, null);
	}

	private static Type GetMonoProfilerType()
	{
		return Type.GetType("Carbon.Components.MonoProfiler, Carbon.Profiler", false) ??
		       throw new InvalidOperationException("Carbon.Components.MonoProfiler type was not loaded");
	}

	private static void ValidateProfilerConfig()
	{
		var configPath = Path.Combine(RunnerPaths.ModPath, "config.profiler.json");
		if (!File.Exists(configPath))
		{
			throw new FileNotFoundException("Profiler config was not created", configPath);
		}

		var config = JObject.Parse(File.ReadAllText(configPath));
		if (config.Value<bool?>("Enabled") != true)
		{
			throw new InvalidOperationException("Profiler config Enabled is not true");
		}
	}

	private void ClearProfiles()
	{
		Directory.CreateDirectory(_profilesPath);
		foreach (var file in Directory.GetFiles(_profilesPath, "*.json"))
		{
			File.Delete(file);
		}
	}

	private void ValidateExport()
	{
		var file = Directory.GetFiles(_profilesPath, "*.json")
			.Select(path => new FileInfo(path))
			.OrderByDescending(info => info.LastWriteTimeUtc)
			.FirstOrDefault();

		if (file == null)
		{
			throw new FileNotFoundException("Profiler JSON export was not created");
		}

		if (file.Length == 0)
		{
			throw new InvalidOperationException("Profiler JSON export is empty: " + file.FullName);
		}

		JToken.Parse(File.ReadAllText(file.FullName));
		RunnerLog.Info("Profiler JSON export created: " + file.FullName + " (" + file.Length + " bytes)");
	}
}
