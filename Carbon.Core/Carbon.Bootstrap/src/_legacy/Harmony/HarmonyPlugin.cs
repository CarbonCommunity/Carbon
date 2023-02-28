using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using API.Contracts;
using Legacy.ASM;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Legacy.Harmony;

internal class HarmonyPlugin : IDisposable
{
	private string _identifier;
	private HarmonyLib.Harmony _handler;
	private List<IHarmonyMod> _hooks;


	private Assembly _assembly;

	private Type[] types
	{ get => _assembly.GetTypes() ?? null; }

	internal string Name
	{ get => _assembly.GetName().Name ?? null; }

	internal System.Reflection.Assembly Assembly
	{ get => _assembly ?? null; }

	internal string FileName
	{ get; private set; }

	internal string Location
	{ get; private set; }


	private bool _enabled;

	internal bool Enabled
	{
		get => _enabled;
		set
		{
			if (_enabled == value) return;
			_enabled = value;

			if (_enabled)
			{
				OnEnable();
			}
			else
			{
				OnDisable();
			}
		}
	}

	public override string ToString()
		=> $"{Name}[{_identifier.Substring(0, 6)}]";

	private HarmonyPlugin()
	{
		_identifier = $"{Guid.NewGuid():N}";
		_hooks = new List<IHarmonyMod>();
		_handler = new HarmonyLib.Harmony(_identifier);
	}

	internal HarmonyPlugin(string file, string location) : this()
	{
		FileName = file;
		Location = location;

		_assembly = AssemblyManager.GetInstance()
			.LoadAssembly(Path.Combine(location, file));
	}

	internal void Awake()
	{
		foreach (Type type in types)
		{
			if (!typeof(IHarmonyMod).IsAssignableFrom(type)) continue;

			try
			{
				if (Activator.CreateInstance(type) is not IHarmonyMod hook)
					throw new NullReferenceException();

				Utility.Logger.Log($" - Instance of '{hook}' created");
				_hooks.Add(hook);
			}
			catch (System.Exception e)
			{
				Utility.Logger.Error($"Failed to instantiate hook from '{type}'", e);
				continue;
			}
		}

		Enabled = true;

		// foreach (var method in HarmonyLib.Harmony.GetAllPatchedMethods())
		// 	Logger.Log($" > {method.Name} at {method.Module} by " +
		// 		$"{string.Join(",", HarmonyLib.Harmony.GetPatchInfo(method).Owners)}");
	}

	private void OnEnable()
	{
		try
		{
			Utility.Logger.Debug($" - Apply '{this}' harmony patches");
			_handler.PatchAll(_assembly);
		}
		catch (System.Exception e)
		{
			Utility.Logger.Error($"Failed to apply harmony patches for '{this}'", e);
			throw;
		}
	}

	private void OnDisable()
	{
		try
		{
			Utility.Logger.Debug($" - Remove '{this}' harmony patches");
			_handler.UnpatchAll(_identifier);
		}
		catch (System.Exception e)
		{
			Utility.Logger.Error($"Failed to remove harmony patches for '{this}'", e);
			throw;
		}
	}

	internal void OnLoaded()
	{
		foreach (IHarmonyMod hook in _hooks)
		{
			try
			{
				Utility.Logger.Log($" - Trigger '{this}' OnLoaded hook");
				hook.OnLoaded(args: new EventArgs());
			}
			catch (System.Exception e)
			{
				Utility.Logger.Error($"Failed to trigger 'OnLoaded' hook for '{this}'", e);
			}
		}
	}

	internal void OnUnloaded()
	{
		foreach (IHarmonyMod hook in _hooks)
		{
			try
			{
				Utility.Logger.Log($" - Trigger '{this}' OnUnloaded hook");
				hook.OnUnloaded(args: new EventArgs());
			}
			catch (System.Exception e)
			{
				Utility.Logger.Error($"Failed to trigger 'OnUnloaded' hook for '{this}'", e);
			}
		}
	}

	public void Dispose()
	{
		Enabled = false;

		_hooks.Clear();
		_hooks = default;
		_handler = default;
	}
}
