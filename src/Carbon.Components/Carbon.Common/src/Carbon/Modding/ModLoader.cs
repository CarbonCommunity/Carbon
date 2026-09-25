using Carbon.Events;
using Carbon.Profiler;
using Facepunch;
using Newtonsoft.Json;

namespace Carbon.Core;

/// <summary>
/// Plugin lifecycle: instantiates compiled plugin types, wires config/lang/hooks/commands,
/// tracks packages and inter-plugin requirements, and unloads plugins again.
/// </summary>
public static partial class ModLoader
{
	public static bool IsBatchComplete;
	public static PackageBank Packages = [];
	public static Dictionary<string, CompilationResult> FailedCompilations = new();

	internal static Dictionary<string, Type> TypeDictionaryCache { get; } = new();
	internal static Dictionary<string, List<string>> PendingRequirees { get; } = new();
	internal static List<string> PostBatchFailedRequirees { get; } = new();
	internal static bool FirstLoadSinceStartup { get; set; } = true;

	/// <summary>
	/// Classes plugins are allowed to directly inherit from. Packages providing
	/// alternative plugin bases register theirs here.
	/// </summary>
	public static HashSet<Type> PluginBaseTypes { get; } = [typeof(Plugin), typeof(CarbonPlugin)];

	/// <summary>Namespaces the compiler looks for plugin classes in.</summary>
	public static HashSet<string> PluginNamespaces { get; } = ["Carbon.Plugins"];

	public static bool IsPluginBaseType(Type type) => type != null && PluginBaseTypes.Contains(type);

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
			HookSubscriberIndex.Invalidate();
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
	public static Plugin FindPlugin(string name)
	{
		if (string.IsNullOrEmpty(name)) return null;
		for (var i = 0; i < Packages.Count; i++)
		{
			var plugin = Packages[i].FindPlugin(name);
			if (plugin != null) return plugin;
		}
		return null;
	}

	// Packages touch ModLoader before Community.Runtime exists, so only rely on the early-boot services here
	static ModLoader()
	{
		Services.Events.Subscribe(CarbonEvent.OnServerInitialized, _ => OnPluginProcessFinished());
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
		var plugins = Facepunch.Pool.Get<List<Plugin>>();
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

	/// <summary>Initializes the first plugin type found in <paramref name="assembly"/>.</summary>
	public static Plugin InitializePlugin(Assembly assembly, Package package = default, Action<Plugin> preInit = null, bool precompiled = false)
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
	/// <summary>
	/// Creates and loads a plugin: identity from [Info], constructor, config, default messages, Init/Loaded,
	/// commands, and OnServerInitialized when the server is already up. Replaces an already loaded plugin with the same name.
	/// </summary>
	/// <param name="preInit">Runs right before the constructor, used by the compiler to hand over hook/reference metadata.</param>
	public static bool InitializePlugin(Type type, out Plugin plugin, Package package = default, Action<Plugin> preInit = null, bool precompiled = false)
	{
		var constructor = type.GetConstructor(Type.EmptyTypes);
		var instance = FormatterServices.GetUninitializedObject(type);
		plugin = instance as Plugin;
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
	/// <summary>Unloads a plugin: patches, dependants, Unload hook, commands, libraries.</summary>
	/// <param name="premature">The plugin failed while loading, so skip the unload hooks and logging.</param>
	public static bool UninitializePlugin(Plugin plugin, bool premature = false, bool unloadDependantPlugins = true)
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

			Plugin.InternalApplyAllPluginReferences();
		}

		// plugin.IClearMemory();

		return true;
	}

