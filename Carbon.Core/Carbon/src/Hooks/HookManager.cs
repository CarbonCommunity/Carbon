using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Carbon.Core;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;

public class HookManager : FacepunchBehaviour, IDisposable
{
	public List<HookEx> StaticHooks;
	public List<HookEx> DynamicHooks;

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
		List<HookEx> hooks = StaticHooks.Concat(DynamicHooks).ToList();
		foreach (HookEx hook in hooks) hook.Dispose();

		hooks = default;
		_workQueue = default;
		_subscribers = default;
		StaticHooks = DynamicHooks = default;
	}

	internal void Awake()
	{
		Logger.Log(" Initialized hook processor...");

		_workQueue = new Queue<Payload>();
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
		StaticHooks.Clear();
		DynamicHooks.Clear();

		foreach (string file in Files)
		{
			string path = Path.Combine(Defines.GetManagedFolder(), file);
			if (Supervisor.ASM.IsLoaded(Path.GetFileName(path)))
				Supervisor.ASM.UnloadModule(path, false);
			LoadHooksFromFile(path);
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
			foreach (HookEx hook in DynamicHooks.Where(x => HookHasSubscribers(x.HookName)))
				_workQueue.Enqueue(item: new Payload(hook.HookName, null, "Carbon.Core"));
		}

		try
		{
			if (_subscribers.Count == 0) return;

			// the code block bellow is ugly but the idea is to update the oxide
			// hook list and reload the running plugins when HookManager "restarts".

			Carbon.Core.HookValidator.Refresh();

			foreach (Subscription item in _subscribers.ToList())
			{
				_subscribers.Remove(item);
				Subscribe(item.HookName, item.Subscriber);
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
			// reverse order, dynamics get removed first, then statics.
			foreach (HookEx hook in StaticHooks.Where(x => x.IsInstalled))
				hook.RemovePatch();
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

				bool hasSubscribers = HookHasSubscribers(hook.HookName);
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
				throw new Exception($"External hooks module '{fileName}' not found");
		}
		catch (System.Exception e)
		{
			Logger.Error(e.Message, e);
			return;
		}

		Type t = hooks.GetType("Carbon.Hooks.HookAttribute.Patch")
			?? typeof(HookAttribute.Patch);

		IEnumerable<TypeInfo> types = hooks.DefinedTypes
			.Where(x => Attribute.IsDefined(x, t)).ToList();

		int x = 0, y = 0;
		foreach (TypeInfo type in types)
		{
			try
			{
				HookEx hook = new HookEx(type);

				if (hook is null)
					throw new Exception($"Hook is null, this is a bug");

				if (IsHookLoaded(hook))
					throw new Exception($"Found duplicated hook '{hook}'");

				if (hook.IsStaticHook)
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

		Logger.Log($" - Successfully loaded static:{y} dynamic:{x} ({types.Count()}) hooks from assembly '{Path.GetFileName(fileName)}'");
		types = default;
	}

	internal void Subscribe(string hookName, string requester)
	{
		try
		{
			List<HookEx> hooks = GetHookByName(hookName).ToList();
			if (hooks.Count == 0) throw new Exception($"Hook fileName not found");

			foreach (HookEx hook in hooks.Where(hook => !HookIsSubscribedBy(hook.HookName, requester)).ToList())
			{
				AddSubscriber(hook.HookName, requester);
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

			foreach (HookEx hook in hooks.Where(hook => HookIsSubscribedBy(hook.HookName, requester)).ToList())
			{
				RemoveSubscriber(hook.HookName, requester);
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
			if (hook.HasDependencies())
			{
				List<HookEx> dependencies;

				foreach (string item in hook.Dependencies)
				{
					dependencies = GetHookByName(item).ToList();

					if (dependencies.Count < 1)
						throw new Exception($"Dependency '{item}' not found, this is a bug");

					foreach (HookEx dependency in dependencies)
					{
						if (dependency is null)
							throw new Exception($"Dependency '{item}' is null, this is a bug");

						if (dependency.IsInstalled) continue;

						if (!dependency.ApplyPatch())
							throw new Exception($"Dependency '{dependency}' installation failed");

						AddSubscriber(dependency.HookName, requester);
						Logger.Log($"Installed dependency '{dependency}'");
					}
				}

				dependencies = default;
			}

			if (!hook.ApplyPatch())
				throw new Exception($"Unable to apply patch");
			Logger.Log($"Installed hook '{hook}");
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
			if (!hook.RemovePatch())
				throw new Exception($"Unable to remove patch");
			Logger.Log($"Uninstalled hook '{hook}'");

			if (!hook.HasDependencies()) return;

			List<HookEx> dependencies;

			foreach (string item in hook.Dependencies)
			{
				dependencies = GetHookByName(item).ToList();

				if (dependencies.Count < 1)
					throw new Exception($"Dependency '{item}' not found, this is a bug");

				foreach (HookEx dependency in dependencies)
				{
					if (dependency is null)
						throw new Exception($"Dependency '{item}' is null, this is a bug");

					if (dependency.IsInstalled) continue;

					if (!dependency.RemovePatch())
						throw new Exception($"Dependency '{dependency}' uninstallation failed");

					RemoveSubscriber(dependency.HookName, requester);
					Logger.Log($"Uninstalled dependency '{dependency}'");
				}
			}

			dependencies = default;
		}
		catch (System.Exception e)
		{
			GetHookById(hook.Identifier).SetStatus(HookState.Failure, e);
			Logger.Error($"Uninstall hook '{hook}' failed", e);
		}
	}

	private IEnumerable<HookEx> GetHookByName(string name)
		=> DynamicHooks.Where(x => x.HookName == name) ?? null;

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


	internal IEnumerable<HookEx> LoadedStaticHooks
	{ get => StaticHooks.Where(x => x.IsLoaded); }

	internal IEnumerable<HookEx> LoadedDynamicHooks
	{ get => DynamicHooks.Where(x => x.IsLoaded); }

	internal IEnumerable<HookEx> InstalledStaticHooks
	{ get => StaticHooks.Where(x => x.IsInstalled); }

	internal IEnumerable<HookEx> InstalledDynamicHooks
	{ get => DynamicHooks.Where(x => x.IsInstalled); }


	internal bool HookIsSubscribedBy(string hookName, string subscriber)
		=> _subscribers?.Where(x => x.HookName == hookName).Any(x => x.Subscriber == subscriber) ?? false;

	internal bool HookHasSubscribers(string hookName)
		=> _subscribers?.Any(x => x.HookName == hookName) ?? false;

	internal int GetHookSubscriberCount(string hookName)
		=> _subscribers.Where(x => x.HookName == hookName).ToList().Count;


	internal void AddSubscriber(string hookName, string subscriber)
		=> _subscribers.Add(item: new Subscription { HookName = hookName, Subscriber = subscriber });

	internal void RemoveSubscriber(string hookName, string subscriber)
		=> _subscribers.RemoveAll(x => x.HookName == hookName && x.Subscriber == subscriber);

}
