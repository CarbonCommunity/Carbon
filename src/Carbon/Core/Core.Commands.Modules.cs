using System.Text;
using Carbon.Base.Interfaces;

namespace Carbon.Core;

public partial class CorePlugin
{
	[ConsoleCommand("setmodule", "Enables or disables Carbon modules. Visit root/carbon/modules and use the config file names as IDs.")]
	[AuthLevel(2)]
	private void SetModule(ConsoleSystem.Arg arg)
	{
		if (!arg.HasArgs(2)) return;

		var moduleName = arg.GetString(0);
		var module = BaseModule.FindModule(moduleName);

		if (module == null)
		{
			arg.ReplyWith($"Couldn't find that module. Try 'c.modules' to print them all.");
			return;
		}

		if (module.ForceEnabled)
		{
			arg.ReplyWith($"That module is forcefully enabled, you may not change its status.");
			return;
		}

		if (module.ForceDisabled)
		{
			arg.ReplyWith($"That module is forcefully disabled, you may not change its status.");
			return;
		}

		var previousEnabled = module.IsEnabled();
		var newEnabled = arg.GetBool(1);

		if (previousEnabled != newEnabled)
		{
			module.SetEnabled(newEnabled);

			module.Save();
			arg.ReplyWith($"{module.Name} marked {(module.IsEnabled() ? "enabled" : "disabled")}.");
		}
		else
		{
			arg.ReplyWith($"{module.Name} is already {(module.IsEnabled() ? "enabled" : "disabled")}.");
		}
	}

	[ConsoleCommand("savemodules", "Saves the configs and data files of all available modules.")]
	[AuthLevel(2)]
	private void SaveModules(ConsoleSystem.Arg arg)
	{
		foreach (var hookable in Community.Runtime.ModuleProcessor.Modules)
		{
			hookable.To<IModule>().Save();
		}

		arg.ReplyWith($"Saved {Community.Runtime.ModuleProcessor.Modules.Count:n0} module configs and data files.");
	}

	[ConsoleCommand("savemodule", "Saves Carbon module config & data file.")]
	[AuthLevel(2)]
	private void SaveModule(ConsoleSystem.Arg arg)
	{
		if (!arg.HasArgs(1)) return;

		var moduleName = arg.GetString(0);
		var module = BaseModule.FindModule(moduleName);

		if (module == null)
		{
			arg.ReplyWith($"Couldn't find that module.");
			return;
		}

		module.Save();

		arg.ReplyWith($"Saved '{module.Name}' module config & data file.");
	}

	[ConsoleCommand("loadmodules", "Loads the configs and data files of all available modules.")]
	[AuthLevel(2)]
	private void LoadModules(ConsoleSystem.Arg arg)
	{
		foreach (var hookable in Community.Runtime.ModuleProcessor.Modules)
		{
			hookable.To<IModule>().Load();
		}

		arg.ReplyWith($"Loaded {Community.Runtime.ModuleProcessor.Modules.Count:n0} module configs and data files.");
	}

	[ConsoleCommand("loadmodule", "Loads Carbon module config & data file.")]
	[AuthLevel(2)]
	private void LoadModule(ConsoleSystem.Arg arg)
	{
		if (!arg.HasArgs(1)) return;

		var moduleName = arg.GetString(0);

		if (BaseModule.FindModule(moduleName) is not IModule module)
		{
			arg.ReplyWith($"Couldn't find that module.");
			return;
		}

		try
		{
			module.Load();

			arg.ReplyWith($"Reloaded '{module.Name}' module config & data.");
		}
		catch (Exception ex)
		{
			Logger.Error($"Failed module Load for {module.Name} [Reload Request]", ex);
		}
	}

