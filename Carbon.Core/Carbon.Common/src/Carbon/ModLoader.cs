using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using API.Events;
using Carbon.Base;
using Carbon.Base.Interfaces;
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

public static class ModLoader
{
	public static List<Assembly> AssemblyCache { get; } = new();
	public static Dictionary<string, Assembly> AssemblyDictionaryCache { get; } = new();
	public static Dictionary<string, List<string>> PendingRequirees { get; } = new();
	public static bool IsBatchComplete { get; set; }
	public static List<string> PostBatchFailedRequirees { get; } = new();

	static ModLoader()
	{
		Community.Runtime.Events.Subscribe(
			CarbonEvent.OnServerInitialized,
			x => OnPluginProcessFinished()
		);
	}

	public static List<string> GetRequirees(Plugin initial)
	{
		if (string.IsNullOrEmpty(initial.FilePath)) return null;

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

		var list = Facepunch.Pool.GetList<ModPackage>();
		list.AddRange(LoadedPackages);

		foreach (var mod in list)
		{
			if (mod.IsCoreMod) continue;

			UnloadCarbonMod(mod.Name);
		}

		Facepunch.Pool.FreeList(ref list);
	}
	public static bool UnloadCarbonMod(string name)
	{
		var mod = GetPackage(name);
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

	public static void InitializePlugins(ModPackage mod)
	{
		Logger.Warn($"Initializing mod '{mod.Name}'");

		foreach (var type in mod.AllTypes)
		{
			try
			{
				if (!(type.Namespace.Equals("Oxide.Plugins") || type.Namespace.Equals("Carbon.Plugins"))) continue;

				if (!IsValidPlugin(type)) continue;

				if (!InitializePlugin(type, out var plugin, mod)) continue;
				plugin.HasInitialized = true;

				OnPluginProcessFinished();
			}
			catch (Exception ex) { Logger.Error($"Failed loading '{mod.Name}'", ex); }
		}
	}
	public static void UninitializePlugins(ModPackage mod)
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

	public static bool InitializePlugin(Type type, out RustPlugin plugin, ModPackage package = null, Action<RustPlugin> preInit = null, bool precompiled = false)
	{
		var constructor = type.GetConstructor(Type.EmptyTypes);
		var instance = FormatterServices.GetUninitializedObject(type);
		plugin = instance as RustPlugin;
		var info = type.GetCustomAttribute<InfoAttribute>();
		var desc = type.GetCustomAttribute<DescriptionAttribute>();

		if (info == null)
		{
			Logger.Warn($"Failed loading '{type.Name}'. The plugin doesn't have the Info attribute.");
			return false;
		}

		var title = info.Title?.Replace(" ", string.Empty);
		var author = info.Author;
		var version = info.Version;
		var description = desc == null ? string.Empty : desc.Description;

		plugin.SetProcessor(Community.Runtime.ScriptProcessor);
		plugin.SetupMod(package, title, author, version, description);

		plugin.IsPrecompiled = precompiled;

		try
		{
			constructor?.Invoke(instance, null);
		}
		catch (Exception ex)
		{
			Logger.Error($"Failed invoking {plugin.ToString()} constructor", ex);
		}

		if (precompiled)
		{
			ProcessPrecompiledType(plugin);
		}

		preInit?.Invoke(plugin);

		plugin.ILoadConfig();
		plugin.ILoadDefaultMessages();
		plugin.IInit();
		plugin.Load();

		package?.Plugins.Add(plugin);
		ProcessCommands(type, plugin);

		Logger.Log($"Loaded plugin {plugin.ToString()} [{plugin.CompileTime:0}ms]");
		return true;
	}
	public static bool UninitializePlugin(RustPlugin plugin)
	{
		plugin.CallHook("Unload");

		RemoveCommands(plugin);
		plugin.IUnload();

		HookCaller.CallStaticHook("OnPluginUnloaded", plugin);

		plugin.Dispose();
		Logger.Log($"Unloaded plugin {plugin.ToString()}");
		return true;
	}

	public static void ProcessPrecompiledType(RustPlugin plugin)
	{
		var type = plugin.GetType();
		var hooks = plugin.Hooks ??= new();
		var hookMethods = plugin.HookMethods ??= new();
		var pluginReferences = plugin.PluginReferences ??= new();

		foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
		{
			var hash = HookCallerCommon.StringPool.GetOrAdd(method.Name);

			if (Community.Runtime.HookManager.IsHookLoaded(method.Name))
			{
				var priority = method.GetCustomAttribute<HookPriority>();
				if (!hooks.ContainsKey(hash)) hooks.Add(hash, priority == null ? Priorities.Normal : priority.Priority);
			}
			else
			{
				var attribute = method.GetCustomAttribute<HookMethodAttribute>();
				if (attribute == null) continue;

				attribute.Method = method;
				hookMethods.Add(attribute);
			}
		}

		foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
		{
			var attribute = field.GetCustomAttribute<PluginReferenceAttribute>();
			if (attribute == null) continue;

			attribute.Field = field;
			pluginReferences.Add(attribute);
		}
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
			var protectedCommand = method.GetCustomAttribute<ProtectedCommandAttribute>();
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

			if (protectedCommand != null)
			{
				Community.Runtime.CorePlugin.cmd.AddConsoleCommand(Community.Protect(string.IsNullOrEmpty(prefix) ? protectedCommand.Name : $"{prefix}.{protectedCommand.Name}"), hookable, method.Name, help: protectedCommand.Help, reference: method, permissions: ps, groups: gs, authLevel: authLevel, cooldown: cooldownTime, isHidden: true);
			}

			if (ps != null && ps.Length > 0)
			{
				foreach (var permission in ps)
				{
					if (hookable is RustPlugin plugin && !plugin.permission.PermissionExists(permission, hookable))
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

			foreach (var mod in LoadedPackages)
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
				if (plugin is IModule module && (!module.GetEnabled() || plugin.HasInitialized)) continue;

				try
				{
					HookCaller.CallHook(plugin, "OnServerInitialized", Community.IsServerFullyInitialized);
				}
				catch (Exception initException)
				{
					Logger.Error($"[{plugin.Name}] Failed OnServerInitialized.", initException);
				}
			}

			if (counter > 1)
			{
				Logger.Log($" Batch completed! OSI on {counter:n0} {counter.Plural("plugin", "plugins")}.");
			}

			Report.OnProcessEnded?.Invoke();
			Community.Runtime.Events.Trigger(CarbonEvent.AllPluginsLoaded, EventArgs.Empty);

#if DEBUG_VERBOSE
			Logger.Error($"realtimeSinceStartup: {UnityEngine.Time.realtimeSinceStartup}");
#endif
		}
	}

	#endregion

	internal static ModPackage GetPackage(string name)
	{
		foreach (var mod in LoadedPackages)
		{
			if (mod.Name.StartsWith(name, StringComparison.OrdinalIgnoreCase)) return mod;
		}

		return null;
	}

	public static List<ModPackage> LoadedPackages = new();
	public static List<FailedMod> FailedMods = new();

	[JsonObject(MemberSerialization.OptIn)]
	public class ModPackage
	{
		public Assembly Assembly { get; set; }
		public Type[] AllTypes { get; set; }

		[JsonProperty]
		public string Name { get; set; } = string.Empty;

		[JsonProperty]
		public string File { get; set; } = string.Empty;

		[JsonProperty]
		public bool IsCoreMod { get; set; } = false;

		[JsonProperty]
		public List<RustPlugin> Plugins { get; set; } = new List<RustPlugin>();
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class FailedMod
	{
		[JsonProperty]
		public string File { get; set; } = string.Empty;

		[JsonProperty]
		public Error[] Errors { get; set; }

		[JsonProperty]
		public Error[] Warnings { get; set; }

		[JsonObject(MemberSerialization.OptIn)]
		public class Error
		{
			[JsonProperty]
			public string Number { get; set; }

			[JsonProperty]
			public string Message { get; set; }

			[JsonProperty]
			public int Column { get; set; }

			[JsonProperty]
			public int Line { get; set; }
		}
	}
}
