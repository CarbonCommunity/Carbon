﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using API.Events;
using API.Hooks;
using Carbon.Contracts;
using Carbon.Core;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;

public class HookManager : FacepunchBehaviour, IHookManager
{
	public IEnumerable<IHook> Patches { get => _patches; }
	public IEnumerable<IHook> StaticHooks { get => _staticHooks; }
	public IEnumerable<IHook> DynamicHooks { get => _dynamicHooks; }

	internal List<HookEx> _patches { get; set; }
	internal List<HookEx> _staticHooks { get; set; }
	internal List<HookEx> _dynamicHooks { get; set; }

	public int PatchesCount => _patches.Count;
	public int StaticHooksCount => _staticHooks.Count;
	public int DynamicHooksCount => _dynamicHooks.Count;

	private bool _doReload;
	private Queue<Payload> _workQueue;
	private List<Subscription> _subscribers;

	private static readonly string[] Files =
	{
		Path.Combine("hooks", "Carbon.Hooks.Base.dll"),
		Path.Combine("hooks", "Carbon.Hooks.Extra.dll"),
		Path.Combine("hooks", "Carbon.Hooks.Community.dll"),
	};

	private void Awake()
	{
		Logger.Log("Initialized hook processor...");

		_workQueue = new Queue<Payload>();
		_patches = new List<HookEx>();
		_staticHooks = new List<HookEx>();
		_dynamicHooks = new List<HookEx>();
		_subscribers = new List<Subscription>();

		if (Community.Runtime.Config.AutoUpdate)
		{
			Logger.Log("Updating hooks...");
			enabled = false;

			Carbon.Hooks.Updater.DoUpdate((bool result) =>
			{
				if (!result)
					Logger.Error($"Unable to update the hooks at this time, please try again later");

				enabled = true;
			});
		}
	}

	private void OnEnable()
	{
		_patches.Clear();
		_staticHooks.Clear();
		_dynamicHooks.Clear();

		foreach (string file in Files)
		{
			string path = Path.Combine(Defines.GetManagedFolder(), file);
			if (Supervisor.ASM.IsLoaded(Path.GetFileName(path)))
				Supervisor.ASM.UnloadModule(path, false);
			LoadHooksFromFile(path);
		}

		if (_patches.Count > 0)
		{
			Logger.Log($" - Installing patches");
			// I don't like this, patching stuff that may not be used but for the
			// sake of time I will let it go for now but this needs to be reviewed.
			foreach (HookEx hook in _patches.Where(x => !x.IsInstalled && !x.HasDependencies()))
				Install(hook, "Carbon.Core");
		}

		if (_staticHooks.Count > 0)
		{
			Logger.Log($" - Installing static hooks");
			foreach (HookEx hook in _staticHooks.Where(x => !x.IsInstalled))
				Install(hook, "Carbon.Core");
		}

		if (_dynamicHooks.Count > 0)
		{
			Logger.Log($" - Installing dynamic hooks");
			foreach (HookEx hook in _dynamicHooks.Where(x => HookHasSubscribers(x.Identifier)))
				_workQueue.Enqueue(item: new Payload(hook.HookName, null, "Carbon.Core"));
		}

		Community.Runtime.Events.Trigger(CarbonEvent.HooksInstalled, EventArgs.Empty);
	}

	private void OnDisable()
	{
		Logger.Log("Stopping hook processor...");

		if (_dynamicHooks.Count > 0)
		{
			Logger.Log($" - Uninstalling dynamic hooks");
			// the disable event will make sure the patches are removed but the
			// subscriber list is kept unchanged. this will be used on hot reloads.
			foreach (HookEx hook in _dynamicHooks.Where(x => x.IsInstalled))
				hook.RemovePatch();
		}

		if (_staticHooks.Count > 0)
		{
			Logger.Log($" - Uninstalling static hooks");
			// reverse order, dynamics get removed first, then statics, then patches
			foreach (HookEx hook in _staticHooks.Where(x => x.IsInstalled))
				hook.RemovePatch();
		}

		if (_patches.Count > 0)
		{
			Logger.Log($" - Uninstalling patches");
			foreach (HookEx patch in _patches.Where(x => x.IsInstalled))
				patch.RemovePatch();
		}

		if (!_doReload) return;

		Logger.Log("Reloading hook processor...");
		_doReload = false;
		enabled = true;
	}

