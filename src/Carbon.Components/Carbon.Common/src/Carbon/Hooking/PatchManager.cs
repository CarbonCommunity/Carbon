using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Carbon.Events;
using Carbon.Hooks;
using Carbon.Core;
using Carbon.Extensions;
using Carbon.Pooling;
using Network;

namespace Carbon.Hooks;
#pragma warning disable IDE0051

/// <summary>
/// Loads hook assemblies, and applies or removes each hook's Harmony patch as plugins subscribe and unsubscribe.
/// </summary>
public class PatchManager : FacepunchBehaviour, IDisposable
{
	internal List<HookEx> _patches { get; set; }
	internal List<HookEx> _staticHooks { get; set; }
	internal List<HookEx> _dynamicHooks { get; set; }
	internal List<HookEx> _metadataHooks { get; set; }

	// TODO --------------------------------------------------------------------
	// Allows patch uninstallation on the correct order and should be replaced
	// with Harmony After/Before mechanics. To be able to do that we need to
	// modify the autogen tool to take into consideration the offset created by
	// the first applied patch when dealing with the InjectionIndex of the
	// second patch.
	internal List<HookEx> _installed { get; set; }

#if DEBUG
	// Number of patches applied by update cycle
	private readonly int PatchLimitPerCycle = 50;
#else
	// Number of patches applied by update cycle
	private readonly int PatchLimitPerCycle = 25;
#endif

	private Stopwatch sw;
	private bool _doReload;
	private Queue<string> _workQueue;
	private readonly HashSet<string> _workQueueSet = new();
	private List<Subscription> _subscribers;
	private static bool FullFramePatch;
	private static bool InitialOnEnable;
	private static string _rustAssemblyChecksum;

	private readonly Dictionary<string, HookEx> _hooksById = new();
	private readonly Dictionary<string, List<HookEx>> _hooksByName = new();
	private readonly Dictionary<string, List<HookEx>> _hooksByFullName = new();
	private readonly Dictionary<string, int> _subscriberCounts = new();
	private readonly HashSet<(string identifier, string subscriber)> _subscriberPairs = new();

	public void Enqueue(string identifier)
	{
		if (!_workQueueSet.Add(identifier))
		{
			return;
		}

		_workQueue.Enqueue(identifier);
	}

	private string DequeueWork()
	{
		var identifier = _workQueue.Dequeue();
		_workQueueSet.Remove(identifier);
		return identifier;
	}

	private static string GetRustAssemblyChecksum()
	{
		return _rustAssemblyChecksum ??= BitConverter.ToUInt32(new MD5CryptoServiceProvider().ComputeHash(File.ReadAllBytes(typeof(ServerMgr).Assembly.Location)), 0).ToString();
	}

	private void IndexHook(HookEx hook)
	{
		if (!_hooksById.ContainsKey(hook.Identifier))
		{
			_hooksById[hook.Identifier] = hook;
		}

		AddToIndex(_hooksByName, hook.HookName, hook);
		AddToIndex(_hooksByFullName, hook.HookFullName, hook);

		static void AddToIndex(Dictionary<string, List<HookEx>> index, string key, HookEx hook)
		{
			if (key == null)
			{
				return;
			}

			if (!index.TryGetValue(key, out var list))
			{
				index[key] = list = new List<HookEx>();
			}

			list.Add(hook);
		}
	}

	private void ClearHookIndices()
	{
		_hooksById.Clear();
		_hooksByName.Clear();
		_hooksByFullName.Clear();
	}

	// Patches, then dynamic, then static
	private static int GetHookCategoryOrder(HookEx hook)
	{
		return hook.IsPatch ? 0 : hook.IsStaticHook ? 2 : 1;
	}

	private static IEnumerable<HookEx> GetLoadedFromIndex(Dictionary<string, List<HookEx>> index, string key)
	{
		if (key == null || !index.TryGetValue(key, out var list))
		{
			return Enumerable.Empty<HookEx>();
		}

		return list.Where(x => x.IsLoaded).OrderBy(GetHookCategoryOrder);
	}