	/// <summary>Indexes hooks, [HookMethod]s and [PluginReference]s of a plugin that didn't go through the compiler.</summary>
	public static void ProcessPrecompiledType(Plugin plugin)
	{
		try
		{
			var type = plugin.GetType();
			var hooks = plugin.Hooks ??= new();
			var hookMethods = plugin.HookMethods ??= new();
			var pluginReferences = plugin.PluginReferences ??= new();

			foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
			{
				if (Generator.InternalCallHook.HasRefLikeSignature(method))
				{
					continue;
				}

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

	/// <summary>Whether <paramref name="type"/> is one of the <see cref="PluginBaseTypes"/> (or derives from one when recursive).</summary>
	public static bool IsValidPlugin(Type type, bool recursive)
	{
		if (type == null)
		{
			return false;
		}

		if (IsPluginBaseType(type))
		{
			return true;
		}

		return recursive && IsValidPlugin(type.BaseType, recursive);
	}

	/// <summary>Access requirements shared by every command attribute on a member ([Permission], [Group], [AuthLevel], [Cooldown]).</summary>
	private struct CommandRequirements
	{
		public string[] Permissions;
		public string[] Groups;
		public int AuthLevel;
		public int Cooldown;
		public bool CooldownPenalty;

		public static CommandRequirements From(object[] attributes)
		{
			var result = new CommandRequirements { AuthLevel = -1 };
			List<string> permissions = null, groups = null;

			foreach (var attribute in attributes)
			{
				switch (attribute)
				{
					case PermissionAttribute permission: (permissions ??= new()).Add(permission.Name); break;
					case GroupAttribute group: (groups ??= new()).Add(group.Name); break;
					case AuthLevelAttribute authLevel: result.AuthLevel = authLevel.AuthLevel; break;
					case CooldownAttribute cooldown:
						result.Cooldown = cooldown.Miliseconds;
						result.CooldownPenalty = cooldown.DoCooldownPenalty;
						break;
				}
			}

			result.Permissions = permissions?.ToArray();
			result.Groups = groups?.ToArray();
			return result;
		}

		public void RegisterPermissions(BaseHookable hookable)
		{
			if (Permissions == null)
			{
				return;
			}

			var permission = Community.Runtime.Permission;

			foreach (var name in Permissions)
			{
				if (!permission.PermissionExists(name, hookable))
				{
					permission.RegisterPermission(name, hookable);
				}
			}
		}
	}

	/// <summary>
	/// Registers every command declared through attributes on <paramref name="type"/>:
	/// [Command], [ChatCommand], [ConsoleCommand], [ProtectedCommand], [RConCommand] methods and [CommandVar] fields/properties.
	/// </summary>
	public static void ProcessCommands(Type type, BaseHookable hookable = null, BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance, string prefix = null, bool hidden = false)
	{
		string Name(string name) => string.IsNullOrEmpty(prefix) ? name : $"{prefix}.{name}";
		var cmd = Community.Runtime.Core.cmd;

		foreach (var method in type.GetMethods(flags))
		{
			var attributes = method.GetCustomAttributes(false);
			if (attributes.Length == 0 || !attributes.Any(IsCommandAttribute))
			{
				continue;
			}

			var req = CommandRequirements.From(attributes);
			var parameterCount = method.GetParameters().Length;

			foreach (var attribute in attributes)
			{
				switch (attribute)
				{
					case CommandAttribute command:
						foreach (var commandName in command.Names)
						{
							cmd.AddChatCommand(Name(commandName), hookable, method, help: string.Empty, reference: method, permissions: req.Permissions, groups: req.Groups, authLevel: req.AuthLevel, cooldown: req.Cooldown, isHidden: hidden, silent: true, doCooldownPenalty: req.CooldownPenalty);
							cmd.AddConsoleCommand(Name(commandName), hookable, method, help: string.Empty, reference: method, permissions: req.Permissions, groups: req.Groups, authLevel: req.AuthLevel, cooldown: req.Cooldown, isHidden: hidden, silent: true, doCooldownPenalty: req.CooldownPenalty);
						}
						break;

					case ChatCommandAttribute chatCommand:
						cmd.AddChatCommand(Name(chatCommand.Name), hookable, method, help: chatCommand.Help, reference: method, permissions: req.Permissions, groups: req.Groups, authLevel: req.AuthLevel, cooldown: req.Cooldown, isHidden: hidden, silent: true, doCooldownPenalty: req.CooldownPenalty);
						break;

					case ConsoleCommandAttribute consoleCommand:
						cmd.AddConsoleCommand(Name(consoleCommand.Name), hookable, arg => InvokeConsoleCommand(hookable, method, parameterCount, arg, reportErrors: true),
							help: consoleCommand.Help, reference: method, permissions: req.Permissions, groups: req.Groups, authLevel: req.AuthLevel, cooldown: req.Cooldown, isHidden: hidden, silent: true, doCooldownPenalty: req.CooldownPenalty);
						break;

					case ProtectedCommandAttribute protectedCommand:
						// Protected commands get a per-boot randomized name, see Community.Protect
						cmd.AddConsoleCommand(Community.Protect(Name(protectedCommand.Name)), hookable, arg => InvokeConsoleCommand(hookable, method, parameterCount, arg, reportErrors: false),
							help: protectedCommand.Help, reference: method, permissions: req.Permissions, groups: req.Groups, authLevel: req.AuthLevel, cooldown: req.Cooldown, isHidden: true, silent: true, doCooldownPenalty: req.CooldownPenalty);
						break;

					case RConCommandAttribute rconCommand:
						Community.Runtime.CommandManager.RegisterCommand(new Carbon.Commands.Command.RCon
						{
							Name = Name(rconCommand.Name),
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
						}, out _);
						break;
				}
			}

			req.RegisterPermissions(hookable);
		}

		foreach (var field in type.GetFields(flags | BindingFlags.Public))
		{
			ProcessCommandVar(field, field.FieldType, () => field.GetValue(hookable), value => field.SetValue(hookable, value));
		}

		foreach (var property in type.GetProperties(flags | BindingFlags.Public))
		{
			ProcessCommandVar(property, property.PropertyType, () => property.GetValue(hookable), value => property.SetValue(hookable, value));
		}

		// [CommandVar] exposes a field/property as a console variable: no args reads it, one arg sets it
		void ProcessCommandVar(MemberInfo member, Type valueType, Func<object> get, Action<object> set)
		{
			var attributes = member.GetCustomAttributes(false);
			var cmdVar = attributes.OfType<CommandVarAttribute>().FirstOrDefault();

			if (cmdVar == null)
			{
				return;
			}

			var req = CommandRequirements.From(attributes);

			cmd.AddConsoleCommand(Name(cmdVar.Name), hookable, args =>
			{
				if (args != null && args.HasArgs(1))
				{
					try
					{
						var value = ParseCommandVar(valueType, args);
						if (value != null)
						{
							set(value);
						}
					}
					catch { }
				}

				var current = get();
				if (current != null && cmdVar.Protected) current = new string('*', current.ToString().Length);

				args.ReplyWith($"{args.cmd.FullName}: \"{current}\"");
				return true;
			}, help: cmdVar.Help, reference: member, permissions: req.Permissions, groups: req.Groups, authLevel: req.AuthLevel, cooldown: req.Cooldown, @protected: cmdVar.Protected, isHidden: hidden, silent: true, doCooldownPenalty: req.CooldownPenalty);

			req.RegisterPermissions(hookable);
		}
	}

	private static bool IsCommandAttribute(object attribute)
	{
		return attribute is ChatCommandAttribute or ConsoleCommandAttribute or RConCommandAttribute or ProtectedCommandAttribute or CommandAttribute;
	}

	private static object ParseCommandVar(Type type, ConsoleSystem.Arg args)
	{
		if (type == typeof(string)) return args.GetString(0);
		if (type == typeof(bool)) return args.GetBool(0);
		if (type == typeof(int)) return args.GetInt(0);
		if (type == typeof(uint)) return args.GetUInt(0);
		if (type == typeof(float)) return args.GetFloat(0);
		if (type == typeof(long)) return args.GetLong(0);
		if (type == typeof(ulong)) return args.GetULong(0);
		return null;
	}

	private static bool InvokeConsoleCommand(BaseHookable hookable, MethodInfo method, int parameterCount, ConsoleSystem.Arg arg, bool reportErrors)
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
		catch (Exception ex) when (reportErrors)
		{
			ex = ex.InnerException ?? ex;
			if (arg.IsRcon)
			{
				arg.ReplyWith($"Failed executing command ({ex.Message})\n{ex.StackTrace}");
			}
			else
			{
				Logger.Error("Failed executing command", ex);
			}
		}
		finally
		{
			HookCaller.Caller.ReturnBuffer(argBuffer);
		}

		return true;
	}
	public static void RemoveCommands(BaseHookable hookable)
	{
		if (hookable == null) return;

		Community.Runtime.CommandManager.ClearCommands(command => command.Reference == hookable);
	}

	/// <summary>
	/// Runs once a compile batch settles: retries plugins whose requirements showed up, re-applies plugin references
	/// and fires OnServerInitialized on plugins that missed it.
	/// </summary>
	public static void OnPluginProcessFinished()
	{
		var temp = Facepunch.Pool.Get<List<string>>();
		temp.AddRange(PostBatchFailedRequirees);

		foreach (var plugin in temp)
		{
			Community.Runtime.PluginSources.Retry(plugin);
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
		var plugins = Facepunch.Pool.Get<List<Plugin>>();
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
