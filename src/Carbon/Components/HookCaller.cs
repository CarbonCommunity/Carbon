using System;
using System.Text;
using Carbon.Base.Interfaces;
using HarmonyLib;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Carbon.HookCallerCommon;
using Pool = Facepunch.Pool;

namespace Carbon;

public class HookCallerCommon
{
	public Dictionary<int, HookArgPool> _argumentBuffer = new();
	public Dictionary<uint, DateTime> _lastDeprecatedWarningAt = new();

	public struct HookArgPool
	{
		internal Queue<object[]> _pool;
		internal int _length;

		public HookArgPool(int length, int count)
		{
			this._length = length;

			_pool = new Queue<object[]>(count);

			for (int i = 0; i < count; i++)
			{
				_pool.Enqueue(new object[length]);
			}
		}

		public object[] Rent()
		{
			return _pool.Count > 0 ? _pool.Dequeue() : new object[_length];
		}
		public void Return(object[] array)
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = default;
			}

			_pool.Enqueue(array);
		}
	}

	public virtual object[] AllocateBuffer(int count) => null;
	public virtual object[] RescaleBuffer(object[] oldBuffer, int newScale, BaseHookable.CachedHook hook) => null;
	public virtual void ProcessDefaults(object[] buffer, BaseHookable.CachedHook hook) { }
	public virtual void ReturnBuffer(object[] buffer) { }

	public virtual object CallHook<T>(T hookable, uint hookId, BindingFlags flags, object[] args) where T : BaseHookable => null;
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
}

public static class HookCaller
{
	public static HookCallerCommon Caller { get; set; }

	public static IEnumerable<BaseHookable.CachedHook> GetAllFor(uint hook)
	{
		foreach (var cacheHook in from package in ModLoader.Packages
		         from plugin in package.Plugins
		         from cache in plugin.HookPool where cache.Key == hook
		         from cacheHook in cache.Value.Hooks select cacheHook)
		{
			yield return cacheHook;
		}

		foreach (var cacheHook in
		         from module in Community.Runtime.ModuleProcessor.Modules
		         from cache in module.HookPool where cache.Key == hook
		         from cacheHook in cache.Value.Hooks select cacheHook)
		{
			yield return cacheHook;
		}
	}

	public static TimeSpan GetTotalTime(uint hook)
	{
		TimeSpan finalValue = default;

		foreach (var cacheInstance in GetAllFor(hook))
		{
			finalValue += cacheInstance.HookTime;
		}

		return finalValue;
	}
	public static int GetTotalFires(uint hook)
	{
		int finalValue = default;

		foreach (var cacheInstance in GetAllFor(hook))
		{
			finalValue += cacheInstance.TimesFired;
		}

		return finalValue;
	}
	public static double GetTotalMemory(uint hook)
	{
		double finalValue = default;

		foreach (var cacheInstance in GetAllFor(hook))
		{
			finalValue += cacheInstance.MemoryUsage;
		}

		return finalValue;
	}
	public static double GetTotalLagSpikes(uint hook)
	{
		double finalValue = default;

		foreach (var cacheInstance in GetAllFor(hook))
		{
			finalValue += cacheInstance.LagSpikes;
		}

		return finalValue;
	}
	public static int GetTotalExceptions(uint hook)
	{
		int finalValue = default;

		foreach (var cacheInstance in GetAllFor(hook))
		{
			finalValue += cacheInstance.Exceptions;
		}

		return finalValue;
	}

	private static object CallStaticHook(uint hookId, BindingFlags flag = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, object[] args = null)
	{
		if (Community.Runtime == null || Community.Runtime.ModuleProcessor == null)
		{
			return null;
		}

		var result = (object)null;
		var conflicts = Pool.Get<List<Conflict>>();

		for (int i = 0; i < Community.Runtime.ModuleProcessor.Modules.Count; i++)
		{
			var hookable = Community.Runtime.ModuleProcessor.Modules[i];

			try
			{
				if (hookable is IModule modules && !modules.IsEnabled()) continue;

				var methodResult = Caller.CallHook(hookable, hookId, flags: flag, args: args);

				if (methodResult == null) continue;

				result = methodResult;
				ResultOverride(conflicts, hookable, hookId, result);
			}
			catch (Exception ex)
			{
				var exception = ex.InnerException ?? ex;
				var readableHook = HookStringPool.GetOrAdd(hookId);
				Logger.Error($"Failed to call hook '{readableHook}' on module '{hookable.Name} v{hookable.Version}'", exception);
			}
		}