	/// <summary>
	/// Hook assemblies loaded from carbon/managed/hooks, in order. Packages can append their own before Carbon initializes.
	/// </summary>
	public static List<string> HookAssemblies { get; } =
	[
		"Carbon.Hooks.Base.dll",
		"Carbon.Hooks.Community.dll",
	];

	private void Awake()
	{
		if (!Community.Runtime.Config.Logging.ReducedLogging)
		{
			Logger.Log($"Initializing {this}..");
		}

		sw = new Stopwatch();

		_dynamicHooks = new List<HookEx>();
		_installed = new List<HookEx>();
		_patches = new List<HookEx>();
		_staticHooks = new List<HookEx>();
		_metadataHooks = new List<HookEx>();
		_subscribers = new List<Subscription>();
		_workQueue = new Queue<string>();

		enabled = false;

		if (Community.Runtime.Config.SelfUpdating.HookUpdates)
		{
			if (!Community.Runtime.Config.Logging.ReducedLogging)
			{
				Logger.Log("Updating hooks...");
			}

			Updater.DoUpdate(result =>
			{
				if (!result)
					Logger.Error($"Unable to update the hooks at this time, please try again later");
				enabled = true;
			});
		}
		else
		{
			Logger.Log("Hook updates disabled, loading from disk...");
			Invoke(() => enabled = true, 0f);
		}
	}

	private void OnEnable()
	{
		_patches.Clear();
		_staticHooks.Clear();
		_dynamicHooks.Clear();
		ClearHookIndices();

		foreach (string file in HookAssemblies)
		{
			//FIXMENOW
			//string path = Path.Combine(Defines.GetManagedFolder(), file);

			//if (Supervisor.ASM.IsLoaded(Path.GetFileName(path)))
			//	Supervisor.ASM.UnloadModule(path, false);

			LoadHooksFromFile(file);
		}

		if (_patches.Count > 0)
		{
			// I don't like this, patching stuff that may not be used but for the
			// sake of time I will let it go for now but this needs to be reviewed.
			for(int i = 0; i < _patches.Count; i++)
			{
				var hook = _patches[i];
				if (!hook.IsInstalled && !hook.HasDependencies())
				{
					Subscribe(hook.Identifier, "Carbon.Patch");
				}
			}
		}

		if (_staticHooks.Count > 0)
		{
			for(int i = 0; i < _staticHooks.Count; i++)
			{
				var hook = _staticHooks[i];
				if (!hook.IsInstalled)
				{
					Subscribe(hook.Identifier, "Carbon.Static");
				}
			}

			FullFramePatch = true;

			Update();
		}

		if (!InitialOnEnable)
		{
			InitialOnEnable = true;

			Invoke(() => Community.Runtime.Events.Trigger(CarbonEvent.HooksInstalled, EventArgs.Empty), 0f);
		}
	}

	private void OnDisable()
	{
		if (!Community.Runtime.Config.Logging.ReducedLogging)
		{
			Logger.Log("Stopping hook processor...");
		}

		for(int i = 0; i < _installed.Count; i++)
		{
			var item = _installed[i];
			if (!item.RemovePatch())
			{
				Logger.Warn($" Failed uninstalling patch: {item.HookFullName}[{item.Identifier}]");
				continue;
			}

			_installed.RemoveAt(i);
			i--;
		}

		if (!_doReload) return;

		if (!Community.Runtime.Config.Logging.ReducedLogging)
		{
			Logger.Log("Reloading hook processor...");
		}

		_doReload = false;
		enabled = true;
	}

