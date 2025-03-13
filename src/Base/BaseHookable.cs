using System.Diagnostics;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace Carbon.Base;

public class BaseHookable
{
	public List<uint> Hooks;
	public List<HookMethodAttribute> HookMethods;
	public List<PluginReferenceAttribute> PluginReferences;

	public HookCachePool HookPool = new();
	public List<uint> IgnoredHooks = new();

	public class HookCachePool : Dictionary<uint, CachedHookInstance>
	{
		public void Reset()
		{
			foreach (var hook in Values.SelectMany(value => value.Hooks))
			{
				hook.Reset();
			}
		}

		public void EnableDebugging(bool wants)
		{
			foreach (var hook in Values.SelectMany(value => value.Hooks))
			{
				hook.EnableDebugging(wants);
			}
		}
	}
	public class CachedHookInstance
	{
		public CachedHook PrimaryHook;
		public List<CachedHook> Hooks;

		public bool IsValid() => Hooks != null && Hooks.Count > 0;
		public void RefreshPrimary()
		{
			PrimaryHook = Hooks.OrderByDescending(x => x.Parameters.Length).FirstOrDefault();
		}
	}
	public class CachedHook
	{
		public string Name;
		public uint Id;
		public BaseHookable Hookable;
		public string HookableName;
		public MethodInfo Method;
		public Type[] Parameters;
		public ParameterInfo[] InfoParameters;
		public bool IsByRef;
		public bool IsAsync;
		public bool IsDebugged;

		public int Exceptions;
		public int LagSpikes;
		public int TimesFired;
		public TimeSpan HookTime;
		public double MemoryUsage;

		public void EnableDebugging(bool wants)
		{
			IsDebugged = wants;
		}

		public void Reset()
		{
			Exceptions = 0;
			LagSpikes = 0;
			TimesFired = 0;
			HookTime = default;
			MemoryUsage = 0;
		}

		public void OnFired(BaseHookable hookable, TimeSpan hookTime, double memoryUsed)
		{
			hookable.TotalHookTime += hookTime;
			hookable.TotalMemoryUsed += memoryUsed;
			hookable.TotalHookFires++;

			HookTime += hookTime;
			MemoryUsage += memoryUsed;

			Interlocked.Increment(ref TimesFired);

			if (IsDebugged)
			{
				Logger.Log($" {Name}[{Id}] fired on {HookableName} {Hookable.ToPrettyString()} [{TimesFired:n0}|{HookTime.TotalMilliseconds:0}ms|{ByteEx.Format(MemoryUsage, shortName: true, stringFormat: "{0}{1}").ToLower()}]");
			}
		}
		public void OnLagSpike(BaseHookable hookable)
		{
			hookable.TotalHookLagSpikes++;
			Interlocked.Increment(ref LagSpikes);
		}
		public void OnException()
		{
			Interlocked.Increment(ref Exceptions);
		}

		public static CachedHook Make(string hookName, uint hookId, BaseHookable hookable, MethodInfo method)
		{
			var parameters = method.GetParameters();
			var hook = new CachedHook
			{
				Name = hookName,
				Id = hookId,
				Hookable = hookable,
				HookableName = hookable is BaseModule ? "module" : "plugin",
				Method = method,
				IsByRef = parameters.Any(x => x.ParameterType.IsByRef),
				IsAsync = method.ReturnType?.GetMethod("GetAwaiter") != null ||
						  method.GetCustomAttribute<AsyncStateMachineAttribute>() != null,
				Parameters = parameters.Select(x => x.ParameterType).ToArray(),
				InfoParameters = parameters,
#if DEBUG
				IsDebugged = CorePlugin.EnforceHookDebugging,
#endif
			};

			return hook;
		}
	}

	public virtual bool ManualSubscriptions => false;

	[JsonProperty]
	public string Name { get; set; }

	[JsonProperty]
	public virtual VersionNumber Version { get; set; }