		for (int i = 0; i < ModLoader.Packages.Count; i++)
		{
			var package = ModLoader.Packages[i];

			for(int o = 0; o < package.Plugins.Count; o++)
			{
				var plugin = package.Plugins[o];

				try
				{
					var methodResult = Caller.CallHook(plugin, hookId, flags: flag, args: args);

					if (methodResult == null)
					{
						continue;
					}

					result = methodResult;
					ResultOverride(conflicts, plugin, hookId, result);
				}
				catch (Exception ex)
				{
					var exception = ex.InnerException ?? ex;
					var readableHook = HookStringPool.GetOrAdd(hookId);
					Logger.Error($"Failed to call hook '{readableHook}' on plugin '{plugin.Name} v{plugin.Version}'", exception);
				}
			}
		}

		ConflictCheck(conflicts, ref result, hookId);

		Pool.FreeUnmanaged(ref conflicts);

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

	public static void ResultOverride(List<Conflict> conflicts, BaseHookable hookable, uint hookId, object result)
	{
		if (result == null) return;

		conflicts.Add(Conflict.Make(hookable, hookId, result));
	}
	public static void ConflictCheck(List<Conflict> conflicts, ref object result, uint hookId)
	{
		if (conflicts == null || conflicts.Count <= 1)
		{
			return;
		}

		var localResult = result = conflicts[0].Result;
		var differentResults = false;

		foreach (var conflict in conflicts)
		{
			if (localResult == null || (conflict.Result != null && conflict.Result.Equals(localResult)))
			{
				continue;
			}
			differentResults = true;
			break;
		}

		if (!differentResults)
		{
			return;
		}

		Logger.Warn($" Hook conflict while calling '{ HookStringPool.GetOrAdd(hookId)}[{hookId}]': {conflicts.Select(x => $"{x.Hookable.Name} {x.Hookable.Version} [{x.Result}]").ToString(", ", " and ")}");
		result = conflicts[^1].Result;
	}

	#region Hook Overrides

