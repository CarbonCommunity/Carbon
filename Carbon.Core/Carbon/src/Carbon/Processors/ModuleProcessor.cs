using System;
using System.Collections.Generic;
using System.Linq;
using Carbon.Base;
using Carbon.Base.Interfaces;
using Carbon.Contracts;
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

	internal List<BaseHookable> _modules { get; set; } = new List<BaseHookable>(50);

	public void Init()
	{
		var types = typeof(Community).Assembly.GetExportedTypes().AsEnumerable();
		var modules = Pool.GetList<string>();
		modules.AddRange(Community.Runtime.AssemblyEx.Modules.Loaded);

		foreach (var module in modules)
		{
			var assembly = Community.Runtime.AssemblyEx.Modules.Load(module, "ModuleProcessor.Init");
			types = types.Concat(assembly.GetExportedTypes());
		}

		Pool.FreeList(ref modules);

		foreach (var type in types)
		{
			if (type.BaseType == null || !type.BaseType.Name.Contains("CarbonModule")) continue;

			Setup(Activator.CreateInstance(type) as BaseHookable);
		}
	}
	public void OnServerInit()
	{
		foreach (var hookable in _modules)
		{
			if (hookable is IModule module)
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
			if (hookable is IModule module)
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
			if (hookable is IModule module)
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
			try
			{
				module.Init();

				var phrases = module.GetDefaultPhrases();

				if (phrases != null)
				{
					foreach (var language in phrases)
					{
						Community.Runtime.CorePlugin.lang.RegisterMessages(language.Value, hookable, language.Key);
					}
				}
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
