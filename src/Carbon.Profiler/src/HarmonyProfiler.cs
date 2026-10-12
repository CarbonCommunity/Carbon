using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Carbon.Components;
using Carbon.Profiler;
using Facepunch;
using HarmonyLib;
using Newtonsoft.Json.Linq;
using Steamworks;
using UnityEngine;

namespace Carbon;

public sealed class HarmonyProfiler : IHarmonyModHooks
{
	public static readonly string configPath = Path.Combine(HarmonyLoader.modPath, "config.profiler.json");

	public static string profilesFolderPath
	{
		get
		{
			var path = Path.Combine(HarmonyLoader.modPath, "profiles");
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
			return path;
		}
	}

	private static FacepunchBehaviour _runner = GameObject.Find("Profiler Runner")?.GetComponent<FacepunchBehaviour>();

	public static FacepunchBehaviour Runner => _runner ??= new GameObject("Profiler Runner").AddComponent<ProfilerRunner>();

	public static bool IsCarbonInstalled = Type.GetType("Carbon.Community,Carbon.Common") != null;

	public static bool IsOxideInstalled = Type.GetType("Oxide.Core.Interface,Oxide.Core") != null;

	public static bool IsAlreadyInstalled = _runner != null;

	public static MonoProfiler.Sample ProfileSample = MonoProfiler.Sample.Create();

	private static readonly List<ConsoleSystem.Command> commands = new();

	private static ConsoleSystem.Command[] originalCommands;

	public void OnLoaded(OnHarmonyModLoadedArgs args)
	{
		if (IsAlreadyInstalled)
		{
			Debug.LogError($"Carbon.Profiler was already set up once! To use an updated version of the profiler, a reboot is required!");
			_runner.Invoke(() =>
			{
				ConsoleSystem.Run(ConsoleSystem.Option.Server, "harmony.unload Carbon.Profiler");
			}, 0.1f);
			return;
		}

		if (IsCarbonInstalled)
		{
			Debug.LogWarning($"Carbon is installed! Remove the Carbon.Profiler HarmonyMod since the profiler is already built in.");
			Runner.Invoke(() =>
			{
				ConsoleSystem.Run(ConsoleSystem.Option.Server, "harmony.unload Carbon.Profiler");
			}, 0.1f);
			return;
		}

		if (IsOxideInstalled)
		{
			Debug.LogWarning($"Oxide is installed! Plugin and extension processing is hooked into.");
		}

		MonoProfilerConfig.Load(configPath);
		InitNative();
		MonoProfiler.ConfigPath = configPath;

		var assemblies = AppDomain.CurrentDomain.GetAssemblies();
		for (int i = 0; i < assemblies.Length; i++)
		{
			var assembly = assemblies[i];
			MonoProfiler.TryStartProfileFor(MonoProfilerConfig.ProfileTypes.Assembly, assembly, assembly.GetName().Name);
		}

		Debug.LogWarning($"Carbon.Profiler {(MonoProfiler.Crashed ? "crashed" : "initialized")}! (NATIVE_PROTOCOL:{MonoProfiler.NATIVE_PROTOCOL} MANAGED_PROTOCOL:{MonoProfiler.MANAGED_PROTOCOL})");

		GameObject.DontDestroyOnLoad(Runner.gameObject);

		if (SteamServer.IsValid)
		{
			Patches.Bootstrap_Init_Tier0.Postfix();
		}
	}

	public void OnUnloaded(OnHarmonyModUnloadedArgs args)
	{
		UninstallCommands();
	}

	private static MonoProfiler.ProfilerArgs GetProfilerFlags(ConsoleSystem.Arg arg)
	{
		var flags = MonoProfiler.ProfilerArgs.None;

		if (arg.HasArg("-cm")) flags |= MonoProfiler.ProfilerArgs.CallMemory;
		if (arg.HasArg("-am")) flags |= MonoProfiler.ProfilerArgs.AdvancedMemory;
		if (arg.HasArg("-t")) flags |= MonoProfiler.ProfilerArgs.Timings;
		if (arg.HasArg("-c")) flags |= MonoProfiler.ProfilerArgs.Calls;
		if (arg.HasArg("-gc")) flags |= MonoProfiler.ProfilerArgs.GCEvents;

		return flags == MonoProfiler.ProfilerArgs.None ? MonoProfiler.AllFlags : flags;
	}

