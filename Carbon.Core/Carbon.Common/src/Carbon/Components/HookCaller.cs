using System.Buffers;
using System.Linq.Expressions;
using Carbon.Base.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
	public Dictionary<int, object[]> _argumentBuffer = new();
	public Dictionary<uint, int> _hookTimeBuffer = new();
	public Dictionary<uint, int> _hookTotalTimeBuffer = new();
	public Dictionary<uint, DateTime> _lastDeprecatedWarningAt = new();

	public virtual void AppendHookTime(uint hook, int time) { }
	public virtual void ClearHookTime(uint hook) { }

	public virtual object[] AllocateBuffer(int count) => null;
	public virtual object[] RescaleBuffer(object[] oldBuffer, int newScale) => null;
	public virtual void ClearBuffer(object[] buffer) { }

	public virtual object CallHook<T>(T plugin, uint hookId, BindingFlags flags, object[] args, bool keepArgs = false) where T : BaseHookable => null;
	public virtual object CallDeprecatedHook<T>(T plugin, uint oldHookId, uint newHookId, DateTime expireDate, BindingFlags flags, object[] args) where T : BaseHookable => null;

	public struct Conflict
	{
		public BaseHookable Hookable;
		public uint Hook;
		public object Result;

		public static Conflict Make(BaseHookable hookable, uint hook, object result) => new()
		{
			Hookable = hookable,
			Hook = hook,
			Result = result
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
	internal static Conflict _defaultConflict = new();

	#endregion

	public static int GetHookTime(uint hook)
	{
		if (!Caller._hookTimeBuffer.TryGetValue(hook, out var total))
		{
			return 0;
		}

		return total;
	}
	public static int GetHookTotalTime(uint hook)
	{
		if (!Caller._hookTotalTimeBuffer.TryGetValue(hook, out var total))
		{
			return 0;
		}

		return total;
	}

	private static object CallStaticHook(uint hookId, BindingFlags flag = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, object[] args = null, bool keepArgs = false)
	{
		if (Community.Runtime == null || Community.Runtime.ModuleProcessor == null) return null;

		Caller.ClearHookTime(hookId);

		var result = (object)null;
		var array = args == null || args.Length == 0 ? null : keepArgs ? args : args.ToArray();

		for (int i = 0; i < Community.Runtime.ModuleProcessor.Modules.Count; i++)
		{
			var hookable = Community.Runtime.ModuleProcessor.Modules[i];

			if (hookable is IModule modules && !modules.GetEnabled()) continue;

			var methodResult = Caller.CallHook(hookable, hookId, flags: flag, args: array, keepArgs);

			if (methodResult != null)
			{
				result = methodResult;
				ResultOverride(hookable);
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
					var methodResult = Caller.CallHook(plugin, hookId, flags: flag, args: array, keepArgs);

					if (methodResult != null)
					{
						result = methodResult;
						ResultOverride(plugin);
					}
				}
				catch (Exception ex)
				{
					var exception = ex.InnerException ?? ex;
					var readableHook = HookStringPool.GetOrAdd(hookId);
					Logger.Error($"Failed to call hook '{readableHook}' on plugin '{plugin.Name} v{plugin.Version}'", exception); }
			}
		}

		ConflictCheck();

		_conflictCache.Clear();

		if (array != null && !keepArgs) Array.Clear(array, 0, array.Length);

		void ResultOverride(BaseHookable hookable)
		{
			_conflictCache.Add(Conflict.Make(hookable, hookId, result));
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
				}

				localResult = priorityConflict.Result;
				if (differentResults && Community.Runtime.Config.HigherPriorityHookWarns) Carbon.Logger.Warn($"Hook conflict while calling '{hookId}':\n  {_conflictCache.Select(x => $"{x.Hookable.Name} {x.Hookable.Version} [{x.Result}]").ToString(", ", " and ")}");

				result = localResult;
			}
		}

		return result;
	}
	private static object CallStaticDeprecatedHook(uint oldHookId, uint newHookId, DateTime expireDate, BindingFlags flag = BindingFlags.NonPublic | BindingFlags.Static, object[] args = null)
	{
		if (expireDate < DateTime.Now)
		{
			return null;
		}

		DateTime now = DateTime.Now;

		if (!Caller._lastDeprecatedWarningAt.TryGetValue(oldHookId, out DateTime lastWarningAt) || (now - lastWarningAt).TotalSeconds > 3600f)
		{
			Caller._lastDeprecatedWarningAt[oldHookId] = now;

			Carbon.Logger.Warn($"A plugin is using deprecated hook '{oldHookId}', which will stop working on {expireDate.ToString("D")}. Please ask the author to update to '{newHookId}'");
		}

		return CallStaticHook(oldHookId, flag, args);
	}

	#region Hook Overrides

	public static object CallHook(BaseHookable plugin, uint hookId)
	{
		return Caller.CallHook(plugin, hookId, flags: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, null);
	}
	public static T CallHook<T>(BaseHookable plugin, uint hookId)
	{
		return (T)Caller.CallHook(plugin, hookId, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, null);
	}
	public static T CallDeprecatedHook<T>(BaseHookable plugin, uint oldHookId, uint newHookId, DateTime expireDate)
	{
		return (T)Caller.CallDeprecatedHook(plugin, oldHookId, newHookId, expireDate, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, null);
	}
	public static object CallHook(BaseHookable plugin, uint hookId, object arg1)
	{
		var buffer = Caller.AllocateBuffer(1);
		buffer[0] = arg1;

		var result = Caller.CallHook(plugin, hookId, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static T CallHook<T>(BaseHookable plugin, uint hookId, object arg1)
	{
		var buffer = Caller.AllocateBuffer(1);
		buffer[0] = arg1;

		var result = Caller.CallHook(plugin, hookId, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static T CallDeprecatedHook<T>(BaseHookable plugin, uint oldHookId, uint newHookId, DateTime expireDate, object arg1)
	{
		var buffer = Caller.AllocateBuffer(1);
		buffer[0] = arg1;

		var result = Caller.CallDeprecatedHook(plugin, oldHookId, newHookId, expireDate, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static object CallHook(BaseHookable plugin, uint hookId, object arg1, object arg2)
	{
		var buffer = Caller.AllocateBuffer(2);
		buffer[0] = arg1;
		buffer[1] = arg2;

		var result = Caller.CallHook(plugin, hookId, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static T CallHook<T>(BaseHookable plugin, uint hookId, object arg1, object arg2)
	{
		var buffer = Caller.AllocateBuffer(2);
		buffer[0] = arg1;
		buffer[1] = arg2;

		var result = Caller.CallHook(plugin, hookId, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static T CallDeprecatedHook<T>(BaseHookable plugin, uint oldHookId, uint newHookId, DateTime expireDate, object arg1, object arg2)
	{
		var buffer = Caller.AllocateBuffer(2);
		buffer[0] = arg1;
		buffer[1] = arg2;

		var result = Caller.CallDeprecatedHook(plugin, oldHookId, newHookId, expireDate, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static object CallHook(BaseHookable plugin, uint hookId, object arg1, object arg2, object arg3)
	{
		var buffer = Caller.AllocateBuffer(3);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;

		var result = Caller.CallHook(plugin, hookId, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static T CallHook<T>(BaseHookable plugin, uint hookId, object arg1, object arg2, object arg3)
	{
		var buffer = Caller.AllocateBuffer(3);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;

		var result = Caller.CallHook(plugin, hookId, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static T CallDeprecatedHook<T>(BaseHookable plugin, uint oldHookId, uint newHookId, DateTime expireDate, object arg1, object arg2, object arg3)
	{
		var buffer = Caller.AllocateBuffer(3);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;

		var result = Caller.CallDeprecatedHook(plugin, oldHookId, newHookId, expireDate, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static object CallHook(BaseHookable plugin, uint hookId, object arg1, object arg2, object arg3, object arg4)
	{
		var buffer = Caller.AllocateBuffer(4);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;

		var result = Caller.CallHook(plugin, hookId, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static T CallHook<T>(BaseHookable plugin, uint hookId, object arg1, object arg2, object arg3, object arg4)
	{
		var buffer = Caller.AllocateBuffer(4);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;

		var result = Caller.CallHook(plugin, hookId, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static T CallDeprecatedHook<T>(BaseHookable plugin, uint oldHookId, uint newHookId, DateTime expireDate, object arg1, object arg2, object arg3, object arg4)
	{
		var buffer = Caller.AllocateBuffer(4);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;

		var result = Caller.CallDeprecatedHook(plugin, oldHookId, newHookId, expireDate, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static object CallHook(BaseHookable plugin, uint hookId, object arg1, object arg2, object arg3, object arg4, object arg5)
	{
		var buffer = Caller.AllocateBuffer(5);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;

		var result = Caller.CallHook(plugin, hookId, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static T CallHook<T>(BaseHookable plugin, uint hookId, object arg1, object arg2, object arg3, object arg4, object arg5)
	{
		var buffer = Caller.AllocateBuffer(5);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;

		var result = Caller.CallHook(plugin, hookId, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static T CallDeprecatedHook<T>(BaseHookable plugin, uint oldHookId, uint newHookId, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5)
	{
		var buffer = Caller.AllocateBuffer(5);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;

		var result = Caller.CallDeprecatedHook(plugin, oldHookId, newHookId, expireDate, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static object CallHook(BaseHookable plugin, uint hookId, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
	{
		var buffer = Caller.AllocateBuffer(6);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[5] = arg6;

		var result = Caller.CallHook(plugin, hookId, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static T CallHook<T>(BaseHookable plugin, uint hookId, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
	{
		var buffer = Caller.AllocateBuffer(6);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[5] = arg6;

		var result = Caller.CallHook(plugin, hookId, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static T CallDeprecatedHook<T>(BaseHookable plugin, uint oldHookId, uint newHookId, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
	{
		var buffer = Caller.AllocateBuffer(6);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[5] = arg6;

		var result = Caller.CallDeprecatedHook(plugin, oldHookId, newHookId, expireDate, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static object CallHook(BaseHookable plugin, uint hookId, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
	{
		var buffer = Caller.AllocateBuffer(7);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[5] = arg6;
		buffer[6] = arg7;

		var result = Caller.CallHook(plugin, hookId, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static T CallHook<T>(BaseHookable plugin, uint hookId, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
	{
		var buffer = Caller.AllocateBuffer(7);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[5] = arg6;
		buffer[6] = arg7;

		var result = Caller.CallHook(plugin, hookId, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static T CallDeprecatedHook<T>(BaseHookable plugin, uint oldHookId, uint newHookId, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
	{
		var buffer = Caller.AllocateBuffer(7);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[6] = arg6;
		buffer[7] = arg7;

		var result = Caller.CallDeprecatedHook(plugin, oldHookId, newHookId, expireDate, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static object CallHook(BaseHookable plugin, uint hookId, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
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

		var result = Caller.CallHook(plugin, hookId, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static T CallHook<T>(BaseHookable plugin, uint hookId, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
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

		var result = Caller.CallHook(plugin, hookId, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static T CallDeprecatedHook<T>(BaseHookable plugin, uint oldHookId, uint newHookId, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
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

		var result = Caller.CallDeprecatedHook(plugin, oldHookId, newHookId, expireDate, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static object CallHook(BaseHookable plugin, uint hookId, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
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

		var result = Caller.CallHook(plugin, hookId, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static T CallHook<T>(BaseHookable plugin, uint hookId, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
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

		var result = Caller.CallHook(plugin, hookId, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static T CallDeprecatedHook<T>(BaseHookable plugin, uint oldHookId, uint newHookId, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
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

		var result = Caller.CallDeprecatedHook(plugin, oldHookId, newHookId, expireDate, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static object CallHook(BaseHookable plugin, uint hookId, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
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

		var result = Caller.CallHook(plugin, hookId, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static T CallHook<T>(BaseHookable plugin, uint hookId, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
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

		var result = Caller.CallHook(plugin, hookId, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static T CallDeprecatedHook<T>(BaseHookable plugin, uint oldHookId, uint newHookId, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
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

		var result = Caller.CallDeprecatedHook(plugin, oldHookId, newHookId, expireDate, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static object CallHook(BaseHookable plugin, uint hookId, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
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

		var result = Caller.CallHook(plugin, hookId, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static T CallHook<T>(BaseHookable plugin, uint hookId, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
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

		var result = Caller.CallHook(plugin, hookId, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static T CallDeprecatedHook<T>(BaseHookable plugin, uint oldHookId, uint newHookId, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
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

		var result = Caller.CallDeprecatedHook(plugin, oldHookId, newHookId, expireDate, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static object CallHook(BaseHookable plugin, uint hookId, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
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

		var result = Caller.CallHook(plugin, hookId, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static T CallHook<T>(BaseHookable plugin, uint hookId, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
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

		var result = Caller.CallHook(plugin, hookId, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static T CallDeprecatedHook<T>(BaseHookable plugin, uint oldHookId, uint newHookId, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
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

		var result = Caller.CallDeprecatedHook(plugin, oldHookId, newHookId, expireDate, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static object CallHook(BaseHookable plugin, uint hookId, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13)
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

		var result = Caller.CallHook(plugin, hookId, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static T CallHook<T>(BaseHookable plugin, uint hookId, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13)
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

		var result = Caller.CallHook(plugin, hookId, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}
	public static T CallDeprecatedHook<T>(BaseHookable plugin, uint oldHookId, uint newHookId, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13)
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

		var result = Caller.CallDeprecatedHook(plugin, oldHookId, newHookId, expireDate, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ClearBuffer(buffer);
		return (T)result;
	}

	#endregion

	#region Static Hook Overrides

	public static object CallStaticHook(uint hookId)
	{
		return CallStaticHook(hookId, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, null);
	}
	public static object CallStaticDeprecatedHook(uint oldHookId, uint newHookId, DateTime expireDate)
	{
		return CallStaticDeprecatedHook(oldHookId, newHookId, expireDate, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, null);
	}
	public static object CallStaticHook(uint hookId, object arg1)
	{
		var buffer = Caller.AllocateBuffer(1);
		buffer[0] = arg1;

		var result = CallStaticHook(hookId, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticDeprecatedHook(uint oldHookId, uint newHookId, DateTime expireDate, object arg1)
	{
		var buffer = Caller.AllocateBuffer(1);
		buffer[0] = arg1;

		var result = CallStaticDeprecatedHook(oldHookId, newHookId, expireDate, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticHook(uint hookId, object arg1, object arg2)
	{
		var buffer = Caller.AllocateBuffer(2);
		buffer[0] = arg1;
		buffer[1] = arg2;

		var result = CallStaticHook(hookId, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticDeprecatedHook(uint oldHookId, uint newHookId, DateTime expireDate, object arg1, object arg2)
	{
		var buffer = Caller.AllocateBuffer(2);
		buffer[0] = arg1;
		buffer[1] = arg2;

		var result = CallStaticDeprecatedHook(oldHookId, newHookId, expireDate, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticHook(uint hookId, object arg1, object arg2, object arg3)
	{
		var buffer = Caller.AllocateBuffer(3);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;

		var result = CallStaticHook(hookId, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticDeprecatedHook(uint oldHookId, uint newHookId, DateTime expireDate, object arg1, object arg2, object arg3)
	{
		var buffer = Caller.AllocateBuffer(3);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;

		var result = CallStaticDeprecatedHook(oldHookId, newHookId, expireDate, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticHook(uint hookId, object arg1, object arg2, object arg3, object arg4)
	{
		var buffer = Caller.AllocateBuffer(4);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;

		var result = CallStaticHook(hookId, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticDeprecatedHook(uint oldHookId, uint newHookId, DateTime expireDate, object arg1, object arg2, object arg3, object arg4)
	{
		var buffer = Caller.AllocateBuffer(4);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;

		var result = CallStaticDeprecatedHook(oldHookId, newHookId, expireDate, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticHook(uint hookId, object arg1, object arg2, object arg3, object arg4, object arg5)
	{
		var buffer = Caller.AllocateBuffer(5);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;

		var result = CallStaticHook(hookId, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticDeprecatedHook(uint oldHookId, uint newHookId, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5)
	{
		var buffer = Caller.AllocateBuffer(5);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;

		var result = CallStaticDeprecatedHook(oldHookId, newHookId, expireDate, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticHook(uint hookId, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
	{
		var buffer = Caller.AllocateBuffer(6);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[5] = arg6;

		var result = CallStaticHook(hookId, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticDeprecatedHook(uint oldHookId, uint newHookId, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
	{
		var buffer = Caller.AllocateBuffer(6);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[5] = arg6;

		var result = CallStaticDeprecatedHook(oldHookId, newHookId, expireDate, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticHook(uint hookId, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
	{
		var buffer = Caller.AllocateBuffer(7);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[5] = arg6;
		buffer[6] = arg7;

		var result = CallStaticHook(hookId, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticDeprecatedHook(uint oldHookId, uint newHookId, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
	{
		var buffer = Caller.AllocateBuffer(7);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;
		buffer[4] = arg5;
		buffer[5] = arg6;
		buffer[6] = arg7;

		var result = CallStaticDeprecatedHook(oldHookId, newHookId, expireDate, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticHook(uint hookId, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
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

		var result = CallStaticHook(hookId, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticDeprecatedHook(uint oldHookId, uint newHookId, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
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

		var result = CallStaticDeprecatedHook(oldHookId, newHookId, expireDate, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticHook(uint hookId, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
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

		var result = CallStaticHook(hookId, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticDeprecatedHook(uint oldHookId, uint newHookId, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
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

		var result = CallStaticDeprecatedHook(oldHookId, newHookId, expireDate, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticHook(uint hookId, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
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

		var result = CallStaticHook(hookId, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticDeprecatedHook(uint oldHookId, uint newHookId, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
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

		var result = CallStaticDeprecatedHook(oldHookId, newHookId, expireDate, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticHook(uint hookId, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
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

		var result = CallStaticHook(hookId, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticDeprecatedHook(uint oldHookId, uint newHookId, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
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

		var result = CallStaticDeprecatedHook(oldHookId, newHookId, expireDate, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticHook(uint hookId, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
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

		var result = CallStaticHook(hookId, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticDeprecatedHook(uint oldHookId, uint newHookId, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
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

		var result = CallStaticDeprecatedHook(oldHookId, newHookId, expireDate, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticHook(uint hookId, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13)
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

		var result = CallStaticHook(hookId, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}
	public static object CallStaticDeprecatedHook(uint oldHookId, uint newHookId, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13)
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

		var result = CallStaticDeprecatedHook(oldHookId, newHookId, expireDate, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ClearBuffer(buffer);
		return result;
	}

	public static object CallStaticHook(uint hookId, object[] args, bool keepArgs = false)
	{
		return CallStaticHook(hookId, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args, keepArgs: keepArgs);
	}
	public static object CallStaticDeprecatedHook(uint oldHookId, uint newHookId, DateTime expireDate, object[] args)
	{
		return CallStaticDeprecatedHook(oldHookId, newHookId, expireDate, args);
	}

	#endregion

	#region Generator

	public static void GenerateInternalCallHook(CompilationUnitSyntax input, out CompilationUnitSyntax output, out MethodDeclarationSyntax generatedMethod, bool publicize = true)
	{
		var methodContents = "\n\tvar result = (object)null;\n\ttry\n\t{\n\t\tswitch(hook)\n\t\t{\n";
		var @namespace = input.Members[0] as BaseNamespaceDeclarationSyntax;
		var @class = @namespace.Members[0] as ClassDeclarationSyntax;
		var methodDeclarations = new List<MethodDeclarationSyntax>();
		methodDeclarations.AddRange(@class.ChildNodes().OfType<MethodDeclarationSyntax>());

		#region Handle partials

		foreach (var subNamespace in input.Members.OfType<BaseNamespaceDeclarationSyntax>())
		{
			foreach (var subClass in subNamespace.Members.OfType<ClassDeclarationSyntax>())
			{
				if (subClass != @class && subClass.Modifiers.Any(SyntaxKind.PartialKeyword) && subClass.Identifier.ValueText == @class.Identifier.ValueText)
				{
					methodDeclarations.AddRange(subClass.ChildNodes().OfType<MethodDeclarationSyntax>());
				}
			}
		}

		#endregion

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

		var hookableMethods = new Dictionary<uint, List<MethodDeclarationSyntax>>();
		var privateMethods0 = methodDeclarations.Where(md => (md.Modifiers.Count == 0 || md.Modifiers.All(modifier => !modifier.IsKind(SyntaxKind.PublicKeyword) && !modifier.IsKind(SyntaxKind.StaticKeyword)) || md.AttributeLists.Any(x => x.Attributes.Any(y => y.Name.ToString() == "HookMethod"))) && md.TypeParameterList == null);
		var privateMethods = privateMethods0.OrderBy(x => x.Identifier.ValueText);
		privateMethods0 = null;

		foreach (var method in privateMethods)
		{
			var methodName = method.Identifier.ValueText;
			var id = HookStringPool.GetOrAdd(methodName);

			if (!hookableMethods.TryGetValue(id, out var list))
			{
				hookableMethods[id] = list = new();
			}

			list.Add(method);
		}

		foreach (var group in hookableMethods)
		{
			methodContents += $"\t\t\t// {group.Value[0].Identifier.ValueText} aka {group.Key}\n\t\t\tcase {group.Key}:\n\t\t\t{{";

			for (int i = 0; i < group.Value.Count; i++)
			{
				var parameterIndex = -1;
				var method = group.Value[i];
				var methodName = method.Identifier.ValueText;
				var parameters0 = method.ParameterList.Parameters.Select(x =>
				{
					var type = x.Type.ToString().Replace("?", string.Empty);
					parameterIndex++;

					if (x.Modifiers.Any(x => x.IsKind(SyntaxKind.OutKeyword)))
					{
						return $"out var arg{parameterIndex}_{i}";
					}
					else if (x.Default != null || x.Type is NullableTypeSyntax)
					{
						return $"args[{parameterIndex}] is {type} arg{parameterIndex}_{i} ? arg{parameterIndex}_{i} : ({type})default";
					}
					else if (x.Modifiers.Any(x => x.IsKind(SyntaxKind.RefKeyword)))
					{
						return $"ref arg{parameterIndex}_{i}";
					}

					return $"arg{parameterIndex}_{i}";
				});
				var parameters = parameters0.ToArray();

				var requiredParameters = method.ParameterList.Parameters.Where(x => x.Default == null && x.Type is not NullableTypeSyntax);
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
				var parameterText = string.Empty;
				var varText = string.Empty;
				for (int o = 0; o < method.ParameterList.Parameters.Count; o++)
				{
					var parameter = method.ParameterList.Parameters[o];
					parameterIndex++;

					if (parameter.Default == null && !parameter.Modifiers.Any(y => y.IsKind(SyntaxKind.OutKeyword)) && parameter.Type is not NullableTypeSyntax)
					{
						var type = parameter.Type.ToString().Replace("global::", string.Empty);
						varText += $"var narg{parameterIndex}_{i} = args[{parameterIndex}] is {type};\nvar arg{parameterIndex}_{i} = narg{parameterIndex}_{i} ? ({type})(args[{parameterIndex}] ?? ({type})default) : ({type})default;\n";
						parameterText += !IsUnmanagedType(type) ? $"narg{parameterIndex}_{i} && " : $"(narg{parameterIndex}_{i} || args[{parameterIndex}] == null) && ";
					}
				}

				if (!string.IsNullOrEmpty(parameterText))
				{
					parameterText = parameterText.Substring(0, parameterText.Length - 3);
				}

				var validLengthCheck = group.Value.Min(y => y.ParameterList.Parameters.Count) != group.Value.Max(y => y.ParameterList.Parameters.Count);
				methodContents += $"\t\t\t\n\t\t\t\t{(requiredParameterCount > 0 ? $"{(validLengthCheck ? $"if(args.Length == {method.ParameterList.Parameters.Count})" : string.Empty)}" : "")} {(requiredParameterCount > 0 && methodName != "OnServerInitialized" && validLengthCheck ? "{" : "")} {varText}{(string.IsNullOrEmpty(parameterText) ? string.Empty : $"if({parameterText}) {{")} {(method.ReturnType.ToString() != "void" ? "result = " : string.Empty)}{methodName}({string.Join(", ", parameters)}); {refSets} {(requiredParameterCount > 0 && methodName != "OnServerInitialized" && validLengthCheck ? "}" : "")}{(string.IsNullOrEmpty(parameterText) ? string.Empty : $"}}")}\n";

				Array.Clear(parameters, 0, parameters.Length);
				parameters = null;
				parameters0 = null;
				requiredParameters = null;
			}

			methodContents += "\t\t\t\tbreak;\n\t\t\t}\n";
		}

		methodContents += "}\n}\ncatch (System.Exception ex)\n{\nCarbon.Logger.Error($\"Failed to call internal hook '{Carbon.Pooling.HookStringPool.GetOrAdd(hook)}' on plugin '{Name} v{Version}'\", ex);\n}\nreturn result;";

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

		output = input.WithMembers(input.Members.RemoveAt(0).Insert(0, @namespace.WithMembers(@namespace.Members.RemoveAt(0).Insert(0, @class.WithMembers(@class.Members.Insert(@class.Members.Count, generatedMethod))))));

		#region Cleanup

		methodDeclarations.Clear();
		methodDeclarations = null;
		foreach (var hookableMethod in hookableMethods)
		{
			hookableMethod.Value.Clear();
		}
		hookableMethods.Clear();
		hookableMethods = null;

		#endregion
	}

	public static bool IsUnmanagedType(string type)
	{
		return type switch
		{
			"string" or "int" or "uint" or "long" or "ulong" or "bool" => true,
			_ => false,
		};
	}

	#endregion
}
