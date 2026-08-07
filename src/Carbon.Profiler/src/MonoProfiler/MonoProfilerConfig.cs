using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Carbon.Profiler;

/*
 *
 * Copyright (c) 2023 Patrette, under the GNU v3 license rights
 * Copyright (c) 2023-2024 Carbon Community, under the GNU v3 license rights
 *
 */

[Serializable]
public class MonoProfilerConfig
{
#if HARMONYMOD
	public bool Enabled = true;
	public bool TrackCalls = true;
	public bool SourceViewer = true;
	public List<string> Assemblies = ["*"];
	public List<string> Plugins = ["*"];
	public List<string> Modules = ["*"];
	public List<string> Extensions = ["*"];
	public List<string> Harmony = ["*"];
#else
	public bool Enabled = false;
	public bool TrackCalls = false;
	public bool SourceViewer = false;
	public List<string> Assemblies = new();
	public List<string> Plugins = new();
	public List<string> Modules = new();
	public List<string> Extensions = new();
	public List<string> Harmony = new();
#endif

	public const string Star = "*";

	public static MonoProfilerConfig Instance { get; set; }

	public enum ProfileTypes
	{
		Assembly,
		Plugin,
		Module,
		Extension,
		Harmony
	}

	public bool AppendProfile(ProfileTypes profile, string value)
	{
		return profile switch
		{
			ProfileTypes.Assembly => Do(Assemblies, value),
			ProfileTypes.Plugin => Do(Plugins, value),
			ProfileTypes.Module => Do(Modules, value),
			ProfileTypes.Extension => Do(Extensions, value),
			ProfileTypes.Harmony => Do(Harmony, value),
			_ => throw new ArgumentOutOfRangeException(nameof(profile), profile, null)
		};

		static bool Do(List<string> list, string value)
		{
			if (list.Contains(value))
			{
				return false;
			}

			list.Add(value);
			return true;
		}
	}
	public bool RemoveProfile(ProfileTypes profile,string value)
	{
		return profile switch
		{
			ProfileTypes.Assembly => Do(Assemblies, value),
			ProfileTypes.Plugin => Do(Plugins, value),
			ProfileTypes.Module => Do(Modules, value),
			ProfileTypes.Extension => Do(Extensions, value),
			ProfileTypes.Harmony => Do(Harmony, value),
			_ => throw new ArgumentOutOfRangeException(nameof(profile), profile, null)
		};

		static bool Do(List<string> list, string value)
		{
			if (!list.Contains(value))
			{
				return false;
			}

			list.Remove(value);
			return true;
		}
	}
	public bool IsWhitelisted(ProfileTypes profile, string value)
	{
		return profile switch
		{
			ProfileTypes.Assembly => Assemblies.Contains(Star) || Assemblies.Contains(value),
			ProfileTypes.Plugin => Plugins.Contains(Star) || Plugins.Contains(value),
			ProfileTypes.Module => Modules.Contains(Star) || Modules.Contains(value),
			ProfileTypes.Extension => Extensions.Contains(Star) || Extensions.Contains(value),
			ProfileTypes.Harmony => Harmony.Contains(Star) || Harmony.Contains(value),
			_ => throw new ArgumentOutOfRangeException(nameof(profile), profile, null)
		};
	}

	public static void Load(string filePath)
	{
		if (File.Exists(filePath))
		{
			Instance = JsonConvert.DeserializeObject<MonoProfilerConfig>(File.ReadAllText(filePath));
			return;
		}

		Instance = new MonoProfilerConfig();
		File.WriteAllText(filePath, JsonConvert.SerializeObject(Instance, Formatting.Indented));
	}

	public static void Save(string filePath)
	{
		Instance ??= new MonoProfilerConfig();
		File.WriteAllText(filePath, JsonConvert.SerializeObject(Instance, Formatting.Indented));
	}
}
