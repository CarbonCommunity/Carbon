using System.Text;
using API.Hooks;
using Facepunch;
using Timer = Oxide.Plugins.Timer;

namespace Carbon.Core;

public partial class CorePlugin
{
	[ConsoleCommand("hooks", "Prints total information for all currently active and patched hooks in the server. (syntax: c.hooks [loaded] [-p|-s|-d])")]
	[AuthLevel(2)]
	private void HooksCall(ConsoleSystem.Arg args)
	{
		using var table = new StringTable("#", "Hook", "Id", "Type", "Status", "Time", "Fires", "Memory", "Lag", "Subs");
		var count = 1;
		var success = 0;
		var warning = 0;
		var failure = 0;

		var option1 = args.GetString(0, null);
		var option2 = args.GetString(1, null);

		switch (option1)
		{
			case "loaded":
				{
					IEnumerable<IHook> hooks;

					switch (option2)
					{
						case "-p":
							hooks = Community.Runtime.HookManager.LoadedPatches.Where(x => !x.IsHidden);
							break;

						case "-s":
							hooks = Community.Runtime.HookManager.LoadedStaticHooks.Where(x => !x.IsHidden);
							break;

						case "-d":
							hooks = Community.Runtime.HookManager.LoadedDynamicHooks.Where(x => !x.IsHidden);
							break;

						default:
							hooks = Community.Runtime.HookManager.LoadedPatches.Where(x => !x.IsHidden);
							hooks = hooks.Concat(Community.Runtime.HookManager.LoadedStaticHooks.Where(x => !x.IsHidden));
							hooks = hooks.Concat(Community.Runtime.HookManager.LoadedDynamicHooks.Where(x => !x.IsHidden));
							break;
					}

					switch (option2)
					{
						case "-u":
							hooks = hooks.OrderByDescending(x => HookCaller.GetTotalTime(HookStringPool.GetOrAdd(x.HookName)));
							break;

						default:
							hooks = hooks.OrderBy(x => x.HookFullName);
							break;
					}

					foreach (var iHook in hooks)
					{
						if (iHook.Status == HookState.Failure) failure++;
						if (iHook.Status == HookState.Success) success++;
						if (iHook.Status == HookState.Warning) warning++;

						var hook = HookStringPool.GetOrAdd(iHook.HookName);
						var time = HookCaller.GetTotalTime(hook).TotalMilliseconds;
						var fires = HookCaller.GetTotalFires(hook);
						var memory = HookCaller.GetTotalMemory(hook);
						var lagSpikes = HookCaller.GetTotalLagSpikes(hook);

						table.AddRow(
							$"{count++:n0}",
							iHook.IsHidden ? $"{iHook.HookFullName} (*)" : iHook.HookFullName,
							iHook.Identifier[^6..],
							iHook.IsStaticHook ? "Static" : iHook.IsPatch ? "Patch" : "Dynamic",
							iHook.Status.ToString(),
							time == 0 ? string.Empty : $"{time:0}ms",
							fires == 0 ? string.Empty : $"{fires}",
							memory == 0 ? string.Empty : $"{ByteEx.Format(memory, shortName: true).ToLower()}",
							lagSpikes == 0 ? string.Empty : $"{lagSpikes}",
							iHook.IsStaticHook ? "N/A" : $"{Community.Runtime.HookManager.GetHookSubscriberCount(iHook.Identifier),3}"
						);
					}

					args.ReplyWith($"total:{count} success:{success} warning:{warning} failed:{failure}"
								   + Environment.NewLine + Environment.NewLine + table.ToStringMinimal());
					break;
				}

			default: // list installed
				{
					IEnumerable<IHook> hooks;

					switch (option1)
					{
						case "-p":
							hooks = Community.Runtime.HookManager.InstalledPatches.Where(x => !x.IsHidden);
							break;

						case "-s":
							hooks = Community.Runtime.HookManager.InstalledStaticHooks.Where(x => !x.IsHidden);
							break;

						case "-d":
							hooks = Community.Runtime.HookManager.InstalledDynamicHooks.Where(x => !x.IsHidden);
							break;

						default:
#if DEBUG_VERBOSE
							hooks = Community.Runtime.HookManager.InstalledPatches;
							hooks = hooks.Concat(Community.Runtime.HookManager.InstalledStaticHooks);
							hooks = hooks.Concat(Community.Runtime.HookManager.InstalledDynamicHooks);
#else
							hooks = Community.Runtime.HookManager.InstalledPatches.Where(x => !x.IsHidden);
							hooks = hooks.Concat(
								Community.Runtime.HookManager.InstalledStaticHooks.Where(x => !x.IsHidden));
							hooks = hooks.Concat(
								Community.Runtime.HookManager.InstalledDynamicHooks.Where(x => !x.IsHidden));
#endif
							break;
					}

					if (option1 == "-u" || option2 == "-u")
					{
						hooks = hooks.OrderByDescending(x => HookCaller.GetTotalTime(HookStringPool.GetOrAdd(x.HookName)));
					}
					else
					{
						hooks = hooks.OrderBy(x => x.HookFullName);
					}

					foreach (var iHook in hooks)
					{
						if (iHook.Status == HookState.Failure) failure++;
						if (iHook.Status == HookState.Success) success++;
						if (iHook.Status == HookState.Warning) warning++;

						var hook = HookStringPool.GetOrAdd(iHook.HookName);
						var time = HookCaller.GetTotalTime(hook).TotalMilliseconds;
						var fires = HookCaller.GetTotalFires(hook);
						var memory = HookCaller.GetTotalMemory(hook);
						var lagSpikes = HookCaller.GetTotalLagSpikes(hook);

						table.AddRow(
							$"{count++:n0}",
							iHook.IsHidden ? $"{iHook.HookFullName} (*)" : iHook.HookFullName,
							iHook.Identifier[^6..],
							iHook.IsStaticHook ? "Static" : iHook.IsPatch ? "Patch" : "Dynamic",
							iHook.Status.ToString(),
							time == 0 ? string.Empty : $"{time:0}ms",
							fires == 0 ? string.Empty : $"{fires:n0}",
							memory == 0 ? string.Empty : $"{ByteEx.Format(memory, shortName: true).ToLower()}",
							lagSpikes == 0 ? string.Empty : $"{lagSpikes:n0}",
							(iHook.IsStaticHook) ? "N/A" : $"{Community.Runtime.HookManager.GetHookSubscriberCount(iHook.Identifier),3}"
						);
					}

					args.ReplyWith($"total:{count - 1} success:{success} warning:{warning} failed:{failure}"
								  + Environment.NewLine + Environment.NewLine + table.ToStringMinimal());
					break;
				}
		}
	}

