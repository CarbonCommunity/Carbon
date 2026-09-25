using Cysharp.Text;
using Facepunch;
using Newtonsoft.Json;

namespace Carbon.Plugins;

/// <summary>
/// Base of every plugin. Owns identity, lifecycle (init, load, unload), plugin references,
/// hook calls and the per-plugin libraries (permissions, commands, lang, timers, web requests, config).
/// Derive from <see cref="CarbonPlugin"/> for the Rust-specific helpers.
/// </summary>
[JsonObject(MemberSerialization.OptIn)]
public class Plugin : BaseHookable, IDisposable
{
	/// <summary>Hosts plugin coroutines and timers. Destroyed with the plugin.</summary>
	public class Persistence : FacepunchBehaviour;

	public Persistence persistence;

	public Permission permission;
	public CommandLibrary cmd;
	public Lang lang;
	public PluginTimers timer;
	public WebRequests webrequest;

	public bool IsCorePlugin { get; set; }
	public bool IsPrecompiled { get; set; }
	public bool IsExtension { get; set; }

	[JsonProperty]
	public string Title { get; set; }
	[JsonProperty]
	public string Description { get; set; }
	[JsonProperty]
	public string Author { get; set; }
	public int ResourceId { get; set; }
	public bool HasConfig { get; set; }
	public bool HasMessages { get; set; }
	public bool HasConditionals { get; set; }

	[JsonProperty]
	public TimeSpan CompileTime { get; set; }
	[JsonProperty]
	public TimeSpan InternalCallHookGenTime { get; set; }
	[JsonProperty]
	public ModLoader.Trace[] CompileWarnings { get; set; }

	public string InternalCallHookSource { get; set; }

	[JsonProperty]
	public string FilePath { get; set; }
	[JsonProperty]
	public string FileName { get; set; }

	public string Filename => FileName;

	/// <summary>Opt out of automatic [ChatCommand]/[ConsoleCommand] registration.</summary>
	public virtual bool ManualCommands => false;

	public Plugin[] Requires { get; set; }

	public ModLoader.Package Package;

	/// <summary>The plugin source entry this plugin was compiled from. Null for precompiled plugins.</summary>
	public PluginSource.Entry Source;

	public bool IsLoaded { get; set; }

	public DynamicConfigFile Config { get; internal set; }

	/// <summary>Folder where <see cref="Config"/> lives.</summary>
	public virtual string ConfigFolder => Defines.GetConfigsFolder();

	public static implicit operator bool(Plugin target)
	{
		return target != null;
	}

	public override void TrackStart()
	{
		if (IsCorePlugin) return;

		base.TrackStart();
	}
	public override void TrackEnd()
	{
		if (IsCorePlugin) return;

		base.TrackEnd();
	}

	#region Setup

	public virtual void SetupMod(ModLoader.Package mod, string name, string author, VersionNumber version, string description)
	{
		Package = mod;
		Setup(name, author, version, description);
	}

	/// <summary>Assigns identity and creates the per-plugin libraries. Runs before the constructor body.</summary>
	public virtual void Setup(string name, string author, VersionNumber version, string description)
	{
		Name = GetType().Name;
		Title = name.Replace(":", string.Empty);
		Version = version;
		Author = author;
		Description = description;
		HookableType = GetType();

		permission = Community.Runtime.Permission;
		cmd = CreateCommandLibrary();
		lang = CreateLang();
		timer = CreateTimers();
		webrequest = CreateWebRequests();

		persistence = new GameObject($"Script_{name}").AddComponent<Persistence>();
		UnityEngine.Object.DontDestroyOnLoad(persistence.gameObject);
	}

	// Library factories. Override to plug in custom implementations.
	protected virtual CommandLibrary CreateCommandLibrary() => new();
	protected virtual Lang CreateLang() => new(this);
	protected virtual PluginTimers CreateTimers() => new(this);
	protected virtual WebRequests CreateWebRequests() => new();

	#endregion

	#region Lifecycle

