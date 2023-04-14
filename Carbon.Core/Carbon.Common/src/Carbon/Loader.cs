using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using API.Events;
using Carbon.Base;
using Carbon.Components;
using Carbon.Extensions;
using Facepunch;
using Newtonsoft.Json;
using Oxide.Core.Plugins;
using Oxide.Plugins;
using Report = Carbon.Components.Report;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Core;

public static class Loader
{
	public static List<Assembly> AssemblyCache { get; } = new();
	public static Dictionary<string, Assembly> AssemblyDictionaryCache { get; } = new();
	public static Dictionary<string, List<string>> PendingRequirees { get; } = new();
	public static bool IsBatchComplete { get; set; }
	public static List<string> PostBatchFailedRequirees { get; } = new();

	static Loader()
	{
		Community.Runtime.Events.Subscribe(
			CarbonEvent.OnServerInitialized,
			x => OnPluginProcessFinished()
		);
	}

	public static List<string> GetRequirees(Plugin initial)
	{
		if (PendingRequirees.TryGetValue(initial.FilePath, out var requirees))
		{
			return requirees;
		}

		return null;
	}
	public static void AddPendingRequiree(Plugin initial, Plugin requiree)
	{
		if (!PendingRequirees.TryGetValue(initial.FilePath, out var requirees))
		{
			PendingRequirees.Add(initial.FilePath, requirees = new List<string>(20));
		}

		requirees.Add(requiree.FilePath);
	}
	public static void ClearPendingRequirees(Plugin initial)
	{
		if (PendingRequirees.TryGetValue(initial.FilePath, out var requirees))
		{
			requirees.Clear();
			PendingRequirees[initial.FilePath] = null;
			PendingRequirees.Remove(initial.FilePath);
		}
	}
	public static void ClearAllRequirees()
	{
		var requirees = new Dictionary<string, List<string>>();
		foreach (var requiree in PendingRequirees) requirees.Add(requiree.Key, requiree.Value);

		foreach (var requiree in requirees)
		{
			requiree.Value.Clear();
			PendingRequirees[requiree.Key] = null;
		}

		PendingRequirees.Clear();
		requirees.Clear();
		requirees = null;
	}
	public static void ClearAllErrored()
	{
		foreach (var mod in FailedMods)
		{
			Array.Clear(mod.Errors, 0, mod.Errors.Length);
		}

		FailedMods.Clear();
	}

	public static void AppendAssembly(string key, Assembly assembly)
	{
		if (!AssemblyDictionaryCache.ContainsKey(key)) AssemblyDictionaryCache.Add(key, assembly);
		else AssemblyDictionaryCache[key] = assembly;
	}

	public static void UnloadCarbonMods()
	{
		ClearAllRequirees();

		var list = Facepunch.Pool.GetList<CarbonMod>();
		list.AddRange(LoadedMods);

		foreach (var mod in list)
		{
			if (mod.IsCoreMod) continue;

			UnloadCarbonMod(mod.Name);
		}

		Facepunch.Pool.FreeList(ref list);
	}
	public static bool UnloadCarbonMod(string name)
	{
		var mod = GetMod(name);
		if (mod == null)
		{
			return false;
		}

		//FIXMENOW
		// foreach (var hook in mod.Hooks)
		// {
		// 	try
		// 	{
		// 		var type = hook.GetType();
		// 		if (type.Name.Equals("CarbonInitializer")) continue;

		// 		hook.OnUnloaded(new EventArgs());
		// 	}
		// 	catch (Exception arg)
		// 	{
		// 		LogError(mod.Name, $"Failed to call hook 'OnLoaded' {arg}");
		// 	}
		// }

		UninitializePlugins(mod);
		return true;
	}

	#region Carbon

