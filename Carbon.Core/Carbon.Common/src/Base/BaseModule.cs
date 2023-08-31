using Carbon.Base.Interfaces;
using Defines = Carbon.Core.Defines;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Base;

public abstract class BaseModule : BaseHookable
{
	public virtual bool EnabledByDefault => false;
	public virtual bool ForceModded => false;
	public virtual bool ForceEnabled => false;

	public abstract void OnPostServerInit();
	public abstract void OnServerInit();
	public abstract void OnServerSaved();
	public abstract void Load();
	public abstract void Save();
	public abstract bool GetEnabled();
	public abstract void SetEnabled(bool enable);

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
		return Community.Runtime.ModuleProcessor.Modules.FirstOrDefault(x => x.Type.Name == name) as BaseModule;
	}
}

public class EmptyModuleConfig { }
public class EmptyModuleData { }

public abstract class CarbonModule<C, D> : BaseModule, IModule
{
	public Configuration ModuleConfiguration { get; set; }
	public DynamicConfigFile Config { get; private set; }
	public DynamicConfigFile Data { get; private set; }
	public Lang Lang { get; private set; }

	public new virtual Type Type => default;

	public D DataInstance { get; private set; }
	public C ConfigInstance { get; private set; }

	public new virtual string Name => "Not set";

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
		base.Type ??= Type;
	}
	public virtual bool InitEnd()
	{
		if (HasInitialized) return false;

		Community.Runtime.HookManager.LoadHooksFromType(Type);

		foreach (var method in Type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
		{
			if (Community.Runtime.HookManager.IsHookLoaded(method.Name))
			{
				Community.Runtime.HookManager.Subscribe(method.Name, Name);

				var hash = HookStringPool.GetOrAdd(method.Name);
				if (!Hooks.Contains(hash)) Hooks.Add(hash);
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
		var shouldSave = false;

		Config ??= new DynamicConfigFile(Path.Combine(Defines.GetModulesFolder(), Name, "config.json"));
		Data ??= new DynamicConfigFile(Path.Combine(Defines.GetModulesFolder(), Name, "data.json"));
		Lang ??= new(this);

		var newConfig = !Config.Exists();
		var newData = !Data.Exists();

		if (!Config.Exists())
		{
			ModuleConfiguration = new Configuration { Config = Activator.CreateInstance<C>() };
			if (EnabledByDefault) ModuleConfiguration.Enabled = true;
			shouldSave = true;
		}
		else
		{
			try { ModuleConfiguration = Config.ReadObject<Configuration>(); }
			catch (Exception exception) { Logger.Error($"Failed loading config. JSON file is corrupted and/or invalid.\n{exception.Message}"); }
		}

		ConfigInstance = ModuleConfiguration.Config;
		if (ForceEnabled) ModuleConfiguration.Enabled = true;

		if (typeof(D) != typeof(EmptyModuleData))
		{
			if (!Data.Exists())
			{
				DataInstance = Activator.CreateInstance<D>();
				shouldSave = true;
			}
			else
			{
				try { DataInstance = Data.ReadObject<D>(); }
				catch (Exception exception) { Logger.Error($"Failed loading data. JSON file is corrupted and/or invalid.\n{exception.Message}"); }
			}
		}

		if (PreLoadShouldSave(newConfig, newData)) shouldSave = true;

		if (shouldSave) Save();

		OnEnableStatus();
	}
	public virtual bool PreLoadShouldSave(bool newConfig, bool newData)
	{
		return false;
	}
	public override void Save()
	{
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
	public virtual void Shutdown()
	{

	}

	public override void SetEnabled(bool enable)
	{
		if (ModuleConfiguration != null)
		{
			ModuleConfiguration.Enabled = enable;
			OnEnableStatus();
		}
	}
	public override bool GetEnabled()
	{
		return ModuleConfiguration != null && ModuleConfiguration.Enabled;
	}

	public virtual void OnDisabled(bool initialized)
	{
		if (initialized) ModLoader.RemoveCommands(this);

		UnsubscribeAll();
		UnregisterPermissions();

		if (Hooks.Count > 0) Puts($"Unsubscribed from {Hooks.Count:n0} {Hooks.Count.Plural("hook", "hooks")}.");
	}
	public virtual void OnEnabled(bool initialized)
	{
		if (initialized) ModLoader.ProcessCommands(Type, this, flags: BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

		SubscribeAll();

		if (Hooks.Count > 0) Puts($"Subscribed to {Hooks.Count:n0} {Hooks.Count.Plural("hook", "hooks")}.");

		if (InitEnd())
		{
			if (initialized) OnServerInit();
		}
	}

	public void OnEnableStatus()
	{
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
		try { Save(); }
		catch (Exception ex)
		{
			Logger.Error($"Couldn't save '{Name}'", ex);
		}
	}

	public override void OnServerInit()
	{
		OnEnableStatus();
	}
	public override void OnPostServerInit()
	{

	}

	#region Permission

	public virtual bool PermissionExists(string permission)
	{
		return Community.Runtime.CorePlugin.permission.PermissionExists(permission, Community.Runtime.CorePlugin);
	}
	public virtual void RegisterPermission(string permission)
	{
		if (PermissionExists(permission)) return;

		Community.Runtime.CorePlugin.permission.RegisterPermission(permission, Community.Runtime.CorePlugin);
	}
	public virtual void UnregisterPermissions()
	{
		Community.Runtime.CorePlugin.permission.UnregisterPermissions(this);
	}
	public virtual bool HasPermission(string userId, string permission)
	{
		return Community.Runtime.CorePlugin.permission.UserHasPermission(userId, permission);
	}
	public virtual bool HasPermission(BasePlayer player, string permission)
	{
		return HasPermission(player.UserIDString, permission);
	}
	public virtual bool HasGroup(string userId, string group)
	{
		return Community.Runtime.CorePlugin.permission.UserHasGroup(userId, group);
	}
	public virtual bool HasGroup(BasePlayer player, string group)
	{
		return HasGroup(player.UserIDString, group);
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
		Community.Runtime.CorePlugin.NextFrame(callback);
	}

	public class Configuration : IModuleConfig
	{
		public bool Enabled { get; set; }
		public C Config { get; set; }
	}
}
