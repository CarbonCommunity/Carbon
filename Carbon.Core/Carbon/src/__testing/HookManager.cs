
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Carbon.Hooks;
using Carbon.LoaderEx.Common;
using Carbon.LoaderEx.Components;

/*
 *
 * Copyright (c) 2022 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Processors;

internal sealed class HookManager : FacepunchBehaviour, IDisposable
{
	private bool doReload;
	private Queue<Payload> workQueue;
	private List<CarbonHookEx> staticHooks;
	private List<CarbonHookEx> dynamicHooks;

	private static readonly string[] files = { "Carbon.Hooks" };

	private struct Payload
	{
		// A hook may need more than one patch applied, hookName is mandatory,
		// identifier is optional and should only be sent when a specific patch
		// needs to be applied.
		public string hookName, identifier, requester;

		// Payload does not cary the action (install/uninstall) as this is will
		// be automatically sorted out by the Update() method based on the subscribers.
		public Payload(string name, string id, string req)
		{ hookName = name; identifier = id; requester = req; }
	}

	private struct Subscription
	{
		public string hookName, subscriber;

		public Subscription(string name, string sub)
		{ hookName = name; subscriber = sub; }
	}

	public void Reload()
	{
		doReload = true;
		enabled = false;
	}

	public void Dispose()
	{
		List<CarbonHookEx> hooks = staticHooks.Concat(dynamicHooks).ToList();
		foreach (CarbonHookEx hook in hooks) hook.Dispose();

		workQueue = default;
		staticHooks = dynamicHooks = hooks = default;
	}

	internal void Awake()
	{
		Logger.Log(" Initialized Hook processor...");

		workQueue = new Queue<Payload>();
		staticHooks = new List<CarbonHookEx>();
		dynamicHooks = new List<CarbonHookEx>();

		foreach (string file in files)
			LoadHooksFromAssemblyFile(file);
	}

	internal void OnEnable()
	{
		Logger.Log($" - Installing static hooks");
		// this is based on the assumption that a static hook will never have
		// a depedency on another hook thus it will be always applied first
		foreach (CarbonHookEx hook in staticHooks.Where(x => !x.IsInstalled))
			hook.ApplyPatch();

		Logger.Log($" - Installing dynamic hooks");
		foreach (CarbonHookEx hook in dynamicHooks.Where(x => x.HasSubscribers))
			workQueue.Enqueue(item: new Payload(hook.HookName, null, "Carbon.Core"));

		// the code block bellow is ugly and needs to be refactored..
		// the idea is to update the oxide hook list and reload the running
		// plugins when HookMangerer "restarts". rn due to instantiation or some
		// other random shit both calls will fail when bootstraping carbon, but
		// they will work during runtime.

		try
		{
			Carbon.Core.HookValidator.Refresh();
		}
		catch (System.Exception e)
		{
			Logger.Error("Couldn't refresh HookValidator", e);
		}

		try
		{
			Carbon.Community.ReloadPlugins();
		}
		catch (System.Exception e)
		{
			Logger.Error("Couldn't reload plugins", e);
		}
	}

	internal void OnDisable()
	{
		Logger.Log(" Stopping Hook processor...");

		Logger.Log($" - Unnstalling dynamic hooks");
		// the disable event will make sure the patches are removed but the
		// subscriber list is kept unchaged. this will be used on hot reloads.
		foreach (CarbonHookEx hook in dynamicHooks.Where(x => x.IsInstalled))
			hook.RemovePatch();

		Logger.Log($" - Uninstalling static hooks");
		// reverse order, dynamics get removed first, then statics.
		foreach (CarbonHookEx hook in staticHooks.Where(x => x.IsInstalled))
			hook.RemovePatch();

		if (doReload)
		{
			Logger.Log(" Reloading Hook processor...");
			doReload = false;
			enabled = true;
		}
	}

	internal void OnDestroy()
		=> Dispose();

	internal void Update()
	{
		// get the fuck out as fast as possible
		if (workQueue.Count == 0) return;

		try
		{
			Payload payload = workQueue.Dequeue();
			List<CarbonHookEx> hooks = GetHookByName(payload.hookName).ToList();

			if (payload.identifier != null)
				hooks.RemoveAll(x => x.Identifier != payload.identifier);

			foreach (CarbonHookEx hook in hooks)
			{
				int subscribers = hook.SubscribersCount;
				Logger.Debug($"Hook '{hook.HookName}[{hook.Identifier}]' has {subscribers} subscriber(s)");

				// static hooks are a special case
				if (hook.IsStaticHook) return;

				bool hasSubscribers = hook.HasSubscribers;
				bool isInstalled = hook.IsInstalled;

				// Not installed but has subs, install
				if (hasSubscribers && !isInstalled)
					Install(hook, payload.requester);

				// Installed but no subs found, uninstall
				else if (!hasSubscribers && isInstalled)
					Uninstall(hook, payload.requester);
			}
		}
		catch (System.Exception e)
		{
			Logger.Error("HookManager.Update() failed", e);
		}
	}

	private void LoadHooksFromAssemblyFile(string name)
	{
		CarbonReference hooks;

		try
		{
			// delegates asm loading to Carbon.Loader 
			hooks = AssemblyResolver.GetInstance().GetAssembly(name);
			if (hooks == null || hooks.assembly == null)
				throw new Exception($" - External hooks module '{name}' not found");
		}
		catch (System.Exception e)
		{
			Logger.Error(e.Message, e);
			return;
		}

		IEnumerable<TypeInfo> types = hooks.assembly.DefinedTypes
			.Where(x => x.GetField("metadata", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic) != default);

		int x = 0, y = 0;
		foreach (TypeInfo type in types)
		{
			try
			{
				CarbonHookEx hook = new CarbonHookEx(type);

				if (hook is null)
					throw new Exception($"Hook is null, this is a bug");

				if (IsExisting(hook))
					throw new Exception($"Found duplicated hook '{hook.HookName}'");

				if (hook.IsStaticHook)
				{
					y++;
					staticHooks.Add(hook);
					Logger.Debug($"Loaded static hook '{hook.HookName}'", 3);
				}
				else
				{
					x++;
					dynamicHooks.Add(hook);
					Logger.Debug($"Loaded dynamic hook '{hook.HookName}'", 3);
				}
			}
			catch (System.Exception e)
			{
				Logger.Error($"Error while parsing '{type.Name}'", e);
				continue;
			}
		}

		Logger.Log($" - Successfully loaded static:{y} dynamic:{x} ({types.Count()}) hooks from assembly '{name}.dll'");
	}

	internal void Subscribe(string hookName, string requester)
	{
		try
		{
			List<CarbonHookEx> hooks = GetHookByName(hookName).ToList();
			if (hooks.Count == 0) throw new Exception($"Hook name not found");

			foreach (CarbonHookEx hook in hooks)
			{
				if (hook.IsSubscribedBy(requester)) continue;

				hook.AddSubscriber(requester);
				workQueue.Enqueue(item: new Payload(hook.HookName, hook.Identifier, requester));
				Logger.Debug($"Subscribe to '{hook.HookName}[{hook.Identifier}]' by '{requester}'");
			}
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
			List<CarbonHookEx> hooks = GetHookByName(hookName).ToList();
			if (hooks.Count == 0) throw new Exception($"Hook name not found");

			foreach (CarbonHookEx hook in hooks)
			{
				if (!hook.IsSubscribedBy(requester)) continue;

				hook.RemoveSubscriber(requester);
				workQueue.Enqueue(item: new Payload(hook.HookName, hook.Identifier, requester));
				Logger.Debug($"Unsubscribe from '{hook.HookName}[{hook.Identifier}]' by '{requester}'");
			}
		}
		catch (System.Exception e)
		{
			Logger.Error($"Error while unsubscribing hook '{hookName}'", e);
			return;
		}
	}

	private void Install(CarbonHookEx hook, string requester)
	{
		try
		{
			if (hook.HasDependencies)
			{
				foreach (string item in hook.Dependencies)
				{
					List<CarbonHookEx> dependencies = GetHookByName(item).ToList();

					foreach (CarbonHookEx dependency in dependencies)
					{
						if (dependency is null)
							throw new Exception($"Dependency '{item}' is null, this is a bug");

						if (dependency.IsInstalled) continue;

						if (!dependency.ApplyPatch())
							throw new Exception($"Dependency '{dependency.HookName}[{dependency.Identifier}]' installation failed");

						dependency.AddSubscriber(requester);
						Logger.Log($"Installed dependency '{dependency.HookName}[{dependency.Identifier}]'");
					}
				}
			}

			if (!hook.ApplyPatch())
				throw new Exception($"Hook '{hook.HookName}[{hook.Identifier}]' installation failed");
			Logger.Log($"Installed hook '{hook.HookName}'[{hook.Identifier}]");
		}
		catch (System.Exception e)
		{
			hook.LastError = e;
			hook.Status = CarbonHookEx.State.Failure;
			Logger.Error($"Install hook '{hook.HookName}[{hook.Identifier}]' failed", e);
		}
	}

	private void Uninstall(CarbonHookEx hook, string requester)
	{
		try
		{
			if (!hook.RemovePatch())
				throw new Exception($"Hook '{hook.HookName}[{hook.Identifier}]' uninstallation failed");
			Logger.Log($"Uninstalled hook '{hook.HookName}[{hook.Identifier}]'");

			if (hook.HasDependencies)
			{
				foreach (string item in hook.Dependencies)
				{
					List<CarbonHookEx> dependencies = GetHookByName(item).ToList();

					foreach (CarbonHookEx dependency in dependencies)
					{
						if (dependency is null)
							throw new Exception($"Dependency '{item}' is null, this is a bug");

						if (dependency.IsInstalled) continue;

						if (!dependency.RemovePatch())
							throw new Exception($"Dependency '{dependency.HookName}[{dependency.Identifier}]' uninstallation failed");

						dependency.RemoveSubscriber(requester);
						Logger.Log($"Uninstalled dependency '{dependency.HookName}[{dependency.Identifier}]'");
					}
				}
			}
		}
		catch (System.Exception e)
		{
			hook.LastError = e;
			hook.Status = CarbonHookEx.State.Failure;
			Logger.Error($"Install hook '{hook.HookName}[{hook.Identifier}]' failed", e);
		}
	}

	private IEnumerable<CarbonHookEx> GetHookByName(string name)
		=> dynamicHooks.Where(x => x.HookName == name) ?? null;

	private CarbonHookEx GetHookByID(string identifier)
		=> dynamicHooks.FirstOrDefault(x => x.Identifier == identifier) ?? null;

	private bool IsExisting(CarbonHookEx hook)
		=> dynamicHooks.Any(x => x.PatchMethodName == hook.PatchMethodName);

	internal bool IsExisting(string hookName)
	{
		List<CarbonHookEx> hooks = GetHookByName(hookName).ToList();
		if (hooks.Count == 0) return false;

		foreach (CarbonHookEx hook in hooks)
			if (IsExisting(hook)) return true;
		return false;
	}

	internal IEnumerable<string> LoadedStaticHooksName
	{ get => staticHooks.Where(x => x.IsLoaded).Select(x => x.HookName); }

	internal IEnumerable<string> LoadedDynamicHooksName
	{ get => dynamicHooks.Where(x => x.IsLoaded).Select(x => x.HookName); }


	internal IEnumerable<CarbonHookEx> LoadedStaticHooks
	{ get => staticHooks.Where(x => x.IsLoaded); }

	internal IEnumerable<CarbonHookEx> LoadedDynamicHooks
	{ get => dynamicHooks.Where(x => x.IsLoaded); }

	internal IEnumerable<CarbonHookEx> InstalledStaticHooks
	{ get => staticHooks.Where(x => x.IsInstalled); }

	internal IEnumerable<CarbonHookEx> InstalledDynamicHooks
	{ get => dynamicHooks.Where(x => x.IsInstalled); }

}