	public static void InitializePlugins(CarbonMod mod)
	{
		Logger.Warn($"Initializing mod '{mod.Name}'");

		foreach (var type in mod.AllTypes)
		{
			try
			{
				if (!(type.Namespace.Equals("Oxide.Plugins") || type.Namespace.Equals("Carbon.Plugins"))) continue;

				if (!IsValidPlugin(type)) continue;

				if (Community.Runtime.Config.HookValidation)
				{
					var counter = 0;
					foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
					{
						if (HookValidator.IsIncompatibleOxideHook(method.Name))
						{
							Logger.Warn($" Hook '{method.Name}' is not supported.");
							counter++;
						}
					}

					if (counter > 0)
					{
						Logger.Warn($"Plugin '{type.Name}' uses {counter:n0} Oxide hooks that Carbon doesn't support yet.");
						Logger.Warn($"Plugin '{type.Name}' will not work as expected.");
					}
				}

				if (!InitializePlugin(type, out var plugin, mod)) continue;
				plugin.HasInitialized = true;

				OnPluginProcessFinished();
			}
			catch (Exception ex) { Logger.Error($"Failed loading '{mod.Name}'", ex); }
		}
	}
	public static void UninitializePlugins(CarbonMod mod)
	{
		var plugins = Pool.GetList<RustPlugin>();
		plugins.AddRange(mod.Plugins);

		foreach (var plugin in plugins)
		{
			try
			{
				UninitializePlugin(plugin);
			}
			catch (Exception ex) { Logger.Error($"Failed unloading '{mod.Name}'", ex); }
		}

		Pool.FreeList(ref plugins);
	}

	public static bool InitializePlugin(Type type, out RustPlugin plugin, CarbonMod mod = null, Action<RustPlugin> preInit = null)
	{
		var instance = Activator.CreateInstance(type, false);
		plugin = instance as RustPlugin;
		var info = type.GetCustomAttribute<InfoAttribute>();
		var desc = type.GetCustomAttribute<DescriptionAttribute>();

		if (info == null)
		{
			Logger.Warn($"Failed loading '{type.Name}'. The plugin doesn't have the Info attribute.");
			return false;
		}

		var title = info.Title?.Replace(" ", "");
		var author = info.Author;
		var version = info.Version;
		var description = desc == null ? string.Empty : desc.Description;

		plugin.SetProcessor(Community.Runtime.ScriptProcessor);
		plugin.SetupMod(mod, title, author, version, description);

		preInit?.Invoke(plugin);

		plugin.ILoadConfig();
		plugin.ILoadDefaultMessages();
		plugin.IInit();
		plugin.Load();

		mod?.Plugins.Add(plugin);
		ProcessCommands(type, plugin);

		Logger.Log($"Loaded plugin {plugin.ToString()} [{plugin.CompileTime:0}ms]");
		return true;
	}
	public static bool UninitializePlugin(RustPlugin plugin)
	{
		plugin.CallHook("Unload");
		plugin.IUnload();

		HookCaller.CallStaticHook("OnPluginUnloaded", plugin);

		RemoveCommands(plugin);
		plugin.Dispose();
		Logger.Log($"Unloaded plugin {plugin.ToString()}");

		return true;
	}

	public static bool IsValidPlugin(Type type)
	{
		if (type == null) return false;
		if (type.Name == "RustPlugin" || type.Name == "CarbonPlugin") return true;
		return IsValidPlugin(type.BaseType);
	}

