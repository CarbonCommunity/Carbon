using System;
using System.Linq;
using System.Reflection;
using Carbon.Base;
using Carbon.Extensions;
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

		public override object CallHook<T>(T plugin, string hookName, BindingFlags flags, object[] args, ref Priorities priority)
		{
			priority = Priorities.Normal;

			if (plugin.IsHookIgnored(hookName)) return null;

			var id = $"{hookName}[{(args == null ? 0 : args.Length)}]";
			var result = (object)null;
			var conflicts = Pool.GetList<Conflict>();

			if (plugin.HookMethodAttributeCache.TryGetValue(id, out var hooks))
			{
				foreach (var cachedHook in hooks)
				{
					var methodResult = DoCall(cachedHook.Method, cachedHook.Delegate);
					if (methodResult != null)
					{
						priority = cachedHook.Priority;
						result = methodResult;
						ResultOverride(plugin, priority);
					}
				}
			}

			if (!plugin.HookCache.TryGetValue(id, out hooks))
			{
				plugin.HookCache.Add(id, hooks = new());

				foreach (var method in plugin.Type.GetMethods(flags))
				{
					if (method.Name != hookName) continue;

					var methodPriority = method.GetCustomAttribute<HookPriority>();
					hooks.Add(BaseHookable.CachedHook.Make(method, CreateDelegate(method, plugin), methodPriority == null ? Priorities.Normal : methodPriority.Priority));
				}
			}

			foreach (var cachedHook in hooks)
			{
				try
				{
					var methodResult = DoCall(cachedHook.Method, cachedHook.Delegate);

					if (methodResult != null)
					{
						priority = cachedHook.Priority;
						result = methodResult;
						ResultOverride(plugin, priority);
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

			ConflictCheck();

			Pool.FreeList(ref conflicts);

			void ResultOverride(BaseHookable hookable, Priorities priority)
			{
				conflicts.Add(Conflict.Make(hookable, hookName, result, priority));
			}
			void ConflictCheck()
			{
				var differentResults = false;

				if (conflicts.Count > 1)
				{
					var localResult = conflicts[0].Result;

					switch (conflicts.Count)
					{
						case 1:
							{
								foreach (var conflict in conflicts)
								{
									if (conflict.Result?.ToString() != localResult?.ToString())
									{
										differentResults = true;
									}
									else localResult = conflict.Result;
								}

								if (differentResults) Carbon.Logger.Warn($"Hook conflict while calling '{hookName}': {conflicts.Select(x => $"{x.Hookable.Name} {x.Hookable.Version} [{x.Result}]").ToArray().ToString(", ", " and ")}");

								break;
							}

						default:
							var priorityConflict = _defaultConflict;

							foreach (var conflict in conflicts)
							{
								if (conflict.Result?.ToString() != localResult?.ToString())
								{
									differentResults = true;
								}

								if (conflict.Result != null &&
									conflict.Priority > priorityConflict.Priority)
								{
									priorityConflict = conflict;
								}
							}

							localResult = priorityConflict.Result;
							if (differentResults && !conflicts.All(x => x.Priority == priorityConflict.Priority) && Community.Runtime.Config.HigherPriorityHookWarns) Carbon.Logger.Warn($"Hook conflict while calling '{hookName}', but used {priorityConflict.Hookable.Name} {priorityConflict.Hookable.Version} due to the {_getPriorityName(priorityConflict.Priority)} priority:\n  {conflicts.Select(x => $"{x.Hookable.Name} {x.Hookable.Version} [{x.Priority}:{x.Result}]").ToArray().ToString(", ", " and ")}");
							break;
					}

					result = localResult;
				}
			}

			return result;
		}
		public override object CallDeprecatedHook<T>(T plugin, string oldHook, string newHook, DateTime expireDate, BindingFlags flags, object[] args, ref Priorities priority)
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

			return CallHook(plugin, newHook, flags, args, ref priority);
		}
	}
}
