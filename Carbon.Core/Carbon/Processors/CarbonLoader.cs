///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Harmony;
using Humanlights.Extensions;
using Newtonsoft.Json;
using Oxide.Plugins;
using UnityEngine;

namespace Carbon.Core
{
	public static class CarbonLoader
	{
		public static List<Assembly> AssemblyCache { get; } = new List<Assembly>();
		public static Dictionary<string, Assembly> AssemblyDictionaryCache { get; } = new Dictionary<string, Assembly>();

		public static void AppendAssembly(string key, Assembly assembly)
		{
			if (!AssemblyDictionaryCache.ContainsKey(key)) AssemblyDictionaryCache.Add(key, assembly);
			else AssemblyDictionaryCache[key] = assembly;
		}

		public static void LoadCarbonMods()
		{
			try
			{
				HarmonyInstance.DEBUG = true;
				var path = Path.Combine(CarbonCore.GetLogsFolder(), "..");
				FileLog.logPath = Path.Combine(path, "carbon_log.txt");
				try
				{
					File.Delete(FileLog.logPath);
				}
				catch { }

				_modPath = CarbonCore.GetPluginsFolder();
				if (!Directory.Exists(_modPath))
				{
					try
					{
						Directory.CreateDirectory(_modPath);
						return;
					}
					catch { return; }
				}

				AppDomain.CurrentDomain.AssemblyResolve += delegate (object sender, ResolveEventArgs args)
				{
					if (!Regex.IsMatch(args.Name, @"^(Microsoft|System)\."))
						Debug.Log($"Resolving assembly ref: {args.Name}");

					AssemblyName assemblyName = new AssemblyName(args.Name);
					string assemblyPath = Path.GetFullPath(
						Path.Combine(_modPath, assemblyName.Name, ".dll"));

					// This allows plugins to use Carbon.xxx
					if (Regex.IsMatch(assemblyName.Name, @"^([Cc]arbon(-.+)?)$"))
						assemblyPath = CarbonCore.DllPath;

					if (File.Exists(assemblyPath))
						return LoadAssembly(assemblyPath);
					return null;
				};

				foreach (string text in Directory.EnumerateFiles(_modPath, "*.dll"))
				{
					if (!string.IsNullOrEmpty(text) && !IsKnownDependency(Path.GetFileNameWithoutExtension(text)))
					{
						CarbonCore.Instance.HarmonyProcessor.Prepare(text);
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Instance.Error("Loading all DLLs failed.", ex);
			}
			finally
			{
				FileLog.FlushBuffer();
			}
		}
		public static void UnloadCarbonMods()
		{
			var list = Facepunch.Pool.GetList<CarbonMod>();
			list.AddRange(_loadedMods);

			foreach (var mod in list)
			{
				if (mod.IsCoreMod) continue;

				UnloadCarbonMod(mod.Name);
			}

			Facepunch.Pool.FreeList(ref list);
		}
		public static bool LoadCarbonMod(string fullPath, bool silent = false)
		{
			var fileName = Path.GetFileName(fullPath);

			if (fileName.EndsWith(".dll"))
			{
				fileName = fileName.Substring(0, fileName.Length - 4);
			}

			UnloadCarbonMod(fileName);

			var domain = "com.rust.carbon." + fileName;

			try
			{
				Assembly assembly = LoadAssembly(fullPath);
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
						catch (Exception arg) { LogError(mod.Name, $"Failed to create hook instance {arg}"); }
					}
				}

				mod.Harmony = HarmonyInstance.Create(domain);

				if (!CarbonCore.IsAddon(mod.Name))
				{
					try
					{
						mod.Harmony.PatchAll(assembly);
					}
					catch (Exception arg2)
					{
						LogError(mod.Name, string.Format("Failed to patch all hooks: {0}", arg2));
						return false;
					}
				}
				else
				{
					Logger.Instance.Warn($" Loaded addon '{mod.Name}'");
					CarbonCore.Instance.Addon.Addons.Add(mod.Assembly);
				}

				foreach (var hook in mod.Hooks)
				{
					try
					{
						var type = hook.GetType();
						if (type.Name.Equals("CarbonInitializer")) continue;

						hook.OnLoaded(new OnHarmonyModLoadedArgs());
					}
					catch (Exception arg3)
					{
						LogError(mod.Name, string.Format("Failed to call hook 'OnLoaded' {0}", arg3));
					}
				}

				AppendAssembly(mod.Name, assembly);
				AssemblyCache.Add(assembly);
				_loadedMods.Add(mod);

				InitializePlugins(mod);
			}
			catch (Exception e)
			{
				LogError(domain, "Failed to load: " + fullPath);
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

			if (mod.IsAddon) CarbonCore.Instance.Addon.Addons.Remove(mod.Assembly);
			return true;
		}

		#region Carbon

		public static void InitializePlugins(CarbonMod mod)
		{
			mod.IsAddon = CarbonCore.IsAddon(mod.Name);

			Carbon.Logger.Instance.Warn($"Initializing mod '{mod.Name}'");

			if (mod.IsAddon)
			{
				Log(mod.Name, "Initialized Carbon extension.");
			}

			foreach (var type in mod.AllTypes)
			{
				try
				{
					if (!(type.Namespace.Equals("Oxide.Plugins") ||
						type.Namespace.Equals("Carbon.Plugins"))) return;

					if (!IsValidPlugin(type)) continue;

					if (CarbonCore.Instance.Config.HookValidation)
					{
						var counter = 0;
						foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
						{
							if (CarbonHookValidator.IsIncompatibleOxideHook(method.Name))
							{
								Logger.Instance.Warn($" Hook '{method.Name}' is not supported.");
								counter++;
							}
						}

						if (counter > 0)
						{
							Logger.Instance.Warn($"Plugin '{type.Name}' uses {counter:n0} Oxide hooks that Carbon doesn't support yet.");
							Logger.Instance.Warn("The plugin will not work as expected.");
						}
					}

					var instance = Activator.CreateInstance(type, false);
					var plugin = instance as RustPlugin;
					var info = type.GetCustomAttribute<InfoAttribute>();
					var description = type.GetCustomAttribute<DescriptionAttribute>();

					if (info == null)
					{
						Logger.Instance.Warn($"Failed loading '{type.Name}'. The plugin doesn't have the Info attribute.");
						continue;
					}
					plugin.SetProcessor(CarbonCore.Instance.HarmonyProcessor);
					plugin.CallHook("SetupMod", mod, info.Title, info.Author, info.Version, description == null ? string.Empty : description.Description);
					HookExecutor.CallStaticHook("OnPluginLoaded", plugin);
					plugin.IInit();
					plugin.DoLoadConfig();
					plugin.Load();

					if (CarbonCore.IsServerFullyInitialized)
					{
						plugin.CallHook("OnServerInitialized");
						plugin.CallHook("OnServerInitialized", CarbonCore.IsServerFullyInitialized);
					}

					mod.Plugins.Add(plugin);
					ProcessCommands(type, plugin);

					Logger.Instance.Log($"Loaded plugin {plugin.ToString()}");
				}
				catch (Exception ex) { Logger.Instance.Error($"Failed loading '{mod.Name}'", ex); }
			}
		}
		public static void UninitializePlugins(CarbonMod mod)
		{
			foreach (var plugin in mod.Plugins)
			{
				try
				{
					HookExecutor.CallStaticHook("OnPluginUnloaded", plugin);
					plugin.CallHook("Unload");
					plugin.IUnload();
					RemoveCommands(plugin);
					plugin.Dispose();
					Logger.Instance.Log($"Unloaded plugin {plugin.ToString()}");
				}
				catch (Exception ex) { Logger.Instance.Error($"Failed unloading '{mod.Name}'", ex); }
			}
		}

		public static bool IsValidPlugin(Type type)
		{
			if (type == null) return false;
			if (type.Name == "RustPlugin" || type.Name == "CarbonPlugin") return true;
			return IsValidPlugin(type.BaseType);
		}

		public static void ProcessCommands(Type type, BaseHookable plugin = null, BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance, string prefix = null)
		{
			var methods = type.GetMethods(flags);
			var fields = type.GetFields(flags | BindingFlags.Public);
			var properties = type.GetProperties(flags | BindingFlags.Public);

			foreach (var method in methods)
			{
				var chatCommand = method.GetCustomAttribute<ChatCommandAttribute>();
				var consoleCommand = method.GetCustomAttribute<ConsoleCommandAttribute>();
				var command = method.GetCustomAttribute<CommandAttribute>();

				if (command != null)
				{
					foreach (var commandName in command.Names)
					{
						CarbonCore.Instance.CorePlugin.cmd.AddChatCommand(string.IsNullOrEmpty(prefix) ? commandName : $"{prefix}.{commandName}", plugin, method.Name, help: command.Help);
						CarbonCore.Instance.CorePlugin.cmd.AddConsoleCommand(string.IsNullOrEmpty(prefix) ? commandName : $"{prefix}.{commandName}", plugin, method.Name, help: command.Help);
					}
				}

				if (chatCommand != null)
				{
					CarbonCore.Instance.CorePlugin.cmd.AddChatCommand(string.IsNullOrEmpty(prefix) ? chatCommand.Name : $"{prefix}.{chatCommand.Name}", plugin, method.Name, help: chatCommand.Help);
				}

				if (consoleCommand != null)
				{
					CarbonCore.Instance.CorePlugin.cmd.AddConsoleCommand(string.IsNullOrEmpty(prefix) ? consoleCommand.Name : $"{prefix}.{consoleCommand.Name}", plugin, method.Name, help: consoleCommand.Help);
				}
			}

			foreach (var field in fields)
			{
				var var = field.GetCustomAttribute<CommandVarAttribute>();

				if (var != null)
				{
					CarbonCore.Instance.CorePlugin.cmd.AddConsoleCommand(string.IsNullOrEmpty(prefix) ? var.Name : $"{prefix}.{var.Name}", plugin, (player, command, args) =>
					{
						if (player != null && var.AdminOnly && !player.IsAdmin)
						{
							CarbonCore.LogCommand($"You don't have permission to set this value", player);
							return;
						}

						var value = field.GetValue(plugin);

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

								field.SetValue(plugin, value);
							}
							catch { }
						}

						CarbonCore.LogCommand($"{command}: \"{value}\"", player);
					}, help: var.Help);
				}
			}

