using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Oxide.Core;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Base;

public class BaseHookable
{
	public Dictionary<uint, Priorities> Hooks { get; set; }
	public List<HookMethodAttribute> HookMethods { get; set; }
	public List<PluginReferenceAttribute> PluginReferences { get; set; }

	public Dictionary<uint, List<CachedHook>> HookCache { get; set; } = new();
	public Dictionary<uint, List<CachedHook>> HookMethodAttributeCache { get; set; } = new();
	public List<uint> IgnoredHooks { get; set; } = new();

	public struct CachedHook
	{
		public MethodInfo Method;
		public Delegate Delegate;
		public Priorities Priority;
		public bool IsByRef;

		public static CachedHook Make(MethodInfo method, Priorities priority, object context)
		{
			var isByRef = method.GetParameters().Any(x => x.ParameterType.IsByRef);
			var hook = new CachedHook
			{
				Method = method,
				Delegate = isByRef ? null : HookCallerCommon.CreateDelegate(method, context),
				Priority = priority,
				IsByRef = isByRef
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

	public bool HasInitialized { get; set; }
	public Type Type { get; set; }

	#region Tracking

	internal Stopwatch _trackStopwatch = new();

	public virtual void TrackStart()
	{
		if (!Community.IsServerFullyInitialized)
		{
			return;
		}

		var stopwatch = _trackStopwatch;
		if (stopwatch.IsRunning)
		{
			return;
		}
		stopwatch.Start();
	}
	public virtual void TrackEnd()
	{
		if (!Community.IsServerFullyInitialized)
		{
			return;
		}

		var stopwatch = _trackStopwatch;
		if (!stopwatch.IsRunning)
		{
			return;
		}
		stopwatch.Stop();
		TotalHookTime += stopwatch.Elapsed.TotalSeconds;
		stopwatch.Reset();
	}

	#endregion

	public void Unsubscribe(string hook)
	{
		var hash = HookCallerCommon.StringPool.GetOrAdd(hook);

		if (IgnoredHooks.Contains(hash)) return;

		IgnoredHooks.Add(hash);
	}
	public void Subscribe(string hook)
	{
		var hash = HookCallerCommon.StringPool.GetOrAdd(hook);

		if (!IgnoredHooks.Contains(hash)) return;

		IgnoredHooks.Remove(hash);
	}
	public bool IsHookIgnored(string hook)
	{
		return IgnoredHooks == null || IgnoredHooks.Contains(HookCallerCommon.StringPool.GetOrAdd(hook));
	}

	public void SubscribeAll(Func<string, bool> condition = null)
	{
		foreach (var hook in Hooks)
		{
			var name = HookCallerCommon.StringPool.GetOrAdd(hook.Key);
			if (condition != null && !condition(name)) continue;

			Subscribe(name);
		}
	}
	public void UnsubscribeAll(Func<string, bool> condition = null)
	{
		foreach (var hook in Hooks)
		{
			var name = HookCallerCommon.StringPool.GetOrAdd(hook.Key);
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
