using Carbon.Base.Interfaces;
using Defines = Carbon.Core.Defines;
using Harmony = HarmonyLib.Harmony;

namespace Carbon.Base;

public abstract class BaseModule : BaseHookable
{
	public string Context { get; set; }

	public virtual bool EnabledByDefault => false;
	public virtual bool ForceModded => false;
	public virtual bool ForceEnabled => false;
	public virtual bool ForceDisabled => false;

	public virtual bool ManualCommands => false;
	public virtual bool ConfigVersionChecks => true;

	public abstract void OnServerInit(bool initial);
	public abstract void OnPostServerInit(bool initial);
	public abstract void OnServerSaved();
	public abstract void Load();
	public abstract void Save();
	public abstract void OnUnload();
	public abstract void Reload();
	public abstract bool IsEnabled();
	public abstract void SetEnabled(bool enable);
	public abstract void Shutdown();

	public static T GetModule<T>()
	{
		foreach (var module in Community.Runtime.ModuleProcessor.Modules)
		{
			if (module.GetType() == typeof(T) && module is T result) return result;
		}

		return default;
	}
	public static BaseModule FindModule(string name)
	{
		return Community.Runtime.ModuleProcessor.Modules.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase) || x.Name.Contains(name, CompareOptions.OrdinalIgnoreCase)) as BaseModule;
	}
}

public class EmptyModuleConfig;
public class EmptyModuleData;

public abstract class CarbonModule<C, D> : BaseModule, IModule
{
	public Configuration ModuleConfiguration { get; set; }
	public DynamicConfigFile Config { get; private set; }
	public DynamicConfigFile Data { get; private set; }
	public Lang Lang { get; private set; }

	public virtual Type Type => default;

	public D DataInstance { get; private set; }
	public C ConfigInstance { get; private set; }

	public new virtual string Name => "Not set";
	public Permission Permissions;

	protected void Puts(object message)
		=> Logger.Log($"[{Name}] {message}");
	protected void PutsError(object message, Exception ex = null)
		=> Logger.Error($"[{Name}] {message}", ex);
	protected void PutsWarn(object message)
		=> Logger.Warn($"[{Name}] {message}");

	public virtual void Dispose()
	{
		Config = null;
		ModuleConfiguration = null;
	}