	private void OnDestroy()
	{
		Logger.Log("Destroying hook processor...");

		// make sure all patches are removed
		List<HookEx> hooks = _staticHooks.Concat(_dynamicHooks).Concat(_patches).ToList();
		foreach (HookEx hook in hooks) hook.Dispose();

		hooks = default;
		_workQueue = default;
		_subscribers = default;

		_patches = default;
		_staticHooks = default;
		_dynamicHooks = default;
	}

	private void Update()
	{
		// get the fuck out as fast as possible
		if (_workQueue.Count == 0) return;

		try
		{
			int limit = 10;
			while (_workQueue.Count > 0 && limit-- > 0)
			{
				Payload payload = _workQueue.Dequeue();

				List<HookEx> hooks = GetHookByName(payload.HookName).ToList();

				if (payload.Identifier != null)
					hooks.RemoveAll(x => x.Identifier != payload.Identifier);

				foreach (HookEx hook in hooks)
				{
					// static hooks are a special case
					// and should never be dynamically uninstalled.
					if (hook.IsStaticHook) return;

					bool hasSubscribers = HookHasSubscribers(hook.Identifier);
					bool isInstalled = hook.IsInstalled;

					switch (hasSubscribers)
					{
						// Not installed but has subs, install
						case true when !isInstalled:
							Install(hook, payload.Requester);
							break;
						// Installed but no subs found, uninstall
						case false when isInstalled:
							Uninstall(hook, payload.Requester);
							break;
					}
				}
				hooks = default;
			}
		}
		catch (System.Exception e)
		{
			Logger.Error("HookManager.Update() failed", e);
		}
	}

	private void LoadHooksFromFile(string fileName)
	{
		Assembly hooks;

		try
		{
			// delegates asm loading to Carbon.Loader 
			hooks = Supervisor.ASM.LoadModule(fileName);

			if (hooks == null)
				throw new Exception($"Assembly is null");
		}
		catch (System.Exception e)
		{
			Logger.Error($"Error while loading hooks from '{fileName}'.");
			Logger.Error($"Either the file is corrupt or has an unsuported format/version.", e);
			return;
		}

		Type @base = hooks.GetType("Carbon.Hooks.Patch")
			?? typeof(Patch);

		Type attr = hooks.GetType("Carbon.Hooks.HookAttribute.Patch")
			?? typeof(HookAttribute.Patch);

		IEnumerable<TypeInfo> types = hooks.DefinedTypes
			.Where(type => @base.IsAssignableFrom(type) && Attribute.IsDefined(type, attr)).ToList();

		int x = 0, y = 0, z = 0;
		foreach (TypeInfo type in types)
		{
			try
			{
				HookEx hook = new HookEx(type);

				if (hook is null)
					throw new Exception($"Hook is null, this is a bug");

				if (IsHookLoaded(hook))
					throw new Exception($"Found duplicated hook '{hook}'");

				if (hook.IsPatch)
				{
					z++;
					_patches.Add(hook);
					Logger.Debug($"Loaded patch '{hook}'", 3);
				}
				else if (hook.IsStaticHook)
				{
					y++;
					_staticHooks.Add(hook);
					Logger.Debug($"Loaded static hook '{hook}'", 3);
				}
				else
				{
					x++;
					_dynamicHooks.Add(hook);
					Logger.Debug($"Loaded dynamic hook '{hook}'", 3);
				}
			}
			catch (System.Exception e)
			{
				Logger.Error($"Error while parsing '{type.Name}'", e);
				continue;
			}
		}

		Logger.Log($" - Successfully loaded patches:{z} static:{y} dynamic:{x} "
			+ $"({types.Count()}) hooks from assembly '{Path.GetFileName(fileName)}'");
		types = default;
	}

