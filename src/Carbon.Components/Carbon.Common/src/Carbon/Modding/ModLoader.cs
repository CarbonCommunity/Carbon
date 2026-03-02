using API.Events;
using Carbon.Profiler;
using Facepunch;
using Newtonsoft.Json;

namespace Carbon.Core;

public static partial class ModLoader
{
	public static bool IsBatchComplete;
	public static PackageBank Packages = [];
	public static Dictionary<string, CompilationResult> FailedCompilations = new();

	internal static Dictionary<string, Type> TypeDictionaryCache { get; } = new();
	internal static Dictionary<string, List<string>> PendingRequirees { get; } = new();
	internal static List<string> PostBatchFailedRequirees { get; } = new();
	internal static bool FirstLoadSinceStartup { get; set; } = true;

	internal const string CARBON_PLUGIN = "CarbonPlugin";
	internal const string RUST_PLUGIN = "RustPlugin";
	internal const string COVALENCE_PLUGIN = "CovalencePlugin";

	public static CompilationResult GetCompilationResult(string file, bool clear = false)
	{
		if (!FailedCompilations.TryGetValue(file, out var result))
		{
			FailedCompilations[file] = result = CompilationResult.Create(file);
		}

		if (clear)
		{
			result.Clear();
		}

		return result;
	}
	public static void RegisterPackage(Package package)
	{
		if (!Packages.Contains(package))
		{
			Packages.Add(package);
		}
	}
	public static Package GetPackage(string name)
	{
		foreach (var package in Packages)
		{
			if (package.Name.StartsWith(name, StringComparison.OrdinalIgnoreCase))
			{
				return package;
			}
		}
		return default;
	}
	public static RustPlugin FindPlugin(string name)
	{
		foreach (var package in Packages)
		{
			foreach (var plugin in package.Plugins)
			{
				if (plugin.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
				{
					return plugin;
				}
			}
		}
		return null;
	}

	static ModLoader()
	{
		Community.Runtime.Events.Subscribe(CarbonEvent.OnServerInitialized, _ => OnPluginProcessFinished());
	}

	public static List<string> GetRequirees(Plugin initial)
	{
		if (string.IsNullOrEmpty(initial.FilePath))
		{
			return null;
		}

		if (PendingRequirees.TryGetValue(initial.FilePath, out var requirees))
		{
			return requirees;
		}

		return null;
	}

	public static void AddPendingRequiree(string initial, string requiree)
	{
		if (!PendingRequirees.TryGetValue(initial, out var requirees))
		{
			PendingRequirees.Add(initial, requirees = Pool.Get<List<string>>());
		}

		if (!requirees.Contains(requiree))
		{
			requirees.Add(requiree);
		}
	}
	public static void AddPendingRequiree(Plugin initial, Plugin requiree)
	{
		AddPendingRequiree(initial.FilePath, requiree.FilePath);
	}
	public static void AddPostBatchFailedRequiree(string requiree)
	{
		if (PostBatchFailedRequirees.Contains(requiree))
		{
			return;
		}

		PostBatchFailedRequirees.Add(requiree);
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
		foreach (var requiree in PendingRequirees)
		{
			var self = requiree.Value;
			Pool.FreeUnmanaged(ref self);
		}
		PendingRequirees.Clear();
	}
	public static void ClearAllErrored()
	{
		foreach (var mod in FailedCompilations.Values)
		{
			mod.Clear();
		}
	}

	public static Type GetRegisteredType(string key)
	{
		if (TypeDictionaryCache.TryGetValue(key, out var type))
		{
			return type;
		}

		return null;
	}
	public static void RegisterType(string key, Type assembly)
	{
		TypeDictionaryCache[key] = assembly;
	}

	public static void UnloadCarbonMods(bool includeCore = false)
	{
		ClearAllRequirees();

		var list = Facepunch.Pool.Get<List<Package>>();
		list.AddRange(Packages);

		foreach (var mod in list)
		{
			if (!includeCore && mod.IsCoreMod) continue;

			UnloadCarbonMod(mod.Name);
		}

		Facepunch.Pool.FreeUnmanaged(ref list);
	}
	public static bool UnloadCarbonMod(string name)
	{
		var mod = GetPackage(name);

		if (!mod.IsValid)
		{
			return false;
		}

		UninitializePlugins(mod);
		return true;
	}

	public static void UninitializePlugins(Package mod)
	{
		var plugins = Facepunch.Pool.Get<List<RustPlugin>>();
		plugins.AddRange(mod.Plugins);

		foreach (var plugin in plugins)
		{
			try
			{
				UninitializePlugin(plugin);
			}
			catch (Exception ex) { Logger.Error($"Failed unloading '{mod.Name}'", ex); }
		}

		Facepunch.Pool.FreeUnmanaged(ref plugins);
	}

	public static RustPlugin InitializePlugin(Assembly assembly, Package package = default, Action<RustPlugin> preInit = null, bool precompiled = false)
	{
		foreach (var type in assembly.GetTypes())
		{
			if(type.BaseType == null)
			{
				continue;
			}

			if(!IsValidPlugin(type.BaseType, false))
			{
				continue;
			}

			if(InitializePlugin(type, out var plugin, package, preInit, precompiled))
			{
				return plugin;
			}
		}

		return null;
	}
	public static bool InitializePlugin(Type type, out RustPlugin plugin, Package package = default, Action<RustPlugin> preInit = null, bool precompiled = false)
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

		var title = info.Title;
		var author = info.Author;
		var version = info.Version;
		var description = desc == null ? string.Empty : desc.Description;

		var existentPlugin = FindPlugin(title) ?? FindPlugin(type.Name);

		if (existentPlugin != null)
		{
			UninitializePlugin(existentPlugin);
		}

		plugin.SetProcessor(Community.Runtime.ScriptProcessor, null);
		plugin.SetupMod(package, title, author, version, description);

		plugin.IsPrecompiled = precompiled;

		preInit?.Invoke(plugin);

		try
		{
			constructor?.Invoke(instance, null);
		}
		catch (Exception ex)
		{
			Analytics.plugin_constructor_failure(plugin);

			// OnConstructorFail
			HookCaller.CallStaticHook(2684549964, plugin, ex);

			var innerException = ex.InnerException;
			var compilationFailure = GetCompilationResult(plugin.FilePath);
			Trace trace = default;
			trace.Message = $"Constructor threw an exception ({innerException.Message})";
			trace.Number = ".ctor";
			compilationFailure.AppendError(trace);
			Logger.Error($"Failed executing constructor for {plugin.ToPrettyString()}. This is fatal!", ex);
			return false;
		}

		if (precompiled)
		{
			ProcessPrecompiledType(plugin);
		}

		if(precompiled || !IsValidPlugin(type.BaseType, false))
		{
			plugin.InternalCallHookOverriden = false;
		}

		package.AddPlugin(plugin);

		plugin.ILoadConfig();
		plugin.ILoadDefaultMessages();

		if (!plugin.IInit() || !plugin.ILoad())
		{
			if (UninitializePlugin(plugin, true))
			{
				package.RemovePlugin(plugin);
				return false;
			}
		}

		if (!plugin.ManualCommands)
		{
			ProcessCommands(type, plugin);
		}

		Interface.Oxide.RootPluginManager.AddPlugin(plugin);

		var isProfiled = MonoProfiler.IsRecording && Community.Runtime.MonoProfilerConfig.IsWhitelisted(MonoProfilerConfig.ProfileTypes.Plugin, Path.GetFileNameWithoutExtension(plugin.FileName));

		Logger.Log($"{(precompiled ? "Preloaded" : "Loaded")} plugin {plugin.ToPrettyString()}" +
		           $"{(precompiled ? string.Empty : $" [{plugin.CompileTime.TotalMilliseconds:0}ms]")}" +
		           $"{(isProfiled ? " [PROFILING]" : string.Empty)}");

		var eventArg = Pool.Get<CarbonEventArgs>();
		eventArg.Init(plugin);
		Community.Runtime.Events.Trigger(CarbonEvent.PluginLoaded, eventArg);
		Pool.Free(ref eventArg);

		if (Community.IsServerInitialized)
		{
			plugin.HasInitialized = true;
			plugin.CallHook("OnServerInitialized", FirstLoadSinceStartup);

			if (!plugin.ApplyOrderedPatches(AutoPatchAttribute.Orders.AfterOnServerInitialized))
			{
				return UninitializePlugin(plugin);
			}
		}

		return true;
	}
	public static bool UninitializePlugin(RustPlugin plugin, bool premature = false, bool unloadDependantPlugins = true)
	{
		if (!premature && !plugin.IsLoaded)
		{
			return true;
		}

		plugin.UnapplyOrderedPatches(AutoPatchAttribute.Orders.Delayed);
		plugin.UnapplyOrderedPatches(AutoPatchAttribute.Orders.AfterOnServerInitialized);
		plugin.UnapplyOrderedPatches(AutoPatchAttribute.Orders.AfterPluginLoad);
		plugin.UnapplyOrderedPatches(AutoPatchAttribute.Orders.AfterPluginInit);

		if (unloadDependantPlugins)
		{
			plugin.IUnloadDependantPlugins();
		}

		if (!premature)
		{
			plugin.CallHook("Unload");
		}

		var eventArg = Pool.Get<CarbonEventArgs>();
		eventArg.Init(plugin);
		Community.Runtime.Events.Trigger(CarbonEvent.PluginUnloaded, eventArg);
		Pool.Free(ref eventArg);

		RemoveCommands(plugin);
		plugin.IUnload();

		if (!premature)
		{
			// OnPluginUnloaded
			HookCaller.CallStaticHook(1250294368, plugin);
		}

		plugin.Dispose();

		if (!premature)
		{
			Logger.Log($"Unloaded plugin {plugin.ToPrettyString()}");
			Interface.Oxide.RootPluginManager.RemovePlugin(plugin);

			Plugin.InternalApplyAllPluginReferences();
		}

		// plugin.IClearMemory();

		return true;
	}

