///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using System.IO;
using System.Reflection;
using Oxide.Core.Configuration;

namespace Carbon.Core.Modules
{
	public class BaseModule : BaseHookable
	{
		public virtual bool EnabledByDefault => false;

		public static T GetModule<T>()
		{
			foreach (var module in CarbonCore.Instance.ModuleProcessor.Modules)
			{
				if (module.GetType() == typeof(T) && module is T result) return result;
			}

			return default;
		}
	}
	public class CarbonModule<C, D> : BaseModule, IModule
	{
		public DynamicConfigFile File { get; private set; }
		public DynamicConfigFile Data { get; private set; }

		public new virtual Type Type => default;

		public Configuration ConfigInstance { get; set; }
		public D DataInstance { get; private set; }

		public C Config { get; private set; }

		public new virtual string Name => "Not set";

		/// <summary>
		/// Outputs to the game's console a message with severity level 'NOTICE'.
		/// </summary>
		/// <param name="message"></param>
		[Obsolete("Puts is deprecated, use 'Carbon.Logger.Instance.Log' instead")]
		protected void Puts(object message)
			=> Logger.Instance.Log($"[{Name}] {message}");

		/// <summary>
		/// Outputs to the game's console a message with severity level 'ERROR'.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="ex"></param>
		[Obsolete("PutsError is deprecated, use 'Carbon.Logger.Instance.Error' instead")]
		protected void PutsError(object message, Exception ex = null)
			=> Logger.Instance.Error($"[{Name}] {message}", ex);

		/// <summary>
		/// Outputs to the game's console a message with severity level 'WARNING'.
		/// </summary>
		/// <param name="message"></param>
		[Obsolete("PrintWarning is deprecated, use 'Carbon.Logger.Instance.Warn' instead")]
		protected void PrintWarning(object message)
			=> Logger.Instance.Warn($"[{Name}] {message}");

		/// <summary>
		/// Outputs to the game's console a message with severity level 'ERROR'.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="ex"></param>
		[Obsolete("PrintError is deprecated, use 'Carbon.Logger.Instance.Error' instead")]
		protected void PrintError(object message, Exception ex = null)
			=> Logger.Instance.Error($"[{Name}] {message}", ex);

		public virtual void Dispose()
		{
			File = null;
			ConfigInstance = null;
		}

		public virtual void Init()
		{
			base.Name = Name;
			base.Type = Type;

			foreach (var method in Type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
			{
				CarbonCore.Instance.Addon.InstallHooks(method.Name);
				CarbonCore.Instance.Addon.AppendHook(method.Name);
			}
			Carbon.Logger.Instance.Log($"{Name} Processed hooks");

			CarbonLoader.ProcessCommands(Type, this, flags: BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			Carbon.Logger.Instance.Log($"{Name} Processed commands");

			File = new DynamicConfigFile(Path.Combine(CarbonCore.GetModulesFolder(), Name, "config.json"));
			Data = new DynamicConfigFile(Path.Combine(CarbonCore.GetModulesFolder(), Name, "data.json"));

			Load();
			if (ConfigInstance.Enabled) OnEnableStatus();
		}
		public virtual void InitEnd()
		{
			Carbon.Logger.Instance.Log($"Initialized.");
		}
		public virtual void Load()
		{
			var shouldSave = false;

			if (!File.Exists())
			{
				ConfigInstance = new Configuration { Config = Activator.CreateInstance<C>() };
				if (EnabledByDefault) ConfigInstance.Enabled = true;
				shouldSave = true;
			}
			else
			{
				try { ConfigInstance = File.ReadObject<Configuration>(); }
				catch (Exception exception) { Carbon.Logger.Instance.Error($"Failed loading config. JSON file is corrupted and/or invalid.\n{exception.Message}"); }
			}

			if (!Data.Exists())
			{
				DataInstance = Activator.CreateInstance<D>();
				shouldSave = true;
			}
			else
			{
				try { DataInstance = Data.ReadObject<D>(); }
				catch (Exception exception) { Carbon.Logger.Instance.Error($"Failed loading data. JSON file is corrupted and/or invalid.\n{exception.Message}"); }
			}

			if (shouldSave) Save();

			Config = ConfigInstance.Config;
		}
		public virtual void Save()
		{
			if (ConfigInstance == null)
			{
				ConfigInstance = new Configuration { Config = Activator.CreateInstance<C>() };
				Config = ConfigInstance.Config;
			}

			if (DataInstance == null)
			{
				DataInstance = Activator.CreateInstance<D>();
			}

			File.WriteObject(ConfigInstance);
			Data.WriteObject(DataInstance);
		}

		public void SetEnabled(bool enable)
		{
			if (ConfigInstance != null)
			{
				ConfigInstance.Enabled = enable;
				OnEnableStatus();
			}
		}
		public bool GetEnabled()
		{
			return ConfigInstance.Enabled;
		}

		public virtual void OnDisabled(bool initialized) { }
		public virtual void OnEnabled(bool initialized) { }

		public void OnEnableStatus()
		{
			try
			{
				if (ConfigInstance.Enabled) OnEnabled(CarbonCore.IsServerFullyInitialized); else OnDisabled(CarbonCore.IsServerFullyInitialized);
			}
			catch (Exception ex) { Carbon.Logger.Instance.Error($"Failed {(ConfigInstance.Enabled ? "Enable" : "Disable")} initialization.", ex); }
		}

		private void OnServerInitialized()
		{
			if (GetEnabled()) OnEnableStatus();
		}

		public class Configuration : IModuleConfig
		{
			public bool Enabled { get; set; }
			public C Config { get; set; }
		}
	}
}