	private void Install(HookEx hook, string requester)
	{
		try
		{
			List<HookEx> dependencies = GetHookDependencyTree(hook);

			foreach (HookEx dependency in dependencies)
			{
				if (!dependency.ApplyPatch())
					throw new Exception($"Dependency '{dependency}' for '{hook}' installation failed");
				AddSubscriber(dependency.Identifier, requester);
				Logger.Debug($"Installed dependency '{dependency}' for '{hook}'", 1);
			}

			if (!hook.ApplyPatch())
				throw new Exception($"Unable to apply patch");

			List<HookEx> dependants = GetHookDependantTree(hook);

			foreach (HookEx dependant in dependants)
			{
				if (!dependant.ApplyPatch())
					throw new Exception($"Dependant '{dependant}' for '{hook}' installation failed");
				AddSubscriber(dependant.Identifier, requester);
				Logger.Debug($"Installed dependant '{dependant}' for '{hook}'", 1);
			}

			dependants = default;
			dependencies = default;

			Logger.Debug($"Installed hook '{hook}'", 1);
		}
		catch (System.Exception e)
		{
			GetHookById(hook.Identifier).SetStatus(HookState.Failure, e);
			Logger.Error($"Install hook '{hook}' failed", e);
		}
	}

	private void Uninstall(HookEx hook, string requester)
	{
		try
		{
			List<HookEx> dependants = GetHookDependantTree(hook);
			dependants.Reverse();

			foreach (HookEx dependant in dependants)
			{
				if (!dependant.RemovePatch())
					throw new Exception($"Dependant '{dependant}' for '{hook}' uninstallation failed");
				RemoveSubscriber(dependant.Identifier, requester);
				Logger.Debug($"Uninstalled dependant '{dependant}' for '{hook}'", 1);
			}

			if (!hook.RemovePatch())
				throw new Exception($"Unable to remove patch");

			List<HookEx> dependencies = GetHookDependencyTree(hook);
			dependencies.Reverse();

			foreach (HookEx dependency in dependencies)
			{
				if (!dependency.RemovePatch())
					throw new Exception($"Dependency '{dependency}' for '{hook}' uninstallation failed");
				RemoveSubscriber(dependency.Identifier, requester);
				Logger.Debug($"Uninstalled dependency '{dependency}' for '{hook}'", 1);
			}

			dependants = default;
			dependencies = default;

			Logger.Debug($"Uninstalled hook '{hook}'", 1);
		}
		catch (System.Exception e)
		{
			GetHookById(hook.Identifier).SetStatus(HookState.Failure, e);
			Logger.Error($"Uninstall hook '{hook}' failed", e);
		}
	}

	private List<HookEx> GetHookDependencyTree(HookEx hook)
	{
		List<HookEx> dependencies = new List<HookEx>();

		foreach (string dependency in hook.Dependencies)
		{
			List<HookEx> list = GetHookByFullName(dependency).ToList();

			if (list.Count < 1)
			{
				Logger.Error($"Dependency '{dependency}' for '{hook}' not loaded, this is a bug");
				continue;
			}

			foreach (HookEx item in list)
			{
				dependencies = dependencies.Concat(GetHookDependencyTree(item)).ToList();
				dependencies.Add(item);
			}
		}
		return dependencies.Distinct().ToList();
	}

	private List<HookEx> GetHookDependantTree(HookEx hook)
	{
		List<HookEx> dependants = new List<HookEx>();
		List<HookEx> list = _patches.Where(x => x.Dependencies.Contains(hook.HookFullName)).ToList();

		foreach (HookEx item in list)
		{
			dependants = dependants.Concat(GetHookDependantTree(item)).ToList();
			dependants.Add(item);
		}
		return dependants.Distinct().ToList();
	}

