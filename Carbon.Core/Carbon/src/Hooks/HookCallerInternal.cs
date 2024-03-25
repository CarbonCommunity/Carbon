using System;
using System.Collections.Generic;
using System.Reflection;
using Carbon.Components;
using Carbon.Extensions;
using Carbon.Pooling;
using Facepunch;
using Oxide.Core.Plugins;
using static Carbon.Base.BaseHookable;

/*
 *
 * Copyright (c) 2022-2024 Carbon Community
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;

public class HookCallerInternal : HookCallerCommon
{
	public override object[] AllocateBuffer(int count)
	{
		if (!_argumentBuffer.TryGetValue(count, out var pool))
		{
			_argumentBuffer.Add(count, pool = new HookArgPool(count, 15));
		}

		return pool.Take();
	}
	public override object[] RescaleBuffer(object[] oldBuffer, int newScale, CachedHook hook)
	{
		if (oldBuffer.Length == newScale)
		{
			return oldBuffer;
		}

		var newBuffer = AllocateBuffer(newScale);

		for (int i = 0; i < newScale; i++)
		{
			var value = newBuffer[i];

			if (i > oldBuffer.Length - 1)
			{
				newBuffer[i] = value = null;
			}
			else
			{
				newBuffer[i] = value = oldBuffer[i];
			}

			if (i <= hook.InfoParameters.Length - 1 && value == null)
			{
				var parameter = hook.InfoParameters[i];

				if (parameter.HasDefaultValue)
				{
					newBuffer[i] = parameter.DefaultValue;
				}
				// else if (parameter.ParameterType is { IsValueType: true } parameterType)
				// {
				// 	newBuffer[i] = Activator.CreateInstance(parameterType);
				// }
			}
		}

		return newBuffer;
	}
	public override void ProcessDefaults(object[] buffer, CachedHook hook)
	{
		var length = buffer.Length;

		for (int i = 0; i < length; i++)
		{
			if (i <= hook.InfoParameters.Length - 1 && buffer[i] == null)
			{
				if (hook.InfoParameters[i] is ParameterInfo info && info.HasDefaultValue)
				{
					buffer[i] = info.DefaultValue;
				}
				else
				{
					buffer[i] = null;
				}
			}
		}
	}
	public override void ReturnBuffer(object[] buffer)
	{
		if (_argumentBuffer.TryGetValue(buffer.Length, out var pool))
		{
			pool.Return(buffer);
		}
	}

	internal static Conflict _defaultConflict = new();

	public override object CallHook<T>(T hookable, uint hookId, BindingFlags flags, object[] args)
	{
		if (hookable.IsHookIgnored(hookId))
		{
			return null;
		}

		hookable.BuildHookCache(flags);

		List<CachedHook> hooks = null;

		if (hookable.HookCache != null && !hookable.HookCache.TryGetValue(hookId, out hooks))
		{
			return null;
		}

		var result = (object)null;
		var conflicts = Pool.GetList<Conflict>();
		var hasRescaledBuffer = false;

		if (hookable.InternalCallHookOverriden)
		{
			var cachedHook = (CachedHook)default;

			if (hooks != null && hooks.Count > 0)
			{
				cachedHook = hooks[0];

				if (args != null)
				{
					var actualLength = cachedHook.Parameters.Length;

					if (actualLength != args.Length)
					{
						args = RescaleBuffer(args, actualLength, cachedHook);
						hasRescaledBuffer = true;
					}
					else
					{
						ProcessDefaults(args, cachedHook);
					}
				}
			}

#if DEBUG
			Profiler.StartHookCall(hookable, hookId);
#endif

			hookable.TrackStart();
			var beforeMemory = hookable.TotalMemoryUsed;

			if (cachedHook != null && cachedHook.IsAsync)
			{
				hookable.InternalCallHook(hookId, args);
				hookable.TrackEnd();
			}
			else
			{
				var currentResult = hookable.InternalCallHook(hookId, args);
				hookable.TrackEnd();

				if (currentResult != null)
				{
					HookCaller.ResultOverride(conflicts, hookable, hookId, result = currentResult);
				}
			}

			var afterHookTime = hookable.CurrentHookTime;
			var afterMemory = hookable.TotalMemoryUsed;
			var totalMemory = afterMemory - beforeMemory;

#if DEBUG
			Profiler.EndHookCall(hookable);
#endif

			if (cachedHook != null)
			{
				cachedHook.HookTime += afterHookTime;
				cachedHook.MemoryUsage += totalMemory;
				cachedHook.Tick();
			}

			if (afterHookTime.TotalMilliseconds > 100 && hookable is Plugin basePlugin && !basePlugin.IsCorePlugin)
			{
				var readableHook = HookStringPool.GetOrAdd(hookId);

				Carbon.Logger.Warn($" {hookable.Name} hook '{readableHook}' took longer than 100ms [{afterHookTime.TotalMilliseconds:0}ms]{(hookable.HasGCCollected ? " [GC]" : string.Empty)}");

				Analytics.plugin_time_warn(readableHook, basePlugin, afterHookTime.TotalMilliseconds, totalMemory, cachedHook, hookable);
			}

			HookCaller.ConflictCheck(conflicts, ref result, hookId);
			FrameDispose(hasRescaledBuffer, args, ref conflicts);
		}
		else
		{
			if (hooks != null)
			{
				foreach (var cachedHook in hooks)
				{
					try
					{
						if (cachedHook.IsAsync)
						{
							DoCall(hookable, hookId, cachedHook, args, ref hasRescaledBuffer);
						}
						else
						{
							var currentResult = DoCall(hookable, hookId, cachedHook, args, ref hasRescaledBuffer);

							if (currentResult != null)
							{
								HookCaller.ResultOverride(conflicts, hookable, hookId, result = currentResult);
							}
						}
					}
					catch (Exception ex)
					{
						var exception = ex.InnerException ?? ex;
						var readableHook = HookStringPool.GetOrAdd(hookId);
						Carbon.Logger.Error(
							$"Failed to call hook '{readableHook}' on {(hookable is BaseModule ? "module" : "plugin" )} '{hookable.Name} v{hookable.Version}'",
							exception
						);
					}
				}
			}

			HookCaller.ConflictCheck(conflicts, ref result, hookId);
			FrameDispose(false, args, ref conflicts);

			static object DoCall(T hookable, uint hookId, CachedHook cachedHook, object[] args, ref bool hasRescaledBuffer)
			{
				if (args != null)
				{
					var actualLength = cachedHook.Parameters.Length;

					if (actualLength != args.Length)
					{
						args = HookCaller.Caller.RescaleBuffer(args, actualLength, cachedHook);
						hasRescaledBuffer = true;
					}
					else
					{
						HookCaller.Caller.ProcessDefaults(args, cachedHook);
					}
				}

				if (args == null || SequenceEqual(cachedHook.Parameters, args))
				{
#if DEBUG
					Profiler.StartHookCall(hookable, hookId);
#endif

					var result2 = (object)default;
					hookable.TrackStart();
					var beforeMemory = hookable.TotalMemoryUsed;

					try
					{
						result2 = cachedHook.Method.Invoke(hookable, args);
					}
					catch (Exception ex)
					{
						var exception = ex.InnerException ?? ex;
						var readableHook = HookStringPool.GetOrAdd(hookId);
						Carbon.Logger.Error(
							$"Failed to call hook '{readableHook}' on {(hookable is BaseModule ? "module" : "plugin" )} '{hookable.Name} v{hookable.Version}'",
							exception
						);
					}

					hookable.TrackEnd();
					var afterHookTime = hookable.CurrentHookTime;
					var afterMemory = hookable.TotalMemoryUsed;
					var totalMemory = afterMemory - beforeMemory;

					if (cachedHook != null)
					{
						cachedHook.HookTime += afterHookTime;
						cachedHook.MemoryUsage += totalMemory;
						cachedHook.Tick();
					}

					if (afterHookTime.TotalMilliseconds > 100)
					{
						if (hookable is Plugin basePlugin && !basePlugin.IsCorePlugin)
						{
							var readableHook = HookStringPool.GetOrAdd(hookId);
							Carbon.Logger.Warn($" {hookable.Name} hook '{readableHook}' took longer than 100ms [{afterHookTime.TotalMilliseconds:0}ms]{(hookable.HasGCCollected ? " [GC]" : string.Empty)}");

							Analytics.plugin_time_warn(readableHook, basePlugin, afterHookTime.TotalMilliseconds, totalMemory, cachedHook, hookable);
						}
					}

#if DEBUG
					Profiler.EndHookCall(hookable);
#endif
					if (hasRescaledBuffer)
					{
						HookCaller.Caller.ReturnBuffer(args);
					}

					return result2;
				}
				return null;
			}
		}

		static void FrameDispose(bool hasRescaledBuffer, object[] buffer, ref List<Conflict> conflicts)
		{
			if (hasRescaledBuffer)
			{
				HookCaller.Caller.ReturnBuffer(buffer);
			}

			Pool.FreeList(ref conflicts);
		}

		return result;
	}
	public override object CallDeprecatedHook<T>(T plugin, uint oldHook, uint newHook, DateTime expireDate, BindingFlags flags, object[] args)
	{
		if (expireDate < DateTime.Now)
		{
			return null;
		}

		var now = DateTime.Now;

		if (!_lastDeprecatedWarningAt.TryGetValue(oldHook, out var lastWarningAt) || (now - lastWarningAt).TotalSeconds > 3600f)
		{
			_lastDeprecatedWarningAt[oldHook] = now;

			Carbon.Logger.Warn($"'{plugin.Name} v{plugin.Version}' is using deprecated hook '{oldHook}', which will stop working on {expireDate.ToString("D")}. Please ask the author to update to '{newHook}'");
		}

		return CallHook(plugin, newHook, flags, args);
	}

	internal static bool SequenceEqual(Type[] source, object[] target)
	{
		var equal = true;

		for(int i = 0; i < source.Length; i++)
		{
			var sourceItem = source[i];
			var targetItem = target[i]?.GetType();

			if (targetItem != null && !sourceItem.IsByRef && !targetItem.IsByRef &&
				sourceItem != targetItem &&
				!sourceItem.IsAssignableFrom(targetItem))
			{
				equal = false;
				break;
			}
		}

		return equal;
	}
}
