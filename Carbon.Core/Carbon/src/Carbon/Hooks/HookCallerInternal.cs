using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Facepunch;

namespace Carbon.Hooks
{
	public class HookCallerInternal : HookCallerCommon
	{
		public override void AppendHookTime(string hook, int time)
		{
			if (!Community.Runtime.Config.HookTimeTracker) return;

			if (!_hookTimeBuffer.TryGetValue(hook, out var total))
			{
				_hookTimeBuffer.Add(hook, time);
			}
			else _hookTimeBuffer[hook] = total + time;

			if (!_hookTotalTimeBuffer.TryGetValue(hook, out total))
			{
				_hookTotalTimeBuffer.Add(hook, time);
			}
			else _hookTotalTimeBuffer[hook] = total + time;
		}
		public override void ClearHookTime(string hook)
		{
			if (!Community.Runtime.Config.HookTimeTracker) return;

			if (!_hookTimeBuffer.ContainsKey(hook))
			{
				_hookTimeBuffer.Add(hook, 0);
			}
			else
			{
				_hookTimeBuffer[hook] = 0;
			}
		}

		public override object[] AllocateBuffer(int count)
		{
			if (!_argumentBuffer.TryGetValue(count, out var buffer))
			{
				_argumentBuffer.Add(count, buffer = new object[count]);
			}

			return buffer;
		}
		public override object[] RescaleBuffer(object[] oldBuffer, int newScale)
		{
			if (oldBuffer.Length == newScale)
			{
				return oldBuffer;
			}

			var newBuffer = AllocateBuffer(newScale);

			for (int i = 0; i < newScale; i++)
			{
				if (i > oldBuffer.Length - 1) break;

				newBuffer[i] = oldBuffer[i];
			}

			return newBuffer;
		}
		public override void ClearBuffer(object[] buffer)
		{
			for (int i = 0; i < buffer.Length; i++)
			{
				buffer[i] = null;
			}
		}

		public override object CallHook<T>(T plugin, string hookName, BindingFlags flags, object[] args)
		{
			if (plugin.IsHookIgnored(hookName)) return null;

			var id = $"{hookName}[{(args == null ? 0 : args.Length)}]";
			var result = (object)null;
			var conflicts = Pool.GetList<Conflict>();

			if (plugin.HookMethodAttributeCache.TryGetValue(id, out var hooks))
			{
				foreach (var @delegate in hooks)
				{
					var methodResult = DoCall( @delegate.Key, @delegate.Value);
					if (methodResult != null)
					{
						result = methodResult;
					}
				}
			}

			if (!plugin.HookCache.TryGetValue(id, out hooks))
			{
				plugin.HookCache.Add(id, hooks = new List<KeyValuePair<MethodInfo, Delegate>>());

				foreach (var method in plugin.Type.GetMethods(flags))
				{
					if (method.Name != hookName) continue;

					hooks.Add(new KeyValuePair<MethodInfo, Delegate>(method, CreateDelegate(method, plugin)));
				}
			}

			foreach (var @delegate in hooks)
			{
				try
				{
					var methodResult = DoCall(@delegate.Key, @delegate.Value);

					if (methodResult != null)
					{
						result = methodResult;
					}
				}
				catch (ArgumentException) { }
				catch (TargetParameterCountException) { }
				catch (Exception ex)
				{
					var exception = ex.InnerException ?? ex;
					Carbon.Logger.Error(
						$"Failed to call hook '{hookName}' on plugin '{plugin.Name} v{plugin.Version}'",
						exception
					);
				}
			}

			object DoCall(MethodInfo info, Delegate @delegate)
			{
				if (@delegate == null)
				{
					return null;
				}

				if (args != null)
				{
					var actualLength = info.GetParameters().Length;
					if (actualLength != args.Length)
					{
						args = RescaleBuffer(args, actualLength);
					}
				}

				var beforeTicks = Environment.TickCount;
				plugin.TrackStart();
				var result2 = @delegate.DynamicInvoke(args);
				plugin.TrackEnd();
				var afterTicks = Environment.TickCount;
				var totalTicks = afterTicks - beforeTicks;

				AppendHookTime(hookName, totalTicks);

				if (afterTicks > beforeTicks + 100 && afterTicks > beforeTicks)
				{
					Carbon.Logger.Warn($" {plugin?.Name} hook took longer than 100ms {hookName} [{totalTicks:0}ms]");
				}

				return result2;
			}

			Pool.FreeList(ref conflicts);
			return result;
		}
		public override object CallDeprecatedHook<T>(T plugin, string oldHook, string newHook, DateTime expireDate, BindingFlags flags, object[] args)
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

			return CallDeprecatedHook(plugin, oldHook, newHook, expireDate, flags, args);
		}
	}
}
