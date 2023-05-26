using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Carbon.Base;
using Carbon.Base.Interfaces;
using Carbon.Core;
using Carbon.Extensions;
using Facepunch;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Oxide.Plugins;
using UnityEngine;
using static Carbon.HookCallerCommon;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon;

public class HookCallerCommon
{
	public class StringPool
	{
		public static Dictionary<string, uint> HookNamePoolString = new();
		public static Dictionary<uint, string> HookNamePoolInt = new();

		public static uint GetOrAdd(string name)
		{
			if (HookNamePoolString.TryGetValue(name, out var hash))
			{
				return hash;
			}

			hash = name.ManifestHash();
			HookNamePoolString[name] = hash;
			HookNamePoolInt[hash] = name;
			return hash;
		}

		public static string GetOrAdd(uint name)
		{
			if (HookNamePoolInt.TryGetValue(name, out var hash))
			{
				return hash;
			}

			return string.Empty;
		}
	}

	public Dictionary<int, object[]> _argumentBuffer = new();
	public ConcurrentDictionary<string, int> _hookTimeBuffer = new();
	public ConcurrentDictionary<string, int> _hookTotalTimeBuffer = new();
	public Dictionary<string, DateTime> _lastDeprecatedWarningAt = new();

	public virtual void AppendHookTime(string hook, int time) { }
	public virtual void ClearHookTime(string hook) { }

	public virtual object[] AllocateBuffer(int count) => null;
	public virtual object[] RescaleBuffer(object[] oldBuffer, int newScale) => null;
	public virtual void ClearBuffer(object[] buffer) { }

	public virtual object CallHook<T>(T plugin, string hookName, BindingFlags flags, object[] args, ref Priorities priority, bool keepArgs = false) where T : BaseHookable => null;
	public virtual object CallDeprecatedHook<T>(T plugin, string oldHook, string newHook, DateTime expireDate, BindingFlags flags, object[] args, ref Priorities priority) where T : BaseHookable => null;

	public struct Conflict
	{
		public BaseHookable Hookable;
		public string Hook;
		public object Result;
		public Priorities Priority;

		public static Conflict Make(BaseHookable hookable, string hook, object result, Priorities priority) => new()
		{
			Hookable = hookable,
			Hook = hook,
			Result = result,
			Priority = priority
		};
	}

	public static Delegate CreateDelegate(MethodInfo methodInfo, object target)
	{
		Func<Type[], Type> getType;
		var isAction = methodInfo.ReturnType.Equals(typeof(void));
		var types = methodInfo.GetParameters().Select(p => p.ParameterType);

		if (isAction)
		{
			getType = Expression.GetActionType;
		}
		else
		{
			getType = Expression.GetFuncType;
			types = types.Concat(new[] { methodInfo.ReturnType });
		}

		if (methodInfo.IsStatic)
		{
			return Delegate.CreateDelegate(getType(types.ToArray()), methodInfo);
		}

		return types.Any()
			? Delegate.CreateDelegate(getType(types.ToArray()), target, methodInfo.Name)
			: Delegate.CreateDelegate(getType(Array.Empty<Type>()), target, methodInfo.Name);
	}
}

public static class HookCaller
{
	public static HookCallerCommon Caller { get; set; }

	#region Internals

	internal static List<Conflict> _conflictCache = new(10);
	internal static Priorities _priorityCatcher;
	internal static Conflict _defaultConflict = new()
	{
		Priority = Priorities.Low
	};

	internal static string _getPriorityName(Priorities priority)
	{
		switch (priority)
		{
			case Priorities.Low:
				return "lower";

			case Priorities.Normal:
				return "normal";

			case Priorities.High:
				return "higher";

			case Priorities.Highest:
				return "highest";
		}

		return "normal";
	}

	#endregion

	public static int GetHookTime(string hook)
	{
		if (!Caller._hookTimeBuffer.TryGetValue(hook, out var total))
		{
			return 0;
		}

		return total;
	}
	public static int GetHookTotalTime(string hook)
	{
		if (!Caller._hookTotalTimeBuffer.TryGetValue(hook, out var total))
		{
			return 0;
		}

		return total;
	}

	private static object CallStaticHook(string hookName, BindingFlags flag = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, object[] args = null, bool keepArgs = false)
	{
		Caller.ClearHookTime(hookName);

		var result = (object)null;
		var array = args == null || args.Length == 0 ? null : keepArgs ? args : args.ToArray();

