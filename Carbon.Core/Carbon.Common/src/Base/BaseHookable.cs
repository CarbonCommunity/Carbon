using System.Diagnostics;
using Facepunch.Extend;
using Newtonsoft.Json;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Base;

public class BaseHookable
{
	public Dictionary<uint, Priorities> Hooks;
	public List<HookMethodAttribute> HookMethods;
	public List<PluginReferenceAttribute> PluginReferences;

	public Dictionary<uint, List<CachedHook>> HookCache = new();
	public Dictionary<uint, List<CachedHook>> HookMethodAttributeCache = new();
	public HashSet<uint> IgnoredHooks = new();

	public struct CachedHook
	{
		public MethodInfo Method;
		public Type[] Parameters;
		public Delegate Delegate;
		public Priorities Priority;
		public bool IsByRef;

		public static CachedHook Make(MethodInfo method, Priorities priority, object context)
		{
			var parameters = method.GetParameters();
			var isByRef = parameters.Any(x => x.ParameterType.IsByRef);
			var hook = new CachedHook
			{
				Method = method,
				Delegate = isByRef ? null : HookCallerCommon.CreateDelegate(method, context),
				Priority = priority,
				IsByRef = isByRef,
				Parameters = parameters.Select(x => x.ParameterType).ToArray(),
			};

			return hook;
		}
	}

	[JsonProperty]
	public string Name { get; set; }

	[JsonProperty]
	public virtual VersionNumber Version { get; set; }

	[JsonProperty]
	public double TotalHookTime { get; internal set; }

	[JsonProperty]
	public double TotalMemoryUsed { get; internal set; }

	public bool HasInitialized;
	public Type Type;
	public bool InternalCallHookOverriden = true;

	#region Tracking

	internal Stopwatch _trackStopwatch = new();
	internal long _currentMemory;
	internal int _currentGcCount;

	public static long CurrentMemory => GC.GetTotalMemory(false);
	public static int CurrentGcCount => GC.CollectionCount(0);
	public bool HasGCCollected => _currentGcCount != CurrentGcCount;

	public virtual void TrackStart()
	{
		if (!Community.IsServerFullyInitializedCache)
		{
			return;
		}

		var stopwatch = _trackStopwatch;
		if (stopwatch.IsRunning)
		{
			return;
		}
		stopwatch.Start();
		_currentMemory = CurrentMemory;
		_currentGcCount = CurrentGcCount;
	}
	public virtual void TrackEnd()
	{
		if (!Community.IsServerFullyInitializedCache)
		{
			return;
		}

		var stopwatch = _trackStopwatch;
		if (!stopwatch.IsRunning)
		{
			return;
		}
		stopwatch.Stop();
		TotalHookTime += stopwatch.Elapsed.TotalMilliseconds;
		TotalMemoryUsed += (CurrentMemory - _currentMemory).Clamp(0, long.MaxValue);
		stopwatch.Reset();
	}

	#endregion

	public virtual object InternalCallHook(uint hook, object[] args)
	{
		InternalCallHookOverriden = false;
		return null;
	}

	public void Unsubscribe(string hook)
	{
		if (IgnoredHooks == null) return;

		var hash = HookStringPool.GetOrAdd(hook);

		if (IgnoredHooks.Contains(hash)) return;

		IgnoredHooks.Add(hash);
	}
	public void Subscribe(string hook)
	{
		if (IgnoredHooks == null) return;

		var hash = HookStringPool.GetOrAdd(hook);

		if (!IgnoredHooks.Contains(hash)) return;

		IgnoredHooks.Remove(hash);
	}
	public bool IsHookIgnored(uint hook)
	{
		return IgnoredHooks != null && IgnoredHooks.Contains(hook);
	}

	public void SubscribeAll(Func<string, bool> condition = null)
	{
		foreach (var hook in Hooks)
		{
			var name = HookStringPool.GetOrAdd(hook.Key);
			if (condition != null && !condition(name)) continue;

			Subscribe(name);
		}
	}
	public void UnsubscribeAll(Func<string, bool> condition = null)
	{
		foreach (var hook in Hooks)
		{
			var name = HookStringPool.GetOrAdd(hook.Key);
			if (condition != null && !condition(name)) continue;

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
}
