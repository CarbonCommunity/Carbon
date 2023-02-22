using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using API.Hooks;
using Carbon.Core;
using HarmonyLib;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;

public class HookManager : FacepunchBehaviour, IDisposable
{
	public List<HookEx> Patches, StaticHooks, DynamicHooks;

	private bool _doReload;
	private Queue<Payload> _workQueue;
	private List<Subscription> _subscribers;

	private static readonly string[] Files =
	{
		Path.Combine("hooks", "Carbon.Hooks.Base.dll"),
		Path.Combine("hooks", "Carbon.Hooks.Extended.dll"),
	};

	public void Reload()
	{
		_doReload = true;
		enabled = false;
	}

	public void Dispose()
	{
		List<HookEx> hooks = StaticHooks.Concat(DynamicHooks).Concat(Patches).ToList();
		foreach (HookEx hook in hooks) hook.Dispose();

		hooks = default;
		_workQueue = default;
		_subscribers = default;

		Patches = default;
		StaticHooks = default;
		DynamicHooks = default;
	}

	internal void Awake()
	{
		Logger.Log("Initialized hook processor...");

		_workQueue = new Queue<Payload>();
		Patches = new List<HookEx>();
		StaticHooks = new List<HookEx>();
		DynamicHooks = new List<HookEx>();
		_subscribers = new List<Subscription>();

		if (Community.Runtime.Config.AutoUpdate)
		{
			Logger.Log(" Updating hooks...");
			enabled = false;

			Carbon.Hooks.Updater.DoUpdate((bool result) =>
			{
				if (!result)
					Logger.Error($" Unable to update the hooks at this time, please try again later");

				enabled = true;
			});
		}
	}

	internal void OnEnable()
	{
		Patches.Clear();
		StaticHooks.Clear();
		DynamicHooks.Clear();

		foreach (string file in Files)
		{
			string path = Path.Combine(Defines.GetManagedFolder(), file);
			if (Supervisor.ASM.IsLoaded(Path.GetFileName(path)))
				Supervisor.ASM.UnloadModule(path, false);
			LoadHooksFromFile(path);
		}

		if (Patches.Count > 0)
		{
			Logger.Log($" - Installing patches");
			// I don't like this, patching stuff that may not be used but for the
			// sake of time I will let it go for now but this needs to be reviewed.
			foreach (HookEx patch in Patches.Where(x => !x.IsInstalled && !x.HasDependencies()))
				patch.ApplyPatch();
		}

		if (StaticHooks.Count > 0)
		{
			Logger.Log($" - Installing static hooks");
			// this is based on the assumption that a static hook will never have
			// a dependency on another hook thus it will be always applied first
			foreach (HookEx hook in StaticHooks.Where(x => !x.IsInstalled))
				hook.ApplyPatch();
		}

		if (DynamicHooks.Count > 0)
		{
			Logger.Log($" - Installing dynamic hooks");
			foreach (HookEx hook in DynamicHooks.Where(x => HookHasSubscribers(x.Identifier)))
				_workQueue.Enqueue(item: new Payload(hook.HookName, null, "Carbon.Core"));
		}

		Community.Runtime.Events.Trigger(
			API.Events.CarbonEvent.HooksInstalled, EventArgs.Empty);

		try
		{
			if (_subscribers.Count == 0) return;

			// the code block bellow is ugly but the idea is to update the oxide
			// hook list and reload the running plugins when HookManager "restarts".

			Carbon.Core.HookValidator.Refresh();

			foreach (Subscription item in _subscribers.ToList())
			{
				_subscribers.Remove(item);
				Subscribe(item.Identifier, item.Subscriber);
			}
		}
		catch (System.Exception e)
		{
			Logger.Error("Couldn't refresh HookValidator", e);
		}
	}

	internal void OnDisable()
	{
		Logger.Log(" Stopping hook processor...");

		if (DynamicHooks.Count > 0)
		{
			Logger.Log($" - Uninstalling dynamic hooks");
			// the disable event will make sure the patches are removed but the
			// subscriber list is kept unchanged. this will be used on hot reloads.
			foreach (HookEx hook in DynamicHooks.Where(x => x.IsInstalled))
				hook.RemovePatch();
		}

		if (StaticHooks.Count > 0)
		{
			Logger.Log($" - Uninstalling static hooks");
			// reverse order, dynamics get removed first, then statics, then patches
			foreach (HookEx hook in StaticHooks.Where(x => x.IsInstalled))
				hook.RemovePatch();
		}

		if (Patches.Count > 0)
		{
			Logger.Log($" - Uninstalling patches");
			foreach (HookEx patch in Patches.Where(x => x.IsInstalled))
				patch.RemovePatch();
		}

		if (!_doReload) return;

		Logger.Log(" Reloading hook processor...");
		_doReload = false;
		enabled = true;
	}

