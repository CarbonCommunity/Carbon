using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Carbon.Base;
using Carbon.Components;
using Carbon.Core;
using Carbon.Extensions;
using Carbon.Pooling;
using Facepunch;
using Oxide.Core.Plugins;
using static Carbon.Base.BaseHookable;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;

public class HookCallerInternal : HookCallerCommon
{
	public override void AppendHookTime(uint hook, double time)
	{
		if(!_hookTimeBuffer.ContainsKey(hook))
		{
			_hookTimeBuffer.Add(hook, time);
		}
		else
		{
			_hookTimeBuffer[hook] += time;
		}

		if (!_hookTotalTimeBuffer.ContainsKey(hook))
		{
			_hookTotalTimeBuffer.Add(hook, time);
		}
		else
		{
			_hookTotalTimeBuffer[hook] += time;
		}
	}
	public override void ClearHookTime(uint hook)
	{
		_hookTimeBuffer[hook] = 0;
	}

	public override object[] AllocateBuffer(int count)
	{
		if (!_argumentBuffer.TryGetValue(count, out var buffer))
		{
			_argumentBuffer.Add(count, buffer = new object[count]);
		}

		return buffer;
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
				else if (parameter.ParameterType is Type parameterType && parameterType.IsValueType)
				{
					newBuffer[i] = Activator.CreateInstance(parameterType);
				}
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
	public override void ClearBuffer(object[] buffer)
	{
		for (int i = 0; i < buffer.Length; i++)
		{
			buffer[i] = null;
		}
	}

	internal static Conflict _defaultConflict = new();

	public override object CallHook<T>(T hookable, uint hookId, BindingFlags flags, object[] args, bool keepArgs = false)
	{
		var readableHook = HookStringPool.GetOrAdd(hookId);

		if (hookable.IsHookIgnored(hookId))
		{
			return null;
		}

		hookable.BuildHookCache(flags);

		List<CachedHook> hooks = null;

		if ((hookable.HookMethodAttributeCache?.TryGetValue(hookId, out hooks)).GetValueOrDefault()) { }
		else if ((hookable.HookCache?.TryGetValue(hookId, out hooks)).GetValueOrDefault()) { }

		var result = (object)null;

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
			}
			else
			{
				result = hookable.InternalCallHook(hookId, args);
			}

			hookable.TrackEnd();
			var afterHookTime = hookable.CurrentHookTime;
			var afterMemory = hookable.TotalMemoryUsed;
			var totalMemory = afterMemory - beforeMemory;

#if DEBUG
			Profiler.EndHookCall(hookable);
#endif

			AppendHookTime(hookId, afterHookTime);

			if (cachedHook != null)
			{
				cachedHook.HookTime += afterHookTime;
				cachedHook.MemoryUsage += totalMemory;
			}

			if (afterHookTime > 100)
			{
				if (hookable is Plugin basePlugin && !basePlugin.IsCorePlugin)
				{
					Carbon.Logger.Warn($" {hookable.Name} hook '{readableHook}' took longer than 100ms [{afterHookTime:0}ms]{(hookable.HasGCCollected ? " [GC]" : string.Empty)}");
					Community.Runtime.Analytics.LogEvent("plugin_time_warn",
						segments: Community.Runtime.Analytics.Segments,
						metrics: new Dictionary<string, object>
						{
							{ "name", $"{readableHook} ({basePlugin.Name} v{basePlugin.Version} by {basePlugin.Author})" },
							{ "time", $"{afterHookTime.RoundUpToNearestCount(50)}ms" },
							{ "memory", $"{ByteEx.Format(totalMemory, shortName: true).ToLower()}" },
							{ "hasgc", hookable.HasGCCollected }
						});
				}
			}
		}
		else
		{
			if (hooks != null)
			{
				for (int i = 0; i < hooks.Count; i++)
				{
					try
					{
						var cachedHook = hooks[i];

						if (cachedHook.IsByRef)
						{
							keepArgs = true;
						}

						if (cachedHook.IsAsync)
						{
							DoCall(cachedHook);
						}
						else
						{
							var methodResult = DoCall(cachedHook);

							if (methodResult != null)
							{
								result = methodResult;
							}
						}

						HookCaller.ResultOverride(hookable, hookId, result);
					}
					catch (Exception ex)
					{
						var exception = ex.InnerException ?? ex;
						Carbon.Logger.Error(
							$"Failed to call hook '{readableHook}' on plugin '{hookable.Name} v{hookable.Version}'",
							exception
						);
					}
				}
			}

			object DoCall(CachedHook hook)
			{
				if (args != null)
				{
					var actualLength = hook.Parameters.Length;

					if (actualLength != args.Length)
					{
						args = RescaleBuffer(args, actualLength, hook);
					}
					else
					{
						ProcessDefaults(args, hook);
					}
				}

				if (args == null || SequenceEqual(hook.Parameters, args))
				{
#if DEBUG
					Profiler.StartHookCall(hookable, hookId);
#endif

					var result2 = (object)default;
					hookable.TrackStart();
					var beforeMemory = hookable.TotalMemoryUsed;

					result2 = hook.Method.Invoke(hookable, args);

					hookable.TrackEnd();
					var afterHookTime = hookable.CurrentHookTime;
					var afterMemory = hookable.TotalMemoryUsed;
					var totalMemory = afterMemory - beforeMemory;

					AppendHookTime(hookId, afterHookTime);

					if (hook != null)
					{
						hook.HookTime += afterHookTime;
						hook.MemoryUsage += totalMemory;
					}

					if (afterHookTime > 100)
					{
						if (hookable is Plugin basePlugin && !basePlugin.IsCorePlugin)
						{
							var readableHook = HookStringPool.GetOrAdd(hookId);
							Carbon.Logger.Warn($" {hookable.Name} hook '{readableHook}' took longer than 100ms [{afterHookTime:0}ms]{(hookable.HasGCCollected ? " [GC]" : string.Empty)}");
							Community.Runtime.Analytics.LogEvent("plugin_time_warn",
								segments: Community.Runtime.Analytics.Segments,
								metrics: new Dictionary<string, object>
								{
									{ "name", $"{readableHook} ({basePlugin.Name} v{basePlugin.Version} by {basePlugin.Author})" },
									{ "time", $"{afterHookTime.RoundUpToNearestCount(50)}ms" },
									{ "memory", $"{ByteEx.Format(totalMemory, shortName: true).ToLower()}" },
									{ "hasgc", hookable.HasGCCollected }
								});
						}
					}

#if DEBUG
					Profiler.EndHookCall(hookable);
#endif
					return result2;
				}

				return null;
			}

			HookCaller.ConflictCheck(ref result, hookId);
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

	internal bool SequenceEqual(Type[] source, object[] target)
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
