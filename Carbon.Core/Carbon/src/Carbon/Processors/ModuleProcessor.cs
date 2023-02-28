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

public class ModuleProcessor : IDisposable, IModuleProcessor
{
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
	public void Setup(BaseHookable module)
	{
		if (module is IModule hookable)
		{
			hookable.Init();
			_modules.Add(module);
			hookable.InitEnd();
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

	public void Dispose()
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