	public void Fetch()
	{
		Community.Runtime.Events.Trigger(CarbonEvent.HookFetchStart, EventArgs.Empty);

		foreach (var package in ModLoader.Packages)
		{
			foreach (var plugin in package.Plugins)
			{
				foreach (var hook in plugin.Hooks)
				{
					Unsubscribe(HookStringPool.GetOrAdd(hook), plugin.FileName);
				}
			}
		}

		try
		{
			OnDisable();
		}
		catch (Exception ex)
		{
			Logger.Error($"Reinstall failed: OnDisable failed", ex);
			return;
		}

		Logger.Warn(" Re-downloading hooks...");

		Updater.DoUpdate((bool result) =>
		{
			if (!result)
			{
				Logger.Error($"Unable to update the hooks at this time, please try again later");
				return;
			}

			OnEnable();

			foreach (var package in ModLoader.Packages)
			{
				foreach (var plugin in package.Plugins)
				{
					foreach (var hook in plugin.Hooks)
					{
						var name = HookStringPool.GetOrAdd(hook);

						if (plugin.IsHookIgnored(hook))
						{
							continue;
						}

						Subscribe(name, plugin.FileName);
					}
				}
			}

			Community.Runtime.Events.Trigger(CarbonEvent.HookFetchEnd, EventArgs.Empty);
		});
	}

	private void OnDestroy()
	{
		if (!Community.Runtime.Config.Logging.ReducedLogging)
		{
			Logger.Log("Destroying hook processor...");
		}
		Dispose();
	}

	private void Update()
	{
		if (_workQueue.Count == 0)
		{
			return;
		}

		var limit = FullFramePatch ? int.MaxValue : PatchLimitPerCycle;

		while (_workQueue.Count > 0 && limit-- > 0)
		{
			string identifier = DequeueWork();

			try
			{
				HookEx hook = GetHookById(identifier);

				bool hasSubscribers = HookHasSubscribers(hook.Identifier);
				bool isInstalled = hook.IsInstalled;

				if (!isInstalled && hook.Status == HookState.Failure)
				{
					Logger.Warn($"A hook request for '{hook}' received:");
					Logger.Warn($" - The current status is FAILURE: {hook.LastError}");
					Logger.Warn($" - Check for possible errors on the log file");
				}

				switch (hasSubscribers)
				{
					// Not installed but has subs, install
					case true when !isInstalled:
						// A failed hook keeps its Failure status; keep draining the queue for the rest
						if (!hook.ApplyPatch())
						{
							continue;
						}
						_installed.Add(hook);
						break;

					// Installed but no subs found, uninstall
					case false when isInstalled:
						if (!hook.RemovePatch())
						{
							continue;
						}
						_installed.Remove(hook);

						if (hook.HasDependencies())
						{
							for (int i = 0; i < hook.Dependencies.Length; i++)
							{
								var dependantHooks = GetHookByName(hook.Dependencies[i]);
								foreach (var dependantHook in dependantHooks)
								{
									if (dependantHook != null && dependantHook.IsInstalled)
									{
										dependantHook.RemovePatch();
									}
								}
							}
						}
						break;
				}
			}
			catch (System.ApplicationException e)
			{
				Logger.Error(e.Message);
			}
			catch (System.Exception e)
			{
				Logger.Error($"HookManager.Update() failed at '{identifier}'", e);
			}
		}

		if (FullFramePatch)
		{
			FullFramePatch = false;
			Community.Runtime.Events.Trigger(CarbonEvent.HooksPatchedFullFrame, CarbonEventArgs.Empty);
		}
	}

	private void LoadHooksFromFile(string filePath)
	{
		try
		{
			// delegates asm loading to Carbon.Loader
			Assembly hooks = Community.Runtime.AssemblyEx.Hooks.Load(filePath, "HookManager.LoadHooksFromFile");

			if (hooks == null)
			{
				Logger.Error($"Error while loading hooks from '{filePath}'.");
				Logger.Error($"Either the file is corrupt or has an unsupported format/version.");
				return;
			}

			Type @base = hooks.GetType("Carbon.Hooks.Patch")
				?? typeof(Carbon.Hooks.Patch);

			Type attr = hooks.GetType("Carbon.Hooks.HookAttribute.Patch")
				?? typeof(HookAttribute.Patch);

			IEnumerable<TypeInfo> types = hooks.DefinedTypes
				.Where(type => @base.IsAssignableFrom(type) && Attribute.IsDefined(type, attr));

			TaskStatus stats = LoadHooks(types);
			if (stats.Total == 0) return;

			var fileName = Path.GetFileName(filePath);
			if (!Community.Runtime.Config.Logging.ReducedLogging)
			{
				Logger.Log($"- Loaded {stats.Total} hooks ({stats.Patch}/{stats.Static}/{stats.Dynamic}) from file '{fileName}' in {sw.ElapsedMilliseconds}ms");
			}

#if !(RUST_STAGING || RUST_RELEASE || RUST_AUX01 || RUST_AUX02 || RUST_AUX03 || RUST_AUX04 || EXPERIMENTAL)
			if (hooks.GetType("Carbon.Hooks._Meta") is Type type)
			{
				var checksum = (string)type.GetField("Checksum").GetValue(null);
				var currentChecksum = GetRustAssemblyChecksum();
				if (!checksum.Equals(currentChecksum) && !Community.Runtime.Config.Logging.ReducedLogging)
				{
					Logger.Warn($"Checksum validation failed for Rust has failed ({currentChecksum} =/ {checksum}) - [{fileName}]");
				}
			}
#endif
		}
		catch (System.Exception)
		{
			Logger.Error($"- Error while loading hooks from file '{Path.GetFileName(filePath)}'");
			return;
		}
	}