	[JsonProperty] public TimeSpan TotalHookTime;
	[JsonProperty] public int TotalHookFires;
	[JsonProperty] public double TotalMemoryUsed;
	[JsonProperty] public int TotalHookLagSpikes;
	[JsonProperty] public int TotalHookExceptions;

	[JsonProperty]
	public double Uptime => _initializationTime.GetValueOrDefault();

	public bool HasBuiltHookCache { get; internal set; }
	public bool HasInitialized { get; internal set; }
	public Type HookableType { get; internal set; }
	public bool InternalCallHookOverriden { get; internal set; } = true;

	#region Tracking

	internal Stopwatch _trackStopwatch = new();
	internal int _currentGcCount;
	internal TimeSince? _initializationTime;

	public TimeSpan CurrentHookTime { get; internal set; }
	public static long CurrentMemory => GC.GetTotalMemory(false);
	public static int CurrentGcCount => GC.CollectionCount(0);
	public bool HasGCCollected => _currentGcCount != CurrentGcCount;

	public virtual void TrackInit()
	{
		if (_initializationTime == null)
		{
			_initializationTime = 0;
		}
	}
	public virtual void TrackStart()
	{
		if (!Community.IsServerInitialized || _trackStopwatch.IsRunning)
		{
			return;
		}

		_trackStopwatch.Start();
		_currentGcCount = CurrentGcCount;
	}
	public virtual void TrackEnd()
	{
		if (!Community.IsServerInitialized || !_trackStopwatch.IsRunning)
		{
			return;
		}

		CurrentHookTime = _trackStopwatch.Elapsed;

#if DEBUG
		// if (Community.Runtime.Config.PluginTrackingTime != 0)
		// {
		// 	HookTimeAverage?.Increment(timeElapsed);
		// 	MemoryAverage?.Increment(memoryUsed);
		// }
#endif

		_trackStopwatch.Reset();
	}

	protected void OnException(uint hook)
	{
		Interlocked.Increment(ref TotalHookExceptions);

		var overrides = HookPool[hook].Hooks;
		foreach (var element in overrides)
		{
			element.OnException();
		}
	}

	#endregion

	#region AutoPatch

	private HarmonyLib.Harmony _harmonyInstanceCache;
	protected string HarmonyId => $"com.carbon.{Name}";
	protected HarmonyLib.Harmony HarmonyInstance => _harmonyInstanceCache ??= new HarmonyLib.Harmony(HarmonyId);