	private static bool IsTrackingCalls(ConsoleSystem.Arg arg)
	{
		if (MonoProfilerConfig.Instance.TrackCalls)
		{
			return true;
		}

		arg.ReplyWith("Call tracking is off. Set TrackCalls to true in config.profiler.json and restart the server.");
		return false;
	}

	private static bool TryReadTarget(ConsoleSystem.Arg arg, string command, out string name, out float duration)
	{
		name = arg.GetString(0);
		duration = arg.GetFloat(1);

		if (string.IsNullOrEmpty(name) || duration < 1f)
		{
			arg.ReplyWith($"Syntax: {command} <name> <duration> [-cm] [-am] [-t] [-c] [-gc]");
			return false;
		}

		if (MonoProfiler.IsRecording || TargetedProfile.IsActive)
		{
			arg.ReplyWith("Profiler is actively recording");
			return false;
		}

		return IsTrackingCalls(arg);
	}

	private static void PrepareNextLoad(MonoProfilerConfig.ProfileTypes type, string value, bool tracked)
	{
		if (tracked && type is MonoProfilerConfig.ProfileTypes.Plugin or MonoProfilerConfig.ProfileTypes.Harmony)
		{
			MonoProfiler.Start();
		}

		if (type == MonoProfilerConfig.ProfileTypes.Plugin && IsOxideInstalled)
		{
			OxidePlugins.MarkForRecompile(value);
		}
	}