	public void LoadHooksFromType(Type type)
	{
		Type @base = typeof(Carbon.Hooks.Patch);
		Type attr = typeof(HookAttribute.Patch);

		IEnumerable<TypeInfo> types = type.GetNestedTypes()
			.Where(type => @base.IsAssignableFrom(type) && Attribute.IsDefined(type, attr))
			.Select(x => x.GetTypeInfo());

		TaskStatus stats = LoadHooks(types);
		if (stats.Total == 0) return;

		if (!Community.Runtime.Config.Logging.ReducedLogging)
		{
			Logger.Log($"- Loaded {stats.Total} hooks ({stats.Patch}/{stats.Static}/{stats.Dynamic})"
			           + $" from type '{type}' in {sw.ElapsedMilliseconds}ms");
		}
	}

	private TaskStatus LoadHooks(IEnumerable<TypeInfo> types)
	{
		TaskStatus retvar = new();
		retvar.Hooks = new();
		sw.Restart();

		foreach (TypeInfo type in types)
		{
			try
			{
				HookEx hook = new HookEx(type)
					?? throw new Exception($"Hook is null, this is a bug");

				if (hook.Options.HasFlag(HookFlags.MetadataOnly))
				{
					hook.SetStatus(HookState.Inactive);
					retvar.Metadata++;
					_metadataHooks.Add(hook);
					HookStringPool.GetOrAdd(hook.HookName);
					continue;
				}

				if (IsHookLoaded(hook))
				{
					var assembly = type.Assembly.GetName();
					Logger.Warn($" Attempted to install duplicate hook '{hook}' (from {assembly.Name})");
					continue;
				}

				if (hook.IsPatch)
				{
					retvar.Patch++;
					_patches.Add(hook);
					IndexHook(hook);
					if (!hook.HasDependencies()) Subscribe(hook.Identifier, "Carbon.Patch");
				}
				else if (hook.IsStaticHook)
				{
					retvar.Static++;
					_staticHooks.Add(hook);
					IndexHook(hook);
					Subscribe(hook.Identifier, "Carbon.Static");
				}
				else
				{
					retvar.Dynamic++;
					_dynamicHooks.Add(hook);
					IndexHook(hook);
				}

				HookStringPool.GetOrAdd(hook.HookName);
				retvar.Hooks.Add(hook);
			}
			catch (System.Exception e)
			{
				Logger.Error($"Error while parsing '{type.Name}'", e);
			}
		}

		sw.Stop();
		return retvar;
	}

	private IEnumerable<HookEx> GetHookDependencyTree(HookEx hook)
	{
		foreach (var dependency in hook.Dependencies)
		{
			foreach (var hookEx in GetHookByFullName(dependency))
			{
				foreach (var tree in GetHookDependencyTree(hookEx))
				{
					yield return tree;
				}

				yield return hookEx;
			}
		}
	}

	private IEnumerable<HookEx> GetHookDependantTree(HookEx hook)
	{
		foreach (var hookEx in _patches.Where(x => Array.IndexOf(x.Dependencies, hook.HookFullName) >= 0))
		{
			foreach (var tree in GetHookDependantTree(hookEx))
			{
				yield return tree;
			}

			yield return hookEx;
		}
	}

