using API.Commands;
using Carbon.Base.Interfaces;
using Newtonsoft.Json;
using Oxide.Game.Rust.Cui;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Core;
#pragma warning disable IDE0051

public partial class CorePlugin : CarbonPlugin
{
	#region App

	[ConsoleCommand("shutdown", "Completely unloads Carbon from the game, rendering it fully vanilla.")]
	[AuthLevel(2)]
	private void Shutdown(ConsoleSystem.Arg arg)
	{
		Community.Runtime.Uninitialize();
	}

	[ConsoleCommand("reboot", "Unloads Carbon from the game and then loads it back again with the latest version changes (if any).")]
	private void Reboot(ConsoleSystem.Arg arg)
	{
		var loader = Community.Runtime.AssemblyEx;
		var patcher = Community.Runtime.HookManager;
		Community.Runtime.Uninitialize();

		var timer = new System.Timers.Timer(5000);
		timer.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) =>
		{
			loader.Components.Load("Carbon.dll", "CarbonEvent.StartupShared");
			timer.Dispose();
			timer = null;
		};
		timer.Start();
	}

	[ConsoleCommand("help", "Returns a brief introduction to Carbon.")]
	[AuthLevel(2)]
	private void Help(ConsoleSystem.Arg arg)
	{
		arg.ReplyWith($"To get started, run the `c.find c.` or `c.find carbon` to list all Carbon commands.\n" +
			$"To list all currently loaded plugins, execute `c.plugins`.\n" +
			$"For more information, please visit https://docs.carbonmod.gg or join the Discord server at https://discord.gg/carbonmod\n" +
			$"You're currently running {Community.Runtime.Analytics.InformationalVersion}.");
	}

	[ConsoleCommand("plugins", "Prints the list of mods and their loaded plugins.")]
	[AuthLevel(2)]
	private void Plugins(ConsoleSystem.Arg arg)
	{
		if (!arg.IsPlayerCalledOrAdmin()) return;

		var mode = arg.HasArgs(1) ? arg.Args[0] : null;

		switch (mode)
		{
			case "-j":
			case "--j":
			case "-json":
			case "--json":
				arg.ReplyWith(ModLoader.LoadedPackages);
				break;

			default:
				var result = string.Empty;

				// Loaded plugins
				{
					using var body = new StringTable("#", "Mod", "Author", "Version", "Hook Time", "Memory Usage", "Compile Time");
					var count = 1;

					foreach (var mod in ModLoader.LoadedPackages)
					{
						if (mod.IsCoreMod) continue;

						body.AddRow($"{count:n0}", $"{mod.Name}{(mod.Plugins.Count > 1 ? $" ({mod.Plugins.Count:n0})" : "")}", "", "", "", "", "");

						foreach (var plugin in mod.Plugins)
						{
							body.AddRow(string.Empty, plugin.Name, plugin.Author, $"v{plugin.Version}", $"{plugin.TotalHookTime:0}ms", $"{ByteEx.Format(plugin.TotalMemoryUsed, shortName: true, stringFormat: "{0}{1}").ToLower()}", $"{plugin.CompileTime:0}ms");
						}

						count++;
					}

					result += $"{body.Write(StringTable.FormatTypes.None)}\n";
				}

				// Failed plugins
				{
					using (var body = new StringTable("#", "File", "Errors", "Stack"))
					{
						var count = 1;

						foreach (var mod in ModLoader.FailedMods)
						{
							body.AddRow($"{count:n0}", $"{Path.GetFileName(mod.File)}", $"{mod.Errors.Length:n0}", $"{mod.Errors.Select(x => x.Message).ToArray().ToString(", ").Truncate(75, "...")}");

							count++;
						}

						result += $"Failed plugins:\n{body.Write(StringTable.FormatTypes.None)}\nTo list the full stack trace of failed plugins, run 'c.pluginsfailed'";
					}

					arg.ReplyWith(result);
				}
				break;
		}
	}

	[ConsoleCommand("pluginsunloaded", "Prints the list of unloaded plugins.")]
	[AuthLevel(2)]
	private void PluginsUnloaded(ConsoleSystem.Arg arg)
	{
		var mode = arg.HasArgs(1) ? arg.Args[0] : null;

		switch (mode)
		{
			case "-j":
			case "--j":
			case "-json":
			case "--json":
				arg.ReplyWith(JsonConvert.SerializeObject(Community.Runtime.ScriptProcessor.IgnoreList, Formatting.Indented));
				break;

			default:
				using (var body = new StringTable("#", "File"))
				{
					var count = 1;

					foreach (var ignored in Community.Runtime.ScriptProcessor.IgnoreList)
					{
						body.AddRow($"{count:n0}", $"{ignored}");
						count++;
					}

					arg.ReplyWith(body.Write(StringTable.FormatTypes.None));
				}
				break;
		}
	}

	[ConsoleCommand("pluginsfailed", "Prints the list of plugins that failed to load (most likely due to compilation issues).")]
	[AuthLevel(2)]
	private void PluginsFailed(ConsoleSystem.Arg arg)
	{
		var mode = arg.HasArgs(1) ? arg.Args[0] : null;

		switch (mode)
		{
			case "-j":
			case "--j":
			case "-json":
			case "--json":
				arg.ReplyWith(ModLoader.FailedMods);
				break;

			default:
				var result = string.Empty;
				var count = 1;
				var index = 1;

				foreach (var mod in ModLoader.FailedMods)
				{
					result += $"{count:n0}. {mod.File}\n";

					foreach (var error in mod.Errors)
					{
						result += $" {index}. {error.Message} [{error.Number}]\n" +
								  $"   ({error.Column} line {error.Line})\n";

						index++;
					}

					index = 1;
					result += "\n";
					count++;
				}

				arg.ReplyWith(result);
				break;
		}
	}

	[ConsoleCommand("pluginwarns", "Prints the list of warnings of a specific plugin (or all if no arguments are set).")]
	[AuthLevel(2)]
	private void PluginWarns(ConsoleSystem.Arg arg)
	{
		var filter = arg.GetString(0);

		if (string.IsNullOrEmpty(filter))
		{
			var r = string.Empty;

			foreach (var mod in ModLoader.LoadedPackages)
			{
				foreach (var plugin in mod.Plugins)
				{
					r += $"{Print(plugin)}\n";
				}
			}

			arg.ReplyWith(r);
		}
		else
		{
			var plugin = (Plugin)null;

			foreach (var mod in ModLoader.LoadedPackages)
			{
				foreach (var p in mod.Plugins)
				{
					if (p.Name == filter)
					{
						plugin = p;
						break;
					}
				}
			}

			if (plugin == null)
			{
				arg.ReplyWith($"Couldn't find a plugin with that name: '{filter}'");
				return;
			}

			arg.ReplyWith(Print(plugin));
		}

		static string Print(Plugin plugin)
		{
			var result = string.Empty;
			var count = 1;

			result += $"{plugin.Name} v{plugin.Version} by {plugin.Author}:\n";

			if (plugin.CompileWarnings == null || plugin.CompileWarnings.Length == 0)
			{
				result += $"  No warnings available.\n";
			}
			else
			{
				foreach (var warn in plugin.CompileWarnings)
				{
					result += $"  {count:n0}. {warn.Message} [{warn.Number}]\n     ({warn.Column} line {warn.Line})\n";
					count++;
				}
			}

			return result;
		}
	}

	// DISABLED UNTIL FULLY FUNCTIONAL
	// [ConsoleCommand("update", "Downloads, updates, saves the server and patches Carbon at runtime. (Eg. c.update win develop, c.update unix prod)")]
	// private void Update(ConsoleSystem.Arg arg)
	// {
	// 	if (!arg.IsPlayerCalledAndAdmin()) return;

	// 	Updater.DoUpdate((bool result) =>
	// 	{
	// 		if (!result)
	// 		{
	// 			Logger.Error($"Unknown error while updating Carbon");
	// 			return;
	// 		}
	// 		HookCaller.CallStaticHook("OnServerSave");

	// 		//FIXMENOW
	// 		//Supervisor.ASM.UnloadModule("Carbon.dll", true);
	// 	});
	// }

	#endregion

	#region Conditionals

	[ConsoleCommand("addconditional", "Adds a new conditional compilation symbol to the compiler.")]
	[AuthLevel(2)]
	private void AddConditional(ConsoleSystem.Arg arg)
	{
		var value = arg.Args[0];

		if (!Community.Runtime.Config.ConditionalCompilationSymbols.Contains(value))
		{
			Community.Runtime.Config.ConditionalCompilationSymbols.Add(value);
			Community.Runtime.SaveConfig();
			arg.ReplyWith($"Added conditional '{value}'.");
		}
		else
		{
			arg.ReplyWith($"Conditional '{value}' already exists.");
		}

		foreach (var mod in ModLoader.LoadedPackages)
		{
			var plugins = Facepunch.Pool.GetList<RustPlugin>();
			plugins.AddRange(mod.Plugins);

			foreach (var plugin in plugins)
			{
				if (plugin.HasConditionals)
				{
					plugin.ProcessorInstance.Dispose();
					plugin.ProcessorInstance.Execute();
					mod.Plugins.Remove(plugin);
				}
			}

			Facepunch.Pool.FreeList(ref plugins);
		}
	}

	[ConsoleCommand("remconditional", "Removes an existent conditional compilation symbol from the compiler.")]
	[AuthLevel(2)]
	private void RemoveConditional(ConsoleSystem.Arg arg)
	{
		var value = arg.Args[0];

		if (Community.Runtime.Config.ConditionalCompilationSymbols.Contains(value))
		{
			Community.Runtime.Config.ConditionalCompilationSymbols.Remove(value);
			Community.Runtime.SaveConfig();
			arg.ReplyWith($"Removed conditional '{value}'.");
		}
		else
		{
			arg.ReplyWith($"Conditional '{value}' does not exist.");
		}

		foreach (var mod in ModLoader.LoadedPackages)
		{
			var plugins = Facepunch.Pool.GetList<RustPlugin>();
			plugins.AddRange(mod.Plugins);

			foreach (var plugin in plugins)
			{
				if (plugin.HasConditionals)
				{
					plugin.ProcessorInstance.Dispose();
					plugin.ProcessorInstance.Execute();
					mod.Plugins.Remove(plugin);
				}
			}

			Facepunch.Pool.FreeList(ref plugins);
		}
	}

	[ConsoleCommand("conditionals", "Prints a list of all conditional compilation symbols used by the compiler.")]
	[AuthLevel(2)]
	private void Conditionals(ConsoleSystem.Arg arg)
	{
		arg.ReplyWith($"Conditionals ({Community.Runtime.Config.ConditionalCompilationSymbols.Count:n0}): {Community.Runtime.Config.ConditionalCompilationSymbols.ToArray().ToString(", ", " and ")}");
	}

	#endregion

	#region Config

	[ConsoleCommand("loadconfig", "Loads Carbon config from file.")]
	[AuthLevel(2)]
	private void CarbonLoadConfig(ConsoleSystem.Arg arg)
	{
		if (Community.Runtime == null) return;

		Community.Runtime.LoadConfig();

		arg.ReplyWith("Loaded Carbon config.");
	}

	[ConsoleCommand("saveconfig", "Saves Carbon config to file.")]
	[AuthLevel(2)]
	private void CarbonSaveConfig(ConsoleSystem.Arg arg)
	{
		if (Community.Runtime == null) return;

		Community.Runtime.SaveConfig();

		arg.ReplyWith("Saved Carbon config.");
	}

	[CommandVar("modding", "Mark this server as modded or not.")]
	[AuthLevel(2)]
	private bool Modding { get { return Community.Runtime.Config.IsModded; } set { Community.Runtime.Config.IsModded = value; Community.Runtime.SaveConfig(); } }

	[CommandVar("higherpriorityhookwarns", "Print warns if hooks with higher priority conflict with other hooks. Best to keep this disabled. Same-priority hooks will be printed.")]
	[AuthLevel(2)]
	private bool HigherPriorityHookWarns { get { return Community.Runtime.Config.HigherPriorityHookWarns; } set { Community.Runtime.Config.HigherPriorityHookWarns = value; Community.Runtime.SaveConfig(); } }

	[CommandVar("scriptwatchers", "When disabled, you must load/unload plugins manually with `c.load` or `c.unload`.")]
	[AuthLevel(2)]
	private bool ScriptWatchers { get { return Community.Runtime.Config.ScriptWatchers; } set { Community.Runtime.Config.ScriptWatchers = value; Community.Runtime.SaveConfig(); } }

	[CommandVar("scriptwatchersoption", "Indicates wether the script watcher (whenever enabled) listens to the 'carbon/plugins' folder only, or its subfolders. (0 = Top-only directories, 1 = All directories)")]
	[AuthLevel(2)]
	private int ScriptWatchersOption
	{
		get
		{
			return (int)Community.Runtime.Config.ScriptWatcherOption;
		}
		set
		{
			Community.Runtime.Config.ScriptWatcherOption = (SearchOption)value;
			Community.Runtime.ScriptProcessor.IncludeSubdirectories = value == (int)SearchOption.AllDirectories;
			Community.Runtime.SaveConfig();
		}
	}

	[CommandVar("debug", "The level of debug logging for Carbon. Helpful for very detailed logs in case things break. (Set it to -1 to disable debug logging.)")]
	[AuthLevel(2)]
	private int CarbonDebug { get { return Community.Runtime.Config.LogVerbosity; } set { Community.Runtime.Config.LogVerbosity = value; Community.Runtime.SaveConfig(); } }

	[CommandVar("logfiletype", "The mode for writing the log to file. (0=disabled, 1=saves updates every 5 seconds, 2=saves immediately)")]
	[AuthLevel(2)]
	private int LogFileType { get { return Community.Runtime.Config.LogFileMode; } set { Community.Runtime.Config.LogFileMode = Mathf.Clamp(value, 0, 2); Community.Runtime.SaveConfig(); } }

	[CommandVar("unitystacktrace", "Enables a big chunk of detail of Unity's default stacktrace. Recommended to be disabled as a lot of it is internal and unnecessary for the average user.")]
	[AuthLevel(2)]
	private bool UnityStacktrace
	{
		get { return Community.Runtime.Config.UnityStacktrace; }
		set
		{
			Community.Runtime.Config.UnityStacktrace = value;
			Community.Runtime.SaveConfig();
			ApplyStacktrace();
		}
	}

	[CommandVar("filenamecheck", "It checks if the file name and the plugin name matches. (only applies to scripts)")]
	[AuthLevel(2)]
	private bool FileNameCheck { get { return Community.Runtime.Config.FileNameCheck; } set { Community.Runtime.Config.FileNameCheck = value; Community.Runtime.SaveConfig(); } }

	[CommandVar("entitymapbuffersize", "The entity map buffer size. Gets applied on Carbon reboot.")]
	[AuthLevel(2)]
	private int EntityMapBufferSize { get { return Community.Runtime.Config.EntityMapBufferSize; } set { Community.Runtime.Config.EntityMapBufferSize = value; Community.Runtime.SaveConfig(); } }

	[CommandVar("language", "Server language used by the Language API.")]
	[AuthLevel(2)]
	private string Language { get { return Community.Runtime.Config.Language; } set { Community.Runtime.Config.Language = value; Community.Runtime.SaveConfig(); } }

