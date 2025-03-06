using System.Text;
using Carbon.Base.Interfaces;
using HarmonyLib;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Carbon.HookCallerCommon;

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
		var conflicts = Facepunch.Pool.Get<List<Conflict>>();

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

		Facepunch.Pool.FreeUnmanaged(ref conflicts);

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

	private static char[] _underscoreChar = new[] { '_' };
	private static char[] _dotChar = new[] { '.' };
	private static string[] _operatorsStrings = new[] { "&&", "||" };
	private static string _ifDirective = "#if";
	private static string _elifDirective = "#elif";

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
}