	[ConsoleCommand("hookinfo", "Prints advanced information about a specific hook (takes [uint|string]). From hooks, hook times, hook memory usage to plugin and modules using it and other things.")]
	[AuthLevel(2)]
	private void HookInfo(ConsoleSystem.Arg arg)
	{
		if (!arg.HasArgs(1))
		{
			Logger.Warn("You must provide the name of a hook to print plugin advanced information.");
			return;
		}

		var name = arg.GetString(0);
		var isUid = uint.TryParse(name, out _);

		var hookName = isUid ? HookStringPool.GetOrAdd(name.ToUint()) : name;
		var hookId = isUid ? name.ToUint() : HookStringPool.GetOrAdd(name);

		var output = Pool.Get<StringBuilder>();

		const string byteFormat = "{0}{1}";

		output.AppendLine($"Information for {hookName}[{hookId}]");
		{
			var plugins = Pool.Get<Dictionary<BaseHookable, CachedHookInstance>>();
			{
				foreach (var package in ModLoader.Packages)
				{
					foreach (var plugin in package.Plugins)
					{
						foreach (var hookCache in plugin.HookPool.Where(hookCache => hookCache.Key == hookId))
						{
							plugins.Add(plugin, hookCache.Value);
						}
					}
				}
			}

			using var table = new StringTable(string.Empty, $"Plugins ({plugins.Count:n0})", "Time", "Fires", "Memory", "Lag", "Async & Overrides");

			foreach (var plugin in plugins)
			{
				var hook = plugin.Value.PrimaryHook;
				table.AddRow(string.Empty,
					$"{plugin.Key.Name}",
					hook.HookTime.TotalMilliseconds == 0 ? string.Empty : $"{hook.HookTime.TotalMilliseconds:0}ms",
					hook.TimesFired == 0 ? string.Empty : $"{hook.TimesFired:n0}",
					hook.MemoryUsage == 0 ? string.Empty : ByteEx.Format(hook.MemoryUsage, stringFormat: byteFormat).ToLower(),
					hook.LagSpikes == 0 ? string.Empty : $"{hook.LagSpikes:n0}",
					$"{plugin.Value.Hooks.Count(x => x.IsAsync):n0} / {plugin.Value.Hooks.Count:n0}");
			}

			var modules = Pool.Get<Dictionary<BaseHookable, CachedHookInstance>>();
			{
				foreach (var module in Community.Runtime.ModuleProcessor.Modules)
				{
					foreach (var hookCache in module.HookPool.Where(hookCache => hookCache.Key == hookId))
					{
						modules.Add(module, hookCache.Value);
						break;
					}
				}
			}

			table.AddRow(string.Empty, $"Modules ({modules.Count:n0})", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);

			foreach (var module in modules)
			{
				var hook = module.Value.Hooks.FirstOrDefault();
				table.AddRow(string.Empty,
					$"{module.Key.Name}",
					hook.HookTime.TotalMilliseconds == 0 ? string.Empty : $"{hook.HookTime.TotalMilliseconds:0}ms",
					hook.TimesFired == 0 ? string.Empty : $"{hook.TimesFired:n0}",
					hook.MemoryUsage == 0 ? string.Empty : ByteEx.Format(hook.MemoryUsage, stringFormat: byteFormat).ToLower(),
					hook.LagSpikes == 0 ? string.Empty : $"{hook.LagSpikes:n0}",
					$"{module.Value.Hooks.Count(x => x.IsAsync):n0} / {module.Value.Hooks.Count:n0}");
			}

			var totalTime = plugins.Sum(x => x.Value.Hooks.Sum(y => y.HookTime.TotalMilliseconds)) +
			                modules.Sum(x => x.Value.Hooks.Sum(y => y.HookTime.TotalMilliseconds));
			var totalFires = plugins.Sum(x => x.Value.Hooks.Sum(y => y.TimesFired)) +
			                 modules.Sum(x => x.Value.Hooks.Sum(y => y.TimesFired));
			var totalMemory = plugins.Sum(x => x.Value.Hooks.Sum(y => y.MemoryUsage)) +
			                  modules.Sum(x => x.Value.Hooks.Sum(y => y.MemoryUsage));
			var totalLag = plugins.Sum(x => x.Value.Hooks.Sum(y => y.LagSpikes)) +
			               modules.Sum(x => x.Value.Hooks.Sum(y => y.LagSpikes));

			table.AddRow(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
			table.AddRow(string.Empty, "Total",
				totalTime == 0 ? string.Empty : $"{totalTime:0}ms",
				totalFires == 0 ? string.Empty : $"{totalFires:n0}",
				totalMemory == 0 ? string.Empty : $"{ByteEx.Format(totalMemory).ToLower()}",
				totalLag == 0 ? string.Empty : $"{totalLag:n0}",
				string.Empty);

			output.AppendLine(table.ToStringMinimal().TrimEnd());

			arg.ReplyWith(output.ToString());

			Pool.FreeUnmanaged(ref output);
			Pool.FreeUnmanaged(ref plugins);
			Pool.FreeUnmanaged(ref modules);
		}
	}

#if DEBUG
	private uint _debuggedHook;
	private Timer _debuggedTimer;

	internal static bool EnforceHookDebugging;

	[Conditional("DEBUG")]
	[ConsoleCommand("debughook", "Enables debugging on a specific hook, which logs each time it fires. This can affect server performance, depending on how ofter the hook is firing.")]
	[AuthLevel(2)]
	private void DebugHook(ConsoleSystem.Arg arg)
	{
		DebugHookImpl(arg.GetString(0), arg.GetFloat(1), out var response);

		arg.ReplyWith(response);
	}

	[Conditional("DEBUG")]
	private void DebugHookImpl(string hookString, float time, out string response)
	{
		if (string.IsNullOrEmpty(hookString))
		{
			if (_debuggedHook != 0)
			{
				var hooksDisabled = 0;
				LoopHookableProcess(_debuggedHook, true, ref hooksDisabled);
				response = $"Disabled debugging hook {HookStringPool.GetOrAdd(_debuggedHook)}[{_debuggedHook}] (found {hooksDisabled:n0} {hooksDisabled.Plural("use", "uses")})";
			}
			else
			{
				response = "Empty string. Trust me, that won't work.";
			}

			return;
		}

		var hookId = uint.TryParse(hookString, out var alreadyIdValue) ? alreadyIdValue : HookStringPool.GetOrAdd(hookString);
		var alreadyDebugging = hookId == _debuggedHook;
		var hooksFound = 0;

		LoopHookableProcess(hookId, alreadyDebugging, ref hooksFound);

		static void LoopHookableProcess(uint hookId, bool alreadyDebugging, ref int hooksFound)
		{
			foreach (var plugin in ModLoader.Packages.SelectMany(package => package.Plugins))
			{
				ProcessHookable(plugin, hookId, alreadyDebugging, ref hooksFound);
			}

			foreach (var module in Community.Runtime.ModuleProcessor.Modules)
			{
				ProcessHookable(module, hookId, alreadyDebugging, ref hooksFound);
			}
		}
		static void ProcessHookable(BaseHookable hookable, uint hookId, bool alreadyDebugging, ref int hooksFound)
		{
			foreach (var hook in hookable.HookPool
				         .Where(cache => cache.Key == hookId)
				         .SelectMany(cache => cache.Value.Hooks))
			{
				hooksFound++;
				hook.EnableDebugging(!alreadyDebugging);
			}
		}

		_debuggedHook = alreadyDebugging ? default : hookId;

		response = $"{(alreadyDebugging ? $"Disabled debugging hook {hookString}[{hookId}]" : $"Started debugging hook {hookString}[{hookId}]")} (found {hooksFound:n0} {hooksFound.Plural("use", "uses")})";

		_debuggedTimer?.Destroy();

		if (time > 0)
		{
			_debuggedTimer = timer.In(time, () =>
			{
				DebugHookImpl(hookString, 0, out var response);
				Logger.Log(response);
			});
		}
	}

	[Conditional("DEBUG")]
	[ConsoleCommand("debugallhooks", "Enables debugging on all hooks and future hooks that will be processed (defaults debugging enabled on hooks).")]
	[AuthLevel(2)]
	private void DebugAllHooks(ConsoleSystem.Arg arg)
	{
		foreach (var plugin in ModLoader.Packages.SelectMany(package => package.Plugins))
		{
			plugin.HookPool.EnableDebugging(!EnforceHookDebugging);
		}
		foreach (var module in Community.Runtime.ModuleProcessor.Modules)
		{
			module.HookPool.EnableDebugging(!EnforceHookDebugging);
		}

		EnforceHookDebugging = true;

		arg.ReplyWith($"{(EnforceHookDebugging ? "Enabled" : "Disabled")} debugging across all hooks in plugins and modules (as well as future hooks that will be processed).");
	}

	[Conditional("DEBUG")]
	[ConsoleCommand("firehook", "For debugging purposes, it executes a hook manually. If the hook have arguments, it'll most likely throw plugin/module errors, but we probably want those.")]
	[AuthLevel(2)]
	private void FireHook(ConsoleSystem.Arg arg)
	{
		HookCaller.CallStaticHook(HookStringPool.GetOrAdd(arg.GetString(0)));
	}
	#endif
}