	public static object CallHook(BaseHookable plugin, uint hookId)
	{
		var buffer = Caller.AllocateBuffer(0);
		var result = Caller.CallHook(plugin, hookId, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ReturnBuffer(buffer);
		return result;
	}
	public static T CallHook<T>(BaseHookable plugin, uint hookId)
	{
		var buffer = Caller.AllocateBuffer(0);
		var result = Caller.CallHook(plugin, hookId, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ReturnBuffer(buffer);
		return result == null ? default : (T)TypeEx.ConvertType<T>(result);
	}
	public static T CallDeprecatedHook<T>(BaseHookable plugin, uint oldHookId, uint newHookId, DateTime expireDate)
	{
		var buffer = Caller.AllocateBuffer(0);
		var result = Caller.CallDeprecatedHook(plugin, oldHookId, newHookId, expireDate, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ReturnBuffer(buffer);
		return result == null ? default : (T)TypeEx.ConvertType<T>(result);
	}
	public static object CallHook(BaseHookable plugin, uint hookId, object arg1)
	{
		var buffer = Caller.AllocateBuffer(1);
		buffer[0] = arg1;

		var result = Caller.CallHook(plugin, hookId, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ReturnBuffer(buffer);
		return result;
	}
	public static T CallHook<T>(BaseHookable plugin, uint hookId, object arg1)
	{
		var buffer = Caller.AllocateBuffer(1);
		buffer[0] = arg1;

		var result = Caller.CallHook(plugin, hookId, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ReturnBuffer(buffer);
		return result == null ? default : (T)TypeEx.ConvertType<T>(result);
	}
	public static T CallDeprecatedHook<T>(BaseHookable plugin, uint oldHookId, uint newHookId, DateTime expireDate, object arg1)
	{
		var buffer = Caller.AllocateBuffer(1);
		buffer[0] = arg1;

		var result = Caller.CallDeprecatedHook(plugin, oldHookId, newHookId, expireDate, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ReturnBuffer(buffer);
		return result == null ? default : (T)TypeEx.ConvertType<T>(result);
	}
	public static object CallHook(BaseHookable plugin, uint hookId, object arg1, object arg2)
	{
		var buffer = Caller.AllocateBuffer(2);
		buffer[0] = arg1;
		buffer[1] = arg2;

		var result = Caller.CallHook(plugin, hookId, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ReturnBuffer(buffer);
		return result;
	}
	public static T CallHook<T>(BaseHookable plugin, uint hookId, object arg1, object arg2)
	{
		var buffer = Caller.AllocateBuffer(2);
		buffer[0] = arg1;
		buffer[1] = arg2;

		var result = Caller.CallHook(plugin, hookId, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ReturnBuffer(buffer);
		return result == null ? default : (T)TypeEx.ConvertType<T>(result);
	}
	public static T CallDeprecatedHook<T>(BaseHookable plugin, uint oldHookId, uint newHookId, DateTime expireDate, object arg1, object arg2)
	{
		var buffer = Caller.AllocateBuffer(2);
		buffer[0] = arg1;
		buffer[1] = arg2;

		var result = Caller.CallDeprecatedHook(plugin, oldHookId, newHookId, expireDate, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ReturnBuffer(buffer);
		return result == null ? default : (T)TypeEx.ConvertType<T>(result);
	}
	public static object CallHook(BaseHookable plugin, uint hookId, object arg1, object arg2, object arg3)
	{
		var buffer = Caller.AllocateBuffer(3);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;

		var result = Caller.CallHook(plugin, hookId, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ReturnBuffer(buffer);
		return result;
	}
	public static T CallHook<T>(BaseHookable plugin, uint hookId, object arg1, object arg2, object arg3)
	{
		var buffer = Caller.AllocateBuffer(3);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;

		var result = Caller.CallHook(plugin, hookId, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ReturnBuffer(buffer);
		return result == null ? default : (T)TypeEx.ConvertType<T>(result);
	}
	public static T CallDeprecatedHook<T>(BaseHookable plugin, uint oldHookId, uint newHookId, DateTime expireDate, object arg1, object arg2, object arg3)
	{
		var buffer = Caller.AllocateBuffer(3);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;

		var result = Caller.CallDeprecatedHook(plugin, oldHookId, newHookId, expireDate, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ReturnBuffer(buffer);
		return result == null ? default : (T)TypeEx.ConvertType<T>(result);
	}
	public static object CallHook(BaseHookable plugin, uint hookId, object arg1, object arg2, object arg3, object arg4)
	{
		var buffer = Caller.AllocateBuffer(4);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;

		var result = Caller.CallHook(plugin, hookId, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ReturnBuffer(buffer);
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

		Caller.ReturnBuffer(buffer);
		return result == null ? default : (T)TypeEx.ConvertType<T>(result);
	}
	public static T CallDeprecatedHook<T>(BaseHookable plugin, uint oldHookId, uint newHookId, DateTime expireDate, object arg1, object arg2, object arg3, object arg4)
	{
		var buffer = Caller.AllocateBuffer(4);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;
		buffer[3] = arg4;

		var result = Caller.CallDeprecatedHook(plugin, oldHookId, newHookId, expireDate, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

		Caller.ReturnBuffer(buffer);
		return result == null ? default : (T)TypeEx.ConvertType<T>(result);
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

		Caller.ReturnBuffer(buffer);
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

		Caller.ReturnBuffer(buffer);
		return result == null ? default : (T)TypeEx.ConvertType<T>(result);
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

		Caller.ReturnBuffer(buffer);
		return result == null ? default : (T)TypeEx.ConvertType<T>(result);
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

		Caller.ReturnBuffer(buffer);
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

		Caller.ReturnBuffer(buffer);
		return result == null ? default : (T)TypeEx.ConvertType<T>(result);
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

		Caller.ReturnBuffer(buffer);
		return result == null ? default : (T)TypeEx.ConvertType<T>(result);
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

		Caller.ReturnBuffer(buffer);
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

		Caller.ReturnBuffer(buffer);
		return result == null ? default : (T)TypeEx.ConvertType<T>(result);
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

		Caller.ReturnBuffer(buffer);
		return result == null ? default : (T)TypeEx.ConvertType<T>(result);
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

		Caller.ReturnBuffer(buffer);
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

		Caller.ReturnBuffer(buffer);
		return result == null ? default : (T)TypeEx.ConvertType<T>(result);
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

		Caller.ReturnBuffer(buffer);
		return result == null ? default : (T)TypeEx.ConvertType<T>(result);
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

		Caller.ReturnBuffer(buffer);
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

		Caller.ReturnBuffer(buffer);
		return result == null ? default : (T)TypeEx.ConvertType<T>(result);
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

		Caller.ReturnBuffer(buffer);
		return result == null ? default : (T)TypeEx.ConvertType<T>(result);
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

		Caller.ReturnBuffer(buffer);
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

		Caller.ReturnBuffer(buffer);
		return result == null ? default : (T)TypeEx.ConvertType<T>(result);
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

		Caller.ReturnBuffer(buffer);
		return result == null ? default : (T)TypeEx.ConvertType<T>(result);
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

		Caller.ReturnBuffer(buffer);
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

		Caller.ReturnBuffer(buffer);
		return result == null ? default : (T)TypeEx.ConvertType<T>(result);
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

		Caller.ReturnBuffer(buffer);
		return result == null ? default : (T)TypeEx.ConvertType<T>(result);
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

		Caller.ReturnBuffer(buffer);
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

		Caller.ReturnBuffer(buffer);
		return result == null ? default : (T)TypeEx.ConvertType<T>(result);
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

		Caller.ReturnBuffer(buffer);
		return result == null ? default : (T)TypeEx.ConvertType<T>(result);
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

		Caller.ReturnBuffer(buffer);
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

		Caller.ReturnBuffer(buffer);
		return result == null ? default : (T)TypeEx.ConvertType<T>(result);
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

		Caller.ReturnBuffer(buffer);
		return result == null ? default : (T)TypeEx.ConvertType<T>(result);
	}
	public static object CallHook(BaseHookable plugin, uint hookId, object[] args)
	{
		return Caller.CallHook(plugin, hookId, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args);
	}
	public static T CallHook<T>(BaseHookable plugin, uint hookId, object[] args)
	{
		var result = Caller.CallHook(plugin, hookId, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args);
		return result == null ? default : (T)TypeEx.ConvertType<T>(result);
	}
	#endregion

	#region Static Hook Overrides

	public static object CallStaticHook(uint hookId)
	{
		var buffer = Caller.AllocateBuffer(0);
		var result = CallStaticHook(hookId, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ReturnBuffer(buffer);
		return result;
	}
	public static object CallStaticDeprecatedHook(uint oldHookId, uint newHookId, DateTime expireDate)
	{
		var buffer = Caller.AllocateBuffer(0);
		var result = CallStaticDeprecatedHook(oldHookId, newHookId, expireDate, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ReturnBuffer(buffer);
		return result;
	}
	public static object CallStaticHook(uint hookId, object arg1)
	{
		var buffer = Caller.AllocateBuffer(1);
		buffer[0] = arg1;

		var result = CallStaticHook(hookId, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ReturnBuffer(buffer);
		return result;
	}
	public static object CallStaticDeprecatedHook(uint oldHookId, uint newHookId, DateTime expireDate, object arg1)
	{
		var buffer = Caller.AllocateBuffer(1);
		buffer[0] = arg1;

		var result = CallStaticDeprecatedHook(oldHookId, newHookId, expireDate, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ReturnBuffer(buffer);
		return result;
	}
	public static object CallStaticHook(uint hookId, object arg1, object arg2)
	{
		var buffer = Caller.AllocateBuffer(2);
		buffer[0] = arg1;
		buffer[1] = arg2;

		var result = CallStaticHook(hookId, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ReturnBuffer(buffer);
		return result;
	}
	public static object CallStaticDeprecatedHook(uint oldHookId, uint newHookId, DateTime expireDate, object arg1, object arg2)
	{
		var buffer = Caller.AllocateBuffer(2);
		buffer[0] = arg1;
		buffer[1] = arg2;

		var result = CallStaticDeprecatedHook(oldHookId, newHookId, expireDate, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ReturnBuffer(buffer);
		return result;
	}
	public static object CallStaticHook(uint hookId, object arg1, object arg2, object arg3)
	{
		var buffer = Caller.AllocateBuffer(3);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;

		var result = CallStaticHook(hookId, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ReturnBuffer(buffer);
		return result;
	}
	public static object CallStaticDeprecatedHook(uint oldHookId, uint newHookId, DateTime expireDate, object arg1, object arg2, object arg3)
	{
		var buffer = Caller.AllocateBuffer(3);
		buffer[0] = arg1;
		buffer[1] = arg2;
		buffer[2] = arg3;

		var result = CallStaticDeprecatedHook(oldHookId, newHookId, expireDate, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

		Caller.ReturnBuffer(buffer);
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

		Caller.ReturnBuffer(buffer);
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

		Caller.ReturnBuffer(buffer);
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

		Caller.ReturnBuffer(buffer);
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

		Caller.ReturnBuffer(buffer);
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

		Caller.ReturnBuffer(buffer);
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

		Caller.ReturnBuffer(buffer);
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

		Caller.ReturnBuffer(buffer);
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

		Caller.ReturnBuffer(buffer);
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

		Caller.ReturnBuffer(buffer);
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

		Caller.ReturnBuffer(buffer);
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

		Caller.ReturnBuffer(buffer);
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

		Caller.ReturnBuffer(buffer);
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

		Caller.ReturnBuffer(buffer);
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

		Caller.ReturnBuffer(buffer);
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

		Caller.ReturnBuffer(buffer);
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

		Caller.ReturnBuffer(buffer);
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

		Caller.ReturnBuffer(buffer);
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

		Caller.ReturnBuffer(buffer);
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

		Caller.ReturnBuffer(buffer);
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

		Caller.ReturnBuffer(buffer);
		return result;
	}

	public static object CallStaticHook(uint hookId, object[] args)
	{
		return CallStaticHook(hookId, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: args);
	}
	public static object CallStaticDeprecatedHook(uint oldHookId, uint newHookId, DateTime expireDate, object[] args)
	{
		return CallStaticDeprecatedHook(oldHookId, newHookId, expireDate, args: args);
	}

	#endregion

	#region Generator

	private static char[] _underscoreChar = new [] { '_' };
	private static char[] _dotChar = new [] { '.' };
	private static string[] _operatorsStrings = new [] { "&&", "||" };
	private static string _ifDirective = "#if";
	private static string _elifDirective = "#elif";

	public static void GenerateInternalCallHook(CompilationUnitSyntax input, out CompilationUnitSyntax output, out MethodDeclarationSyntax generatedMethod, out bool isPartial, bool baseCall = false, string baseName = "plugin", List<ClassDeclarationSyntax> classList = null)
	{
		var methodContents = $"\n\tvar length = args?.Length;\ntry {{ switch(hook) {{ ";

		var @namespace = (BaseNamespaceDeclarationSyntax)null;
		var namespaceIndex = 0;
		var classIndex = 0;
		var isTemp = false;

		if (classList == null)
		{
			classList = Pool.Get<List<ClassDeclarationSyntax>>();
			isTemp = true;
			FindPluginInfo(input, out @namespace, out _, out _, classList);
		}
		else
		{
			FindPluginInfo(input, out @namespace, out _, out _, null);

			namespaceIndex = classIndex = 0;
		}

		if(classList.Count == 0)
		{
			if (isTemp)
			{
				Pool.FreeUnmanaged(ref classList);
			}

			output = null;
			generatedMethod = null;
			isPartial = default;
			return;
		}

		var @class = classList[0];

		if (@namespace == null)
		{
			@namespace = @class.Parent as BaseNamespaceDeclarationSyntax;
		}

		isPartial = @class.Modifiers.Any(x => x.IsKind(SyntaxKind.PartialKeyword));

		var methodDeclarations = new List<MethodDeclarationSyntax>();
		methodDeclarations.AddRange(classList.SelectMany(x => x.ChildNodes()).OfType<MethodDeclarationSyntax>());

		if (isTemp)
		{
			Pool.FreeUnmanaged(ref classList);
		}

		var hookableMethods = new Dictionary<uint, List<MethodDeclarationSyntax>>();
		var privateMethods0 = methodDeclarations.Where(md => (md.Modifiers.Count == 0 || md.Modifiers.All(modifier => !modifier.IsKind(SyntaxKind.PublicKeyword) && !modifier.IsKind(SyntaxKind.StaticKeyword)) || md.AttributeLists.Any(x => x.Attributes.Any(y => y.Name.ToString() == "HookMethod"))) && md.TypeParameterList == null);
		var privateMethods = privateMethods0.OrderBy(x => x.Identifier.ValueText);
		privateMethods0 = null;

		foreach (var method in privateMethods)
		{
			var hookMethod = method.AttributeLists.Select(x => x.Attributes.FirstOrDefault(x => x.Name.ToString() == "HookMethod")).FirstOrDefault();
			var methodName = hookMethod != null && hookMethod.ArgumentList.Arguments.Count > 0 ? hookMethod.ArgumentList.Arguments[0].ToString().Replace("\"", string.Empty) : method.Identifier.ValueText;

			if (hookMethod != null)
			{
				var context = hookMethod.ArgumentList.Arguments[0];
				var contextString = context.ToString();

				if (contextString.Contains("nameof"))
				{
					methodName = contextString
						.Replace("nameof", string.Empty)
						.Replace("(", string.Empty)
						.Replace(")", string.Empty);

					if (methodName.Contains("."))
					{
						using var temp = TempArray<string>.New(methodName.Split('.'));
						methodName = temp.Get(temp.Length - 1);
					}
				}
				else if (contextString.Contains("."))
				{
					var argument = context.Expression as MemberAccessExpressionSyntax;
					var expression = argument.Expression.ToString();
					var name = argument.Name.ToString();

					var value = AccessTools.Field(AccessTools.TypeByName(expression), name)?.GetValue(null)?.ToString();

					if (!string.IsNullOrEmpty(value))
					{
						methodName = value;
					}
				}
				else if (context.ToString().Contains("\""))
				{
					var value = AccessTools
						.Field(AccessTools.TypeByName(classList.FirstOrDefault().Identifier.Text),
							context.ToString().Replace("\"", string.Empty))?.GetValue(null)?.ToString();

					if (!string.IsNullOrEmpty(value))
					{
						methodName = value;
					}
				}
			}

			var id = HookStringPool.GetOrAdd(methodName);

			if (!hookableMethods.TryGetValue(id, out var list))
			{
				hookableMethods[id] = list = new();
			}

			list.Add(method);
		}

		foreach (var group in hookableMethods)
		{
			methodContents += $"\t\t\t\n// {group.Value[0].Identifier.ValueText} aka {group.Key}\n\t\t\tcase {group.Key}:\n\t\t\t{{";

			var overrideCount = 1;

			for (int i = 0; i < group.Value.Count; i++)
			{
				var parameterIndex = -1;
				var method = group.Value[i];
				var conditional = method.AttributeLists.Select(x => x.Attributes.FirstOrDefault(x => ((IdentifierNameSyntax)x.Name).Identifier.Text == "Conditional"))?.FirstOrDefault()?.ArgumentList?.Arguments[0].ToString().Replace("\"", string.Empty);
				var methodName = method.Identifier.ValueText;
				var parameters0 = method.ParameterList.Parameters.Select(x =>
				{
					var type = x.Type.ToString().Replace("?", string.Empty);
					parameterIndex++;

					if (x.Modifiers.Any(x => x.IsKind(SyntaxKind.OutKeyword)))
					{
						return $"out var arg{parameterIndex}_{i}";
					}

					if (x.Default != null || x.Type is NullableTypeSyntax)
					{
						return $"length > {parameterIndex} && args[{parameterIndex}] is {type} arg{parameterIndex}_{i} ? arg{parameterIndex}_{i} : ({type})default";
					}

					if (x.Modifiers.Any(x => x.IsKind(SyntaxKind.RefKeyword)))
					{
						return $"ref arg{parameterIndex}_{i}";
					}

					return $"arg{parameterIndex}_{i}";
				});

				var parameters = parameters0.ToArray();
				var requiredParameters = method.ParameterList.Parameters.Where(x => x.Default == null && x.Type is not NullableTypeSyntax);

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

					if (parameter.Default == null && !parameter.Modifiers.Any(y => y.IsKind(SyntaxKind.OutKeyword)) && parameter.Type is not NullableTypeSyntax && !(parameter.Type is ITypeSymbol symbol && symbol.IsValueType))
					{
						var type = parameter.Type.ToString().Replace("global::", string.Empty);

						varText += $"var narg{parameterIndex}_{i} = length > {parameterIndex} ? args[{parameterIndex}] is {type} or null : true;\nvar arg{parameterIndex}_{i} = length > {parameterIndex} && narg{parameterIndex}_{i} ? ({type})(args[{parameterIndex}] ?? ({type})default) : ({type})default;\n";
						parameterText += !IsUnmanagedType(parameter.Type) ? $"narg{parameterIndex}_{i} && " : $"(narg{parameterIndex}_{i} || args[{parameterIndex}] == null) && ";
					}
				}

				if (!string.IsNullOrEmpty(parameterText))
				{
					parameterText = parameterText[..^3];
				}

				methodContents += $"{(string.IsNullOrEmpty(conditional) ? string.Empty : $"\n#if {conditional}")}\t\t\t\n\t\t\t\t" +
					$"{varText}{(string.IsNullOrEmpty(parameterText) ? string.Empty : $"if({parameterText}) {{")} {(method.ReturnType.ToString() != "void" ? $"return " : string.Empty)}" +
					$"{methodName}({string.Join(", ", parameters)}); {refSets} " +
					$"{(string.IsNullOrEmpty(parameterText) ? string.Empty : $"}}")}{(string.IsNullOrEmpty(conditional) ? string.Empty : $"\n#endif")}\n";

				Array.Clear(parameters, 0, parameters.Length);
				parameters = null;
				parameters0 = null;
				requiredParameters = null;

				overrideCount++;
			}

			methodContents += "\t\t\t\tbreak;\n\t\t\t}";
		}

		methodContents += "}\n}\ncatch (System.Exception ex)\n{\nCarbon.Logger.Error($\"Failed to call internal hook '{Carbon.Pooling.HookStringPool.GetOrAdd(hook)}' on " + baseName + " '{" + (baseName == "plugin" ? "base.Name" : "this.Name") + "} v{ " + (baseName == "plugin" ? "base.Version" : "this.Version") + "}' [{hook}]\", ex);\n" +
		                  "\nOnException(hook);\n}\n" +
			$"return {(baseCall ? "base.InternalCallHook(hook, args)" : "(object)null")};";

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

		output = input.WithMembers(input.Members.RemoveAt(namespaceIndex).Insert(namespaceIndex, @namespace.WithMembers(@namespace.Members.RemoveAt(classIndex).Insert(classIndex, @class.WithMembers(@class.Members.Insert(@class.Members.Count, generatedMethod))))));

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

	public static void GeneratePartial(CompilationUnitSyntax input, out CompilationUnitSyntax output, CSharpParseOptions options, string fileName, List<ClassDeclarationSyntax> classes = null)
	{
		GenerateInternalCallHook(input, out _, out var method, out var isPartial, classList: classes);

		if(method == null)
		{
			output = null;
			return;
		}

		var @namespace = (BaseNamespaceDeclarationSyntax)null;
		var @class = (ClassDeclarationSyntax)null;

		if (classes == null)
		{
			classes = Facepunch.Pool.Get<List<ClassDeclarationSyntax>>();
			FindPluginInfo(input, out @namespace, out _, out _, classes);

			@class = classes[0];
			Facepunch.Pool.FreeUnmanaged(ref classes);
		}
		else
		{
			@namespace = classes[0].Parent as BaseNamespaceDeclarationSyntax;
			@class = classes[0];
		}

		var usings = input.Usings;
		var subUsings = @namespace.Usings;

		var source = @$"{usings.Select(x => x.ToString()).ToString("\n")}

namespace {@namespace.Name};
{(subUsings.Any() ? $"\n{subUsings.Select(x => x.ToString()).ToString("\n")}" : string.Empty)}
partial class {@class.Identifier.ValueText}
{{
	{method}
}}";

		string path;

#if DEBUG
		if (isPartial)
		{
			path = Path.Combine(Defines.GetScriptDebugFolder(), $"{Path.GetFileNameWithoutExtension(fileName)}.Internal.cs");
			output = CSharpSyntaxTree.ParseText(source, options, path, Encoding.UTF8).GetCompilationUnitRoot().NormalizeWhitespace();
			OsEx.File.Create(path, output.ToFullString());
		}
		else
		{
			path = $"{fileName}/Internal";
			output = CSharpSyntaxTree.ParseText(source, options, path, Encoding.UTF8).GetCompilationUnitRoot();
		}
#else
			path = $"{fileName}/Internal";
			output = CSharpSyntaxTree.ParseText(source, options, path, Encoding.UTF8).GetCompilationUnitRoot();
#endif
	}

	public static void HandleVersionConditionals(CompilationUnitSyntax input, List<string> conditionals)
	{
		var directives = GetDirectives();

		foreach (var directive in directives)
		{
			var processedDirective = directive.Replace(_ifDirective, string.Empty).Replace(_elifDirective, string.Empty).Trim();

			using var subdirectives = TempArray<string>.New(processedDirective.Split(_operatorsStrings, StringSplitOptions.RemoveEmptyEntries));

			foreach (var subdirective in subdirectives.array)
			{
				var processedSubdirective = subdirective.Trim();

				using var split = TempArray<string>.New(processedSubdirective.Split(_underscoreChar));

				if (split.Length < 3)
				{
					continue;
				}

				var mode = split.Get(0);
				var type = split.Get(1);

				var major = split.Get(2).ToInt();
				var minor = split.Get(3).ToInt();
				var patch = split.Get(4).ToInt();
				var expected = new VersionNumber(major, minor, patch);

				switch (mode)
				{
					case "RUST":
					{
						var current = new VersionNumber(Rust.Protocol.network, Rust.Protocol.save, Rust.Protocol.report);

						if ((type.Equals("ABV") && current > expected) ||
							(type.Equals("BLW") && current < expected) ||
							(type.Equals("IS") && current == expected))
						{
							conditionals.Add(processedSubdirective);
						}

						break;
					}

					case "CARBON":
					{
						using var protocol = TempArray<string>.New(Community.Runtime.Analytics.Protocol.Split(_dotChar));

						var current = new VersionNumber(protocol.Get(0).ToInt(), protocol.Get(1).ToInt(), protocol.Get(2).ToInt());

						if ((type.Equals("ABV") && current > expected) ||
							(type.Equals("BLW") && current < expected) ||
							(type.Equals("IS") && current == expected))
						{
							conditionals.Add(processedSubdirective);
						}

						break;
					}
				}
			}

		}

		IEnumerable<string> GetDirectives()
		{
			foreach (var child in input.DescendantNodesAndTokensAndSelf())
			{
				if (!child.ContainsDirectives)
				{
					continue;
				}

				var node = child.AsNode();

				if (node != null && (node.IsKind(SyntaxKind.IfDirectiveTrivia) || node.IsKind(SyntaxKind.ElifDirectiveTrivia)))
				{
					var element = node.GetFirstDirective();

					if (element != null)
					{
						yield return element.GetText().ToString();
					}
				}
				else
				{
					foreach (var element in child.AsToken().LeadingTrivia.Where(x => x.IsDirective && (x.IsKind(SyntaxKind.IfDirectiveTrivia) || x.IsKind(SyntaxKind.ElifDirectiveTrivia))).Select(x => x.GetStructure()))
					{
						yield return element.GetText().ToString();
					}
				}
			}
		}
	}

	public static bool FindPluginInfo(CompilationUnitSyntax input, out BaseNamespaceDeclarationSyntax @namespace, out int namespaceIndex, out int classIndex, List<ClassDeclarationSyntax> classes)
	{
		var @class = (ClassDeclarationSyntax)null;
		@namespace = null;
		namespaceIndex = 0;
		classIndex = 0;

		for(int n = 0; n < input.Members.Count; n++)
		{
			var memberA = input.Members[n];

			if (memberA is not BaseNamespaceDeclarationSyntax ns)
			{
				continue;
			}

			for(int c = 0; c < ns.Members.Count; c++)
			{
				var memberB = ns.Members[c];

				if (memberB is not ClassDeclarationSyntax cls)
				{
					continue;
				}

				if (cls.AttributeLists.Count > 0)
				{
					foreach (var attribute in cls.AttributeLists)
					{
						if (attribute.Attributes[0].Name is IdentifierNameSyntax nameSyntax && nameSyntax.Identifier.Text.Equals("Info"))
						{
							@namespaceIndex = n;
							@namespace = ns;
							classIndex = c;
							@class = cls;
							classes?.Insert(0, @class);
						}
					}
				}
				else if (cls.Modifiers.Any(x => x.IsKind(SyntaxKind.PartialKeyword)))
				{
					classes?.Add(cls);
				}
			}
		}

		return @class != null;
	}

	public static bool IsUnmanagedType(TypeSyntax type)
	{
		return type is ITypeSymbol symbol && symbol.IsUnmanagedType;
	}

	#endregion
}