		for (int i = 0; i < Community.Runtime.ModuleProcessor.Modules.Count; i++)
		{
			var hookable = Community.Runtime.ModuleProcessor.Modules[i];

			if (hookable is IModule modules && !modules.GetEnabled()) continue;

			var priority = (Priorities)default;
			var methodResult = Caller.CallHook(hookable, hookName, flags: flag, args: array, ref priority, keepArgs);

			if (methodResult != null)
			{
				result = methodResult;
				ResultOverride(hookable, priority);
			}
		}

		for (int i = 0; i < ModLoader.LoadedPackages.Count; i++)
		{
			var mod = ModLoader.LoadedPackages[i];

			for (int x = 0; x < mod.Plugins.Count; x++)
			{
				var plugin = mod.Plugins[x];

				try
				{
					var priority = (Priorities)default;
					var methodResult = Caller.CallHook(plugin, hookName, flags: flag, args: array, ref priority, keepArgs);

					if (methodResult != null)
					{
						result = methodResult;
						ResultOverride(plugin, priority);
					}
				}
				catch (Exception ex) { Logger.Error($"Failed to call hook '{hookName}' on plugin {plugin}", ex); }
			}
		}

		ConflictCheck();

		_conflictCache.Clear();

		if (array != null && !keepArgs) Array.Clear(array, 0, array.Length);

		void ResultOverride(BaseHookable hookable, Priorities priority)
		{
			_conflictCache.Add(Conflict.Make(hookable, hookName, result, priority));
		}
		void ConflictCheck()
		{
			var differentResults = false;

			if (_conflictCache.Count > 1)
			{
				var localResult = _conflictCache[0].Result;
				var priorityConflict = _defaultConflict;

				for(int i = 0; i < _conflictCache.Count; i++) 
				{
					var conflict = _conflictCache[i];

					if (conflict.Result?.ToString() != localResult?.ToString())
					{
						differentResults = true;
					}

					if (conflict.Priority > priorityConflict.Priority)
					{
						priorityConflict = conflict;
					}
				}

				localResult = priorityConflict.Result;
				if (differentResults && !_conflictCache.All(x => x.Priority == priorityConflict.Priority) && Community.Runtime.Config.HigherPriorityHookWarns) Carbon.Logger.Warn($"Hook conflict while calling '{hookName}', but used {priorityConflict.Hookable.Name} {priorityConflict.Hookable.Version} due to the {_getPriorityName(priorityConflict.Priority)} priority:\n  {_conflictCache.Select(x => $"{x.Hookable.Name} {x.Hookable.Version} [{x.Priority}:{x.Result}]").ToArray().ToString(", ", " and ")}");

				result = localResult;
			}
		}

		return result;
	}
	private static object CallStaticDeprecatedHook(string oldHook, string newHook, DateTime expireDate, BindingFlags flag = BindingFlags.NonPublic | BindingFlags.Static, object[] args = null)
	{
		if (expireDate < DateTime.Now)
		{
			return null;
		}

		DateTime now = DateTime.Now;

		if (!Caller._lastDeprecatedWarningAt.TryGetValue(oldHook, out DateTime lastWarningAt) || (now - lastWarningAt).TotalSeconds > 3600f)
		{
			Caller._lastDeprecatedWarningAt[oldHook] = now;

			Carbon.Logger.Warn($"A plugin is using deprecated hook '{oldHook}', which will stop working on {expireDate.ToString("D")}. Please ask the author to update to '{newHook}'");
		}

		return CallStaticHook(oldHook, flag, args);
	}

	#region Hook Overrides

