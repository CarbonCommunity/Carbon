/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Carbon.Base;
using Carbon.Extensions;
using Newtonsoft.Json;
using Oxide.Plugins;
using System.Runtime.Remoting.Messaging;
using API.Hooks;

namespace Carbon.Components;

[Serializable]
public class Profiler : IDisposable
{
	public const string Extension = ".cprofile";
	public static Profiler Singleton { get; set; }

	public static bool HasInit { get; set; }

	[NonSerialized, JsonIgnore]
	public bool Running;

	public float Duration = -1f;
	public long StartTicks;
	public Build Runtime;
	public Dictionary<long, Timestamp> Timeline;
	public List<Plugin> Plugins;
	public string Path;
	public bool OverrideExistent;

#if DEBUG

	[NonSerialized, JsonIgnore]
	internal Dictionary<BaseHookable, Plugin> _pluginMap = new();

#endif

	public static Profiler Make(string path, bool overrideExistent = true)
	{
#if DEBUG

		Singleton?.Dispose();

		return Singleton = new Profiler()
		{
			StartTicks = DateTime.UtcNow.Ticks,
			Runtime = new(),
			Timeline = new(),
			Plugins = new(),
			Path = path,
			OverrideExistent = overrideExistent
		};

#else
		return default;
#endif
	}

	public void Begin(float duration)
	{
		Duration = duration;
		Running = true;
		OnBeginProfiling();

		if (duration > 0f)
		{
			Community.Runtime.CorePlugin.timer.In(Duration, () =>
			{
				End();
			});
		}
	}

#if DEBUG
	public async void Export(string path, bool overrideExistent = true)
	{
		if (!overrideExistent && OsEx.File.Exists(path)) return;

		using var fileStream = new FileStream($"{path}{Extension}", FileMode.Create);

		await Task.Run(() =>
		{
			var binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(fileStream, this);

			OsEx.File.Create($"{path}.json", JsonConvert.SerializeObject(this, Formatting.Indented));
		});
	}
#endif

	public static async Task<Profiler> Import(string path)
	{
		using var fileStream = new FileStream(path, FileMode.Open);

		return await Task.Run(() =>
		{
			var binaryFormatter = new BinaryFormatter();
			return (Profiler)binaryFormatter.Deserialize(fileStream);
		});
	}

	public static bool End()
	{
		if (Singleton == null || !Singleton.Running) return false;

		Singleton.Duration = Singleton.TimeElapsed;
		Singleton.Running = false;
		Singleton.OnEndProfiling();
		Singleton.Dispose();
		Singleton = null;

		return true;
	}

	public virtual void OnBeginProfiling()
	{

	}
	public virtual void OnEndProfiling()
	{
#if DEBUG
		foreach (var map in _pluginMap)
		{
			Plugins.Add(map.Value);
		}

		Export(Path, OverrideExistent);
#endif
	}

	public float TimeElapsed => (float)(DateTime.UtcNow - new DateTime(StartTicks)).TotalSeconds;

#region Profiling Methods

	public static void QueryPlugin(BaseHookable hookable, out Plugin plugin)
	{
#if DEBUG
		if (hookable == null)
		{
			plugin = null;
			return;
		}

		if (!Singleton._pluginMap.TryGetValue(hookable, out plugin))
		{
			var rustPlugin = hookable as RustPlugin;

			Singleton._pluginMap[hookable] = plugin = new Plugin
			{
				Name = hookable.Name,
				Path = rustPlugin?.FilePath,
				Author = rustPlugin?.Author,
				Version = hookable.Version.ToString(),
				HookCalls = new()
			};
		}
#else
		plugin = null;
#endif
	}

	public static void StartHookCall(BaseHookable hookable, string hookName)
	{
#if DEBUG
		if (Singleton == null || !Singleton.Running || hookable == null) return;

		QueryPlugin(hookable, out var plugin);

		plugin.StartCall(hookName);
#endif
	}
	public static void EndHookCall(BaseHookable hookable)
	{
#if DEBUG
		if (Singleton == null || !Singleton.Running || hookable == null) return;

		QueryPlugin(hookable, out var plugin);

		plugin.EndCall();
#endif
	}

#endregion

	public void Dispose()
	{
#if DEBUG
		Duration = default;
		StartTicks = default;
		Timeline.Clear();
		Timeline = null;
		Singleton = null;
#endif
	}

	[Serializable]
	public class Timestamp
	{
		public long Tick = DateTime.UtcNow.Ticks;
	}

	[Serializable]
	public class Plugin
	{
		public string Name;
		public string Path;
		public string Author;
		public string Version;
		public List<HookCall> HookCalls;

		[NonSerialized, JsonIgnore]
		internal TimeSince _time;

		[NonSerialized, JsonIgnore]
		internal int _currentHook = -1;

		public void StartCall(string hook)
		{
#if DEBUG
			_currentHook++;
			_time = 0;

			HookCalls.Add(new HookCall
			{
				Name = hook,
			});
#endif
		}
		public void EndCall()
		{
#if DEBUG
			var hook = HookCalls[_currentHook];
			hook.Duration = (float)(DateTime.UtcNow - DateTime.UtcNow.AddTicks(hook.Tick - DateTime.UtcNow.Ticks)).TotalMilliseconds;
			hook.Stacktrace = new StackTrace(2).ToString();
#endif
		}

		[Serializable]
		public class HookCall : Timestamp
		{
			public string Name;
			public string Stacktrace;
			public float Duration;
		}
	}

	[Serializable]
	public class Build
	{
		public string Protocol = Community.Runtime.Analytics.Protocol;
		public string Version = Community.Runtime.Analytics.Version;
		public string InformationalVersion = Community.Runtime.Analytics.InformationalVersion;
		public string Branch = Community.Runtime.Analytics.Branch;
		public string SessionId = Community.Runtime.Analytics.SessionID;
		public string SystemId = Community.Runtime.Analytics.SystemID;
		public string ClientId = Community.Runtime.Analytics.ClientID;
	}
}