	[ConsoleCommand("modules", "Prints a list of all available modules. Eg. c.modules [-abc|--json|-t|-m|-f] [-asc]")]
	[AuthLevel(2)]
	private void Modules(ConsoleSystem.Arg arg)
	{
		var mode = arg.GetString(0);
		var flip = arg.GetString(0).Equals("-asc") || arg.GetString(1).Equals("-asc");

		using var print = new StringTable( "Name", "Enabled", "Version", "Time", "Fires", "Memory", "Lag", "Uptime");

		IEnumerable<BaseHookable> array = mode switch
		{
			"-abc" => Community.Runtime.ModuleProcessor.Modules.OrderBy(x => x.Name),
			"-t" => (flip ? Community.Runtime.ModuleProcessor.Modules.OrderBy(x => x.TotalHookTime) : Community.Runtime.ModuleProcessor.Modules.OrderByDescending(x => x.TotalHookTime)),
			"-m" => (flip ? Community.Runtime.ModuleProcessor.Modules.OrderBy(x => x.TotalMemoryUsed) : Community.Runtime.ModuleProcessor.Modules.OrderByDescending(x => x.TotalMemoryUsed)),
			"-f" => (flip ? Community.Runtime.ModuleProcessor.Modules.OrderBy(x => x.TotalHookFires) : Community.Runtime.ModuleProcessor.Modules.OrderByDescending(x => x.TotalHookFires)),
			"-ls" => (flip ? Community.Runtime.ModuleProcessor.Modules.OrderBy(x => x.TotalHookLagSpikes) : Community.Runtime.ModuleProcessor.Modules.OrderByDescending(x => x.TotalHookLagSpikes)),
			_ => (flip ? Community.Runtime.ModuleProcessor.Modules.AsEnumerable().Reverse() : Community.Runtime.ModuleProcessor.Modules.AsEnumerable())
		};

		print.AddRow("Native", string.Empty, string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty);

		foreach (var hookable in array)
		{
			if (hookable is not BaseModule module)
			{
				continue;
			}

			if (!string.IsNullOrEmpty(module.Context))
			{
				continue;
			}

			print.AddRow($" {hookable.Name}", module.IsEnabled(), module.Version,
				module.TotalHookTime.TotalMilliseconds == 0 ? string.Empty : $"{module.TotalHookTime.TotalMilliseconds:0}ms",
				module.TotalHookFires == 0 ? string.Empty :$"{module.TotalHookFires:n0}",
				module.TotalMemoryUsed == 0 ? string.Empty : $"{ByteEx.Format(module.TotalMemoryUsed, shortName: true, stringFormat: "{0}{1}").ToLower()}",
				module.TotalHookLagSpikes == 0 ? string.Empty : $"{module.TotalHookLagSpikes:n0}",
				$"{TimeEx.Format(module.Uptime)}");
		}

		foreach (var mod in Community.Runtime.AssemblyEx.Modules.Loaded)
		{
			print.AddRow(Path.GetFileNameWithoutExtension(mod.Value.Key), string.Empty, string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty);

			foreach (var hookable in array)
			{
				if (hookable is not BaseModule module)
				{
					continue;
				}

				if (string.IsNullOrEmpty(module.Context))
				{
					continue;
				}

				if (!module.Context.Equals(mod.Value.Key, StringComparison.InvariantCultureIgnoreCase))
				{
					continue;
				}

				print.AddRow($" {hookable.Name}", module.IsEnabled(), module.Version,
					module.TotalHookTime.TotalMilliseconds == 0 ? string.Empty : $"{module.TotalHookTime.TotalMilliseconds:0}ms",
					module.TotalHookFires == 0 ? string.Empty :$"{module.TotalHookFires:n0}",
					module.TotalMemoryUsed == 0 ? string.Empty : $"{ByteEx.Format(module.TotalMemoryUsed, shortName: true, stringFormat: "{0}{1}").ToLower()}",
					module.TotalHookLagSpikes == 0 ? string.Empty : $"{module.TotalHookLagSpikes:n0}",
					$"{TimeEx.Format(module.Uptime)}");
			}
		}

		arg.ReplyWith(print.Write(StringTable.FormatTypes.None));
	}