	public static void ProcessCommands(Type type, BaseHookable hookable = null, BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance, string prefix = null)
	{
		var methods = type.GetMethods(flags);
		var fields = type.GetFields(flags | BindingFlags.Public);
		var properties = type.GetProperties(flags | BindingFlags.Public);

		foreach (var method in methods)
		{
			var chatCommand = method.GetCustomAttribute<ChatCommandAttribute>();
			var consoleCommand = method.GetCustomAttribute<ConsoleCommandAttribute>();
			var rconCommand = method.GetCustomAttribute<RConCommandAttribute>();
			var uiCommand = method.GetCustomAttribute<UiCommandAttribute>();
			var command = method.GetCustomAttribute<CommandAttribute>();
			var permissions = method.GetCustomAttributes<PermissionAttribute>();
			var groups = method.GetCustomAttributes<GroupAttribute>();
			var authLevelAttribute = method.GetCustomAttribute<AuthLevelAttribute>();
			var cooldown = method.GetCustomAttribute<CooldownAttribute>();
			var authLevel = authLevelAttribute == null ? -1 : authLevelAttribute.AuthLevel;
			var ps = permissions.Count() == 0 ? null : permissions?.Select(x => x.Name).ToArray();
			var gs = groups.Count() == 0 ? null : groups?.Select(x => x.Name).ToArray();
			var cooldownTime = cooldown == null ? 0 : cooldown.Miliseconds;

			if (command != null)
			{
				foreach (var commandName in command.Names)
				{
					var name = string.IsNullOrEmpty(prefix) ? commandName : $"{prefix}.{commandName}";
					Community.Runtime.CorePlugin.cmd.AddChatCommand(name, hookable, method.Name, help: string.Empty, reference: method, permissions: ps, groups: gs, authLevel: authLevel, cooldown: cooldownTime);
					Community.Runtime.CorePlugin.cmd.AddConsoleCommand(name, hookable, method.Name, help: string.Empty, reference: method, permissions: ps, groups: gs, authLevel: authLevel, cooldown: cooldownTime);
				}
			}

			if (chatCommand != null)
			{
				Community.Runtime.CorePlugin.cmd.AddChatCommand(string.IsNullOrEmpty(prefix) ? chatCommand.Name : $"{prefix}.{chatCommand.Name}", hookable, method.Name, help: chatCommand.Help, reference: method, permissions: ps, groups: gs, authLevel: authLevel, cooldown: cooldownTime);
			}

			if (consoleCommand != null)
			{
				Community.Runtime.CorePlugin.cmd.AddConsoleCommand(string.IsNullOrEmpty(prefix) ? consoleCommand.Name : $"{prefix}.{consoleCommand.Name}", hookable, method.Name, help: consoleCommand.Help, reference: method, permissions: ps, groups: gs, authLevel: authLevel, cooldown: cooldownTime);
			}

			if (uiCommand != null)
			{
				Community.Runtime.CorePlugin.cmd.AddConsoleCommand(CUI.UniquifyCommand(string.IsNullOrEmpty(prefix) ? uiCommand.Name : $"{prefix}.{uiCommand.Name}"), hookable, method.Name, help: uiCommand.Help, reference: method, permissions: ps, groups: gs, authLevel: authLevel, cooldown: cooldownTime, isHidden: true);
			}

			if (ps != null && ps.Length > 0)
			{
				foreach (var permission in ps)
				{
					if (hookable is RustPlugin plugin)
					{
						plugin.permission.RegisterPermission(permission, hookable);
					}
				}
			}
		}

		foreach (var field in fields)
		{
			var var = field.GetCustomAttribute<CommandVarAttribute>();
			var permissions = field.GetCustomAttributes<PermissionAttribute>();
			var groups = field.GetCustomAttributes<GroupAttribute>();
			var authLevelAttribute = field.GetCustomAttribute<AuthLevelAttribute>();
			var cooldown = field.GetCustomAttribute<CooldownAttribute>();
			var authLevel = authLevelAttribute == null ? -1 : authLevelAttribute.AuthLevel;
			var ps = permissions.Count() == 0 ? null : permissions?.Select(x => x.Name).ToArray();
			var gs = groups.Count() == 0 ? null : groups?.Select(x => x.Name).ToArray();
			var cooldownTime = cooldown == null ? 0 : cooldown.Miliseconds;

			if (var != null)
			{
				Community.Runtime.CorePlugin.cmd.AddConsoleCommand(string.IsNullOrEmpty(prefix) ? var.Name : $"{prefix}.{var.Name}", hookable, (player, command, args) =>
				{
					var value = field.GetValue(hookable);

					if (args != null && args.Length > 0)
					{
						var rawString = args.ToString(" ");

						try
						{
							if (field.FieldType == typeof(string))
							{
								value = rawString;
							}
							else if (field.FieldType == typeof(bool))
							{
								value = rawString.ToBool();
							}
							if (field.FieldType == typeof(int))
							{
								value = rawString.ToInt();
							}
							if (field.FieldType == typeof(uint))
							{
								value = rawString.ToUint();
							}
							else if (field.FieldType == typeof(float))
							{
								value = rawString.ToFloat();
							}
							else if (field.FieldType == typeof(long))
							{
								value = rawString.ToLong();
							}
							else if (field.FieldType == typeof(ulong))
							{
								value = rawString.ToUlong();
							}

							field.SetValue(hookable, value);
						}
						catch { }
					}

					value = field.GetValue(hookable);
					if (value != null && var.Protected) value = new string('*', value.ToString().Length);

					Community.LogCommand($"{command}: \"{value}\"", player);
				}, help: var.Help, reference: field, permissions: ps, groups: gs, authLevel: authLevel, cooldown: cooldownTime, @protected: var.Protected);
			}
		}

		foreach (var property in properties)
		{
			var var = property.GetCustomAttribute<CommandVarAttribute>();
			var permissions = property.GetCustomAttributes<PermissionAttribute>();
			var groups = property.GetCustomAttributes<GroupAttribute>();
			var authLevelAttribute = property.GetCustomAttribute<AuthLevelAttribute>();
			var cooldown = property.GetCustomAttribute<CooldownAttribute>();
			var authLevel = authLevelAttribute == null ? -1 : authLevelAttribute.AuthLevel;
			var ps = permissions.Count() == 0 ? null : permissions?.Select(x => x.Name).ToArray();
			var gs = groups.Count() == 0 ? null : groups?.Select(x => x.Name).ToArray();
			var cooldownTime = cooldown == null ? 0 : cooldown.Miliseconds;

			if (var != null)
			{
				Community.Runtime.CorePlugin.cmd.AddConsoleCommand(string.IsNullOrEmpty(prefix) ? var.Name : $"{prefix}.{var.Name}", hookable, (player, command, args) =>
				{
					var value = property.GetValue(hookable);

					if (args != null && args.Length > 0)
					{
						var rawString = args.ToString(" ");

						try
						{
							if (property.PropertyType == typeof(string))
							{
								value = rawString;
							}
							else if (property.PropertyType == typeof(bool))
							{
								value = rawString.ToBool();
							}
							if (property.PropertyType == typeof(int))
							{
								value = rawString.ToInt();
							}
							if (property.PropertyType == typeof(uint))
							{
								value = rawString.ToUint();
							}
							else if (property.PropertyType == typeof(float))
							{
								value = rawString.ToFloat();
							}
							else if (property.PropertyType == typeof(long))
							{
								value = rawString.ToLong();
							}
							else if (property.PropertyType == typeof(ulong))
							{
								value = rawString.ToUlong();
							}

							property.SetValue(hookable, value);
						}
						catch { }
					}

					value = property.GetValue(hookable);
					if (value != null && var.Protected) value = new string('*', value.ToString().Length);

					Community.LogCommand($"{command}: \"{value}\"", player);
				}, help: var.Help, reference: property, permissions: ps, groups: gs, authLevel: authLevel, cooldown: cooldownTime, @protected: var.Protected);
			}
		}

		Array.Clear(methods, 0, methods.Length);
		Array.Clear(fields, 0, fields.Length);
		Array.Clear(properties, 0, properties.Length);
	}
	public static void RemoveCommands(BaseHookable hookable)
	{
		if (hookable == null) return;

		Community.Runtime.CommandManager.ClearCommands(command => command.Reference == hookable);
	}