			foreach (var property in properties)
			{
				var var = property.GetCustomAttribute<CommandVarAttribute>();

				if (var != null)
				{
					CarbonCore.Instance.CorePlugin.cmd.AddConsoleCommand(string.IsNullOrEmpty(prefix) ? var.Name : $"{prefix}.{var.Name}", plugin, (player, command, args) =>
					{
						if (player != null && var.AdminOnly && !player.IsAdmin)
						{
							CarbonCore.LogCommand($"You don't have permission to set this value", player);
							return;
						}

						var value = property.GetValue(plugin);

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

								property.SetValue(plugin, value);
							}
							catch { }
						}

						CarbonCore.LogCommand($"{command}: \"{value}\"", player);
					}, help: var.Help);
				}
			}

			Facepunch.Pool.Free(ref methods);
			Facepunch.Pool.Free(ref fields);
			Facepunch.Pool.Free(ref properties);
		}
		public static void RemoveCommands(RustPlugin plugin)
		{
			CarbonCore.Instance.AllChatCommands.RemoveAll(x => x.Plugin == plugin);
			CarbonCore.Instance.AllConsoleCommands.RemoveAll(x => x.Plugin == plugin);
		}

		#endregion

		internal static void UnloadMod(CarbonMod mod)
		{
			if (mod.IsCoreMod) return;

			if (mod.Harmony != null)
			{
				Log(mod.Name, "Unpatching hooks...");
				mod.Harmony.UnpatchAll(mod.Harmony.Id);
				Log(mod.Name, "Unloaded mod");
			}

			_loadedMods.Remove(mod);
		}
		internal static CarbonMod GetMod(string name)
		{
			foreach (var mod in _loadedMods)
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
				{
					return null;
				}

				var rawAssembly = File.ReadAllBytes(assemblyPath);
				var path = assemblyPath.Substring(0, assemblyPath.Length - 4) + ".pdb";

				if (File.Exists(path))
				{
					var rawSymbolStore = File.ReadAllBytes(path);
					return Assembly.Load(rawAssembly, rawSymbolStore);
				}

				return Assembly.Load(rawAssembly);
			}
			catch { }

			return null;
		}
		internal static bool IsKnownDependency(string assemblyName)
		{
			return assemblyName.StartsWith("System.", StringComparison.InvariantCultureIgnoreCase) || assemblyName.StartsWith("Microsoft.", StringComparison.InvariantCultureIgnoreCase) || assemblyName.StartsWith("Newtonsoft.", StringComparison.InvariantCultureIgnoreCase) || assemblyName.StartsWith("UnityEngine.", StringComparison.InvariantCultureIgnoreCase);
		}

		internal static void ReportException(string harmonyId, Exception e)
		{
			LogError(harmonyId, e);
			ReflectionTypeLoadException ex;
			if ((ex = (e as ReflectionTypeLoadException)) != null)
			{
				LogError(harmonyId, string.Format("Has {0} LoaderExceptions:", ex.LoaderExceptions));
				foreach (Exception e2 in ex.LoaderExceptions)
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
		{
			Logger.Instance.Format($"[{harmonyId}] {message}");
		}
		internal static void LogError(string harmonyId, object message)
		{
			Logger.Instance.ErrorFormat($"[{harmonyId}] {message}");
		}
		internal static void LogError(object message)
		{
			Logger.Instance.Error(message);
		}

		internal static string _modPath;

		internal static List<CarbonMod> _loadedMods = new List<CarbonMod>();

		[JsonObject(MemberSerialization.OptIn)]
		public class CarbonMod
		{
			[JsonProperty]
			public string Name { get; set; } = string.Empty;
			[JsonProperty]
			public string File { get; set; } = string.Empty;
			[JsonProperty]
			public bool IsCoreMod { get; set; } = false;
			[JsonProperty]
			public bool IsAddon { get; set; } = false;
			public HarmonyInstance Harmony { get; set; }
			public Assembly Assembly { get; set; }
			public Type[] AllTypes { get; set; }
			public List<IHarmonyModHooks> Hooks { get; } = new List<IHarmonyModHooks>();

			[JsonProperty]
			public List<RustPlugin> Plugins { get; set; } = new List<RustPlugin>();
		}
	}
}
