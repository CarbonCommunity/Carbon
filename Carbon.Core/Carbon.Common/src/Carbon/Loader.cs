using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Carbon.Base;
using Carbon.Components;
using Carbon.Extensions;
using Newtonsoft.Json;
using Oxide.Plugins;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Core;

public static class Loader
{
	public static List<Assembly> AssemblyCache { get; } = new List<Assembly>();
	public static Dictionary<string, Assembly> AssemblyDictionaryCache { get; } = new Dictionary<string, Assembly>();
	public static Dictionary<string, List<string>> PendingRequirees { get; } = new Dictionary<string, List<string>>();

	static Loader()
	{
		CommunityCommon.CommonRuntime.Events.Subscribe(
			API.Events.CarbonEvent.OnServerInitialized,
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
	public static bool LoadCarbonMod(string fullPath, bool silent = false)
	{
		if (!File.Exists(fullPath)) return false;

		var fileName = Path.GetFileName(fullPath);

		if (fileName.EndsWith(".dll"))
			fileName = fileName.Substring(0, fileName.Length - 4);
		var domain = "com.rust.carbon." + fileName;

		UnloadCarbonMod(fileName);

		try
		{
			var assembly = LoadAssembly(fullPath);

			if (assembly == null)
			{
				LogError(domain, $"Failed to load harmony mod '{fileName}.dll' from '{_modPath}'");
				return false;
			}

			var mod = new CarbonMod
			{
				Assembly = assembly,
				AllTypes = assembly.GetTypes(),
				Name = fileName,
				File = fullPath
			};

			foreach (var type in mod.AllTypes)
			{
				if (typeof(IHarmonyModHooks).IsAssignableFrom(type))
				{
					try
					{
						var harmonyModHooks = Activator.CreateInstance(type) as IHarmonyModHooks;

						if (harmonyModHooks == null) LogError(mod.Name, "Failed to create hook instance: Is null");
						else mod.Hooks.Add(harmonyModHooks);
					}
					catch (Exception e)
					{
						LogError(mod.Name, $"Failed to create hook instance {e}");
					}
				}
			}

			try
			{
				mod.Harmonyv2 = new HarmonyLib.Harmony(domain);
				mod.Harmonyv2.PatchAll(assembly);
			}
			catch (Exception e)
			{
				if (!silent)
					LogError(mod.Name, string.Format("Failed to patch all v2 hooks: {0}", e));
			}

			foreach (var hook in mod.Hooks)
			{
				try
				{
					var type = hook.GetType();
					if (type.Name.Equals("CarbonInitializer")) continue;

					hook.OnLoaded(new OnHarmonyModLoadedArgs());
				}
				catch (Exception e)
				{
					if (!silent)
						LogError(mod.Name, string.Format("Failed to call hook 'OnLoaded' {0}", e));
				}
			}

			AppendAssembly(mod.Name, assembly);
			AssemblyCache.Add(assembly);
			LoadedMods.Add(mod);

			InitializePlugins(mod);
		}
		catch (Exception e)
		{
			if (!silent) LogError(domain, "Failed to load: " + fullPath);
			ReportException(domain, e);
			return false;
		}
		return true;
	}
	public static bool UnloadCarbonMod(string name)
	{
		var mod = GetMod(name);
		if (mod == null)
		{
			return false;
		}

		foreach (var hook in mod.Hooks)
		{
			try
			{
				var type = hook.GetType();
				if (type.Name.Equals("CarbonInitializer")) continue;

				hook.OnUnloaded(new OnHarmonyModUnloadedArgs());
			}
			catch (Exception arg)
			{
				LogError(mod.Name, $"Failed to call hook 'OnLoaded' {arg}");
			}
		}

		UnloadMod(mod);
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
				if (!(type.Namespace.Equals("Oxide.Plugins") || type.Namespace.Equals("Carbon.Plugins"))) return;

				if (!IsValidPlugin(type)) continue;

				if (CommunityCommon.CommonRuntime.Config.HookValidation)
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
		foreach (var plugin in mod.Plugins)
		{
			try
			{
				UninitializePlugin(plugin);
			}
			catch (Exception ex) { Logger.Error($"Failed unloading '{mod.Name}'", ex); }
		}
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

		plugin.SetProcessor(CommunityCommon.CommonRuntime.ScriptProcessor);
		plugin.SetupMod(mod, title, author, version, description);

		preInit?.Invoke(plugin);

		plugin.ILoadConfig();
		plugin.ILoadDefaultMessages();
		plugin.IInit();
		plugin.Load();
		HookCaller.CallStaticHook("OnPluginLoaded", plugin);

		if (mod != null) mod.Plugins.Add(plugin);
		ProcessCommands(type, plugin);

		Logger.Log($"Loaded plugin {plugin.ToString()}");

		return true;
	}
	public static bool UninitializePlugin(RustPlugin plugin)
	{
		plugin.CallHook("Unload");
		plugin.IUnload();

		RemoveCommands(plugin);
		HookCaller.CallStaticHook("OnPluginUnloaded", plugin);
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
			var uiCommand = method.GetCustomAttribute<UiCommandAttribute>();
			var command = method.GetCustomAttribute<CommandAttribute>();
			var permissions = method.GetCustomAttributes<PermissionAttribute>();
			var groups = method.GetCustomAttributes<GroupAttribute>();
			var authLevelAttribute = method.GetCustomAttribute<AuthLevelAttribute>();
			var cooldown = method.GetCustomAttribute<CooldownAttribute>();
			var authLevel = authLevelAttribute == null ? -1 : (int)authLevelAttribute.Group;
			var ps = permissions.Count() == 0 ? null : permissions?.Select(x => x.Name).ToArray();
			var gs = groups.Count() == 0 ? null : groups?.Select(x => x.Name).ToArray();
			var cooldownTime = cooldown == null ? 0 : cooldown.Miliseconds;

			if (command != null)
			{
				foreach (var commandName in command.Names)
				{
					CommunityCommon.CommonRuntime.CorePlugin.cmd.AddChatCommand(string.IsNullOrEmpty(prefix) ? commandName : $"{prefix}.{commandName}", hookable, method.Name, help: command.Help, reference: method, permissions: ps, groups: gs, authLevel: authLevel, cooldown: cooldownTime);
					CommunityCommon.CommonRuntime.CorePlugin.cmd.AddConsoleCommand(string.IsNullOrEmpty(prefix) ? commandName : $"{prefix}.{commandName}", hookable, method.Name, help: command.Help, reference: method, permissions: ps, groups: gs, authLevel: authLevel, cooldown: cooldownTime);
				}
			}

			if (chatCommand != null)
			{
				CommunityCommon.CommonRuntime.CorePlugin.cmd.AddChatCommand(string.IsNullOrEmpty(prefix) ? chatCommand.Name : $"{prefix}.{chatCommand.Name}", hookable, method.Name, help: chatCommand.Help, reference: method, permissions: ps, groups: gs, authLevel: authLevel, cooldown: cooldownTime);
			}

			if (consoleCommand != null)
			{
				CommunityCommon.CommonRuntime.CorePlugin.cmd.AddConsoleCommand(string.IsNullOrEmpty(prefix) ? consoleCommand.Name : $"{prefix}.{consoleCommand.Name}", hookable, method.Name, help: consoleCommand.Help, reference: method, permissions: ps, groups: gs, authLevel: authLevel, cooldown: cooldownTime);
			}

			if (uiCommand != null)
			{
				CommunityCommon.CommonRuntime.CorePlugin.cmd.AddConsoleCommand(UiCommandAttribute.Uniquify(string.IsNullOrEmpty(prefix) ? uiCommand.Name : $"{prefix}.{uiCommand.Name}"), hookable, method.Name, help: uiCommand.Help, reference: method, permissions: ps, groups: gs, authLevel: authLevel, cooldown: cooldownTime);
			}
		}

		foreach (var field in fields)
		{
			var var = field.GetCustomAttribute<CommandVarAttribute>();
			var permissions = field.GetCustomAttributes<PermissionAttribute>();
			var groups = field.GetCustomAttributes<GroupAttribute>();
			var authLevelAttribute = field.GetCustomAttribute<AuthLevelAttribute>();
			var cooldown = field.GetCustomAttribute<CooldownAttribute>();
			var authLevel = authLevelAttribute == null ? -1 : (int)authLevelAttribute.Group;
			var ps = permissions.Count() == 0 ? null : permissions?.Select(x => x.Name).ToArray();
			var gs = groups.Count() == 0 ? null : groups?.Select(x => x.Name).ToArray();
			var cooldownTime = cooldown == null ? 0 : cooldown.Miliseconds;

			if (var != null)
			{
				CommunityCommon.CommonRuntime.CorePlugin.cmd.AddConsoleCommand(string.IsNullOrEmpty(prefix) ? var.Name : $"{prefix}.{var.Name}", hookable, (player, command, args) =>
				{
					if (player != null && var.AdminOnly && !player.IsAdmin)
					{
						CommunityCommon.LogCommand($"You don't have permission to set this value", player);
						return;
					}

					var value = field.GetValue(hookable);

					if (args != null && args.Length > 0)
					{
						var rawString = args.ToString(" ");

						try
						{
							if (field.FieldType == typeof(string))
							{
								value = rawString.ToFloat();
							}
							if (field.FieldType == typeof(int))
							{
								value = rawString.ToInt();
							}
							else if (field.FieldType == typeof(float))
							{
								value = rawString.ToFloat();
							}
							else if (field.FieldType == typeof(ulong))
							{
								value = rawString.ToUlong();
							}
							else if (field.FieldType == typeof(bool))
							{
								value = rawString.ToBool();
							}

							field.SetValue(hookable, value);
						}
						catch { }
					}

					CommunityCommon.LogCommand($"{command}: \"{value}\"", player);
				}, help: var.Help, reference: field, permissions: ps, groups: gs, authLevel: authLevel, cooldown: cooldownTime);
			}
		}

		foreach (var property in properties)
		{
			var var = property.GetCustomAttribute<CommandVarAttribute>();
			var permissions = property.GetCustomAttributes<PermissionAttribute>();
			var groups = property.GetCustomAttributes<GroupAttribute>();
			var authLevelAttribute = property.GetCustomAttribute<AuthLevelAttribute>();
			var cooldown = property.GetCustomAttribute<CooldownAttribute>();
			var authLevel = authLevelAttribute == null ? -1 : (int)authLevelAttribute.Group;
			var ps = permissions.Count() == 0 ? null : permissions?.Select(x => x.Name).ToArray();
			var gs = groups.Count() == 0 ? null : groups?.Select(x => x.Name).ToArray();
			var cooldownTime = cooldown == null ? 0 : cooldown.Miliseconds;

			if (var != null)
			{
				CommunityCommon.CommonRuntime.CorePlugin.cmd.AddConsoleCommand(string.IsNullOrEmpty(prefix) ? var.Name : $"{prefix}.{var.Name}", hookable, (player, command, args) =>
				{
					if (player != null && var.AdminOnly && !player.IsAdmin)
					{
						CommunityCommon.LogCommand($"You don't have permission to set this value", player);
						return;
					}

					var value = property.GetValue(hookable);

					if (args != null && args.Length > 0)
					{
						var rawString = args.ToString(" ");

						try
						{
							if (property.PropertyType == typeof(string))
							{
								value = rawString.ToFloat();
							}
							if (property.PropertyType == typeof(int))
							{
								value = rawString.ToInt();
							}
							else if (property.PropertyType == typeof(float))
							{
								value = rawString.ToFloat();
							}
							else if (property.PropertyType == typeof(ulong))
							{
								value = rawString.ToUlong();
							}
							else if (property.PropertyType == typeof(bool))
							{
								value = rawString.ToBool();
							}

							property.SetValue(hookable, value);
						}
						catch { }
					}

					CommunityCommon.LogCommand($"{command}: \"{value}\"", player);
				}, help: var.Help, reference: property, permissions: ps, groups: gs, authLevel: authLevel, cooldown: cooldownTime);
			}
		}

		Facepunch.Pool.Free(ref methods);
		Facepunch.Pool.Free(ref fields);
		Facepunch.Pool.Free(ref properties);
	}
	public static void RemoveCommands(RustPlugin plugin)
	{
		CommunityCommon.CommonRuntime.AllChatCommands.RemoveAll(x => x.Plugin == plugin);
		CommunityCommon.CommonRuntime.AllConsoleCommands.RemoveAll(x => x.Plugin == plugin);
	}

	public static void OnPluginProcessFinished()
	{
		if (CommunityCommon.IsServerFullyInitialized)
		{
			var counter = 0;

			foreach (var mod in LoadedMods)
			{
				foreach (var plugin in mod.Plugins)
				{
					try { plugin.InternalApplyPluginReferences(); } catch { }
				}
			}

			foreach (var mod in LoadedMods)
			{
				foreach (var plugin in mod.Plugins)
				{
					if (plugin.HasInitialized) continue;
					counter++;

					try
					{
						plugin.CallHook("OnServerInitialized", CommunityCommon.IsServerFullyInitialized);
					}
					catch (Exception initException)
					{
						plugin.LogError($"Failed OnServerInitialized.", initException);
					}

					plugin.HasInitialized = true;
				}
			}

			foreach (var plugin in CommunityCommon.CommonRuntime.ModuleProcessor.Modules)
			{
				if (plugin.HasInitialized) continue;

				try
				{
					HookCaller.CallHook(plugin, "OnServerInitialized", CommunityCommon.IsServerFullyInitialized);
				}
				catch (Exception initException)
				{
					Logger.Error($"[{plugin.Name}] Failed OnServerInitialized.", initException);
				}

				plugin.HasInitialized = true;
			}

			if (counter > 1) Logger.Log($" Batch completed! OSI on {counter:n0} {counter.Plural("plugin", "plugins")}.");

			Report.OnProcessEnded?.Invoke();
		}
	}

	#endregion

	internal static void UnloadMod(CarbonMod mod)
	{
		if (mod.IsCoreMod) return;

		if (mod.Harmonyv2 != null)
		{
			Log(mod.Name, $"Unpatching hooks for '{mod.Name}' on v2...");

			try
			{
				mod.Harmonyv2.UnpatchAll(mod.Harmonyv2.Id);
				Log(mod.Name, "Unloaded v2 mod");
			}
			catch (Exception ex)
			{
				Logger.Error($"Failed unpatching all v2 patches.", ex);
			}

			mod.Harmonyv2 = null;
		}

		LoadedMods.Remove(mod);
	}
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

	internal static string _modPath;

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
		public HarmonyLib.Harmony Harmonyv2 { get; set; }
		public Assembly Assembly { get; set; }
		public Type[] AllTypes { get; set; }
		public List<IHarmonyModHooks> Hooks { get; } = new List<IHarmonyModHooks>();

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