	public virtual void Init()
	{
		base.Hooks ??= new();
		base.Name ??= Name;
		base.HookableType ??= Type;

		if (ForceDisabled)
		{
			return;
		}

		Permissions = Interface.Oxide.Permission;

		TrackInit();
	}
	public virtual bool InitEnd()
	{
		if (ForceDisabled || HasInitialized)
		{
			return false;
		}

		Community.Runtime.HookManager.LoadHooksFromType(Type);

		BuildHookCache(BindingFlags.Instance | BindingFlags.NonPublic);

		foreach (var method in Type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
		{
			if (Community.Runtime.HookManager.IsHook(method.Name))
			{
				Community.Runtime.HookManager.Subscribe(method.Name, Name);

				var hash = HookStringPool.GetOrAdd(method.Name);

				if (!Hooks.Contains(hash))
				{
					Hooks.Add(hash);
				}
			}
		}

		var phrases = GetDefaultPhrases();

		if (phrases != null)
		{
			foreach (var language in phrases)
			{
				Lang.RegisterMessages(language.Value, this, language.Key);
			}
		}

		Puts("Initialized.");
		HasInitialized = true;

		return true;
	}

	public override void Load()
	{
		if (ForceDisabled) return;

		var shouldSave = false;

		Config ??= new DynamicConfigFile(GetConfigPath());
		Data ??= new DynamicConfigFile(GetDataPath());
		Lang ??= new Lang(this);

		var newConfig = !Config.Exists();
		var newData = !Data.Exists();

		if (!Config.Exists())
		{
			ModuleConfiguration = new Configuration
			{
				Config = Activator.CreateInstance<C>()
			};

			if (EnabledByDefault)
			{
				ModuleConfiguration.Enabled = true;
			}

			shouldSave = true;
		}
		else
		{
			try
			{
				ModuleConfiguration = Config.ReadObject<Configuration>();

				if (ConfigVersionChecks && ModuleConfiguration.HasConfigStructureChanged())
				{
					shouldSave = true;
				}
			}
			catch (Exception exception)
			{
				Logger.Error($"Failed loading config. JSON file is corrupted and/or invalid.", exception);
			}
		}

		ConfigInstance = ModuleConfiguration.Config;

		if (ForceEnabled)
		{
			ModuleConfiguration.Enabled = true;
		}

		if (typeof(D) != typeof(EmptyModuleData))
		{
			if (!Data.Exists())
			{
				DataInstance = Activator.CreateInstance<D>();
				shouldSave = true;
			}
			else
			{
				try
				{
					DataInstance = Data.ReadObject<D>();
				}
				catch (Exception exception)
				{
					Logger.Error($"Failed loading data. JSON file is corrupted and/or invalid.", exception);
				}
			}
		}

		if (PreLoadShouldSave(newConfig, newData))
		{
			shouldSave = true;
		}

		if (shouldSave)
		{
			Save();
		}
	}
	public override void Save()
	{
		if (ForceDisabled) return;

		if (ModuleConfiguration == null)
		{
			ModuleConfiguration = new Configuration { Config = Activator.CreateInstance<C>() };
			ConfigInstance = ModuleConfiguration.Config;
		}

		if (DataInstance == null && typeof(D) != typeof(EmptyModuleData))
		{
			DataInstance = Activator.CreateInstance<D>();
		}

		if (ForceEnabled)
		{
			ModuleConfiguration.Enabled = true;
		}

		Config.WriteObject(ModuleConfiguration);
		if (DataInstance != null) Data?.WriteObject(DataInstance);
	}
	public override void Reload()
	{
		if (ForceDisabled) return;

		try
		{
			OnUnload();
		}
		catch (Exception ex)
		{
			Logger.Error($"Failed module Unload for {Name} [Reload Request]", ex);
		}

		try
		{
			Load();
		}
		catch (Exception ex)
		{
			Logger.Error($"Failed module Load for {Name} [Reload Request]", ex);
		}

		try
		{
			OnServerInit(false);
		}
		catch (Exception ex)
		{
			Logger.Error($"Failed module OnServerInit for {Name} [Reload Request]", ex);
		}

		try
		{
			OnPostServerInit(false);
		}
		catch (Exception ex)
		{
			Logger.Error($"Failed module OnPostServerInit for {Name} [Reload Request]", ex);
		}
	}

	public virtual bool PreLoadShouldSave(bool newConfig, bool newData)
	{
		return false;
	}

	public virtual string GetConfigPath()
	{
		return Path.Combine(Defines.GetModulesFolder(), Name, "config.json");
	}
	public virtual string GetDataPath()
	{
		return Path.Combine(Defines.GetModulesFolder(), Name, "data.json");
	}

	public override void SetEnabled(bool enable)
	{
		if (ForceDisabled) return;

		if (ModuleConfiguration != null)
		{
			ModuleConfiguration.Enabled = enable;
			OnEnableStatus();
		}

		if (enable && Community.IsServerInitialized)
		{
			try
			{
				OnServerInit(false);
			}
			catch (Exception ex)
			{
				Logger.Error($"Failed OnServerInit on '{Name} v{Version}'", ex);
			}

			try
			{
				OnPostServerInit(false);
			}
			catch (Exception ex)
			{
				Logger.Error($"Failed OnPostServerInit on '{Name} v{Version}'", ex);
			}
		}
	}
	public override bool IsEnabled()
	{
		return !ForceDisabled && ModuleConfiguration is { Enabled: true };
	}

	public virtual void OnDisabled(bool initialized)
	{
		if (ForceDisabled) return;

		OnUnload();
	}
	public virtual void OnEnabled(bool initialized)
	{
		if (ForceDisabled) return;

		if (!ManualCommands)
		{
			ModLoader.ProcessCommands(Type, this, flags: BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
		}

		if (!ManualSubscriptions)
		{
			SubscribeAll();

			if (Hooks.Count > 0)
			{
				Puts($"Subscribed to {Hooks.Count:n0} {Hooks.Count.Plural("hook", "hooks")}.");
			}
		}

		DoHarmonyPatch();
	}

	public void OnEnableStatus()
	{
		if (ForceDisabled) return;

		try
		{
			if (ModuleConfiguration == null) return;

			if (ModuleConfiguration.Enabled) OnEnabled(Community.IsServerInitialized);
			else OnDisabled(Community.IsServerInitialized);
		}
		catch (Exception ex) { Logger.Error($"Failed {(ModuleConfiguration.Enabled ? "Enable" : "Disable")} initialization.", ex); }
	}

	public override void OnServerSaved()
	{

	}

	public override void OnServerInit(bool initial)
	{
	}
	public override void OnPostServerInit(bool initial)
	{

	}
	public override void OnUnload()
	{
		ModLoader.RemoveCommands(this);

		UnsubscribeAll();
		Permissions.UnregisterPermissions(this);

		if (Hooks.Count > 0)
		{
			Puts($"Unsubscribed from {Hooks.Count:n0} {Hooks.Count.Plural("hook", "hooks")}.");
		}

		DoHarmonyUnpatch();
	}
	public override void Shutdown()
	{
		OnUnload();

		Community.Runtime.ModuleProcessor.Uninstall(this);
	}

	#region Harmony

	public Harmony HarmonyInstance;

	public virtual string HarmonyDomain => $"com.carbon-module.{Name}".Replace(" ", string.Empty).ToLower();

	public virtual bool AutoPatch => false;

	public virtual void DoHarmonyPatch()
	{
		if (!AutoPatch)
		{
			return;
		}

		if (HarmonyInstance == null)
		{
			HarmonyInstance = new(HarmonyDomain);
		}

		foreach (var type in Type.GetNestedTypes(BindingFlags.DeclaredOnly | BindingFlags.Public |
		                                         BindingFlags.NonPublic | BindingFlags.Static))
		{
			try
			{
				var harmonyMethods = HarmonyInstance.CreateClassProcessor(type)?.Patch();

				if (harmonyMethods == null || harmonyMethods.Count == 0)
				{
					continue;
				}

				foreach (MethodInfo method in harmonyMethods)
				{
					Logger.Warn($"[{HarmonyDomain}] Patched '{method.Name}' method. ({type.Name})");
				}
			}
			catch (Exception ex)
			{
				Logger.Error($"[{HarmonyDomain}] Failed to patch '{type.Name}'", ex);
			}
		}
	}

	public virtual void DoHarmonyUnpatch()
	{
		if (!AutoPatch)
		{
			return;
		}

		try
		{
			if (HarmonyInstance != null)
			{
				foreach (var method in HarmonyInstance.GetPatchedMethods())
				{
					Logger.Warn($"[{HarmonyDomain}] Unpatched '{method.Name}' method. ({method.DeclaringType.Name})");
				}
			}

			HarmonyInstance?.UnpatchAll(HarmonyDomain);
			HarmonyInstance = null;
		}
		catch (Exception ex)
		{
			Logger.Error($"[{HarmonyDomain}] Failed unpatching for {ToPrettyString()}", ex);
		}
	}

	#endregion

	#region Localisation

	public virtual Dictionary<string, Dictionary<string, string>> GetDefaultPhrases() => null;

	public virtual string GetPhrase(string key)
	{
		return Lang.GetMessage(key, this);
	}
	public virtual string GetPhrase(string key, string playerId)
	{
		return Lang.GetMessage(key, this, playerId);
	}
	public virtual string GetPhrase(string key, ulong playerId)
	{
		return Lang.GetMessage(key, this, playerId == 0 ? string.Empty : playerId.ToString());
	}

	#endregion

	public void NextFrame(Action callback)
	{
		Community.Runtime.Core.NextFrame(callback);
	}

	public class Configuration : IModuleConfig
	{
		public bool Enabled { get; set; }
		public C Config { get; set; }
		public string Version { get; set; }

		public string GetVersion()
		{
			if (Config == null)
			{
				return null;
			}

			var type = Config.GetType();
			var content = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(x => x.PropertyType.FullName + x.Name)
				.Concat(type.GetFields(BindingFlags.Public | BindingFlags.Instance).Select(x => x.FieldType.FullName + x.Name))
				.Select(x => x).ToString(string.Empty);

			return StringPool.Add(content).ToString();
		}

		public bool HasConfigStructureChanged()
		{
			var version = GetVersion();
			var isValid = version == Version;

			if (!isValid)
			{
				Version = version;
			}

			return !isValid;
		}
	}
}