	internal void OnDestroy()
		=> Dispose();

	internal void Update()
	{
		// get the fuck out as fast as possible
		if (_workQueue.Count == 0) return;

		try
		{
			Payload payload = _workQueue.Dequeue();
			List<HookEx> hooks = GetHookByName(payload.HookName).ToList();

			if (payload.Identifier != null)
				hooks.RemoveAll(x => x.Identifier != payload.Identifier);

			foreach (HookEx hook in hooks)
			{
				// int subscribers = GetHookSubscriberCount(hook.HookName);
				// Logger.Debug($"Hook '{hook.HookName}[{hook.Identifier}]' has {subscribers} subscriber(s)");

				// static hooks are a special case
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

		Type t = hooks.GetType("Carbon.Hooks.HookAttribute.Patch")
			?? typeof(HookAttribute.Patch);

		IEnumerable<TypeInfo> types = hooks.DefinedTypes
			.Where(x => Attribute.IsDefined(x, t)).ToList();

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
					Patches.Add(hook);
					Logger.Debug($"Loaded patch '{hook}'", 3);
				}
				else if (hook.IsStaticHook)
				{
					y++;
					StaticHooks.Add(hook);
					Logger.Debug($"Loaded static hook '{hook}'", 3);
				}
				else
				{
					x++;
					DynamicHooks.Add(hook);
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

	internal void Subscribe(string hookName, string requester)
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

	internal void Unsubscribe(string hookName, string requester)
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

		if (hook.HasDependencies())
		{
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
		}
		return dependencies.Distinct().ToList();
	}

	private List<HookEx> GetHookDependantTree(HookEx hook)
	{
		List<HookEx> dependants = new List<HookEx>();

		foreach (HookEx item in Patches.Where(x => x.Dependencies.Contains(hook.HookFullName)).ToList())
		{
			dependants = dependants.Concat(GetHookDependantTree(item)).ToList();
			dependants.Add(item);
		}
		return dependants.Distinct().ToList();
	}

	private IEnumerable<HookEx> GetHookByName(string name)
		=> DynamicHooks.Where(x => x.HookName == name) ?? null;

	private IEnumerable<HookEx> GetHookByFullName(string name)
		=> DynamicHooks.Where(x => x.HookFullName == name) ?? null;

	private HookEx GetHookById(string identifier)
		=> DynamicHooks.FirstOrDefault(x => x.Identifier == identifier) ?? null;

	private bool IsHookLoaded(HookEx hook)
		=> DynamicHooks.Any(x => x.PatchMethodName == hook.PatchMethodName);

	internal bool IsHookLoaded(string hookName)
	{
		List<HookEx> hooks = GetHookByName(hookName).ToList();
		return hooks.Count != 0 && hooks.Any(IsHookLoaded);
	}

	internal IEnumerable<string> LoadedStaticHooksName
	{ get => StaticHooks.Where(x => x.IsLoaded).Select(x => x.HookName); }

	internal IEnumerable<string> LoadedDynamicHooksName
	{ get => DynamicHooks.Where(x => x.IsLoaded).Select(x => x.HookName); }


	internal IEnumerable<HookEx> InstalledPatches
	{ get => Patches.Where(x => x.IsInstalled); }

	internal IEnumerable<HookEx> InstalledStaticHooks
	{ get => StaticHooks.Where(x => x.IsInstalled); }

	internal IEnumerable<HookEx> InstalledDynamicHooks
	{ get => DynamicHooks.Where(x => x.IsInstalled); }


	internal bool HookIsSubscribedBy(string identifier, string subscriber)
		=> _subscribers?.Where(x => x.Identifier == identifier).Any(x => x.Subscriber == subscriber) ?? false;

	internal bool HookHasSubscribers(string identifier)
		=> _subscribers?.Any(x => x.Identifier == identifier) ?? false;

	internal int GetHookSubscriberCount(string identifier)
		=> _subscribers.Where(x => x.Identifier == identifier).ToList().Count;


	internal void AddSubscriber(string identifier, string subscriber)
		=> _subscribers.Add(item: new Subscription { Identifier = identifier, Subscriber = subscriber });

	internal void RemoveSubscriber(string identifier, string subscriber)
		=> _subscribers.RemoveAll(x => x.Identifier == identifier && x.Subscriber == subscriber);

}