	public virtual bool IInit()
	{
		BuildHookCache(BindingFlags.NonPublic | BindingFlags.Instance);

		using (TimeMeasure.New($"Processing PluginReferences on '{this}'"))
		{
			if (!InternalApplyPluginReferences())
			{
				Logger.Warn($"Failed vibe check {ToPrettyString()}");
				return false;
			}
		}

		if (Hooks != null && !ManualSubscriptions)
		{
			string requester = FileName ?? $"{this}";
			using (TimeMeasure.New($"Processing Hooks on '{ToPrettyString()}'"))
			{
				foreach (var hook in Hooks)
				{
					Community.Runtime.HookManager.Subscribe(HookStringPool.GetOrAdd(hook), requester);
				}
			}
		}

		CallHook("Init");

		TrackInit();

		return ApplyOrderedPatches(AutoPatchAttribute.Orders.AfterPluginInit);
	}
	internal virtual bool ILoad()
	{
		using (TimeMeasure.New($"Load on '{ToPrettyString()}'"))
		{
			IsLoaded = true;
			CallHook("OnLoaded");
			CallHook("Loaded");
		}

		// Plugins that were waiting on us can compile now
		using (TimeMeasure.New($"Load.PendingRequirees on '{ToPrettyString()}'"))
		{
			var requirees = ModLoader.GetRequirees(this);

			if (requirees != null)
			{
				foreach (var requiree in requirees)
				{
					Logger.Warn($" [{Name}] Loading '{Path.GetFileNameWithoutExtension(requiree)}' to parent's request: '{ToPrettyString()}'");
					Community.Runtime.PluginSources.Prepare(requiree);
				}

				ModLoader.ClearPendingRequirees(this);
			}
		}

		Load();

		return ApplyOrderedPatches(AutoPatchAttribute.Orders.AfterPluginLoad);
	}
	public virtual void Load()
	{
	}
	public virtual void IUnload()
	{
		try
		{
			using (TimeMeasure.New($"IUnload.UnprocessHooks on '{this}'"))
			{
				if (Hooks != null)
				{
					foreach (var hook in Hooks)
					{
						Community.Runtime.HookManager.Unsubscribe(HookStringPool.GetOrAdd(hook), FileName);
					}
				}

				foreach (var method in HookableType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic))
				{
					InternalHooks.Handle(method.Name, false);
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Error($"Failed calling Plugin.IUnload.UnprocessHooks on {this}", ex);
		}

		HasInitialized = false;
	}

	public virtual void Dispose()
	{
		permission?.UnregisterPermissions(this);

		timer?.Clear();
		timer = null;

		_createdLogFolders = null;
		_cachedLogFolder = null;
		_cachedDateStr = null;

		if (persistence != null)
		{
			var go = persistence.gameObject;
			UnityEngine.Object.DestroyImmediate(persistence);
			UnityEngine.Object.Destroy(go);
		}

		try
		{
			using (TimeMeasure.New($"IUnload.Disposal on '{this}'"))
			{
				IgnoredHooks?.Clear();
				HookPool?.Clear();
				Hooks?.Clear();
				HookMethods?.Clear();
				PluginReferences?.Clear();

				IgnoredHooks = null;
				HookPool = null;
				Hooks = null;
				HookMethods = null;
				PluginReferences = null;
				HookSubscriberIndex.Invalidate();
			}
		}
		catch (Exception ex)
		{
			Logger.Error($"Failed calling Plugin.IUnload.Disposal on {this}", ex);
		}

		IsLoaded = false;
	}

	#endregion

	#region Plugin references

	internal bool InternalApplyPluginReferences()
	{
		if (PluginReferences == null)
		{
			return true;
		}

		foreach (var attribute in PluginReferences)
		{
			var field = attribute.Field;
			var name = string.IsNullOrEmpty(attribute.Name) ? field.Name : attribute.Name;
			var path = Path.Combine(Defines.GetScriptsFolder(), $"{name}.cs");

			try
			{
				// Typed references ([PluginReference] MyPlugin x) resolve through the referenced type's [Info]
				var plugin = (Plugin)null;
				if (!ModLoader.IsPluginBaseType(field.FieldType))
				{
					var info = field.FieldType.GetCustomAttribute<InfoAttribute>();
					if (info == null)
					{
						Logger.Warn($"You're trying to reference a non-plugin instance: {name}[{field.FieldType.Name}]");
						continue;
					}

					plugin = ModLoader.FindPlugin(info.Title);
				}
				else
				{
					plugin = ModLoader.FindPlugin(name);
				}

				if (plugin != null)
				{
					var version = new VersionNumber(attribute.MinVersion);

					if (version.IsValid() && plugin.Version < version)
					{
						Logger.Warn($"Plugin '{Name} by {Author} v{Version}' references a required plugin which is outdated: {plugin.Name} by {plugin.Author} v{plugin.Version} < v{version}");
						return false;
					}

					field.SetValue(this, plugin);

					if (attribute.IsRequired)
					{
						ModLoader.AddPendingRequiree(plugin, this);
					}
				}
				else
				{
					field.SetValue(this, null);

					if (attribute.IsRequired)
					{
						ModLoader.PostBatchFailedRequirees.Add(FilePath);
						ModLoader.AddPendingRequiree(path, FilePath);
						Logger.Warn($"Plugin '{Name} by {Author} v{Version}' references a required plugin which is not loaded: {name}");
						return false;
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error($"Plugin '{ToPrettyString()}' failed to assign PluginReference on field {name} ({field.FieldType.Name})", ex);
			}
		}

		return true;
	}
	internal void IUnloadDependantPlugins()
	{
		try
		{
			using (TimeMeasure.New($"IUnload.UnloadRequirees on '{ToPrettyString()}'"))
			{
				var plugins = Pool.Get<List<Plugin>>();

				foreach (var mod in ModLoader.Packages)
				{
					plugins.Clear();
					plugins.AddRange(mod.Plugins);

					foreach (var plugin in plugins.Where(plugin => plugin.Requires != null && plugin.Requires.Contains(this)))
					{
						Logger.Warn($" [{Name}] Unloading '{plugin.ToPrettyString()}' because parent '{ToPrettyString()}' has been unloaded.");
						ModLoader.AddPendingRequiree(this, plugin);

						plugin.Source?.Loader?.Dispose();
						ModLoader.UninitializePlugin(plugin);
					}
				}

				Pool.FreeUnmanaged(ref plugins);
			}
		}
		catch (Exception ex)
		{
			Logger.Error($"Failed calling Plugin.IUnload.UnloadRequirees on {ToPrettyString()}", ex);
		}
	}

	/// <summary>Re-resolves references on every plugin and unloads the ones missing a required dependency.</summary>
	public static void InternalApplyAllPluginReferences()
	{
		var list = Pool.Get<List<Plugin>>();

		foreach (var package in ModLoader.Packages)
		{
			foreach (var plugin in package.Plugins)
			{
				if (!plugin.InternalApplyPluginReferences())
				{
					list.Add(plugin);
				}
			}
		}

		foreach (var plugin in list)
		{
			ModLoader.UninitializePlugin(plugin);
		}

		Pool.FreeUnmanaged(ref list);
	}

	/// <summary>First loaded plugin of type <typeparamref name="T"/>.</summary>
	public static T Singleton<T>()
	{
		foreach (var mod in ModLoader.Packages)
		{
			foreach (var plugin in mod.Plugins)
			{
				if (plugin is T result)
				{
					return result;
				}
			}
		}

		return default;
	}

	#endregion

	#region Calls

	public T Call<T>(string hook)
	{
		return HookCaller.CallHook<T>(this, HookStringPool.GetOrAdd(hook));
	}
	public T Call<T>(string hook, object arg1)
	{
		return HookCaller.CallHook<T>(this, HookStringPool.GetOrAdd(hook), arg1);
	}
	public T Call<T>(string hook, object arg1, object arg2)
	{
		return HookCaller.CallHook<T>(this, HookStringPool.GetOrAdd(hook), arg1, arg2);
	}
	public T Call<T>(string hook, object arg1, object arg2, object arg3)
	{
		return HookCaller.CallHook<T>(this, HookStringPool.GetOrAdd(hook), arg1, arg2, arg3);
	}
	public T Call<T>(string hook, object arg1, object arg2, object arg3, object arg4)
	{
		return HookCaller.CallHook<T>(this, HookStringPool.GetOrAdd(hook), arg1, arg2, arg3, arg4);
	}
	public T Call<T>(string hook, object arg1, object arg2, object arg3, object arg4, object arg5)
	{
		return HookCaller.CallHook<T>(this, HookStringPool.GetOrAdd(hook), arg1, arg2, arg3, arg4, arg5);
	}
	public T Call<T>(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
	{
		return HookCaller.CallHook<T>(this, HookStringPool.GetOrAdd(hook), arg1, arg2, arg3, arg4, arg5, arg6);
	}
	public T Call<T>(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
	{
		return HookCaller.CallHook<T>(this, HookStringPool.GetOrAdd(hook), arg1, arg2, arg3, arg4, arg5, arg6, arg7);
	}
	public T Call<T>(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
	{
		return HookCaller.CallHook<T>(this, HookStringPool.GetOrAdd(hook), arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
	}
	public T Call<T>(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
	{
		return HookCaller.CallHook<T>(this, HookStringPool.GetOrAdd(hook), arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
	}
	public T Call<T>(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
	{
		return HookCaller.CallHook<T>(this, HookStringPool.GetOrAdd(hook), arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
	}
	public T Call<T>(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
	{
		return HookCaller.CallHook<T>(this, HookStringPool.GetOrAdd(hook), arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
	}
	public T Call<T>(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
	{
		return HookCaller.CallHook<T>(this, HookStringPool.GetOrAdd(hook), arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
	}
	public T Call<T>(string hook, object[] args)
	{
		return HookCaller.CallHook<T>(this, HookStringPool.GetOrAdd(hook), args: args);
	}

	public object Call(string hook)
	{
		return HookCaller.CallHook(this, HookStringPool.GetOrAdd(hook));
	}
	public object Call(string hook, object arg1)
	{
		return HookCaller.CallHook(this, HookStringPool.GetOrAdd(hook), arg1);
	}
	public object Call(string hook, object arg1, object arg2)
	{
		return HookCaller.CallHook(this, HookStringPool.GetOrAdd(hook), arg1, arg2);
	}
	public object Call(string hook, object arg1, object arg2, object arg3)
	{
		return HookCaller.CallHook(this, HookStringPool.GetOrAdd(hook), arg1, arg2, arg3);
	}
	public object Call(string hook, object arg1, object arg2, object arg3, object arg4)
	{
		return HookCaller.CallHook(this, HookStringPool.GetOrAdd(hook), arg1, arg2, arg3, arg4);
	}
	public object Call(string hook, object arg1, object arg2, object arg3, object arg4, object arg5)
	{
		return HookCaller.CallHook(this, HookStringPool.GetOrAdd(hook), arg1, arg2, arg3, arg4, arg5);
	}
	public object Call(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
	{
		return HookCaller.CallHook(this, HookStringPool.GetOrAdd(hook), arg1, arg2, arg3, arg4, arg5, arg6);
	}
	public object Call(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
	{
		return HookCaller.CallHook(this, HookStringPool.GetOrAdd(hook), arg1, arg2, arg3, arg4, arg5, arg6, arg7);
	}
	public object Call(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
	{
		return HookCaller.CallHook(this, HookStringPool.GetOrAdd(hook), arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
	}
	public object Call(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
	{
		return HookCaller.CallHook(this, HookStringPool.GetOrAdd(hook), arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
	}
	public object Call(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
	{
		return HookCaller.CallHook(this, HookStringPool.GetOrAdd(hook), arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
	}
	public object Call(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
	{
		return HookCaller.CallHook(this, HookStringPool.GetOrAdd(hook), arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
	}
	public object Call(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
	{
		return HookCaller.CallHook(this, HookStringPool.GetOrAdd(hook), arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
	}
	public object Call(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13)
	{
		return HookCaller.CallHook(this, HookStringPool.GetOrAdd(hook), arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
	}
	public object Call(string hook, object[] args)
	{
		return HookCaller.CallHook(this, HookStringPool.GetOrAdd(hook), args: args);
	}

	public T CallHook<T>(string hook)
	{
		return HookCaller.CallHook<T>(this, HookStringPool.GetOrAdd(hook));
	}
	public T CallHook<T>(string hook, object arg1)
	{
		return HookCaller.CallHook<T>(this, HookStringPool.GetOrAdd(hook), arg1);
	}
	public T CallHook<T>(string hook, object arg1, object arg2)
	{
		return HookCaller.CallHook<T>(this, HookStringPool.GetOrAdd(hook), arg1, arg2);
	}
	public T CallHook<T>(string hook, object arg1, object arg2, object arg3)
	{
		return HookCaller.CallHook<T>(this, HookStringPool.GetOrAdd(hook), arg1, arg2, arg3);
	}
	public T CallHook<T>(string hook, object arg1, object arg2, object arg3, object arg4)
	{
		return HookCaller.CallHook<T>(this, HookStringPool.GetOrAdd(hook), arg1, arg2, arg3, arg4);
	}
	public T CallHook<T>(string hook, object arg1, object arg2, object arg3, object arg4, object arg5)
	{
		return HookCaller.CallHook<T>(this, HookStringPool.GetOrAdd(hook), arg1, arg2, arg3, arg4, arg5);
	}
	public T CallHook<T>(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
	{
		return HookCaller.CallHook<T>(this, HookStringPool.GetOrAdd(hook), arg1, arg2, arg3, arg4, arg5, arg6);
	}
	public T CallHook<T>(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
	{
		return HookCaller.CallHook<T>(this, HookStringPool.GetOrAdd(hook), arg1, arg2, arg3, arg4, arg5, arg6, arg7);
	}
	public T CallHook<T>(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
	{
		return HookCaller.CallHook<T>(this, HookStringPool.GetOrAdd(hook), arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
	}
	public T CallHook<T>(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
	{
		return HookCaller.CallHook<T>(this, HookStringPool.GetOrAdd(hook), arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
	}
	public T CallHook<T>(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
	{
		return HookCaller.CallHook<T>(this, HookStringPool.GetOrAdd(hook), arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
	}
	public T CallHook<T>(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
	{
		return HookCaller.CallHook<T>(this, HookStringPool.GetOrAdd(hook), arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
	}
	public T CallHook<T>(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
	{
		return HookCaller.CallHook<T>(this, HookStringPool.GetOrAdd(hook), arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
	}
	public T CallHook<T>(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13)
	{
		return HookCaller.CallHook<T>(this, HookStringPool.GetOrAdd(hook), arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
	}
	public T CallHook<T>(string hook, object[] args)
	{
		return HookCaller.CallHook<T>(this, HookStringPool.GetOrAdd(hook), args: args);
	}

	public object CallHook(string hook)
	{
		return HookCaller.CallHook(this, HookStringPool.GetOrAdd(hook));
	}
	public object CallHook(string hook, object arg1)
	{
		return HookCaller.CallHook(this, HookStringPool.GetOrAdd(hook), arg1);
	}
	public object CallHook(string hook, object arg1, object arg2)
	{
		return HookCaller.CallHook(this, HookStringPool.GetOrAdd(hook), arg1, arg2);
	}
	public object CallHook(string hook, object arg1, object arg2, object arg3)
	{
		return HookCaller.CallHook(this, HookStringPool.GetOrAdd(hook), arg1, arg2, arg3);
	}
	public object CallHook(string hook, object arg1, object arg2, object arg3, object arg4)
	{
		return HookCaller.CallHook(this, HookStringPool.GetOrAdd(hook), arg1, arg2, arg3, arg4);
	}
	public object CallHook(string hook, object arg1, object arg2, object arg3, object arg4, object arg5)
	{
		return HookCaller.CallHook(this, HookStringPool.GetOrAdd(hook), arg1, arg2, arg3, arg4, arg5);
	}
	public object CallHook(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
	{
		return HookCaller.CallHook(this, HookStringPool.GetOrAdd(hook), arg1, arg2, arg3, arg4, arg5, arg6);
	}
	public object CallHook(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
	{
		return HookCaller.CallHook(this, HookStringPool.GetOrAdd(hook), arg1, arg2, arg3, arg4, arg5, arg6, arg7);
	}
	public object CallHook(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
	{
		return HookCaller.CallHook(this, HookStringPool.GetOrAdd(hook), arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
	}
	public object CallHook(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
	{
		return HookCaller.CallHook(this, HookStringPool.GetOrAdd(hook), arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
	}
	public object CallHook(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
	{
		return HookCaller.CallHook(this, HookStringPool.GetOrAdd(hook), arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
	}
	public object CallHook(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
	{
		return HookCaller.CallHook(this, HookStringPool.GetOrAdd(hook), arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
	}
	public object CallHook(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
	{
		return HookCaller.CallHook(this, HookStringPool.GetOrAdd(hook), arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
	}
	public object CallHook(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13)
	{
		return HookCaller.CallHook(this, HookStringPool.GetOrAdd(hook), arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
	}
	public object CallHook(string hook, object[] args)
	{
		return HookCaller.CallHook(this, HookStringPool.GetOrAdd(hook), args: args);
	}

	#endregion

	#region Threading

	/// <summary>Runs <paramref name="callback"/> on the main thread next frame.</summary>
	public void NextTick(Action callback) => Community.Runtime.Scheduler.NextFrame(callback);
	public void NextFrame(Action callback) => Community.Runtime.Scheduler.NextFrame(callback);

	public void QueueWorkerThread(Action<object> callback)
	{
		ThreadPool.QueueUserWorkItem(context =>
		{
			try
			{
				callback(context);
			}
			catch (Exception ex)
			{
				Logger.Error($"Worker thread callback failed in '{Name} v{Version}'", ex);
			}
		});
	}

	#endregion

	#region Config

	public void ILoadConfig()
	{
		try
		{
			LoadConfig();
		}
		catch (Exception ex)
		{
			LogError("Failed ILoadConfig", ex);
		}
	}

	protected virtual void LoadConfig()
	{
		Config = new DynamicConfigFile(Path.Combine(ConfigFolder, Name + ".json"));

		if (!Config.Exists(null))
		{
			CallHook(nameof(LoadDefaultConfig));
			SaveConfig();
		}
		try
		{
			if (Config.Exists(null)) Config.Load(null);
		}
		catch (Exception ex)
		{
			Logger.Error("Failed to load config file (is the config file corrupt?)", ex);
		}
	}
	protected virtual void LoadDefaultConfig()
	{
	}
	protected virtual void SaveConfig()
	{
		if (Config == null)
		{
			return;
		}
		try
		{
			if (Config.Count() > 0) Config.Save(null);
		}
		catch (Exception ex)
		{
			Logger.Error("Failed to save config file (does the config have illegal objects in it?) (" + ex.Message + ")", ex);
		}
	}

	private bool _loadedDefaultMessages;

	public void ILoadDefaultMessages()
	{
		if (_loadedDefaultMessages)
		{
			return;
		}

		CallHook("LoadDefaultMessages");

		_loadedDefaultMessages = true;
	}

	protected virtual void LoadDefaultMessages()
	{
	}

	#endregion

	#region Logging

	private string _cachedLogFolder;
	private string _cachedDateStr;
	private int _cachedDay;
	private HashSet<string> _createdLogFolders;

	private string Format(object message, object[] args)
		=> args == null || args.Length == 0 ? message?.ToString() : string.Format(message?.ToString() ?? string.Empty, args);

	/// <summary>Prints a notice to the console, prefixed with the plugin title.</summary>
	public void Puts(object message) => Logger.Log($"[{Title}] {message}");
	public void Puts(object message, params object[] args) => Logger.Log($"[{Title}] {Format(message, args)}");
	public void Puts(string message, params object[] args) => Puts((object)message, args: args);

	public void Log(object message) => Logger.Log($"[{Title}] {message}");
	public void Log(object message, params object[] args) => Logger.Log($"[{Title}] {Format(message, args)}");

	public void LogWarning(object message) => Logger.Warn($"[{Title}] {message}");
	public void LogWarning(object message, params object[] args) => Logger.Warn($"[{Title}] {Format(message, args)}");

	public void LogError(object message) => Logger.Error($"[{Title}] {message}");
	public void LogError(object message, params object[] args) => Logger.Error($"[{Title}] {Format(message, args)}");
	public void LogError(object message, Exception ex) => Logger.Error($"[{Title}] {message}", ex);
	public void LogError(object message, Exception ex, params object[] args) => Logger.Error($"[{Title}] {Format(message, args)}", ex);

	public void PrintWarning(object format, params object[] args) => Logger.Warn($"[{Title}] {Format(format, args)}");
	public void PrintError(object format, params object[] args) => Logger.Error($"[{Title}] {Format(format, args)}");
	public void RaiseError(object message) => Logger.Error($"[{Title}] {message}", null);

	protected void PrintWarning(object message) => LogWarning(message);
	protected void PrintWarning(string format, params object[] args) => LogWarning(format, args);

	/// <summary>Appends a line to carbon/logs/[filename]-[date].txt (or per plugin when <paramref name="plugin"/> is set).</summary>
	protected void LogToFile(string filename, string text, Plugin plugin = null, bool timeStamp = true, bool anotherBool = false)
	{
		if (string.IsNullOrEmpty(filename) || string.IsNullOrEmpty(text))
			return;

		var now = DateTime.Now;

		if (_cachedDay != now.Day)
		{
			_cachedDay = now.Day;
			_cachedDateStr = now.ToString("yyyy-MM-dd");
		}

		string logFolder, finalFileName;

		if (plugin == null)
		{
			var subFolder = Path.GetDirectoryName(filename);
			var fileOnly = Path.GetFileNameWithoutExtension(filename);

			logFolder = string.IsNullOrEmpty(subFolder)
				? Defines.GetLogsFolder()
				: Path.Combine(Defines.GetLogsFolder(), subFolder);

			finalFileName = timeStamp
				? ZString.Concat(fileOnly, "-", _cachedDateStr, ".txt")
				: ZString.Concat(fileOnly, ".txt");
		}
		else
		{
			logFolder = _cachedLogFolder ??= Path.Combine(Defines.GetLogsFolder(), plugin.Name);

			finalFileName = timeStamp
				? ZString.Concat(plugin.Name, "_", filename, "-", _cachedDateStr, ".txt").ToLower()
				: ZString.Concat(plugin.Name, "_", filename, ".txt").ToLower();
		}

		_createdLogFolders ??= new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		if (_createdLogFolders.Add(logFolder))
		{
			OsEx.Folder.Create(logFolder);
		}

		var fullPath = Path.Combine(logFolder, PathEx.CleanPath(finalFileName));

		var logEntry = timeStamp
			? ZString.Concat("[", _cachedDateStr, " ", now.ToString("HH:mm:ss"), "] ", text, Environment.NewLine)
			: ZString.Concat(text, Environment.NewLine);

		OsEx.File.Append(fullPath, logEntry);
	}

	#endregion

	public override string ToPrettyString()
	{
		return $"{Title} v{Version} by {Author}";
	}
}