	private IEnumerable<HookEx> Hooks => _patches.Concat(_dynamicHooks).Concat(_staticHooks).Concat(_metadataHooks);

	private IEnumerable<HookEx> GetHookByName(string name) => GetLoadedFromIndex(_hooksByName, name);

	private IEnumerable<HookEx> GetHookByFullName(string name) => GetLoadedFromIndex(_hooksByFullName, name);

	private HookEx GetHookById(string identifier) => identifier != null && _hooksById.TryGetValue(identifier, out var hook) && hook.IsLoaded ? hook : null;

	private IEnumerable<HookEx> GetHookByNameAll(string name) => Hooks.Where(x => x.HookName.Equals(name)) ?? null;

	private IEnumerable<HookEx> GetHookByFullNameAll(string name) => Hooks.Where(x => x.HookFullName.Equals(name)) ?? null;

	private HookEx GetHookByIdAll(string identifier) => Hooks.FirstOrDefault(x => x.Identifier.Equals(identifier)) ?? null;

	internal bool IsHookLoaded(HookEx hook) => GetHookByFullName(hook.HookFullName).Any(x => x.HookFullName.Equals(hook.HookFullName) && x.TargetType == hook.TargetType && x.TargetMethod == hook.TargetMethod && (x.TargetMethodArgs?.SequenceEqual(hook.TargetMethodArgs) ?? true));

	public bool IsHook(string hookName) => GetHookByNameAll(hookName).Any();

	public bool IsHookLoaded(string hookName) => GetHookByName(hookName).Any(IsHookLoaded);

	public void ForceUpdateHooks()
	{
		FullFramePatch = true;
		Update();
	}

	public IEnumerable<HookEx> LoadedPatches
	{ get => _patches; }

	public IEnumerable<HookEx> InstalledPatches
	{ get => _patches.Where(x => x.IsInstalled); }

	public IEnumerable<HookEx> LoadedStaticHooks
	{ get => _staticHooks; }

	public IEnumerable<HookEx> InstalledStaticHooks
	{ get => _staticHooks.Where(x => x.IsInstalled); }

	public IEnumerable<HookEx> LoadedDynamicHooks
	{ get => _dynamicHooks; }

	public IEnumerable<HookEx> InstalledDynamicHooks
	{ get => _dynamicHooks.Where(x => x.IsInstalled); }

	private bool HookIsSubscribedBy(string hookName, string subscriber)
		=> _subscribers != null && _subscriberPairs.Contains((hookName, subscriber));

	private bool HookHasSubscribers(string hookName)
		=> _subscribers != null && hookName != null && _subscriberCounts.TryGetValue(hookName, out var count) && count > 0;

	public int GetHookSubscriberCount(string hookName) => hookName != null && _subscriberCounts.TryGetValue(hookName, out var count) ? count : 0;

	private void AddSubscriber(string hookName, string subscriber)
	{
		_subscribers.Add(item: new Subscription { Identifier = hookName, Subscriber = subscriber });
		_subscriberPairs.Add((hookName, subscriber));
		_subscriberCounts[hookName] = GetHookSubscriberCount(hookName) + 1;
	}

	private void RemoveSubscriber(string hookName, string subscriber)
	{
		var removed = _subscribers.RemoveAll(x => x.Identifier.Equals(hookName) && x.Subscriber.Equals(subscriber));
		_subscriberPairs.Remove((hookName, subscriber));

		if (removed == 0)
		{
			return;
		}

		var remaining = GetHookSubscriberCount(hookName) - removed;
		if (remaining > 0)
		{
			_subscriberCounts[hookName] = remaining;
		}
		else
		{
			_subscriberCounts.Remove(hookName);
		}
	}


