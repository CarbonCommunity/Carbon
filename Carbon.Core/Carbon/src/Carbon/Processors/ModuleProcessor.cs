using System;
using System.Collections.Generic;
using System.Linq;
using API.Assembly;
using Carbon.Base;
using Carbon.Base.Interfaces;
using Carbon.Contracts;
using Carbon.Extensions;
using Facepunch;

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

	internal List<BaseHookable> _modules { get; set; } = new List<BaseHookable>(200);

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

		var temporaryTypes = types.ToArray();
		Build(temporaryTypes);

		Facepunch.Pool.FreeList(ref types);
		Array.Clear(temporaryTypes, 0, temporaryTypes.Length);
		temporaryTypes = null;
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
		if (hookable is IModule)
		{
			_modules.Add(hookable);
		}
	}
	public void Build(params Type[] types)
	{
		var modules = Facepunch.Pool.GetList<BaseHookable>();

		foreach (var type in types)
		{
			if (type.IsAbstract ||
				type.BaseType == null ||
				!type.IsSubclassOf(typeof(BaseModule)))
			{
				continue;
			}

			var module = Activator.CreateInstance(type) as BaseHookable;
			Setup(module);
			modules.Add(module);
		}

		foreach (var hookable in modules)
		{
			if (hookable is IModule module)
			{
				try
				{
					module.Init();
				}
				catch (Exception ex) { Logger.Error($"Failed module Init for {module?.GetType().FullName}", ex); }
			}
		}

		foreach (var hookable in modules)
		{
			if (hookable is IModule module)
			{
				try
				{
					module.Load();
				}
				catch (Exception ex) { Logger.Error($"Failed module Load for {module?.GetType().FullName}", ex); }
			}
		}

		foreach (var hookable in modules)
		{
			if (hookable is IModule module && module.GetEnabled())
			{
				try
				{
					module.InitEnd();
				}
				catch (Exception ex) { Logger.Error($"Failed module InitEnd for {module?.GetType().FullName}", ex); }
			}
		}

		foreach (var hookable in modules)
		{
			if (hookable is IModule module && module.GetEnabled())
			{
				try
				{
					module.OnEnableStatus();
				}
				catch (Exception ex) { Logger.Error($"Failed module OnEnableStatus [{module?.GetEnabled()}] for {module?.GetType().FullName}", ex); }
			}
		}

		Facepunch.Pool.FreeList(ref modules);
	}
	public void Uninstall(IModule module)
	{
		module.Shutdown();
		_modules.RemoveAll(x => x == module);
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
