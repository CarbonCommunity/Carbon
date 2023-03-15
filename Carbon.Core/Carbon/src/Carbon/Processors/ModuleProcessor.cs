using System;
using System.Collections.Generic;
using System.Reflection;
using Carbon.Base;
using Carbon.Base.Interfaces;
using Carbon.Contracts;
using Carbon.Extensions;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Processors;

public class ModuleProcessor : BaseProcessor, IDisposable, IModuleProcessor
{
	public override string Name => "Module Processor";

	List<BaseHookable> IModuleProcessor.Modules { get => _modules; }

	internal List<BaseHookable> _modules { get; set; } = new List<BaseHookable>(50);

	public void Init()
	{
		foreach (var type in AccessToolsEx.AllTypes())
		{
			if (type.BaseType == null || !type.BaseType.Name.Contains("CarbonModule")) continue;

			Setup(Activator.CreateInstance(type) as BaseHookable);
		}
	}
	public void OnServerInit()
	{
		foreach(var hookable in _modules)
		{
			if (hookable is IModule module)
			{
				try
				{
					module.OnServerInit();
				}
				catch (Exception ex)
				{
					Logger.Error($"[ModuleProcessor] Failed OnServerInit for '{hookable.Name}'.", ex);
				}
			}
		}
	}
	public void Setup(BaseHookable hookable)
	{
		if (hookable is IModule module)
		{
			try
			{
				module.Init();
			}
			catch (Exception ex)
			{
				Logger.Error($"[ModuleProcessor] Failed initializing '{hookable.Name}'.", ex);
			}

			_modules.Add(hookable);
			module.InitEnd();
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
			module.OnEnableStatus();
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