	public void Subscribe(string hookName, string requester)
	{
		try
		{
			var hook = GetHookById(hookName);

			if (hook != null && !HookIsSubscribedBy(hook.Identifier, requester))
			{
				Subscribe(hook, requester);
				return;
			}

			var hooks = GetHookByName(hookName);

			if (!hooks.Any())
			{
				return;
			}

			foreach (var item in hooks.Where(hook => !HookIsSubscribedBy(hook.Identifier, requester)))
			{
				Subscribe(item, requester);
			}

			hooks = default;
		}
		catch (Exception e)
		{
			Logger.Error($"Error while subscribing hook '{hookName}'", e);
		}
	}

	private void Subscribe(HookEx hook, string requester)
	{
		try
		{
			foreach (HookEx dependency in GetHookDependencyTree(hook))
			{
				AddSubscriber(dependency.Identifier, requester);
				Enqueue(dependency.Identifier);
			}

			AddSubscriber(hook.Identifier, requester);
			Enqueue(hook.Identifier);

			foreach (HookEx dependant in GetHookDependantTree(hook))
			{
				AddSubscriber(dependant.Identifier, requester);
				Enqueue(dependant.Identifier);
			}
		}
		catch (Exception e)
		{
			Logger.Error($"Error while subscribing hook '{hook}'", e);
		}
	}

	public IEnumerable<string> GetHookSubscribers(string hookName)
	{
		return _subscribers.Where(x => x.Identifier.Equals(hookName)).Select(x => x.Subscriber);
	}

	public void Unsubscribe(string hookName, string requester)
	{
		try
		{
			HookEx single = GetHookById(hookName);

			if (single != null && !HookIsSubscribedBy(single.Identifier, requester))
			{
				Unsubscribe(single, requester);
				return;
			}

			IEnumerable<HookEx> hooks = GetHookByName(hookName);

			if (!hooks.Any())
			{
				return;
			};

			foreach (HookEx hook in hooks.Where(hook => HookIsSubscribedBy(hook.Identifier, requester)).Reverse())
			{
				Unsubscribe(hook, requester);
			}
		}
		catch (Exception e)
		{
			Logger.Error($"Error while unsubscribing hook '{hookName}'", e);
		}
	}

	private void Unsubscribe(HookEx hook, string requester)
	{
		try
		{
			foreach (HookEx dependant in GetHookDependantTree(hook))
			{
				RemoveSubscriber(dependant.Identifier, requester);
				Enqueue(dependant.Identifier);
			}

			RemoveSubscriber(hook.Identifier, requester);
			Enqueue(hook.Identifier);

			foreach (HookEx dependency in GetHookDependencyTree(hook))
			{
				RemoveSubscriber(dependency.Identifier, requester);
				Enqueue(dependency.Identifier);
			}
		}
		catch (Exception e)
		{
			Logger.Error($"Error while unsubscribing hook '{hook}'", e);
		}
	}

	public void UnsubscribeAll(string hookName)
	{
		var subscribers = GetHookSubscribers(hookName);

		foreach (var subscriber in subscribers)
		{
			Unsubscribe(hookName, subscriber);
		}
	}

	public bool AnySubscribers(string hookName) => HookHasSubscribers(hookName);

	public static string GetMethodMSILHash(MethodInfo method)
		=> SHA1(method?.GetMethodBody()?.GetILAsByteArray());

	public static string SHA1(byte[] raw)
	{
		if (raw == null || raw.Length == 0) return null;
		using SHA1Managed sha1 = new SHA1Managed();
		byte[] bytes = sha1.ComputeHash(raw);
		return string.Concat(bytes.Select(b => b.ToString("x2"))).ToLower();
	}

	private bool _disposing;

	internal void Dispose(bool disposing)
	{
		if (!_disposing)
		{
			if (disposing)
			{
				foreach (HookEx item in _dynamicHooks) item.Dispose();
				_dynamicHooks = default;

				foreach (HookEx item in _installed) item.Dispose();
				_installed = default;

				foreach (HookEx item in _patches) item.Dispose();
				_patches = default;

				foreach (HookEx item in _staticHooks) item.Dispose();
				_staticHooks = default;

				_workQueue = default;
				_workQueueSet.Clear();
				_subscribers = default;
				_subscriberPairs.Clear();
				_subscriberCounts.Clear();
				ClearHookIndices();
			}

			// unmanaged resources
			_disposing = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