	public static void InstallCommands()
	{
		originalCommands ??= ConsoleSystem.Index.All;

		commands.Clear();
		commands.AddRange(originalCommands);
		AddCommand("carbon", "profile", arg =>
		{
			var duration = arg.GetFloat(0);
			var flags = GetProfilerFlags(arg);

			if (MonoProfiler.IsRecording)
			{
				MonoProfiler.ToggleProfiling(flags);
				ProfileSample.Resample();
				MonoProfiler.Clear();
				TargetedProfile.End();
				return;
			}

			if (duration <= 0)
			{
				MonoProfiler.ToggleProfiling(flags);
			}
			else
			{
				MonoProfiler.ToggleProfilingTimed(duration, flags, args =>
				{
					ProfileSample.Resample();
					MonoProfiler.Clear();
				});
			}
		}, description: "Toggles the current state of the Carbon.Profiler", arguments: "[duration] [-cm] [-am] [-t] [-c] [-gc]");
		AddCommand("carbon", "abort_profile", arg =>
		{
			if (!MonoProfiler.IsRecording)
			{
				arg.ReplyWith("No profiling process active");
				return;
			}

			MonoProfiler.ToggleProfiling(MonoProfiler.ProfilerArgs.Abort);
			ProfileSample.Clear();
			TargetedProfile.End();
		}, description: "Stops a current profile from running");
		AddCommand("carbon", "profile_mod", arg =>
		{
			if (TryReadTarget(arg, "carbon.profile_mod", out var name, out var duration))
			{
				arg.ReplyWith(TargetedProfile.StartMod(name, duration, GetProfilerFlags(arg)));
			}
		}, description: "Profiles one HarmonyMod: reloads it with profiler hooks, records for the duration, then reloads it without them", arguments: "<name> <duration> [-cm] [-am] [-t] [-c] [-gc]");
		AddCommand("carbon", "profile_plugin", arg =>
		{
			if (TryReadTarget(arg, "carbon.profile_plugin", out var name, out var duration))
			{
				arg.ReplyWith(TargetedProfile.StartPlugin(name, duration, GetProfilerFlags(arg)));
			}
		}, description: "Profiles one Oxide plugin: recompiles it with profiler hooks, records for the duration, then recompiles it without them", arguments: "<name> <duration> [-cm] [-am] [-t] [-c] [-gc]");
		AddCommand("carbon", "export_profile", arg =>
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
				var file = Path.Combine(profilesFolderPath, $"profile-{date.Year}_{date.Month:00}_{date.Day:00}_{date.Hour:00}{date.Minute:00}{date.Second:00}.{extension}");
				File.WriteAllText(file, data);

				return $"Exported profile output at '{file}'";
			}
			static string WriteFileBytes(string extension, byte[] data)
			{
				var date = DateTime.Now;
				var file = Path.Combine(profilesFolderPath, $"profile-{date.Year}_{date.Month:00}_{date.Day:00}_{date.Hour:00}{date.Minute:00}{date.Second:00}.{extension}");
				File.WriteAllBytes(file, data);

				return $"Exported profile output at '{file}'";
			}
		}, description: "Exports to disk the most recent profile", arguments: "-c=CSV, -j=JSON, -t=Table, -p=ProtoBuf [default]");
		AddCommand("carbon", "tracked", arg =>
		{
			arg.ReplyWith($"Tracked Assemblies ({MonoProfilerConfig.Instance.Assemblies.Count:n0}):\n" +
			              $"{string.Join("\n", MonoProfilerConfig.Instance.Assemblies.Select(x => $"- {x}"))}\n" +
			              $"Tracked Plugins ({MonoProfilerConfig.Instance.Plugins.Count:n0}):\n" +
			              $"{string.Join("\n", MonoProfilerConfig.Instance.Plugins.Select(x => $"- {x}"))}\n" +
			              $"Tracked Modules ({MonoProfilerConfig.Instance.Modules.Count:n0}):\n" +
			              $"{string.Join("\n", MonoProfilerConfig.Instance.Modules.Select(x => $"- {x}"))}\n" +
			              $"Tracked Extensions ({MonoProfilerConfig.Instance.Extensions.Count:n0}):\n" +
			              $"{string.Join("\n", MonoProfilerConfig.Instance.Extensions.Select(x => $"- {x}"))}\n" +
			              $"Tracked HarmonyMods ({MonoProfilerConfig.Instance.Harmony.Count:n0}):\n" +
			              $"{string.Join("\n", MonoProfilerConfig.Instance.Harmony.Select(x => $"- {x}"))}\n" +
			              $"Use wildcard (*) to include all.");
		}, description: "All tracking lists present in the config which are used by the Mono profiler for tracking");
		AddCommand("carbon", "track", arg =>
		{
			if (!arg.HasArgs(2))
			{
				InvalidReturn(arg);
				return;
			}

			if (!IsTrackingCalls(arg))
			{
				return;
			}

			var type = arg.GetString(0);
			var value = arg.GetString(1);
			MonoProfilerConfig.ProfileTypes returnType = default;

			bool? returnVal = type switch
			{
				"assembly" => MonoProfilerConfig.Instance.AppendProfile(
					returnType = MonoProfilerConfig.ProfileTypes.Assembly, value),
				"plugin" => MonoProfilerConfig.Instance.AppendProfile(
					returnType = MonoProfilerConfig.ProfileTypes.Plugin, value),
				"module" => MonoProfilerConfig.Instance.AppendProfile(
					returnType = MonoProfilerConfig.ProfileTypes.Module, value),
				"ext" => MonoProfilerConfig.Instance.AppendProfile(
					returnType = MonoProfilerConfig.ProfileTypes.Extension, value),
				"harmony" => MonoProfilerConfig.Instance.AppendProfile(
					returnType = MonoProfilerConfig.ProfileTypes.Harmony, value),
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

			if (returnVal.Value)
			{
				MonoProfilerConfig.Save(configPath);
				PrepareNextLoad(returnType, value, tracked: true);
			}

			static void InvalidReturn(ConsoleSystem.Arg arg)
			{
				arg.ReplyWith("Syntax: carbon.track [assembly|plugin|module|ext|harmony] [value]");
			}
		}, description: "Adds an object to be tracked. Reload a tracked plugin or HarmonyMod to profile it. Assemblies, modules and extensions need Enabled in config.profiler.json and a server restart", arguments: "[assembly|plugin|module|ext|harmony] [value]");
		AddCommand("carbon", "untrack", arg =>
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
				"assembly" => MonoProfilerConfig.Instance.RemoveProfile(
					returnType = MonoProfilerConfig.ProfileTypes.Assembly, value),
				"plugin" => MonoProfilerConfig.Instance.RemoveProfile(
					returnType = MonoProfilerConfig.ProfileTypes.Plugin, value),
				"module" => MonoProfilerConfig.Instance.RemoveProfile(
					returnType = MonoProfilerConfig.ProfileTypes.Module, value),
				"ext" => MonoProfilerConfig.Instance.RemoveProfile(
					returnType = MonoProfilerConfig.ProfileTypes.Extension, value),
				"harmony" => MonoProfilerConfig.Instance.RemoveProfile(
					returnType = MonoProfilerConfig.ProfileTypes.Harmony, value),
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

			if (returnVal.Value)
			{
				MonoProfilerConfig.Save(configPath);
				PrepareNextLoad(returnType, value, tracked: false);
			}

			static void InvalidReturn(ConsoleSystem.Arg arg)
			{
				arg.ReplyWith("Syntax: carbon.untrack [assembly|plugin|module|ext|harmony] [value]");
			}
		}, description: "Removes an object from being tracked. Reload an untracked plugin or HarmonyMod to remove its profiler hooks. Assemblies, modules and extensions need a server restart", arguments: "[assembly|plugin|module|ext|harmony] [value]");
		AddCommand("carbon", "profiler_version", arg =>
		{
			var table = Pool.Get<TextTable>();
			table.Clear();
			table.AddColumns("version", "protocol", "managed", "native");
			table.AddRow(SelfUpdate.CurrentVersion.ToString(), string.Empty, MonoProfiler.MANAGED_PROTOCOL.ToString(), MonoProfiler.NATIVE_PROTOCOL.ToString());
			var result = table.ToString().TrimEnd();
			Pool.FreeUnsafe(ref table);
			arg.ReplyWith(result);
		}, description: "Prints the version of Carbon profiler");
		AddCommand("carbon", "update_profiler", arg =>
		{
			SelfUpdate.Api(data =>
			{
				var profiler = data.FirstOrDefault(x => x["name"].ToObject<string>().Equals("profiler_build"));
				var version = new Version($"{profiler?["version"].ToObject<string>()}.0");
				if (!SelfUpdate.CurrentVersion.Equals(version))
				{
					Debug.Log($"Carbon.Profiler is out of date! (current {SelfUpdate.CurrentVersion}, newer {version})");
					SelfUpdate.Update(() =>
					{
						Debug.Log("Updated Carbon.Profiler. Restart the server to load it.");
					});
				}
				else
				{
					Debug.Log($"Carbon.Profiler is up to date! ({SelfUpdate.CurrentVersion})");
				}
			});
			arg.ReplyWith("Checking for updates..");
		}, description: "Checks if the profiler is out of date, and updates itself if it is.");
		ConsoleSystem.Index.All = commands.ToArray();

		static void AddCommand(string parent, string name, Action<ConsoleSystem.Arg> callback, string description = null, string arguments = null)
		{
			var command = new ConsoleSystem.Command
			{
				Name = name,
				Parent = parent,
				FullName = parent + "." + name,
				Call = callback,
				ServerAdmin = true,
				Description = description ?? string.Empty,
				Arguments = arguments ?? string.Empty
			};
			commands.Add(command);
			AddCommandToServerIndex(command);
			Debug.LogWarning($"Carbon.Profiler: Installed '{command.FullName}'");
		}
	}

	// release vs staging branch fix for now
	private static void AddCommandToServerIndex(ConsoleSystem.Command command)
	{
		try
		{
			var dictField = AccessTools.Field(typeof(ConsoleSystem.Index.Server), "Dict");
			if (dictField?.GetValue(null) is not IDictionary dict)
			{
				throw new InvalidOperationException("ConsoleSystem.Index.Server.Dict is not available");
			}

			var keyType = dict.GetType().GetGenericArguments().FirstOrDefault();
			if (keyType == null)
			{
				throw new InvalidOperationException($"Couldn't determine server command index key type '{dict.GetType().FullName}'");
			}

			var key = keyType == typeof(string) ? command.FullName : AccessTools.Constructor(keyType, [typeof(string)])?.Invoke([command.FullName]);
			if (key == null)
			{
				throw new InvalidOperationException($"Couldn't create server command index key '{keyType.FullName}'");
			}

			dict[key] = command;
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException($"Carbon.Profiler couldn't add '{command.FullName}' to server command index", ex);
		}
	}

	public static void UninstallCommands()
	{
		ConsoleSystem.Index.All = originalCommands;
	}

	#region Native MonoProfiler

	[DllImport("CarbonNative")]
	public static unsafe extern void init_profiler(char* ptr, int length);

	[DllImport("__Internal", CharSet = CharSet.Ansi)]
	public static extern void mono_dllmap_insert(ModuleHandle assembly, string dll, string func, string tdll, string tfunc);

	public static unsafe void InitNative()
	{
#if UNIX
        mono_dllmap_insert(ModuleHandle.EmptyHandle, "CarbonNative", null, Path.Combine(HarmonyLoader.modPath, "native", "libCarbonNative.so"), null);
#elif WIN
		mono_dllmap_insert(ModuleHandle.EmptyHandle, "CarbonNative", null, Path.Combine(HarmonyLoader.modPath, "native", "CarbonNative.dll"), null);
#endif

		fixed (char* ptr = configPath)
		{
			init_profiler(ptr, configPath.Length);
		}
	}

	#endregion

	public class ProfilerRunner : FacepunchBehaviour;
}
