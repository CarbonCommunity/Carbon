using System;
using System.Collections.Generic;
using System.Linq;
using Carbon.Base;
using Carbon.Base.Interfaces;
using Carbon.Contracts;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Managers;

public class ModuleProcessor : BaseProcessor, IDisposable, IModuleProcessor
{
	public override string Name => "Module Processor";

	List<BaseHookable> IModuleProcessor.Modules { get => _modules; }

	internal List<BaseHookable> _modules { get; set; } = new List<BaseHookable>(50);

	public void Init()
	{
		// TODO ---------------------------------------------------------------
		// This needs to go into the ModuleManager. Each module must implement
		// the ICarbonModule. Currently is just a workaround to be compatible
		// with the single modules file we ship. After migrating this the 
		// IAddonCache.Types and ITypeManager.LoadedTypes should no longer be
		// required and will be removed.

		var types = typeof(Community).Assembly.GetExportedTypes().ToList();
		types.AddRange(Community.Runtime.AssemblyEx.Modules.LoadedTypes);

		foreach (var type in types)
		{
			if (type.BaseType == null || !type.BaseType.Name.Contains("CarbonModule")) continue;
			Setup(Activator.CreateInstance(type) as BaseHookable);
		}

		foreach (var hookable in _modules)
		{
			if (hookable is IModule module)
			{
				module.Init();
			}
		}

		foreach (var hookable in _modules)
		{
			if (hookable is IModule module)
			{
				module.Load();
			}
		}

		foreach (var hookable in _modules)
		{
			if (hookable is IModule module && module.GetEnabled())
			{
				module.InitEnd();
			}
		}

		foreach (var hookable in _modules)
		{
			if (hookable is IModule module && module.GetEnabled())
			{
				module.OnEnableStatus();
			}
		}
	}
	public void OnServerInit()
	{
		foreach (var hookable in _modules)
		{
			if (hookable is IModule module && module.GetEnabled())
			{
				try
				{
					module.OnServerInit();
				}
				catch (Exception ex)
				{
					Logger.Error($"[ModuleProcessor] Failed OnServerInit for '{hookable.Name}'", ex);
				}
			}
		}

		foreach (var hookable in _modules)
		{
			if (hookable is IModule module && module.GetEnabled())
			{
				try
				{
					module.OnPostServerInit();
				}
				catch (Exception ex)
				{
					Logger.Error($"[ModuleProcessor] Failed OnPostServerInit for '{hookable.Name}'", ex);
				}
			}
		}
	}
	public void OnServerSave()
	{
		foreach (var hookable in _modules)
		{
			if (hookable is IModule module && module.GetEnabled())
			{
				try
				{
					module.OnServerSaved();
				}
				catch (Exception ex)
				{
					Logger.Error($"[ModuleProcessor] Failed OnServerSave for '{hookable.Name}'", ex);
				}
			}
		}
	}
	public void Setup(BaseHookable hookable)
	{
		if (hookable is IModule module)
		{
			_modules.Add(hookable);
		}
	}

	public void Save()
	{
		foreach (var hookable in _modules)
		{
			var module = hookable.To<IModule>();

			module.Save();
		}
	}
	public void Load()
	{
		foreach (var hookable in _modules)
		{
			var module = hookable.To<IModule>();

			module.Load();
		}
	}

	public override void Dispose()
	{
		foreach (var hookable in _modules)
		{
			var module = hookable.To<IModule>();

			module.Dispose();
		}

		_modules.Clear();
	}

	void IDisposable.Dispose()
	{
		Dispose();
	}
}
