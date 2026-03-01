using System.Diagnostics;
using System.Runtime.CompilerServices;
using Carbon.Test;
using HarmonyLib;
using Newtonsoft.Json;

namespace Carbon.Base;

public class BaseHookable : Integrations.ITestable
{
	public List<uint> Hooks;
	public List<HookMethodAttribute> HookMethods;
	public List<PluginReferenceAttribute> PluginReferences;

	public HookCachePool HookPool = new();
	public HashSet<uint> IgnoredHooks = new();

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
	public int TestCount { get; internal set; }

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

	private Dictionary<AutoPatchAttribute.Orders, HarmonyLib.Harmony> _harmonyInstanceCache = new();

	protected string GetHarmonyId(AutoPatchAttribute.Orders order = AutoPatchAttribute.Orders.AfterPluginInit)
	{
		return $"com.carbon.{Name}.{order}";
	}

	protected string HarmonyId => GetHarmonyId();

	protected HarmonyLib.Harmony GetHarmonyInstance(AutoPatchAttribute.Orders order, bool createIfNotExists = false)
	{
		if (_harmonyInstanceCache.TryGetValue(order, out var instance))
		{
			return instance;
		}
		if (!createIfNotExists)
		{
			return null;
		}
		return _harmonyInstanceCache[order] = new HarmonyLib.Harmony(GetHarmonyId(order));
	}

	protected HarmonyLib.Harmony HarmonyInstance => GetHarmonyInstance(AutoPatchAttribute.Orders.AfterPluginInit, true);

	protected int RemoveHarmonyInstance(AutoPatchAttribute.Orders order, string category = null)
	{
		if (_harmonyInstanceCache.TryGetValue(order, out var instance))
		{
			var count = instance.GetPatchedMethods()?.Count();
			if (count > 0)
			{
				if (!string.IsNullOrEmpty(category))
				{
					instance.UnpatchCategory(category);
				}
				else
				{
					instance.UnpatchAll(GetHarmonyId(order));
				}
			}
			_harmonyInstanceCache.Remove(order);
			return count.GetValueOrDefault();
		}

		return 0;
	}

	public bool ApplyOrderedPatches(AutoPatchAttribute.Orders order, string category = null)
	{
		var instance = GetHarmonyInstance(order);
		if (instance != null)
		{
			return true;
		}

		instance = GetHarmonyInstance(order, true);
		var types = HookableType.GetNestedTypes(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
		foreach (var type in types)
		{
			var attribute = type.GetCustomAttribute<AutoPatchAttribute>(false);

			if (attribute == null)
			{
				continue;
			}

			if (attribute.Order != order)
			{
				continue;
			}

			var patchCategory = type.GetCustomAttribute<HarmonyPatchCategory>();

			if (patchCategory != null && patchCategory.info != null && patchCategory.info.category != category)
			{
				continue;
			}

			try
			{
				var harmonyMethods = instance.CreateClassProcessor(type)?.Patch();

				if (harmonyMethods == null || harmonyMethods.Count == 0)
				{
					if (!attribute.Silent)
					{
						Logger.Warn($"[{Name}] AutoPatch attribute found on '{type.Name}' but no HarmonyPatch methods found. Skipping.. [{order}]");
					}
					continue;
				}

				if (!attribute.Silent)
				{
					foreach (MethodInfo method in harmonyMethods)
					{
						Logger.Log($"[{Name}] Automatically Harmony patched '{method.Name}' ({type.Name}) method [{order}].");
					}
				}

				if (!string.IsNullOrEmpty(attribute.PatchSuccessCallback))
				{
					var successMethod = HookableType.GetMethod(attribute.PatchSuccessCallback);
					if (successMethod != null)
					{
						try
						{
							successMethod.Invoke(this, []);
						}
						catch (Exception ex)
						{
							Logger.Error($"[{Name}] Failed to execute successful automatic Harmony patch callback '{type.Name}': {attribute.PatchSuccessCallback} [{order}]", ex);
						}
					}
				}
			}
			catch (Exception ex)
			{
				if (!string.IsNullOrEmpty(attribute.PatchFailureCallback))
				{
					var failureMethod = HookableType.GetMethod(attribute.PatchFailureCallback);
					if (failureMethod != null)
					{
						try
						{
							failureMethod.Invoke(this, []);
						}
						catch (Exception ex2)
						{
							Logger.Error($"[{Name}] Failed to execute successful automatic Harmony patch callback '{type.Name}': {attribute.PatchSuccessCallback} [{order}]", ex2);
						}
					}
				}

				Logger.Error($"[{Name}] Failed to automatically Harmony patch '{type.Name}' [{order}]", ex);

				if (attribute.IsRequired)
				{
					return false;
				}
			}
		}
		return true;
	}

	public void UnapplyOrderedPatches(AutoPatchAttribute.Orders order, bool silent = true, string category = null)
	{
		try
		{
			var count = RemoveHarmonyInstance(order, category);
			if (!silent && count > 0)
			{
				Logger.Log($"[{Name}] Automatically Harmony unpatched {count:n0} {count.Plural("method", "methods")} [{order}].");
			}
		}
		catch (Exception ex)
		{
			Logger.Error($"[{Name}] Failed auto unpatching {GetHarmonyId(order)} [{order}]", ex);
		}
	}

	public bool ApplyDelayedPatches(string category = null)
	{
		return ApplyOrderedPatches(AutoPatchAttribute.Orders.Delayed, category);
	}

	public void UnapplyDelayedPatches(string category = null, bool silent = true)
	{
		UnapplyOrderedPatches(AutoPatchAttribute.Orders.Delayed, silent, category);
	}

	#endregion

	#region Tests

	public virtual void CollectTests(int channel = Integrations.DEFAULT_CHANNEL)
	{
		TestCount = 0;
		var parentTests = Integrations.Get(HookableType.Name, HookableType, this, channel);
		if (parentTests != null)
		{
			TestCount++;
			Integrations.EnqueueBed(parentTests);
		}

		foreach(var type in HookableType.GetNestedTypes(BindingFlags.Public))
		{
			var bed = Integrations.Get(type.Name, type, channel: channel);
			if (bed == null)
			{
				continue;
			}

			TestCount++;
			Integrations.EnqueueBed(bed);
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