	[ConsoleCommand("moduleinfo", "Prints advanced information about a currently loaded module. From hooks, hook times, hook memory usage and other things.")]
	[AuthLevel(2)]
	private void ModuleInfo(ConsoleSystem.Arg arg)
	{
		if (!arg.HasArgs(1))
		{
			Logger.Warn("You must provide the name of a module to print module advanced information.");
			return;
		}

		var name = arg.GetString(0);
		var mode = arg.GetString(1);
		var flip = arg.GetString(2).Equals("-asc");
		var module = BaseModule.FindModule(name);

		if (module == null)
		{
			arg.ReplyWith("Couldn't find that module.");
			return;
		}

		using (var table = new StringTable(string.Empty, "Id", "Hook", "Time", "Fires", "Memory", "Lag", "Exceptions", "Subscribed", "Async & Overrides"))
		{
			IEnumerable<List<CachedHook>> array = mode switch
			{
				"-t" => (flip ? module.HookPool.OrderBy(x => x.Value.Hooks.Sum(x => x.HookTime.TotalMilliseconds)) : module.HookPool.OrderByDescending(x => x.Value.Hooks.Sum(x => x.HookTime.TotalMilliseconds))).Select(x => x.Value.Hooks),
				"-m" => (flip ? module.HookPool.OrderBy(x => x.Value.Hooks.Sum(x => x.MemoryUsage)) : module.HookPool.OrderByDescending(x => x.Value.Hooks.Sum(x => x.MemoryUsage))).Select(x => x.Value.Hooks),
				"-f" => (flip ? module.HookPool.OrderBy(x => x.Value.Hooks.Sum(x => x.TimesFired)) : module.HookPool.OrderByDescending(x => x.Value.Hooks.Sum(x => x.TimesFired))).Select(x => x.Value.Hooks),
				"-ls" => (flip ? module.HookPool.OrderBy(x => x.Value.Hooks.Sum(x => x.LagSpikes)) : module.HookPool.OrderByDescending(x => x.Value.Hooks.Sum(x => x.LagSpikes))).Select(x => x.Value.Hooks),
				"-ex" => (flip ? module.HookPool.OrderBy(x => x.Value.Hooks.Sum(x => x.Exceptions)) : module.HookPool.OrderByDescending(x => x.Value.Hooks.Sum(x => x.Exceptions))).Select(x => x.Value.Hooks),
				_ => module.HookPool.Select(x => x.Value.Hooks)
			};

			foreach (var hook in array)
			{
				if (hook.Count == 0)
				{
					continue;
				}

				var current = hook[0];
				var hookName = current.Method.Name;

				var hookId = HookStringPool.GetOrAdd(hookName);

				if (!module.Hooks.Contains(hookId))
				{
					continue;
				}

				var hookTime = hook.Sum(x => x.HookTime.TotalMilliseconds);
				var hookMemoryUsage = hook.Sum(x => x.MemoryUsage);
				var hookCount = hook.Count;
				var hookAsyncCount = hook.Count(x => x.IsAsync);
				var hookTimesFired = hook.Sum(x => x.TimesFired);
				var hookLagSpikes = hook.Sum(x => x.LagSpikes);
				var hookException = hook.Sum(x => x.Exceptions);

				table.AddRow(string.Empty,
					hookId,
					$"{hookName}",
					hookTime == 0 ? string.Empty : $"{hookTime:0}ms",
					hookTimesFired == 0 ? string.Empty : $"{hookTimesFired:n0}",
					hookMemoryUsage == 0 ? string.Empty : $"{ByteEx.Format(hookMemoryUsage, shortName: true).ToLower()}",
					hookLagSpikes == 0 ? string.Empty : $"{hookLagSpikes:n0}",
					hookException == 0 ? string.Empty : $"{hookException:n0}",
					!module.IgnoredHooks.Contains(hookId) ? "*" : string.Empty,
					$"{hookAsyncCount:n0} / {hookCount:n0}");
			}

			var builder = new StringBuilder();

			builder.AppendLine($"Additional information for {module.Name} v{module.Version}{(module.ForceEnabled ? $" [force enabled]" : string.Empty)}");
			builder.AppendLine($"  Enabled:                {module.IsEnabled()}");
			builder.AppendLine($"  Enabled (default):      {module.EnabledByDefault}");
			builder.AppendLine($"  Context:                {module.Context}");
			builder.AppendLine($"  Uptime:                 {TimeEx.Format(module.Uptime, true).ToLower()}");
			builder.AppendLine($"  Total Hook Time:        {module.TotalHookTime.TotalMilliseconds:0}ms");
			builder.AppendLine($"  Total Memory Used:      {ByteEx.Format(module.TotalMemoryUsed, shortName: true).ToLower()}");
			builder.AppendLine($"  Internal Hook Override: {module.InternalCallHookOverriden}");
			builder.AppendLine($"Hooks:");
			builder.AppendLine(table.ToStringMinimal());

			arg.ReplyWith(builder.ToString());
		}
	}

	[ConsoleCommand("reloadmodules", "Fully reloads all modules.")]
	[AuthLevel(2)]
	private void ReloadModules(ConsoleSystem.Arg arg)
	{
		var entrypointName = arg.GetString(0);

		var entry = Community.Runtime.AssemblyEx.Modules.Loaded.FirstOrDefault(x =>
			Path.GetFileNameWithoutExtension(x.Value.Key)
				.Equals(entrypointName, StringComparison.InvariantCultureIgnoreCase));

		if (entry.Key == null)
		{
			Logger.Warn($"Couldn't find entrypoint with that name: '{entrypointName}'");
			return;
		}

		Community.Runtime.AssemblyEx.Modules.Unload(entry.Value.Key, "Core.ReloadModules");
		Community.Runtime.AssemblyEx.Modules.Load(entry.Value.Key, "Core.ReloadModules");
	}

	[ConsoleCommand("reloadmodule", "Reloads a currently loaded module assembly entirely.")]
	[AuthLevel(2)]
	private void ReloadModule(ConsoleSystem.Arg arg)
	{
		if (!arg.HasArgs(1)) return;

		var module = BaseModule.FindModule(arg.GetString(0));

		if (module == null)
		{
			arg.ReplyWith($"Couldn't find that module.");
			return;
		}

		module.Reload();

		arg.ReplyWith($"Reloaded '{module.Name}' module.");
	}
}