	public static void ProcessPrecompiledType(RustPlugin plugin)
	{
		try
		{
			var type = plugin.GetType();
			var hooks = plugin.Hooks ??= new();
			var hookMethods = plugin.HookMethods ??= new();
			var pluginReferences = plugin.PluginReferences ??= new();

			foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
			{
				var hash = HookStringPool.GetOrAdd(method.Name);

				if (Community.Runtime.HookManager.IsHook(method.Name))
				{
					if (!hooks.Contains(hash)) hooks.Add(hash);
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
		catch (Exception ex)
		{
			Logger.Error($"Failed ProcessPrecompiledType for plugin '{plugin.ToPrettyString()}'", ex);
		}
	}

	public static bool IsValidPlugin(Type type, bool recursive)
	{
		if (type == null)
		{
			return false;
		}

		if (type.Name is CARBON_PLUGIN or RUST_PLUGIN or COVALENCE_PLUGIN)
		{
			return true;
		}

		return recursive && IsValidPlugin(type.BaseType, recursive);
	}

	public static void ProcessCommands(Type type, BaseHookable hookable = null, BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance, string prefix = null, bool hidden = false)
	{
		var methods = type.GetMethods(flags);
		var fields = type.GetFields(flags | BindingFlags.Public);
		var properties = type.GetProperties(flags | BindingFlags.Public);

		var hasPrefix = !string.IsNullOrEmpty(prefix);

		foreach (var method in methods)
		{
			var allAttrs = method.GetCustomAttributes(false);
			if (allAttrs.Length == 0) continue;

			int permCount = 0, groupCount = 0;
			int authLevel = -1;
			int cooldownTime = 0;
			bool doCooldownPenalty = false;
			bool hasAnyCommand = false;

			foreach (var attr in allAttrs)
			{
				switch (attr)
				{
					case PermissionAttribute: permCount++; break;
					case GroupAttribute: groupCount++; break;
					case AuthLevelAttribute al: authLevel = al.AuthLevel; break;
					case CooldownAttribute cd:
						cooldownTime = cd.Miliseconds;
						doCooldownPenalty = cd.DoCooldownPenalty;
						break;
					case ChatCommandAttribute:
					case ConsoleCommandAttribute:
					case RConCommandAttribute:
					case ProtectedCommandAttribute:
					case CommandAttribute:
						hasAnyCommand = true;
						break;
				}
			}

			if (!hasAnyCommand) continue;

			string[] ps = null;
			if (permCount > 0)
			{
				ps = new string[permCount];
				int idx = 0;
				foreach (var attr in allAttrs)
					if (attr is PermissionAttribute p) ps[idx++] = p.Name;
			}

			string[] gs = null;
			if (groupCount > 0)
			{
				gs = new string[groupCount];
				int idx = 0;
				foreach (var attr in allAttrs)
					if (attr is GroupAttribute g) gs[idx++] = g.Name;
			}

			var parameterCount = method.GetParameters().Length;

			foreach (var attr in allAttrs)
			{
				switch (attr)
				{
					case CommandAttribute command:
						foreach (var commandName in command.Names)
						{
							var name = hasPrefix ? $"{prefix}.{commandName}" : commandName;
							Community.Runtime.Core.cmd.AddChatCommand(name, hookable, method, help: string.Empty, reference: method, permissions: ps, groups: gs, authLevel: authLevel, cooldown: cooldownTime, isHidden: hidden, silent: true, doCooldownPenalty: doCooldownPenalty);
							Community.Runtime.Core.cmd.AddConsoleCommand(name, hookable, method, help: string.Empty, reference: method, permissions: ps, groups: gs, authLevel: authLevel, cooldown: cooldownTime, isHidden: hidden, silent: true, doCooldownPenalty: doCooldownPenalty);
						}
						break;

					case ChatCommandAttribute chatCommand:
						Community.Runtime.Core.cmd.AddChatCommand(hasPrefix ? $"{prefix}.{chatCommand.Name}" : chatCommand.Name, hookable, method, help: chatCommand.Help, reference: method, permissions: ps, groups: gs, authLevel: authLevel, cooldown: cooldownTime, isHidden: hidden, silent: true, doCooldownPenalty: doCooldownPenalty);
						break;

					case ConsoleCommandAttribute consoleCommand:
						Community.Runtime.Core.cmd.AddConsoleCommand(hasPrefix ? $"{prefix}.{consoleCommand.Name}" : consoleCommand.Name, hookable,
							arg =>
							{
								var argBuffer = HookCaller.Caller.AllocateBuffer(parameterCount);
								if (argBuffer.Length >= 1)
								{
									argBuffer[0] = arg;
								}

								try
								{
									var result = method.Invoke(hookable, argBuffer);
									if (result != null && arg.Option.PrintOutput)
									{
										Logger.Log(result);
									}
								}
								catch (Exception ex)
								{
									ex = ex.InnerException;
									if (arg.IsRcon)
									{
										arg.ReplyWith($"Failed executing command ({ex.Message})\n{ex.StackTrace}");
									}
									else
									{
										Logger.Error($"Failed executing command", ex);
									}
								}
								finally
								{
									HookCaller.Caller.ReturnBuffer(argBuffer);
								}
								return true;
							}, help: consoleCommand.Help, reference: method, permissions: ps, groups: gs, authLevel: authLevel, cooldown: cooldownTime, isHidden: hidden, silent: true, doCooldownPenalty: doCooldownPenalty);
						break;

					case ProtectedCommandAttribute protectedCommand:
						Community.Runtime.Core.cmd.AddConsoleCommand(Community.Protect(hasPrefix ? $"{prefix}.{protectedCommand.Name}" : protectedCommand.Name), hookable,
							arg =>
							{
								var argBuffer = HookCaller.Caller.AllocateBuffer(parameterCount);
								if (argBuffer.Length >= 1)
								{
									argBuffer[0] = arg;
								}
								try
								{
									var result = method.Invoke(hookable, argBuffer);
									if (result != null && arg.Option.PrintOutput)
									{
										Logger.Log(result);
									}
								}
								finally
								{
									HookCaller.Caller.ReturnBuffer(argBuffer);
								}
								return true;
							}, help: protectedCommand.Help, reference: method, permissions: ps, groups: gs, authLevel: authLevel, cooldown: cooldownTime, isHidden: true, silent: true, doCooldownPenalty: doCooldownPenalty);
						break;

					case RConCommandAttribute rconCommand:
						var cmd = new API.Commands.Command.RCon
						{
							Name = hasPrefix ? $"{prefix}.{rconCommand.Name}" : rconCommand.Name,
							Reference = hookable,
							Callback = arg =>
							{
								var argBuffer = HookCaller.Caller.AllocateBuffer(parameterCount);
								if (argBuffer.Length >= 1)
								{
									argBuffer[0] = arg.Token ?? arg;
								}
								try
								{
									var result = method.Invoke(hookable, argBuffer);
									if (result != null && arg.PrintOutput)
									{
										Logger.Log(result);
									}
								}
								finally
								{
									HookCaller.Caller.ReturnBuffer(argBuffer);
								}
							},
							Help = rconCommand.Help,
							Token = rconCommand,
							CanExecute = (_, _) => true
						};

						Community.Runtime.CommandManager.RegisterCommand(cmd, out _);
						break;
				}
			}

			if (ps != null && ps.Length > 0)
			{
				var perm = Interface.Oxide.Permission;

				foreach (var permission in ps)
				{
					if (!perm.PermissionExists(permission, hookable))
					{
						perm.RegisterPermission(permission, hookable);
					}
				}
			}
		}

		foreach (var field in fields)
		{
			var allAttrs = field.GetCustomAttributes(false);
			if (allAttrs.Length == 0) continue;

			CommandVarAttribute cmdVar = null;
			int authLevel = -1;
			int cooldownTime = 0;
			bool doCooldownPenalty = false;
			int permCount = 0, groupCount = 0;

			foreach (var attr in allAttrs)
			{
				switch (attr)
				{
					case CommandVarAttribute cv: cmdVar = cv; break;
					case AuthLevelAttribute al: authLevel = al.AuthLevel; break;
					case CooldownAttribute cd:
						cooldownTime = cd.Miliseconds;
						doCooldownPenalty = cd.DoCooldownPenalty;
						break;
					case PermissionAttribute: permCount++; break;
					case GroupAttribute: groupCount++; break;
				}
			}

			if (cmdVar == null) continue;

			string[] ps = null;
			if (permCount > 0)
			{
				ps = new string[permCount];
				int idx = 0;
				foreach (var attr in allAttrs)
					if (attr is PermissionAttribute p) ps[idx++] = p.Name;
			}

			string[] gs = null;
			if (groupCount > 0)
			{
				gs = new string[groupCount];
				int idx = 0;
				foreach (var attr in allAttrs)
					if (attr is GroupAttribute g) gs[idx++] = g.Name;
			}

			{
				Community.Runtime.Core.cmd.AddConsoleCommand(hasPrefix ? $"{prefix}.{cmdVar.Name}" : cmdVar.Name, hookable, args =>
				{
					var value = field.GetValue(hookable);

					if (args != null && args.HasArgs(1))
					{
						try
						{
							if (field.FieldType == typeof(string))
							{
								value = args.GetString(0);
							}
							else if (field.FieldType == typeof(bool))
							{
								value = args.GetBool(0);
							}
							if (field.FieldType == typeof(int))
							{
								value = args.GetInt(0);
							}
							if (field.FieldType == typeof(uint))
							{
								value = args.GetUInt(0);
							}
							else if (field.FieldType == typeof(float))
							{
								value = args.GetFloat(0);
							}
							else if (field.FieldType == typeof(long))
							{
								value = args.GetLong(0);
							}
							else if (field.FieldType == typeof(ulong))
							{
								value = args.GetULong(0);
							}

							field.SetValue(hookable, value);
						}
						catch { }
					}

					value = field.GetValue(hookable);
					if (value != null && cmdVar.Protected) value = new string('*', value.ToString().Length);

					args.ReplyWith($"{args.cmd.FullName}: \"{value}\"");
					return true;
				}, help: cmdVar.Help, reference: field, permissions: ps, groups: gs, authLevel: authLevel, cooldown: cooldownTime, @protected: cmdVar.Protected, isHidden: hidden, silent: true, doCooldownPenalty: doCooldownPenalty);
			}
		}

		foreach (var property in properties)
		{
			var allAttrs = property.GetCustomAttributes(false);
			if (allAttrs.Length == 0) continue;

			CommandVarAttribute cmdVar = null;
			int authLevel = -1;
			int cooldownTime = 0;
			bool doCooldownPenalty = false;
			int permCount = 0, groupCount = 0;

			foreach (var attr in allAttrs)
			{
				switch (attr)
				{
					case CommandVarAttribute cv: cmdVar = cv; break;
					case AuthLevelAttribute al: authLevel = al.AuthLevel; break;
					case CooldownAttribute cd:
						cooldownTime = cd.Miliseconds;
						doCooldownPenalty = cd.DoCooldownPenalty;
						break;
					case PermissionAttribute: permCount++; break;
					case GroupAttribute: groupCount++; break;
				}
			}

			if (cmdVar == null) continue;

			string[] ps = null;
			if (permCount > 0)
			{
				ps = new string[permCount];
				int idx = 0;
				foreach (var attr in allAttrs)
					if (attr is PermissionAttribute p) ps[idx++] = p.Name;
			}

			string[] gs = null;
			if (groupCount > 0)
			{
				gs = new string[groupCount];
				int idx = 0;
				foreach (var attr in allAttrs)
					if (attr is GroupAttribute g) gs[idx++] = g.Name;
			}

			{
				Community.Runtime.Core.cmd.AddConsoleCommand(hasPrefix ? $"{prefix}.{cmdVar.Name}" : cmdVar.Name, hookable, args =>
				{
					var value = property.GetValue(hookable);

					if (args != null && args.HasArgs(1))
					{
						try
						{
							var pt = property.PropertyType;
							if (pt == typeof(string)) value = args.GetString(0);
							else if (pt == typeof(bool)) value = args.GetBool(0);
							else if (pt == typeof(int)) value = args.GetInt(0);
							else if (pt == typeof(uint)) value = args.GetUInt(0);
							else if (pt == typeof(float)) value = args.GetFloat(0);
							else if (pt == typeof(long)) value = args.GetLong(0);
							else if (pt == typeof(ulong)) value = args.GetULong(0);

							property.SetValue(hookable, value);
						}
						catch { }
					}

					value = property.GetValue(hookable);
					if (value != null && cmdVar.Protected) value = new string('*', value.ToString().Length);

					args.ReplyWith($"{args.cmd.FullName}: \"{value}\"");
					return true;
				}, help: cmdVar.Help, reference: property, permissions: ps, groups: gs, authLevel: authLevel, cooldown: cooldownTime, @protected: cmdVar.Protected, isHidden: hidden, silent: true, doCooldownPenalty: doCooldownPenalty);
			}
		}
	}
	public static void RemoveCommands(BaseHookable hookable)
	{
		if (hookable == null) return;

		Community.Runtime.CommandManager.ClearCommands(command => command.Reference == hookable);
	}

	public static void OnPluginProcessFinished()
	{
		var temp = Facepunch.Pool.Get<List<string>>();
		temp.AddRange(PostBatchFailedRequirees);

		foreach (var plugin in temp)
		{
			var file = Path.GetFileNameWithoutExtension(plugin);
			Community.Runtime.ScriptProcessor.ClearIgnore(file);
			Community.Runtime.ScriptProcessor.Prepare(file, plugin);
			Community.Runtime.ZipScriptProcessor.ClearIgnore(file);
			Community.Runtime.ZipScriptProcessor.Prepare(file, plugin);
#if DEBUG
			Community.Runtime.ZipDevScriptProcessor.ClearIgnore(file);
			Community.Runtime.ZipDevScriptProcessor.Prepare(file, plugin);
#endif
		}

		PostBatchFailedRequirees.Clear();

		if (temp.Count == 0)
		{
			IsBatchComplete = true;
		}

		temp.Clear();
		Facepunch.Pool.FreeUnmanaged(ref temp);

		Community.Runtime.Events.Trigger(CarbonEvent.AllPluginsLoaded, EventArgs.Empty);

		if (!Community.IsServerInitialized)
		{
			return;
		}

		var counter = 0;
		var plugins = Facepunch.Pool.Get<List<RustPlugin>>();
		plugins.AddRange(Packages.SelectMany(mod => mod.Plugins));

		foreach (var plugin in plugins)
		{
			try
			{
				plugin.InternalApplyPluginReferences();
			}
			catch(Exception exception)
			{
				Logger.Error($"Failed applying PluginReferences for '{plugin.ToPrettyString()}'", exception);
			}

			if (!plugin.HasInitialized)
			{
				counter++;

				plugin.HasInitialized = true;
				plugin.CallHook("OnServerInitialized", FirstLoadSinceStartup);

				if (!plugin.ApplyOrderedPatches(AutoPatchAttribute.Orders.AfterOnServerInitialized))
				{
					UninitializePlugin(plugin);
				}
			}
		}

		FirstLoadSinceStartup = false;

		Facepunch.Pool.FreeUnmanaged(ref plugins);

		if (counter > 1)
		{
			Analytics.batch_plugin_types();

			Logger.Log($" Batch completed! OSI on {counter:n0} {counter.Plural("plugin", "plugins")}.");
		}

		Community.Runtime.Events.Trigger(CarbonEvent.AllPluginsInitialized, EventArgs.Empty);
	}
}