	private IEnumerable<HookEx> LoadedHooks
	{ get => _patches.Concat(_dynamicHooks).Concat(_staticHooks).Where(x => x.IsLoaded); }

	private IEnumerable<HookEx> GetHookByName(string name)
		=> LoadedHooks.Where(x => x.HookName == name) ?? null;

	private IEnumerable<HookEx> GetHookByFullName(string name)
		=> LoadedHooks.Where(x => x.HookFullName == name) ?? null;

	private HookEx GetHookById(string identifier)
		=> LoadedHooks.FirstOrDefault(x => x.Identifier == identifier) ?? null;

	internal bool IsHookLoaded(HookEx hook)
		=> LoadedHooks.Any(x => x.PatchMethodName == hook.PatchMethodName);

	public bool IsHookLoaded(string hookName)
	{
		List<HookEx> hooks = GetHookByName(hookName).ToList();
		return hooks.Count != 0 && hooks.Any(IsHookLoaded);
	}

	public IEnumerable<IHook> InstalledPatches
	{ get => _patches.Where(x => x.IsInstalled); }

	public IEnumerable<IHook> InstalledStaticHooks
	{ get => _staticHooks.Where(x => x.IsInstalled); }

	public IEnumerable<IHook> InstalledDynamicHooks
	{ get => _dynamicHooks.Where(x => x.IsInstalled); }


	private bool HookIsSubscribedBy(string identifier, string subscriber)
		=> _subscribers?.Where(x => x.Identifier == identifier).Any(x => x.Subscriber == subscriber) ?? false;

	private bool HookHasSubscribers(string identifier)
		=> _subscribers?.Any(x => x.Identifier == identifier) ?? false;

	public int GetHookSubscriberCount(string identifier)
		=> _subscribers.Where(x => x.Identifier == identifier).ToList().Count;

	private void AddSubscriber(string identifier, string subscriber)
		=> _subscribers.Add(item: new Subscription { Identifier = identifier, Subscriber = subscriber });

	private void RemoveSubscriber(string identifier, string subscriber)
		=> _subscribers.RemoveAll(x => x.Identifier == identifier && x.Subscriber == subscriber);

	public void Subscribe(string hookName, string requester)
	{
		try
		{
			List<HookEx> hooks = GetHookByName(hookName).ToList();
			if (hooks.Count == 0) throw new Exception($"Hook fileName not found");

			foreach (HookEx hook in hooks.Where(hook => !HookIsSubscribedBy(hook.Identifier, requester)).ToList())
			{
				AddSubscriber(hook.Identifier, requester);
				_workQueue.Enqueue(item: new Payload(hook.HookName, hook.Identifier, requester));
				Logger.Debug($"Subscribe to '{hook}' by '{requester}'");
			}

			hooks = default;
		}
		catch (System.Exception e)
		{
			Logger.Error($"Error while subscribing hook '{hookName}'", e);
			return;
		}
	}

	public void Unsubscribe(string hookName, string requester)
	{
		try
		{
			List<HookEx> hooks = GetHookByName(hookName).ToList();
			if (hooks.Count == 0) throw new Exception($"Hook fileName not found");

			foreach (HookEx hook in hooks.Where(hook => HookIsSubscribedBy(hook.Identifier, requester)).ToList())
			{
				RemoveSubscriber(hook.Identifier, requester);
				_workQueue.Enqueue(item: new Payload(hook.HookName, hook.Identifier, requester));
				Logger.Debug($"Unsubscribe from '{hook}' by '{requester}'");
			}

			hooks = default;
		}
		catch (System.Exception e)
		{
			Logger.Error($"Error while unsubscribing hook '{hookName}'", e);
			return;
		}
	}
}