	public static object CallHook(BaseHookable plugin, string hookName)
	{
		return Caller.CallHook(plugin, hookName, flags: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, null, ref _priorityCatcher);
	}
	public static T CallHook<T>(BaseHookable plugin, string hookName)
	{
		return (T)Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, null, ref _priorityCatcher);
	}
	public static T CallDeprecatedHook<T>(BaseHookable plugin, string oldHook, string newHook, DateTime expireDate)
	{
		return (T)Caller.CallDeprecatedHook(plugin, oldHook, newHook, expireDate, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, null, ref _priorityCatcher);
	}
	public static object CallHook(BaseHookable plugin, string hookName, object arg1)
	{
		var buffer = Caller.AllocateBuffer(1);
		buffer[0] = arg1;

		var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer, ref _priorityCatcher);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static T CallHook<T>(BaseHookable plugin, string hookName, object arg1)
	{
		var buffer = Caller.AllocateBuffer(1);
		buffer[0] = arg1;

		var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer, ref _priorityCatcher);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static T CallDeprecatedHook<T>(BaseHookable plugin, string oldHook, string newHook, DateTime expireDate, object arg1)
	{
		var buffer = Caller.AllocateBuffer(1);
		buffer[0] = arg1;

		var result = Caller.CallDeprecatedHook(plugin, oldHook, newHook, expireDate, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer, ref _priorityCatcher);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static object CallHook(BaseHookable plugin, string hookName, object arg1, object arg2)
	{
		var buffer = Caller.AllocateBuffer(2);
		buffer[0] = arg1;
		buffer[1] = arg2;

		var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer, ref _priorityCatcher);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static T CallHook<T>(BaseHookable plugin, string hookName, object arg1, object arg2)
	{
		var buffer = Caller.AllocateBuffer(2);
		buffer[0] = arg1;
		buffer[1] = arg2;

		var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer, ref _priorityCatcher);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static T CallDeprecatedHook<T>(BaseHookable plugin, string oldHook, string newHook, DateTime expireDate, object arg1, object arg2)
	{
		var buffer = Caller.AllocateBuffer(2);
		buffer[0] = arg1;
		buffer[1] = arg2;

		var result = Caller.CallDeprecatedHook(plugin, oldHook, newHook, expireDate, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer, ref _priorityCatcher);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static object CallHook(BaseHookable plugin, string hookName, object arg1, object arg2, object arg3)
	{
		var buffer = Caller.AllocateBuffer(3);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;

		var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer, ref _priorityCatcher);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static T CallHook<T>(BaseHookable plugin, string hookName, object arg1, object arg2, object arg3)
	{
		var buffer = Caller.AllocateBuffer(3);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;

		var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer, ref _priorityCatcher);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static T CallDeprecatedHook<T>(BaseHookable plugin, string oldHook, string newHook, DateTime expireDate, object arg1, object arg2, object arg3)
	{
		var buffer = Caller.AllocateBuffer(3);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;

		var result = Caller.CallDeprecatedHook(plugin, oldHook, newHook, expireDate, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer, ref _priorityCatcher);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static object CallHook(BaseHookable plugin, string hookName, object arg1, object arg2, object arg3, object arg4)
	{
		var buffer = Caller.AllocateBuffer(4);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;

		var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer, ref _priorityCatcher);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static T CallHook<T>(BaseHookable plugin, string hookName, object arg1, object arg2, object arg3, object arg4)
	{
		var buffer = Caller.AllocateBuffer(4);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;

		var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer, ref _priorityCatcher);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static T CallDeprecatedHook<T>(BaseHookable plugin, string oldHook, string newHook, DateTime expireDate, object arg1, object arg2, object arg3, object arg4)
	{
		var buffer = Caller.AllocateBuffer(4);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;

		var result = Caller.CallDeprecatedHook(plugin, oldHook, newHook, expireDate, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer, ref _priorityCatcher);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static object CallHook(BaseHookable plugin, string hookName, object arg1, object arg2, object arg3, object arg4, object arg5)
	{
		var buffer = Caller.AllocateBuffer(5);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;

		var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer, ref _priorityCatcher);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static T CallHook<T>(BaseHookable plugin, string hookName, object arg1, object arg2, object arg3, object arg4, object arg5)
	{
		var buffer = Caller.AllocateBuffer(5);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;

		var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer, ref _priorityCatcher);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static T CallDeprecatedHook<T>(BaseHookable plugin, string oldHook, string newHook, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5)
	{
		var buffer = Caller.AllocateBuffer(5);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;

		var result = Caller.CallDeprecatedHook(plugin, oldHook, newHook, expireDate, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer, ref _priorityCatcher);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static object CallHook(BaseHookable plugin, string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
	{
		var buffer = Caller.AllocateBuffer(6);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[5] = arg6;

		var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer, ref _priorityCatcher);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static T CallHook<T>(BaseHookable plugin, string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
	{
		var buffer = Caller.AllocateBuffer(6);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[5] = arg6;

		var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer, ref _priorityCatcher);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static T CallDeprecatedHook<T>(BaseHookable plugin, string oldHook, string newHook, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
	{
		var buffer = Caller.AllocateBuffer(6);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[5] = arg6;

		var result = Caller.CallDeprecatedHook(plugin, oldHook, newHook, expireDate, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer, ref _priorityCatcher);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static object CallHook(BaseHookable plugin, string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
	{
		var buffer = Caller.AllocateBuffer(7);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[5] = arg6;
		buffer[6] = arg7;

		var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer, ref _priorityCatcher);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static T CallHook<T>(BaseHookable plugin, string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
	{
		var buffer = Caller.AllocateBuffer(7);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[5] = arg6;
		buffer[6] = arg7;

		var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer, ref _priorityCatcher);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static T CallDeprecatedHook<T>(BaseHookable plugin, string oldHook, string newHook, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
	{
		var buffer = Caller.AllocateBuffer(7);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[6] = arg6;
		buffer[7] = arg7;

		var result = Caller.CallDeprecatedHook(plugin, oldHook, newHook, expireDate, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer, ref _priorityCatcher);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static object CallHook(BaseHookable plugin, string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
	{
		var buffer = Caller.AllocateBuffer(8);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[5] = arg6;
		buffer[6] = arg7;
		buffer[7] = arg8;

		var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer, ref _priorityCatcher);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static T CallHook<T>(BaseHookable plugin, string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
	{
		var buffer = Caller.AllocateBuffer(8);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[5] = arg6;
		buffer[6] = arg7;
		buffer[7] = arg8;

		var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer, ref _priorityCatcher);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static T CallDeprecatedHook<T>(BaseHookable plugin, string oldHook, string newHook, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
	{
		var buffer = Caller.AllocateBuffer(8);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[5] = arg6;
		buffer[6] = arg7;
		buffer[7] = arg8;

		var result = Caller.CallDeprecatedHook(plugin, oldHook, newHook, expireDate, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer, ref _priorityCatcher);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static object CallHook(BaseHookable plugin, string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
	{
		var buffer = Caller.AllocateBuffer(9);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[5] = arg6;
		buffer[6] = arg7;
		buffer[7] = arg8;
		buffer[8] = arg9;

		var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer, ref _priorityCatcher);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static T CallHook<T>(BaseHookable plugin, string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
	{
		var buffer = Caller.AllocateBuffer(9);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[5] = arg6;
		buffer[6] = arg7;
		buffer[7] = arg8;
		buffer[8] = arg9;

		var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer, ref _priorityCatcher);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static T CallDeprecatedHook<T>(BaseHookable plugin, string oldHook, string newHook, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
	{
		var buffer = Caller.AllocateBuffer(9);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[5] = arg6;
		buffer[6] = arg7;
		buffer[7] = arg8;
		buffer[8] = arg9;

		var result = Caller.CallDeprecatedHook(plugin, oldHook, newHook, expireDate, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer, ref _priorityCatcher);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static object CallHook(BaseHookable plugin, string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
	{
		var buffer = Caller.AllocateBuffer(10);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[5] = arg6;
		buffer[6] = arg7;
		buffer[7] = arg8;
		buffer[8] = arg9;
		buffer[9] = arg10;

		var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer, ref _priorityCatcher);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static T CallHook<T>(BaseHookable plugin, string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
	{
		var buffer = Caller.AllocateBuffer(10);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[5] = arg6;
		buffer[6] = arg7;
		buffer[7] = arg8;
		buffer[8] = arg9;
		buffer[9] = arg10;

		var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer, ref _priorityCatcher);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static T CallDeprecatedHook<T>(BaseHookable plugin, string oldHook, string newHook, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
	{
		var buffer = Caller.AllocateBuffer(10);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[5] = arg6;
		buffer[6] = arg7;
		buffer[7] = arg8;
		buffer[8] = arg9;
		buffer[9] = arg10;

		var result = Caller.CallDeprecatedHook(plugin, oldHook, newHook, expireDate, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer, ref _priorityCatcher);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static object CallHook(BaseHookable plugin, string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
	{
		var buffer = Caller.AllocateBuffer(11);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[5] = arg6;
		buffer[6] = arg7;
		buffer[7] = arg8;
		buffer[8] = arg9;
		buffer[9] = arg10;
		buffer[10] = arg11;

		var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer, ref _priorityCatcher);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static T CallHook<T>(BaseHookable plugin, string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
	{
		var buffer = Caller.AllocateBuffer(11);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[5] = arg6;
		buffer[6] = arg7;
		buffer[7] = arg8;
		buffer[8] = arg9;
		buffer[9] = arg10;
		buffer[10] = arg11;

		var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer, ref _priorityCatcher);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static T CallDeprecatedHook<T>(BaseHookable plugin, string oldHook, string newHook, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
	{
		var buffer = Caller.AllocateBuffer(11);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[5] = arg6;
		buffer[6] = arg7;
		buffer[7] = arg8;
		buffer[8] = arg9;
		buffer[9] = arg10;
		buffer[10] = arg11;

		var result = Caller.CallDeprecatedHook(plugin, oldHook, newHook, expireDate, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer, ref _priorityCatcher);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static object CallHook(BaseHookable plugin, string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
	{
		var buffer = Caller.AllocateBuffer(12);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[5] = arg6;
		buffer[6] = arg7;
		buffer[7] = arg8;
		buffer[8] = arg9;
		buffer[9] = arg10;
		buffer[10] = arg11;
		buffer[11] = arg12;

		var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer, ref _priorityCatcher);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static T CallHook<T>(BaseHookable plugin, string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
	{
		var buffer = Caller.AllocateBuffer(12);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[5] = arg6;
		buffer[6] = arg7;
		buffer[7] = arg8;
		buffer[8] = arg9;
		buffer[9] = arg10;
		buffer[10] = arg11;
		buffer[11] = arg12;

		var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer, ref _priorityCatcher);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static T CallDeprecatedHook<T>(BaseHookable plugin, string oldHook, string newHook, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
	{
		var buffer = Caller.AllocateBuffer(12);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[5] = arg6;
		buffer[6] = arg7;
		buffer[7] = arg8;
		buffer[8] = arg9;
		buffer[9] = arg10;
		buffer[10] = arg11;
		buffer[11] = arg12;

		var result = Caller.CallDeprecatedHook(plugin, oldHook, newHook, expireDate, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer, ref _priorityCatcher);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static object CallHook(BaseHookable plugin, string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13)
	{
		var buffer = Caller.AllocateBuffer(13);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[5] = arg6;
		buffer[6] = arg7;
		buffer[7] = arg8;
		buffer[8] = arg9;
		buffer[9] = arg10;
		buffer[10] = arg11;
		buffer[11] = arg12;
		buffer[12] = arg13;

		var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer, ref _priorityCatcher);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static T CallHook<T>(BaseHookable plugin, string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13)
	{
		var buffer = Caller.AllocateBuffer(13);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[5] = arg6;
		buffer[6] = arg7;
		buffer[7] = arg8;
		buffer[8] = arg9;
		buffer[9] = arg10;
		buffer[10] = arg11;
		buffer[11] = arg12;
		buffer[12] = arg13;

		var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer, ref _priorityCatcher);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static T CallDeprecatedHook<T>(BaseHookable plugin, string oldHook, string newHook, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13)
	{
		var buffer = Caller.AllocateBuffer(13);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[5] = arg6;
		buffer[6] = arg7;
		buffer[7] = arg8;
		buffer[8] = arg9;
		buffer[9] = arg10;
		buffer[10] = arg11;
		buffer[11] = arg12;
		buffer[12] = arg13;
		buffer[12] = arg13;

		var result = Caller.CallDeprecatedHook(plugin, oldHook, newHook, expireDate, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer, ref _priorityCatcher);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}

	#endregion

	#region Static Hook Overrides

	public static object CallStaticHook(string hookName)
	{
		return CallStaticHook(hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, null);
	}
	public static object CallStaticDeprecatedHook(string oldHook, string newHook, DateTime expireDate)
	{
		return CallStaticDeprecatedHook(oldHook, newHook, expireDate, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, null);
	}
	public static object CallStaticHook(string hookName, object arg1)
	{
		var buffer = Caller.AllocateBuffer(1);
		buffer[0] = arg1;

		var result = CallStaticHook(hookName, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticDeprecatedHook(string oldHook, string newHook, DateTime expireDate, object arg1)
	{
		var buffer = Caller.AllocateBuffer(1);
		buffer[0] = arg1;

		var result = CallStaticDeprecatedHook(oldHook, newHook, expireDate, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticHook(string hookName, object arg1, object arg2)
	{
		var buffer = Caller.AllocateBuffer(2);
		buffer[0] = arg1;
		buffer[1] = arg2;

		var result = CallStaticHook(hookName, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticDeprecatedHook(string oldHook, string newHook, DateTime expireDate, object arg1, object arg2)
	{
		var buffer = Caller.AllocateBuffer(2);
		buffer[0] = arg1;
		buffer[1] = arg2;

		var result = CallStaticDeprecatedHook(oldHook, newHook, expireDate, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticHook(string hookName, object arg1, object arg2, object arg3)
	{
		var buffer = Caller.AllocateBuffer(3);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;

		var result = CallStaticHook(hookName, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticDeprecatedHook(string oldHook, string newHook, DateTime expireDate, object arg1, object arg2, object arg3)
	{
		var buffer = Caller.AllocateBuffer(3);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;

		var result = CallStaticDeprecatedHook(oldHook, newHook, expireDate, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticHook(string hookName, object arg1, object arg2, object arg3, object arg4)
	{
		var buffer = Caller.AllocateBuffer(4);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;

		var result = CallStaticHook(hookName, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticDeprecatedHook(string oldHook, string newHook, DateTime expireDate, object arg1, object arg2, object arg3, object arg4)
	{
		var buffer = Caller.AllocateBuffer(4);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;

		var result = CallStaticDeprecatedHook(oldHook, newHook, expireDate, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticHook(string hookName, object arg1, object arg2, object arg3, object arg4, object arg5)
	{
		var buffer = Caller.AllocateBuffer(5);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;

		var result = CallStaticHook(hookName, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticDeprecatedHook(string oldHook, string newHook, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5)
	{
		var buffer = Caller.AllocateBuffer(5);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;

		var result = CallStaticDeprecatedHook(oldHook, newHook, expireDate, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticHook(string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
	{
		var buffer = Caller.AllocateBuffer(6);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[5] = arg6;

		var result = CallStaticHook(hookName, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticDeprecatedHook(string oldHook, string newHook, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
	{
		var buffer = Caller.AllocateBuffer(6);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[5] = arg6;

		var result = CallStaticDeprecatedHook(oldHook, newHook, expireDate, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticHook(string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
	{
		var buffer = Caller.AllocateBuffer(7);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[5] = arg6;
		buffer[6] = arg7;

		var result = CallStaticHook(hookName, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticDeprecatedHook(string oldHook, string newHook, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
	{
		var buffer = Caller.AllocateBuffer(7);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[5] = arg6;
		buffer[6] = arg7;

		var result = CallStaticDeprecatedHook(oldHook, newHook, expireDate, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticHook(string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
	{
		var buffer = Caller.AllocateBuffer(8);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[5] = arg6;
		buffer[6] = arg7;
		buffer[7] = arg8;

		var result = CallStaticHook(hookName, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticDeprecatedHook(string oldHook, string newHook, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
	{
		var buffer = Caller.AllocateBuffer(8);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[5] = arg6;
		buffer[6] = arg7;
		buffer[7] = arg8;

		var result = CallStaticDeprecatedHook(oldHook, newHook, expireDate, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticHook(string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
	{
		var buffer = Caller.AllocateBuffer(9);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[5] = arg6;
		buffer[6] = arg7;
		buffer[7] = arg8;
		buffer[8] = arg9;

		var result = CallStaticHook(hookName, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticDeprecatedHook(string oldHook, string newHook, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
	{
		var buffer = Caller.AllocateBuffer(9);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[5] = arg6;
		buffer[6] = arg7;
		buffer[7] = arg8;
		buffer[8] = arg9;

		var result = CallStaticDeprecatedHook(oldHook, newHook, expireDate, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticHook(string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
	{
		var buffer = Caller.AllocateBuffer(10);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[5] = arg6;
		buffer[6] = arg7;
		buffer[7] = arg8;
		buffer[8] = arg9;
		buffer[9] = arg10;

		var result = CallStaticHook(hookName, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticDeprecatedHook(string oldHook, string newHook, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
	{
		var buffer = Caller.AllocateBuffer(10);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[5] = arg6;
		buffer[6] = arg7;
		buffer[7] = arg8;
		buffer[8] = arg9;
		buffer[9] = arg10;

		var result = CallStaticDeprecatedHook(oldHook, newHook, expireDate, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticHook(string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
	{
		var buffer = Caller.AllocateBuffer(11);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[5] = arg6;
		buffer[6] = arg7;
		buffer[7] = arg8;
		buffer[8] = arg9;
		buffer[9] = arg10;
		buffer[10] = arg11;

		var result = CallStaticHook(hookName, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticDeprecatedHook(string oldHook, string newHook, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
	{
		var buffer = Caller.AllocateBuffer(11);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[5] = arg6;
		buffer[6] = arg7;
		buffer[7] = arg8;
		buffer[8] = arg9;
		buffer[9] = arg10;
		buffer[10] = arg11;

		var result = CallStaticDeprecatedHook(oldHook, newHook, expireDate, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticHook(string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
	{
		var buffer = Caller.AllocateBuffer(12);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[5] = arg6;
		buffer[6] = arg7;
		buffer[7] = arg8;
		buffer[8] = arg9;
		buffer[9] = arg10;
		buffer[10] = arg11;
		buffer[11] = arg12;

		var result = CallStaticHook(hookName, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticDeprecatedHook(string oldHook, string newHook, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
	{
		var buffer = Caller.AllocateBuffer(12);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[5] = arg6;
		buffer[6] = arg7;
		buffer[7] = arg8;
		buffer[8] = arg9;
		buffer[9] = arg10;
		buffer[10] = arg11;
		buffer[11] = arg12;

		var result = CallStaticDeprecatedHook(oldHook, newHook, expireDate, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticHook(string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13)
	{
		var buffer = Caller.AllocateBuffer(13);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[5] = arg6;
		buffer[6] = arg7;
		buffer[7] = arg8;
		buffer[8] = arg9;
		buffer[9] = arg10;
		buffer[10] = arg11;
		buffer[11] = arg12;
		buffer[12] = arg13;

		var result = CallStaticHook(hookName, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticDeprecatedHook(string oldHook, string newHook, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13)
	{
		var buffer = Caller.AllocateBuffer(13);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[5] = arg6;
		buffer[6] = arg7;
		buffer[7] = arg8;
		buffer[8] = arg9;
		buffer[9] = arg10;
		buffer[10] = arg11;
		buffer[11] = arg12;
		buffer[12] = arg13;

		var result = CallStaticDeprecatedHook(oldHook, newHook, expireDate, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}

	public static object CallStaticHook(string hookName, object[] args, bool keepArgs = false)
	{
		return CallStaticHook(hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args, keepArgs: keepArgs);
	}
	public static object CallStaticDeprecatedHook(string oldHook, string newHook, DateTime expireDate, object[] args)
	{
		return CallStaticDeprecatedHook(oldHook, newHook, expireDate, args);
	}

	#endregion

	private static object CallPublicHook<T>(this T plugin, string hookName, object[] args) where T : BaseHookable
	{
		return CallHook(plugin, hookName, BindingFlags.Public | BindingFlags.Instance, args);
	}
	private static object CallPublicStaticHook(string hookName, object[] args)
	{
		return CallStaticHook(hookName, BindingFlags.Public | BindingFlags.Instance, args);
	}

	#region Generator

	public static void GenerateInternalCallHook(CompilationUnitSyntax input, out CompilationUnitSyntax output, out MethodDeclarationSyntax generatedMethod, bool publicize = true)
	{
		var methodContents = "\n\tvar result = (object)null;\n\ttry\n\t{\n\t\tswitch(hook)\n\t\t{\n";
		var @namespace = input.Members[0] as BaseNamespaceDeclarationSyntax;
		var @class = @namespace.Members[0] as ClassDeclarationSyntax;

		if (publicize)
		{
			ClassDeclarationSyntax PublicizeRecursively(ClassDeclarationSyntax @cls)
			{
				for (int i = 0; i < @cls.Modifiers.Count; i++)
				{
					var modifier = @cls.Modifiers[i];

					if (modifier.IsKind(SyntaxKind.PrivateKeyword) || modifier.IsKind(SyntaxKind.ProtectedKeyword) || modifier.IsKind(SyntaxKind.InternalKeyword))
					{
						@cls = @cls.WithModifiers(@cls.Modifiers.RemoveAt(i));
					}
				}

				if (!@cls.Modifiers.Any(x => x.IsKind(SyntaxKind.PublicKeyword)))
				{
					@cls = @cls.WithModifiers(@cls.Modifiers.Insert(0, SyntaxFactory.Token(SyntaxKind.PublicKeyword).WithTrailingTrivia(SyntaxFactory.Space)));
				}

				for (int i = 0; i < cls.Members.Count; i++)
				{
					if (cls.Members[i] is ClassDeclarationSyntax @cls2)
					{
						cls = cls.WithMembers(cls.Members.Replace(cls2, PublicizeRecursively(@cls2)));
					}
				}

				return cls;
			}

			@class = PublicizeRecursively(@class);
		}

		var methodDeclarations = @class.ChildNodes().OfType<MethodDeclarationSyntax>();
		var hookableMethods = new Dictionary<uint, List<MethodDeclarationSyntax>>();
		var privateMethods0 = methodDeclarations.Where(md => (md.Modifiers.Count == 0 || md.Modifiers.Any(SyntaxKind.PrivateKeyword) || md.Modifiers.Any(SyntaxKind.ProtectedKeyword) || md.AttributeLists.Any(x => x.Attributes.Any(y => y.Name.ToString() == "HookMethod"))) && md.TypeParameterList == null);
		var privateMethods = privateMethods0.OrderBy(x => x.Identifier.ValueText);
		privateMethods0 = null;

		foreach (var method in privateMethods)
		{
			var methodName = method.Identifier.ValueText; 
			var id = HookCallerCommon.StringPool.GetOrAdd(methodName);

			if (!hookableMethods.TryGetValue(id, out var list))
			{
				hookableMethods[id] = list = new();
			}

			list.Add(method);
		}

		foreach (var group in hookableMethods)
		{
			methodContents += $"\t\t\tcase {group.Key}:\n\t\t\t{{";

			for (int i = 0; i < group.Value.Count; i++)
			{
				var parameterIndex = -1;
				var method = group.Value[i];
				var methodName = method.Identifier.ValueText;
				var parameters0 = method.ParameterList.Parameters.Select(x =>
				{
					parameterIndex++;
					return x.Default != null ?
						$"args[{parameterIndex}] is {x.Type} arg{parameterIndex}_{i} ? arg{parameterIndex}_{i} : default" :
						$"{(x.Modifiers.Any(x => x.IsKind(SyntaxKind.RefKeyword)) ? "ref " : x.Modifiers.Any(x => x.IsKind(SyntaxKind.OutKeyword)) ? "out var " : "")}arg{parameterIndex}_{i}";
				});
				var parameters = parameters0.ToArray();

				var requiredParameters = method.ParameterList.Parameters.Where(x => x.Default == null);
				var requiredParameterCount = requiredParameters.Count(x => !x.Modifiers.Any(y => y.IsKind(SyntaxKind.OutKeyword)));

				var refSets = string.Empty;
				parameterIndex = 0;
				foreach (var @ref in method.ParameterList.Parameters)
				{
					if (@ref.Modifiers.Any(x => x.IsKind(SyntaxKind.RefKeyword) || x.IsKind(SyntaxKind.OutKeyword)))
					{
						refSets += $"args[{parameterIndex}] = arg{parameterIndex}_{i}; ";
					}

					parameterIndex++;
				}

				parameterIndex = -1;
				methodContents += $"\t\t\t\n\t\t\t\t{(requiredParameterCount > 0 ? $"if({(group.Value.Min(y => y.ParameterList.Parameters.Count) != group.Value.Max(y => y.ParameterList.Parameters.Count) ? $"args.Length == {method.ParameterList.Parameters.Count} && " : string.Empty)}{method.ParameterList.Parameters.Select(x => { parameterIndex++; return x.Default == null && !x.Modifiers.Any(y => y.IsKind(SyntaxKind.OutKeyword)) ? $"args[{parameterIndex}] is {x.Type.ToString().Replace("?", string.Empty)} arg{parameterIndex}_{i}" : null; }).Where(x => !string.IsNullOrEmpty(x)).ToArray().ToString(" && ")})" : "")} {(requiredParameterCount > 0 && methodName != "OnServerInitialized" ? "{" : "")} {(method.ReturnType.ToString() != "void" ? "result = " : string.Empty)}{methodName}({string.Join(", ", parameters)}); {refSets} {(requiredParameterCount > 0 && methodName != "OnServerInitialized" ? "}" : "")}";

				Array.Clear(parameters, 0, parameters.Length);
				parameters = null;
				parameters0 = null;
				requiredParameters = null;
			}

			methodContents += "\t\t\t\tbreak;\n\t\t\t}\n";
		}

		methodContents += "}\n}\ncatch (System.Exception ex)\n{\nCarbon.Logger.Error($\"Failed to call internal hook '{Carbon.HookCallerCommon.StringPool.GetOrAdd(hook)}' on plugin '{Name} v{Version}'\", ex);\n}\nreturn result;";

		generatedMethod = SyntaxFactory.MethodDeclaration(
			SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ObjectKeyword).WithTrailingTrivia(SyntaxFactory.Space)),
			"InternalCallHook").AddParameterListParameters(
				SyntaxFactory.Parameter(SyntaxFactory.Identifier("hook")).WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.UIntKeyword)).WithTrailingTrivia(SyntaxFactory.Space)),
				SyntaxFactory.Parameter(SyntaxFactory.Identifier("args")).WithType(SyntaxFactory.ArrayType(
				SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ObjectKeyword)),
				SyntaxFactory.SingletonList(
						SyntaxFactory.ArrayRankSpecifier(
							SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(
								SyntaxFactory.OmittedArraySizeExpression()
							)
						)
					)
				).WithTrailingTrivia(SyntaxFactory.Space)))
				.WithTrailingTrivia(SyntaxFactory.LineFeed)
			.AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword).WithTrailingTrivia(SyntaxFactory.Space), SyntaxFactory.Token(SyntaxKind.OverrideKeyword).WithTrailingTrivia(SyntaxFactory.Space))
			.AddBodyStatements(SyntaxFactory.ParseStatement(methodContents)).WithTrailingTrivia(SyntaxFactory.LineFeed);

		output = input.WithMembers(input.Members.RemoveAt(0).Insert(0, @namespace.WithMembers(@namespace.Members.RemoveAt(0).Insert(0, @class.WithMembers(@class.Members.Insert(0, generatedMethod))))));

		#region Cleanup Pass

		methodDeclarations = null;
		foreach (var hookableMethod in hookableMethods)
		{
			hookableMethod.Value.Clear();
		}
		hookableMethods.Clear();
		hookableMethods = null;

		#endregion
	}

	#endregion
}