	public static void OnPluginProcessFinished()
	{
		var temp = Pool.GetList<string>();
		temp.AddRange(PostBatchFailedRequirees);

		foreach (var plugin in temp)
		{
			var file = System.IO.Path.GetFileNameWithoutExtension(plugin);
			Community.Runtime.ScriptProcessor.ClearIgnore(file);
			Community.Runtime.ScriptProcessor.Prepare(file, plugin);
		}

		PostBatchFailedRequirees.Clear();

		if (PostBatchFailedRequirees.Count == 0)
		{
			IsBatchComplete = true;
		}

		temp.Clear();
		Pool.FreeList(ref temp);

		if (Community.IsServerFullyInitialized)
		{
			var counter = 0;
			var plugins = Pool.GetList<RustPlugin>();

			foreach (var mod in LoadedMods)
			{
				foreach (var plugin in mod.Plugins)
				{
					plugins.Add(plugin);
				}
			}

			foreach (var plugin in plugins)
			{
				try { plugin.InternalApplyPluginReferences(); } catch { }
			}

			foreach (var plugin in plugins)
			{
				if (plugin.HasInitialized) continue;
				counter++;

				try
				{
					plugin.CallHook("OnServerInitialized", Community.IsServerFullyInitialized);
				}
				catch (Exception initException)
				{
					plugin.LogError($"Failed OnServerInitialized.", initException);
				}

				plugin.HasInitialized = true;
			}

			Pool.FreeList(ref plugins);

			foreach (var plugin in Community.Runtime.ModuleProcessor.Modules)
			{
				if (plugin.HasInitialized) continue;

				try
				{
					HookCaller.CallHook(plugin, "OnServerInitialized", Community.IsServerFullyInitialized);
				}
				catch (Exception initException)
				{
					Logger.Error($"[{plugin.Name}] Failed OnServerInitialized.", initException);
				}

				plugin.HasInitialized = true;
			}

			if (counter > 1)
			{
				Logger.Log($" Batch completed! OSI on {counter:n0} {counter.Plural("plugin", "plugins")}.");
			}

			Report.OnProcessEnded?.Invoke();
			Community.Runtime.Events.Trigger(CarbonEvent.AllPluginsLoaded, EventArgs.Empty);
		}
	}

