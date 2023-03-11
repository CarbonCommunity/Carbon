using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Carbon.Base;
using Carbon.Core;
using Carbon.Extensions;
using Facepunch;
using Oxide.Plugins;
using static Carbon.HookCallerCommon;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon
{
	public class HookCallerCommon
	{
		public Dictionary<int, object[]> _argumentBuffer { get; } = new Dictionary<int, object[]>();
		public Dictionary<string, int> _hookTimeBuffer { get; } = new Dictionary<string, int>();
		public Dictionary<string, int> _hookTotalTimeBuffer { get; } = new Dictionary<string, int>();
		public Dictionary<string, DateTime> _lastDeprecatedWarningAt { get; } = new Dictionary<string, DateTime>();

		public virtual void AppendHookTime(string hook, int time) { }
		public virtual void ClearHookTime(string hook) { }

		public virtual object[] AllocateBuffer(int count) => null;
		public virtual object[] RescaleBuffer(object[] oldBuffer, int newScale) => null;
		public virtual void ClearBuffer(object[] buffer) { }

		public virtual object CallHook<T>(T plugin, string hookName, BindingFlags flags, object[] args) where T : BaseHookable => null;
		public virtual object CallDeprecatedHook<T>(T plugin, string oldHook, string newHook, DateTime expireDate, BindingFlags flags, object[] args) where T : BaseHookable => null;

		public struct Conflict
		{
			public BaseHookable Hookable;
			public string Hook;
			public object Result;

			public static Conflict Make(BaseHookable hookable, string hook, object result) => new() { Hookable = hookable, Hook = hook, Result = result };
		}

		public static Delegate CreateDelegate(MethodInfo methodInfo, object target)
		{
			Func<Type[], Type> getType;
			var isAction = methodInfo.ReturnType.Equals((typeof(void)));
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

			return Delegate.CreateDelegate(getType(types.ToArray()), target, methodInfo.Name);
		}
	}

	public static class HookCaller
	{
		public static HookCallerCommon Caller { get; set; }

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

		private static object CallStaticHook(string hookName, BindingFlags flag = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, object[] args = null)
		{
			Caller.ClearHookTime(hookName);

			var result = (object)null;
			var conflicts = Pool.GetList<Conflict>();

			foreach (var module in Community.Runtime.ModuleProcessor.Modules)
			{
				var methodResult = Caller.CallHook(module, hookName, flags: flag, args: args);

				if (methodResult != null)
				{
					result = methodResult;
					ResultOverride(module);
				}
			}

			var array = args == null || args.Length == 0 ? null : args.ToArray();
			var plugins = Pool.GetList<RustPlugin>();

			foreach (var mod in Loader.LoadedMods)
			{
				foreach (var plugin in mod.Plugins)
				{
					plugins.Add(plugin);
				}
			}

			foreach (var plugin in plugins)
			{
				try
				{
					var methodResult = Caller.CallHook(plugin, hookName, flags: flag, args: array);

					if (methodResult != null)
					{
						result = methodResult;
						ResultOverride(plugin);
					}
				}
				catch (Exception ex) { Logger.Error("Fuck:", ex); }
			}

			ConflictCheck();

			Pool.FreeList(ref plugins);

			if (array != null) Pool.Free(ref array);

			void ResultOverride(BaseHookable hookable)
			{
				conflicts.Add(Conflict.Make(hookable, hookName, result));
			}
			void ConflictCheck()
			{
				if (conflicts.Count > 1)
				{
					var differentResults = false;
					var localResult = conflicts[0].Result;
					foreach (var conflict in conflicts)
					{
						if (conflict.Result != localResult)
						{
							localResult = conflict.Result;
						}
						else differentResults = true;
					}

					if (differentResults) Carbon.Logger.Warn($"Calling hook '{hookName}' resulted in a conflict between the following plugins: {conflicts.Select(x => $"{x.Hookable.Name} {x.Hookable.Version} [{x.Result}]").ToArray().ToString(", ", " and ")}");
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

		public static object CallHook(BaseHookable plugin, string hookName)
		{
			return CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, null);
		}
		public static T CallHook<T>(BaseHookable plugin, string hookName)
		{
			return (T)CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, null);
		}
		public static T CallDeprecatedHook<T>(BaseHookable plugin, string oldHook, string newHook, DateTime expireDate)
		{
			return (T)Caller.CallDeprecatedHook(plugin, oldHook, newHook, expireDate, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, null);
		}
		public static object CallHook(BaseHookable plugin, string hookName, object arg1)
		{
			var buffer = Caller.AllocateBuffer(1);
			buffer[0] = arg1;

			var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

			Caller.ClearBuffer(buffer);
			return result;
		}
		public static T CallHook<T>(BaseHookable plugin, string hookName, object arg1)
		{
			var buffer = Caller.AllocateBuffer(1);
			buffer[0] = arg1;

			var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

			Caller.ClearBuffer(buffer);
			return (T)result;
		}
		public static T CallDeprecatedHook<T>(BaseHookable plugin, string oldHook, string newHook, DateTime expireDate, object arg1)
		{
			var buffer = Caller.AllocateBuffer(1);
			buffer[0] = arg1;

			var result = Caller.CallDeprecatedHook(plugin, oldHook, newHook, expireDate, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

			Caller.ClearBuffer(buffer);
			return (T)result;
		}
		public static object CallHook(BaseHookable plugin, string hookName, object arg1, object arg2)
		{
			var buffer = Caller.AllocateBuffer(2);
			buffer[0] = arg1;
			buffer[1] = arg2;

			var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

			Caller.ClearBuffer(buffer);
			return result;
		}
		public static T CallHook<T>(BaseHookable plugin, string hookName, object arg1, object arg2)
		{
			var buffer = Caller.AllocateBuffer(2);
			buffer[0] = arg1;
			buffer[1] = arg2;

			var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

			Caller.ClearBuffer(buffer);
			return (T)result;
		}
		public static T CallDeprecatedHook<T>(BaseHookable plugin, string oldHook, string newHook, DateTime expireDate, object arg1, object arg2)
		{
			var buffer = Caller.AllocateBuffer(2);
			buffer[0] = arg1;
			buffer[1] = arg2;

			var result = Caller.CallDeprecatedHook(plugin, oldHook, newHook, expireDate, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

			Caller.ClearBuffer(buffer);
			return (T)result;
		}
		public static object CallHook(BaseHookable plugin, string hookName, object arg1, object arg2, object arg3)
		{
			var buffer = Caller.AllocateBuffer(3);
			buffer[0] = arg1;
			buffer[1] = arg2;
			buffer[2] = arg3;

			var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

			Caller.ClearBuffer(buffer);
			return result;
		}
		public static T CallHook<T>(BaseHookable plugin, string hookName, object arg1, object arg2, object arg3)
		{
			var buffer = Caller.AllocateBuffer(3);
			buffer[0] = arg1;
			buffer[1] = arg2;
			buffer[2] = arg3;

			var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

			Caller.ClearBuffer(buffer);
			return (T)result;
		}
		public static T CallDeprecatedHook<T>(BaseHookable plugin, string oldHook, string newHook, DateTime expireDate, object arg1, object arg2, object arg3)
		{
			var buffer = Caller.AllocateBuffer(3);
			buffer[0] = arg1;
			buffer[1] = arg2;
			buffer[2] = arg3;

			var result = Caller.CallDeprecatedHook(plugin, oldHook, newHook, expireDate, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

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

			var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

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

			var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

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

			var result = Caller.CallDeprecatedHook(plugin, oldHook, newHook, expireDate, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

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

			var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

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

			var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

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

			var result = Caller.CallDeprecatedHook(plugin, oldHook, newHook, expireDate, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

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
			buffer[6] = arg6;

			var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

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
			buffer[6] = arg6;

			var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

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
			buffer[6] = arg6;

			var result = Caller.CallDeprecatedHook(plugin, oldHook, newHook, expireDate, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

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
			buffer[6] = arg6;
			buffer[7] = arg7;

			var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

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
			buffer[6] = arg6;
			buffer[7] = arg7;

			var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

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

			var result = Caller.CallDeprecatedHook(plugin, oldHook, newHook, expireDate, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

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
			buffer[6] = arg6;
			buffer[7] = arg7;
			buffer[8] = arg8;

			var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

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
			buffer[6] = arg6;
			buffer[7] = arg7;
			buffer[8] = arg8;

			var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

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
			buffer[6] = arg6;
			buffer[7] = arg7;
			buffer[8] = arg8;

			var result = Caller.CallDeprecatedHook(plugin, oldHook, newHook, expireDate, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

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
			buffer[6] = arg6;
			buffer[7] = arg7;
			buffer[8] = arg8;
			buffer[9] = arg9;

			var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

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
			buffer[6] = arg6;
			buffer[7] = arg7;
			buffer[8] = arg8;
			buffer[9] = arg9;

			var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

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
			buffer[6] = arg6;
			buffer[7] = arg7;
			buffer[8] = arg8;
			buffer[9] = arg9;

			var result = Caller.CallDeprecatedHook(plugin, oldHook, newHook, expireDate, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

			Caller.ClearBuffer(buffer);
			return (T)result;
		}
		public static object CallHook(BaseHookable plugin, string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
		{
			var buffer = Caller.AllocateBuffer(9);
			buffer[0] = arg1;
			buffer[1] = arg2;
			buffer[2] = arg3;
			buffer[3] = arg4;
			buffer[4] = arg5;
			buffer[6] = arg6;
			buffer[7] = arg7;
			buffer[8] = arg8;
			buffer[9] = arg9;
			buffer[10] = arg10;

			var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

			Caller.ClearBuffer(buffer);
			return result;
		}
		public static T CallHook<T>(BaseHookable plugin, string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
		{
			var buffer = Caller.AllocateBuffer(9);
			buffer[0] = arg1;
			buffer[1] = arg2;
			buffer[2] = arg3;
			buffer[3] = arg4;
			buffer[4] = arg5;
			buffer[6] = arg6;
			buffer[7] = arg7;
			buffer[8] = arg8;
			buffer[9] = arg9;
			buffer[10] = arg10;

			var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

			Caller.ClearBuffer(buffer);
			return (T)result;
		}
		public static T CallDeprecatedHook<T>(BaseHookable plugin, string oldHook, string newHook, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
		{
			var buffer = Caller.AllocateBuffer(9);
			buffer[0] = arg1;
			buffer[1] = arg2;
			buffer[2] = arg3;
			buffer[3] = arg4;
			buffer[4] = arg5;
			buffer[6] = arg6;
			buffer[7] = arg7;
			buffer[8] = arg8;
			buffer[9] = arg9;
			buffer[10] = arg10;

			var result = Caller.CallDeprecatedHook(plugin, oldHook, newHook, expireDate, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

			Caller.ClearBuffer(buffer);
			return (T)result;
		}
		public static object CallHook(BaseHookable plugin, string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
		{
			var buffer = Caller.AllocateBuffer(9);
			buffer[0] = arg1;
			buffer[1] = arg2;
			buffer[2] = arg3;
			buffer[3] = arg4;
			buffer[4] = arg5;
			buffer[6] = arg6;
			buffer[7] = arg7;
			buffer[8] = arg8;
			buffer[9] = arg9;
			buffer[10] = arg10;
			buffer[11] = arg11;

			var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

			Caller.ClearBuffer(buffer);
			return result;
		}
		public static T CallHook<T>(BaseHookable plugin, string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
		{
			var buffer = Caller.AllocateBuffer(9);
			buffer[0] = arg1;
			buffer[1] = arg2;
			buffer[2] = arg3;
			buffer[3] = arg4;
			buffer[4] = arg5;
			buffer[6] = arg6;
			buffer[7] = arg7;
			buffer[8] = arg8;
			buffer[9] = arg9;
			buffer[10] = arg10;
			buffer[11] = arg11;

			var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

			Caller.ClearBuffer(buffer);
			return (T)result;
		}
		public static T CallDeprecatedHook<T>(BaseHookable plugin, string oldHook, string newHook, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
		{
			var buffer = Caller.AllocateBuffer(9);
			buffer[0] = arg1;
			buffer[1] = arg2;
			buffer[2] = arg3;
			buffer[3] = arg4;
			buffer[4] = arg5;
			buffer[6] = arg6;
			buffer[7] = arg7;
			buffer[8] = arg8;
			buffer[9] = arg9;
			buffer[10] = arg10;
			buffer[11] = arg11;

			var result = Caller.CallDeprecatedHook(plugin, oldHook, newHook, expireDate, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

			Caller.ClearBuffer(buffer);
			return (T)result;
		}
		public static object CallHook(BaseHookable plugin, string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
		{
			var buffer = Caller.AllocateBuffer(9);
			buffer[0] = arg1;
			buffer[1] = arg2;
			buffer[2] = arg3;
			buffer[3] = arg4;
			buffer[4] = arg5;
			buffer[6] = arg6;
			buffer[7] = arg7;
			buffer[8] = arg8;
			buffer[9] = arg9;
			buffer[10] = arg10;
			buffer[11] = arg11;
			buffer[12] = arg12;

			var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

			Caller.ClearBuffer(buffer);
			return result;
		}
		public static T CallHook<T>(BaseHookable plugin, string hookName, object arg1 = null, object arg2 = null, object arg3 = null, object arg4 = null, object arg5 = null, object arg6 = null, object arg7 = null, object arg8 = null, object arg9 = null, object arg10 = null, object arg11 = null, object arg12 = null)
		{
			var buffer = Caller.AllocateBuffer(9);
			buffer[0] = arg1;
			buffer[1] = arg2;
			buffer[2] = arg3;
			buffer[3] = arg4;
			buffer[4] = arg5;
			buffer[6] = arg6;
			buffer[7] = arg7;
			buffer[8] = arg8;
			buffer[9] = arg9;
			buffer[10] = arg10;
			buffer[11] = arg11;
			buffer[12] = arg12;

			var result = Caller.CallHook(plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

			Caller.ClearBuffer(buffer);
			return (T)result;
		}
		public static T CallDeprecatedHook<T>(BaseHookable plugin, string oldHook, string newHook, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
		{
			var buffer = Caller.AllocateBuffer(9);
			buffer[0] = arg1;
			buffer[1] = arg2;
			buffer[2] = arg3;
			buffer[3] = arg4;
			buffer[4] = arg5;
			buffer[6] = arg6;
			buffer[7] = arg7;
			buffer[8] = arg8;
			buffer[9] = arg9;
			buffer[10] = arg10;
			buffer[11] = arg11;
			buffer[12] = arg12;

			var result = Caller.CallDeprecatedHook(plugin, oldHook, newHook, expireDate, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, buffer);

			Caller.ClearBuffer(buffer);
			return (T)result;
		}

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
			buffer[6] = arg6;

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
			buffer[6] = arg6;

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
			buffer[6] = arg6;
			buffer[7] = arg7;

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
			buffer[6] = arg6;
			buffer[7] = arg7;

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
			buffer[6] = arg6;
			buffer[7] = arg7;
			buffer[8] = arg8;

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
			buffer[6] = arg6;
			buffer[7] = arg7;
			buffer[8] = arg8;

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
			buffer[6] = arg6;
			buffer[7] = arg7;
			buffer[8] = arg8;
			buffer[9] = arg9;

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
			buffer[6] = arg6;
			buffer[7] = arg7;
			buffer[8] = arg8;
			buffer[9] = arg9;

			var result = CallStaticDeprecatedHook(oldHook, newHook, expireDate, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

			Caller.ClearBuffer(buffer);
			return result;
		}
		public static object CallStaticHook(string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
		{
			var buffer = Caller.AllocateBuffer(9);
			buffer[0] = arg1;
			buffer[1] = arg2;
			buffer[2] = arg3;
			buffer[3] = arg4;
			buffer[4] = arg5;
			buffer[6] = arg6;
			buffer[7] = arg7;
			buffer[8] = arg8;
			buffer[9] = arg9;
			buffer[10] = arg10;

			var result = CallStaticHook(hookName, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

			Caller.ClearBuffer(buffer);
			return result;
		}
		public static object CallStaticDeprecatedHook(string oldHook, string newHook, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
		{
			var buffer = Caller.AllocateBuffer(9);
			buffer[0] = arg1;
			buffer[1] = arg2;
			buffer[2] = arg3;
			buffer[3] = arg4;
			buffer[4] = arg5;
			buffer[6] = arg6;
			buffer[7] = arg7;
			buffer[8] = arg8;
			buffer[9] = arg9;
			buffer[10] = arg10;

			var result = CallStaticDeprecatedHook(oldHook, newHook, expireDate, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

			Caller.ClearBuffer(buffer);
			return result;
		}
		public static object CallStaticHook(string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
		{
			var buffer = Caller.AllocateBuffer(9);
			buffer[0] = arg1;
			buffer[1] = arg2;
			buffer[2] = arg3;
			buffer[3] = arg4;
			buffer[4] = arg5;
			buffer[6] = arg6;
			buffer[7] = arg7;
			buffer[8] = arg8;
			buffer[9] = arg9;
			buffer[10] = arg10;
			buffer[11] = arg11;

			var result = CallStaticHook(hookName, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

			Caller.ClearBuffer(buffer);
			return result;
		}
		public static object CallStaticDeprecatedHook(string oldHook, string newHook, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
		{
			var buffer = Caller.AllocateBuffer(9);
			buffer[0] = arg1;
			buffer[1] = arg2;
			buffer[2] = arg3;
			buffer[3] = arg4;
			buffer[4] = arg5;
			buffer[6] = arg6;
			buffer[7] = arg7;
			buffer[8] = arg8;
			buffer[9] = arg9;
			buffer[10] = arg10;
			buffer[11] = arg11;

			var result = CallStaticDeprecatedHook(oldHook, newHook, expireDate, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

			Caller.ClearBuffer(buffer);
			return result;
		}
		public static object CallStaticHook(string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
		{
			var buffer = Caller.AllocateBuffer(9);
			buffer[0] = arg1;
			buffer[1] = arg2;
			buffer[2] = arg3;
			buffer[3] = arg4;
			buffer[4] = arg5;
			buffer[6] = arg6;
			buffer[7] = arg7;
			buffer[8] = arg8;
			buffer[9] = arg9;
			buffer[10] = arg10;
			buffer[11] = arg11;
			buffer[12] = arg12;

			var result = CallStaticHook(hookName, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

			Caller.ClearBuffer(buffer);
			return result;
		}
		public static object CallStaticDeprecatedHook(string oldHook, string newHook, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
		{
			var buffer = Caller.AllocateBuffer(9);
			buffer[0] = arg1;
			buffer[1] = arg2;
			buffer[2] = arg3;
			buffer[3] = arg4;
			buffer[4] = arg5;
			buffer[6] = arg6;
			buffer[7] = arg7;
			buffer[8] = arg8;
			buffer[9] = arg9;
			buffer[10] = arg10;
			buffer[11] = arg11;
			buffer[12] = arg12;

			var result = CallStaticDeprecatedHook(oldHook, newHook, expireDate, flag: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, args: buffer);

			Caller.ClearBuffer(buffer);
			return result;
		}

		private static object CallPublicHook<T>(this T plugin, string hookName, object[] args) where T : BaseHookable
		{
			return CallHook(plugin, hookName, BindingFlags.Public | BindingFlags.Instance, args);
		}
		private static object CallPublicStaticHook(string hookName, object[] args)
		{
			return CallStaticHook(hookName, BindingFlags.Public | BindingFlags.Instance, args);
		}
	}
}