	public bool IProcessPatches()
	{
		if (_harmonyInstanceCache != null)
		{
			return false;
		}

		var types = HookableType.GetNestedTypes(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
		foreach (var type in types)
		{
			var attribute = type.GetCustomAttribute<AutoPatchAttribute>(false);

			if (attribute == null)
			{
				continue;
			}

			try
			{
				var harmonyMethods = HarmonyInstance.CreateClassProcessor(type)?.Patch();

				if (harmonyMethods == null || harmonyMethods.Count == 0)
				{
					Logger.Warn($"[{Name}] AutoPatch attribute found on '{type.Name}' but no HarmonyPatch methods found. Skipping..");
					continue;
				}

				foreach (MethodInfo method in harmonyMethods)
				{
					Logger.Log($"[{Name}] Automatically Harmony patched '{method.Name}' ({type.Name}) method.");
				}
			}
			catch (Exception ex)
			{
				Logger.Error($"[{Name}] Failed to automatically Harmony patch '{type.Name}'", ex);

				if (attribute.IsRequired)
				{
					return false;
				}
			}
		}
		return true;
	}
	public void IProcessUnpatches(bool silent = true)
	{
		try
		{
			var count = _harmonyInstanceCache == null ? 0 : _harmonyInstanceCache.GetPatchedMethods().Count();
			_harmonyInstanceCache?.UnpatchAll(HarmonyId);
			_harmonyInstanceCache = null;
			if (!silent && count > 0)
			{
				Logger.Log($"[{Name}] Automatically Harmony unpatched {count:n0} {count.Plural("method", "methods")}.");
			}
		}
		catch (Exception ex)
		{
			Logger.Error($"[{Name}] Failed auto unpatching {HarmonyId}", ex);
		}
	}

	#endregion

	public virtual async ValueTask OnAsyncServerShutdown()
	{
		await Task.CompletedTask;
	}

	public void BuildHookCache(BindingFlags flag)
	{
		if (HasBuiltHookCache)
		{
			return;
		}

		var hooksPresent = Hooks.Count != 0;

		HookPool.Clear();

		var methods = HookableType.GetMethods(flag);

		foreach (var method in methods)
		{
			var id = HookStringPool.GetOrAdd(method.Name);

			if (!hooksPresent)
			{
				if (Community.Runtime.HookManager.IsHook(method.Name) && !Hooks.Contains(id))
				{
					Hooks.Add(id);
				}
			}

			if (!HookPool.TryGetValue(id, out var instance))
			{
				instance = new();
				instance.Hooks = new(5);

				HookPool.Add(id, instance);
			}

			instance.Hooks.Add(CachedHook.Make(method.Name, id, this, method));
			instance.RefreshPrimary();

			InternalHooks.Handle(method.Name, true);
		}

		var methodAttributes = HookableType.GetMethods(flag | BindingFlags.Public);

		foreach (var method in methodAttributes)
		{
			var methodAttribute = method.GetCustomAttribute<HookMethodAttribute>();

			if (methodAttribute == null)
			{
				continue;
			}

			var id = HookStringPool.GetOrAdd(string.IsNullOrEmpty(methodAttribute.Name) ? method.Name : methodAttribute.Name);

			if (!HookPool.TryGetValue(id, out var instance))
			{
				instance = new();
				instance.Hooks = new(5);

				HookPool.Add(id, instance);
			}

			if(instance.Hooks.Any(x => x.Method == method))
			{
				continue;
			}

			instance.Hooks.Add(CachedHook.Make(method.Name, id, this, method));
			instance.RefreshPrimary();
		}

		HasBuiltHookCache = true;
		Logger.Debug(Name, $"Built hook cache", 2);

		InternalCallHook(0, null);
	}
	public virtual object InternalCallHook(uint hook, object[] args)
	{
		InternalCallHookOverriden = false;
		return null;
	}

	public void Subscribe(string hook)
	{
		if (IgnoredHooks == null)
		{
			return;
		}

		var hash = HookStringPool.GetOrAdd(hook);

		if (!IgnoredHooks.Contains(hash))
		{
			return;
		}

		Community.Runtime.HookManager.Subscribe(hook, Name);
		IgnoredHooks.Remove(hash);
	}
	public void Unsubscribe(string hook)
	{
		if (IgnoredHooks == null)
		{
			return;
		}

		var hash = HookStringPool.GetOrAdd(hook);

		if (IgnoredHooks.Contains(hash))
		{
			return;
		}

		Community.Runtime.HookManager.Unsubscribe(hook, Name);
		IgnoredHooks.Add(hash);
	}
	public bool IsHookIgnored(uint hook)
	{
		return IgnoredHooks != null && IgnoredHooks.Contains(hook);
	}

	public void SubscribeAll(Func<string, bool> condition = null)
	{
		foreach (var name in Hooks.Select(hook => HookStringPool.GetOrAdd(hook)).Where(name => condition == null || condition(name)))
		{
			Subscribe(name);
		}
	}
	public void UnsubscribeAll(Func<string, bool> condition = null)
	{
		foreach (var name in Hooks.Select(hook => HookStringPool.GetOrAdd(hook)).Where(name => condition == null || condition(name)))
		{
			Unsubscribe(name);
		}
	}

	public T To<T>()
	{
		if (this is T result)
		{
			return result;
		}

		return default;
	}

	public override string ToString()
	{
		return GetType().FullName;
	}

	public virtual string ToPrettyString()
	{
		return $"{Name} v{Version}";
	}
}