#if WIN
	[CommandVar("consoleinfo", "Show the Windows-only Carbon information at the bottom of the console.")]
	[AuthLevel(2)]
	private bool ConsoleInfo
	{
		get { return Community.Runtime.Config.ShowConsoleInfo; }
		set
		{
			Community.Runtime.Config.ShowConsoleInfo = value;

			if (value)
			{
				Community.Runtime.RefreshConsoleInfo();
			}
			else
			{
				if (ServerConsole.Instance != null && ServerConsole.Instance.input != null)
				{
					ServerConsole.Instance.input.statusText = new string[3];
				}
			}
		}
	}
#endif

	[CommandVar("ocommandchecks", "Prints a reminding warning if RCON/console attempts at calling an o.* command.")]
	[AuthLevel(2)]
	private bool oCommandChecks { get { return Community.Runtime.Config.oCommandChecks; } set { Community.Runtime.Config.oCommandChecks = value; Community.Runtime.SaveConfig(); } }

	#endregion

	#region Commands

	[ConsoleCommand("find", "Searches through Carbon-processed console commands.")]
	[AuthLevel(2)]
	private void Find(ConsoleSystem.Arg arg)
	{
		using var body = new StringTable("Console Command", "Value", "Help");
		var filter = arg.Args != null && arg.Args.Length > 0 ? arg.Args[0] : null;

		foreach (var command in Community.Runtime.CommandManager.ClientConsole)
		{
			if (command.HasFlag(CommandFlags.Hidden) || (!string.IsNullOrEmpty(filter) && !command.Name.Contains(filter))) continue;

			var value = " ";

			if (command.Token != null)
			{
				if (command.Token is FieldInfo field) value = field.GetValue(command.Reference as RustPlugin)?.ToString();
				else if (command.Token is PropertyInfo property) value = property.GetValue(command.Reference as RustPlugin)?.ToString();
			}

			if (command.HasFlag(CommandFlags.Protected))
			{
				value = new string('*', value.Length);
			}

			body.AddRow($" {command.Name}", value, command.Help);
		}

		arg.ReplyWith(body.Write(StringTable.FormatTypes.None));
	}

	[ConsoleCommand("findchat", "Searches through Carbon-processed chat commands.")]
	[AuthLevel(2)]
	private void FindChat(ConsoleSystem.Arg arg)
	{
		using var body = new StringTable("Chat Command", "Help");
		var filter = arg.Args != null && arg.Args.Length > 0 ? arg.Args[0] : null;

		foreach (var command in Community.Runtime.CommandManager.Chat)
		{
			if (command.HasFlag(CommandFlags.Hidden) || (!string.IsNullOrEmpty(filter) && !command.Name.Contains(filter))) continue;

			body.AddRow($" {command.Name}", command.Help);
		}

		arg.ReplyWith(body.Write(StringTable.FormatTypes.None));
	}

	#endregion

	#region Report

	[ConsoleCommand("report", "Reloads all current plugins, and returns a report based on them at the output path.")]
	[AuthLevel(2)]
	private void Report(ConsoleSystem.Arg arg)
	{
		new Carbon.Components.Report().Init();
	}

	#endregion

	#region Modules

	[ConsoleCommand("setmodule", "Enables or disables Carbon modules. Visit root/carbon/modules and use the config file names as IDs.")]
	[AuthLevel(2)]
	private void SetModule(ConsoleSystem.Arg arg)
	{
		if (!arg.HasArgs(2)) return;

		var hookable = Community.Runtime.ModuleProcessor.Modules.FirstOrDefault(x => x.Name == arg.Args[0]);
		var module = hookable?.To<IModule>();

		if (module == null)
		{
			arg.ReplyWith($"Couldn't find that module. Try 'c.modules' to print them all.");
			return;
		}
		else if (module is BaseModule baseModule && baseModule.ForceEnabled)
		{
			arg.ReplyWith($"That module is forcefully enabled, you may not change its status.");
			return;
		}

		var previousEnabled = module.GetEnabled();
		var newEnabled = arg.Args[1].ToBool();

		if (previousEnabled != newEnabled)
		{
			module.SetEnabled(newEnabled);
			module.Save();
		}

		arg.ReplyWith($"{module.Name} marked {(module.GetEnabled() ? "enabled" : "disabled")}.");
	}

	[ConsoleCommand("saveallmodules", "Saves the configs and data files of all available modules.")]
	[AuthLevel(2)]
	private void SaveAllModules(ConsoleSystem.Arg arg)
	{
		foreach (var hookable in Community.Runtime.ModuleProcessor.Modules)
		{
			var module = hookable.To<IModule>();
			module.Save();
		}

		arg.ReplyWith($"Saved {Community.Runtime.ModuleProcessor.Modules.Count:n0} module configs and data files.");
	}

	[ConsoleCommand("savemoduleconfig", "Saves Carbon module config & data file.")]
	[AuthLevel(2)]
	private void SaveModuleConfig(ConsoleSystem.Arg arg)
	{
		if (!arg.HasArgs(1)) return;

		var hookable = Community.Runtime.ModuleProcessor.Modules.FirstOrDefault(x => x.Name == arg.Args[0]);
		var module = hookable.To<IModule>();

		if (module == null)
		{
			arg.ReplyWith($"Couldn't find that module.");
			return;
		}

		module.Save();

		arg.ReplyWith($"Saved '{module.Name}' module config & data file.");
	}

	[ConsoleCommand("loadmoduleconfig", "Loads Carbon module config & data file.")]
	[AuthLevel(2)]
	private void LoadModuleConfig(ConsoleSystem.Arg arg)
	{
		if (!arg.HasArgs(1)) return;

		var hookable = Community.Runtime.ModuleProcessor.Modules.FirstOrDefault(x => x.Name == arg.Args[0]);
		var module = hookable.To<IModule>();

		if (module == null)
		{
			arg.ReplyWith($"Couldn't find that module.");
			return;
		}

		if (module.GetEnabled()) module.SetEnabled(false);
		module.Load();
		if (module.GetEnabled()) module.OnEnableStatus();

		arg.ReplyWith($"Reloaded '{module.Name}' module config.");
	}

	[ConsoleCommand("modules", "Prints a list of all available modules.")]
	[AuthLevel(2)]
	private void Modules(ConsoleSystem.Arg arg)
	{
		using var print = new StringTable("Name", "Is Enabled", "Quick Command");
		foreach (var hookable in Community.Runtime.ModuleProcessor.Modules)
		{
			if (hookable is not IModule module) continue;

			print.AddRow(hookable.Name, module.GetEnabled() ? "Yes" : "No", $"c.setmodule \"{hookable.Name}\" 0/1");
		}

		arg.ReplyWith(print.Write(StringTable.FormatTypes.None));
	}

	#endregion

	#region Plugin

	[ConsoleCommand("reload", "Reloads all or specific mods / plugins. E.g 'c.reload *' to reload everything.")]
	[AuthLevel(2)]
	private void Reload(ConsoleSystem.Arg arg)
	{
		if (!arg.HasArgs(1)) return;

		RefreshOrderedFiles();

		var name = arg.Args[0];
		switch (name)
		{
			case "*":
				Community.Runtime.ReloadPlugins();
				break;

			default:
				var path = GetPluginPath(name);

				if (!string.IsNullOrEmpty(path))
				{
					Community.Runtime.ScriptProcessor.ClearIgnore(path);
					Community.Runtime.ScriptProcessor.Prepare(name, path);
					return;
				}

				var pluginFound = false;
				var pluginPrecompiled = false;

				foreach (var mod in ModLoader.LoadedPackages)
				{
					var plugins = Facepunch.Pool.GetList<RustPlugin>();
					plugins.AddRange(mod.Plugins);

					foreach (var plugin in plugins)
					{
						if (plugin.IsPrecompiled) continue;

						if (plugin.Name == name)
						{
							pluginFound = true;

							if (plugin.IsPrecompiled)
							{
								pluginPrecompiled = true;
							}
							else
							{
								plugin.ProcessorInstance.Dispose();
								plugin.ProcessorInstance.Execute();
								mod.Plugins.Remove(plugin);
							}
						}
					}

					Facepunch.Pool.FreeList(ref plugins);
				}

				if (!pluginFound)
				{
					Logger.Warn($"Plugin {name} was not found or was typed incorrectly.");
				}
				else if (pluginPrecompiled)
				{
					Logger.Warn($"Plugin {name} is a precompiled plugin which can only be reloaded programmatically.");
				}
				break;
		}
	}

	[ConsoleCommand("load", "Loads all mods and/or plugins. E.g 'c.load *' to load everything you've unloaded.")]
	[AuthLevel(2)]
	private void LoadPlugin(ConsoleSystem.Arg arg)
	{
		if (!arg.HasArgs(1))
		{
			Logger.Warn("You must provide the name of a plugin or use * to load all plugins.");
			return;
		}

		RefreshOrderedFiles();

		var name = arg.Args[0];
		switch (name)
		{
			case "*":
				//
				// Scripts
				//
				{
					Community.Runtime.ScriptProcessor.IgnoreList.Clear();

					foreach (var plugin in OrderedFiles)
					{
						if (Community.Runtime.ScriptProcessor.InstanceBuffer.ContainsKey(plugin.Key))
						{
							continue;
						}

						Community.Runtime.ScriptProcessor.Prepare(plugin.Key, plugin.Value);
					}
					break;
				}

			default:
				{
					var path = GetPluginPath(name);
					if (!string.IsNullOrEmpty(path))
					{
						Community.Runtime.ScriptProcessor.ClearIgnore(path);
						Community.Runtime.ScriptProcessor.Prepare(path);
						return;
					}

					Logger.Warn($"Plugin {name} was not found or was typed incorrectly.");

					/*var module = BaseModule.GetModule<DRMModule>();
					foreach (var drm in module.Config.DRMs)
					{
						foreach (var entry in drm.Entries)
						{
							if (entry.Id == name) drm.RequestEntry(entry);
						}
					}*/
					break;
				}
		}
	}

	[ConsoleCommand("unload", "Unloads all mods and/or plugins. E.g 'c.unload *' to unload everything. They'll be marked as 'ignored'.")]
	[AuthLevel(2)]
	private void UnloadPlugin(ConsoleSystem.Arg arg)
	{
		if (!arg.HasArgs(1))
		{
			Logger.Warn("You must provide the name of a plugin or use * to unload all plugins.");
			return;
		}

		RefreshOrderedFiles();

		var name = arg.Args[0];
		switch (name)
		{
			case "*":
				//
				// Scripts
				//
				{
					var tempList = Facepunch.Pool.GetList<string>();

					foreach (var bufferInstance in Community.Runtime.ScriptProcessor.InstanceBuffer)
					{
						tempList.Add(bufferInstance.Value.File);
					}

					Community.Runtime.ScriptProcessor.IgnoreList.Clear();
					Community.Runtime.ScriptProcessor.Clear();

					foreach (var plugin in tempList)
					{
						Community.Runtime.ScriptProcessor.Ignore(plugin);
					}
				}

				//
				// Web-Scripts
				//
				{
					var tempList = Facepunch.Pool.GetList<string>();
					tempList.AddRange(Community.Runtime.WebScriptProcessor.IgnoreList);
					Community.Runtime.WebScriptProcessor.IgnoreList.Clear();
					Community.Runtime.WebScriptProcessor.Clear();

					foreach (var plugin in tempList)
					{
						Community.Runtime.WebScriptProcessor.Ignore(plugin);
					}
					Facepunch.Pool.FreeList(ref tempList);
					break;
				}

			default:
				{
					var path = GetPluginPath(name);
					if (!string.IsNullOrEmpty(path))
					{
						Community.Runtime.ScriptProcessor.Ignore(path);
						Community.Runtime.WebScriptProcessor.Ignore(path);
					}

					var pluginFound = false;
					var pluginPrecompiled = false;

					foreach (var mod in ModLoader.LoadedPackages)
					{
						var plugins = Facepunch.Pool.GetList<RustPlugin>();
						plugins.AddRange(mod.Plugins);

						foreach (var plugin in plugins)
						{
							if (plugin.Name == name)
							{
								pluginFound = true;

								if (plugin.IsPrecompiled)
								{
									pluginPrecompiled = true;
								}
								else
								{
									plugin.ProcessorInstance?.Dispose();
									mod.Plugins.Remove(plugin);
								}
							}
						}

						Facepunch.Pool.FreeList(ref plugins);
					}

					if (!pluginFound)
					{
						if (string.IsNullOrEmpty(path)) Logger.Warn($"Plugin {name} was not found or was typed incorrectly.");
						else Logger.Warn($"Plugin {name} was not loaded but was marked as ignored.");
					}
					else if (pluginPrecompiled)
					{
						Logger.Warn($"Plugin {name} is a precompiled plugin which can only be unloaded programmatically.");
					}
					break;
				}
		}
	}

	[ConsoleCommand("reloadconfig", "Reloads a plugin's config file. This might have unexpected results, use cautiously.")]
	[AuthLevel(2)]
	private void ReloadConfig(ConsoleSystem.Arg arg)
	{
		if (!arg.HasArgs(1))
		{
			Logger.Warn("You must provide the name of a plugin or use * to reload all plugin configs.");
			return;
		}

		RefreshOrderedFiles();

		var name = arg.Args[0];
		switch (name)
		{
			case "*":
				{

					foreach (var package in ModLoader.LoadedPackages)
					{
						foreach (var plugin in package.Plugins)
						{
							plugin.ILoadConfig();
							plugin.Load();
							plugin.Puts($"Reloaded plugin's config.");
						}
					}

					break;
				}

			default:
				{
					var pluginFound = false;

					foreach (var mod in ModLoader.LoadedPackages)
					{
						var plugins = Facepunch.Pool.GetList<RustPlugin>();
						plugins.AddRange(mod.Plugins);

						foreach (var plugin in plugins)
						{
							if (plugin.Name == name)
							{
								plugin.ILoadConfig();
								plugin.Load();
								plugin.Puts($"Reloaded plugin's config.");
								pluginFound = true;
							}
						}

						Facepunch.Pool.FreeList(ref plugins);
					}

					if (!pluginFound)
					{
						Logger.Warn($"Plugin {name} was not found or was typed incorrectly.");
					}
					break;
				}
		}
	}

	#endregion

	#region Permissions

	[CommandVar("defaultplayergroup", "The default group for any player with the regular authority level they get assigned to.")]
	[AuthLevel(2)]
	private string DefaultPlayerGroup { get { return Community.Runtime.Config.PlayerDefaultGroup; } set { Community.Runtime.Config.PlayerDefaultGroup = value; } }

	[CommandVar("defaultadmingroup", "The default group players with the admin flag get assigned to.")]
	[AuthLevel(2)]
	private string DefaultAdminGroup { get { return Community.Runtime.Config.AdminDefaultGroup; } set { Community.Runtime.Config.AdminDefaultGroup = value; } }

	[ConsoleCommand("grant", "Grant one or more permissions to users or groups. Do 'c.grant' for syntax info.")]
	[AuthLevel(2)]
	private void Grant(ConsoleSystem.Arg arg)
	{
		void PrintWarn()
		{
			arg.ReplyWith($"Syntax: c.grant <user|group> <name|id> <perm>");
		}

		if (!arg.HasArgs(3))
		{
			PrintWarn();
			return;
		}

		var action = arg.Args[0];
		var name = arg.Args[1];
		var perm = arg.Args[2];
		var user = permission.FindUser(name);

		switch (action)
		{
			case "user":
				if (permission.GrantUserPermission(user.Key, perm, null))
				{
					arg.ReplyWith($"Granted user '{user.Value.LastSeenNickname}' permission '{perm}'");
				}
				else
				{
					arg.ReplyWith($"Couldn't grant user permission.");
				}
				break;

			case "group":
				if (permission.GrantGroupPermission(name, perm, null))
				{
					arg.ReplyWith($"Granted group '{name}' permission '{perm}'");
				}
				else
				{
					arg.ReplyWith($"Couldn't grant group permission.");
				}
				break;

			default:
				PrintWarn();
				break;
		}
	}

	[ConsoleCommand("revoke", "Revoke one or more permissions from users or groups. Do 'c.revoke' for syntax info.")]
	[AuthLevel(2)]
	private void Revoke(ConsoleSystem.Arg arg)
	{
		void PrintWarn()
		{
			arg.ReplyWith($"Syntax: c.revoke <user|group> <name|id> <perm>");
		}

		if (!arg.HasArgs(3))
		{
			PrintWarn();
			return;
		}

		var action = arg.Args[0];
		var name = arg.Args[1];
		var perm = arg.Args[2];
		var user = permission.FindUser(name);

		switch (action)
		{
			case "user":
				if (permission.RevokeUserPermission(user.Key, perm))
				{
					arg.ReplyWith($"Revoked user '{user.Value?.LastSeenNickname}' permission '{perm}'");
				}
				else
				{
					arg.ReplyWith($"Couldn't revoke user permission.");
				}
				break;

			case "group":
				if (permission.RevokeGroupPermission(name, perm))
				{
					arg.ReplyWith($"Revoked group '{name}' permission '{perm}'");
				}
				else
				{
					arg.ReplyWith($"Couldn't revoke group permission.");
				}
				break;

			default:
				PrintWarn();
				break;
		}
	}

	[ConsoleCommand("show", "Displays information about a specific player or group (incl. permissions, groups and user list). Do 'c.show' for syntax info.")]
	[AuthLevel(2)]
	private void Show(ConsoleSystem.Arg arg)
	{
		void PrintWarn()
		{
			arg.ReplyWith($"Syntax: c.show <groups|perms>\n" +
				$"Syntax: c.show <group|user> <name|id>");
		}

		if (!arg.HasArgs(1)) { PrintWarn(); return; }

		var action = arg.Args[0];

		switch (action)
		{
			case "user":
				{
					if (!arg.HasArgs(2)) { PrintWarn(); return; }

					var name = arg.Args[1];
					var user = permission.FindUser(name);

					if (user.Value == null)
					{
						arg.ReplyWith($"Couldn't find that user.");
						return;
					}

					arg.ReplyWith($"User {user.Value.LastSeenNickname}[{user.Key}] found in {user.Value.Groups.Count:n0} groups:\n  {user.Value.Groups.Select(x => x).ToArray().ToString(", ", " and ")}\n" +
						$"and has {user.Value.Perms.Count:n0} permissions:\n  {user.Value.Perms.Select(x => x).ToArray().ToString(", ", " and ")}");
					break;
				}
			case "group":
				{
					if (!arg.HasArgs(2)) { PrintWarn(); return; }

					var name = arg.Args[1];

					if (!permission.GroupExists(name))
					{
						arg.ReplyWith($"Couldn't find that group.");
						return;
					}

					var users = permission.GetUsersInGroup(name);
					var permissions = permission.GetGroupPermissions(name, false);
					arg.ReplyWith($"Group {name} has {users.Length:n0} users:\n  {users.Select(x => x).ToArray().ToString(", ", " and ")}\n" +
						$"and has {permissions.Length:n0} permissions:\n  {permissions.Select(x => x).ToArray().ToString(", ", " and ")}");
					break;
				}
			case "groups":
				{
					var groups = permission.GetGroups();
					if (groups.Count() == 0)
					{
						arg.ReplyWith($"Couldn't find any group.");
						return;
					}

					arg.ReplyWith($"Groups:\n {String.Join(", ", groups)}");
					break;
				}
			case "perms":
				{
					var perms = permission.GetPermissions();
					if (perms.Count() == 0)
					{
						arg.ReplyWith($"Couldn't find any permission.");
					}

					arg.ReplyWith($"Permissions:\n {String.Join(", ", perms)}");

					break;
				}

			default:
				PrintWarn();
				break;
		}
	}

	[ConsoleCommand("usergroup", "Adds or removes a player from a group. Do 'c.usergroup' for syntax info.")]
	[AuthLevel(2)]
	private void UserGroup(ConsoleSystem.Arg arg)
	{
		void PrintWarn()
		{
			arg.ReplyWith($"Syntax: c.usergroup <add|remove> <player> <group>");
		}

		if (!arg.HasArgs(3))
		{
			PrintWarn();
			return;
		}

		var action = arg.Args[0];
		var player = arg.Args[1];
		var group = arg.Args[2];

		var user = permission.FindUser(player);

		if (user.Value == null)
		{
			arg.ReplyWith($"Couldn't find that player.");
			return;
		}

		if (!permission.GroupExists(group))
		{
			arg.ReplyWith($"Group '{group}' could not be found.");
			return;
		}

		switch (action)
		{
			case "add":
				if (permission.UserHasGroup(user.Key, group))
				{
					arg.ReplyWith($"{user.Value.LastSeenNickname}[{user.Key}] is already in '{group}' group.");
					return;
				}

				permission.AddUserGroup(user.Key, group);
				arg.ReplyWith($"Added {user.Value.LastSeenNickname}[{user.Key}] to '{group}' group.");
				break;

			case "remove":
				if (!permission.UserHasGroup(user.Key, group))
				{
					arg.ReplyWith($"{user.Value.LastSeenNickname}[{user.Key}] isn't in '{group}' group.");
					return;
				}

				permission.RemoveUserGroup(user.Key, group);
				arg.ReplyWith($"Removed {user.Value.LastSeenNickname}[{user.Key}] from '{group}' group.");
				break;

			default:
				PrintWarn();
				break;
		}
	}

	[ConsoleCommand("group", "Adds or removes a group. Do 'c.group' for syntax info.")]
	[AuthLevel(2)]
	private void Group(ConsoleSystem.Arg arg)
	{
		void PrintWarn()
		{
			arg.ReplyWith($"Syntax: c.group add <group> [<displayName>] [<rank>]\n" +
				$"Syntax: c.group remove <group>\n" +
				$"Syntax: c.group set <group> <title|rank> <value>\n" +
				$"Syntax: c.group parent <group> [<parent>]");
		}

		if (!arg.HasArgs(1)) { PrintWarn(); return; }

		var action = arg.Args[0];

		switch (action)
		{
			case "add":
				{
					if (!arg.HasArgs(2)) { PrintWarn(); return; }

					var group = arg.Args[1];

					if (permission.GroupExists(group))
					{
						arg.ReplyWith($"Group '{group}' already exists. To set any values for this group, use 'c.group set'.");
						return;
					}

					if (permission.CreateGroup(group, arg.HasArgs(3) ? arg.Args[2] : group, arg.HasArgs(4) ? arg.Args[3].ToInt() : 0))
					{
						arg.ReplyWith($"Created '{group}' group.");
					}
				}
				break;

			case "set":
				{
					if (!arg.HasArgs(4)) { PrintWarn(); return; }

					var group = arg.Args[1];

					if (!permission.GroupExists(group))
					{
						arg.ReplyWith($"Group '{group}' does not exists.");
						return;
					}

					var set = arg.Args[2];
					var value = arg.Args[3];

					switch (set)
					{
						case "title":
							permission.SetGroupTitle(group, value);
							break;

						case "rank":
							permission.SetGroupRank(group, value.ToInt());
							break;
					}

					arg.ReplyWith($"Set '{group}' group.");
				}
				break;

			case "remove":
				{
					if (!arg.HasArgs(2)) { PrintWarn(); return; }

					var group = arg.Args[1];

					if (permission.RemoveGroup(group)) arg.ReplyWith($"Removed '{group}' group.");
					else arg.ReplyWith($"Couldn't remove '{group}' group.");
				}
				break;

			case "parent":
				{
					if (!arg.HasArgs(3)) { PrintWarn(); return; }

					var group = arg.Args[1];
					var parent = arg.Args[2];

					if (permission.SetGroupParent(group, parent)) arg.ReplyWith($"Changed '{group}' group's parent to '{parent}'.");
					else arg.ReplyWith($"Couldn't change '{group}' group's parent to '{parent}'.");
				}
				break;

			default:
				PrintWarn();
				break;
		}
	}

	#endregion

	#region CUI

	[ConsoleCommand("wipeui", "Clears the entire CUI containers and their elements from the caller's client.")]
	[AuthLevel(2)]
	private void WipeUI(ConsoleSystem.Arg arg)
	{
		CuiHelper.DestroyActivePanelList(arg.Player());
	}

	#endregion

	#region Markers

	[ConsoleCommand("wipemarkers", "Removes all markers of the calling player or argument filter.")]
	[AuthLevel(2)]
	private void ClearMarkers(ConsoleSystem.Arg arg)
	{
		var player = arg.Player();

		if (arg.HasArgs(1))
		{
			player = BasePlayer.FindAwakeOrSleeping(arg.GetString(0));
		}

		if (player == null)
		{
			arg.ReplyWith($"Couldn't find that player.");
			return;
		}

		arg.ReplyWith(arg.IsServerside ? $"Removed {player.displayName}'s map notes." : $"Removed all map notes.");

		player.Server_ClearMapMarkers(default);
		player.SendMarkersToClient();
		player.SendNetworkUpdate();
	}

	#endregion

#if DEBUG

	#region Profiling

	[ConsoleCommand("beginprofile", "Starts profiling the server.")]
	[AuthLevel(2)]
	private void BeginProfile(ConsoleSystem.Arg arg)
	{
		var date = DateTime.UtcNow;
		var duration = arg.GetFloat(0, -1);
		var name = arg.GetString(1, $"carbonprofile_{date.Year}-{date.Month}-{date.Day}_{date.Hour}{date.Minute}{date.Second}");

		Profiler.Make(name).Begin(duration, onEnd: duration == -1 ? null : profiler =>
		{
			Logger.Log($"Ended profiling, writing to disk: {profiler.Path}");
		});
		arg.ReplyWith("Began profiling...");
	}

	[ConsoleCommand("endprofile", "Ends profiling the server and asynchronously writes it to disk.")]
	[AuthLevel(2)]
	private void EndProfile(ConsoleSystem.Arg arg)
	{
		var path = Profiler.Singleton.Path;
		arg.ReplyWith(Profiler.End() ? $"Ended profiling, writing to disk: {path}" : "Couldn't end profile. Most likely because there's none started.");
	}

	#endregion

#endif
}