	#endregion

	internal static CarbonMod GetMod(string name)
	{
		foreach (var mod in LoadedMods)
		{
			if (mod.Name.StartsWith(name, StringComparison.OrdinalIgnoreCase)) return mod;
		}

		return null;
	}
	internal static Assembly LoadAssembly(string assemblyPath)
	{
		try
		{
			if (!File.Exists(assemblyPath))
				throw new FileNotFoundException($"File not found '{assemblyPath}'");

			var rawAssembly = File.ReadAllBytes(assemblyPath);
			if (rawAssembly == null) throw new Exception("No bytes read from file");

			return Assembly.Load(rawAssembly);
		}
		catch (Exception ex)
		{
			Logger.Error($"[LoadAssembly] Failed processing '{assemblyPath}'\n{ex}");
		}

		return null;
	}
	internal static bool IsKnownDependency(string assemblyName)
	{
		return assemblyName.StartsWith("System.", StringComparison.InvariantCultureIgnoreCase)
			|| assemblyName.StartsWith("Microsoft.", StringComparison.InvariantCultureIgnoreCase)
			|| assemblyName.StartsWith("Newtonsoft.", StringComparison.InvariantCultureIgnoreCase)
			|| assemblyName.StartsWith("UnityEngine.", StringComparison.InvariantCultureIgnoreCase);
	}

	internal static void ReportException(string harmonyId, Exception e)
	{
		LogError(harmonyId, e);
		ReflectionTypeLoadException ex;
		if ((ex = e as ReflectionTypeLoadException) != null)
		{
			LogError(harmonyId, string.Format("Has {0} LoaderExceptions:", ex.LoaderExceptions));
			foreach (var e2 in ex.LoaderExceptions)
			{
				ReportException(harmonyId, e2);
			}
		}
		if (e.InnerException != null)
		{
			LogError(harmonyId, "Has InnerException:");
			ReportException(harmonyId, e.InnerException);
		}
	}
	internal static void Log(string harmonyId, object message)
		=> Logger.Log($"[{harmonyId}] {message}");

	internal static void LogError(string harmonyId, object message)
		=> Logger.Error($"[{harmonyId}] {message}");

	public static List<CarbonMod> LoadedMods = new();
	public static List<FailedMod> FailedMods = new();

	[JsonObject(MemberSerialization.OptIn)]
	public class CarbonMod
	{
		[JsonProperty]
		public string Name { get; set; } = string.Empty;
		[JsonProperty]
		public string File { get; set; } = string.Empty;
		[JsonProperty]
		public bool IsCoreMod { get; set; } = false;
		public Assembly Assembly { get; set; }
		public Type[] AllTypes { get; set; }
		//public List<IHarmonyMod> Hooks { get; } = new List<IHarmonyMod>();

		[JsonProperty]
		public List<RustPlugin> Plugins { get; set; } = new List<RustPlugin>();
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class FailedMod
	{
		[JsonProperty]
		public string File { get; set; } = string.Empty;

		[JsonProperty]
		public string[] Errors { get; set; }
	}
}
