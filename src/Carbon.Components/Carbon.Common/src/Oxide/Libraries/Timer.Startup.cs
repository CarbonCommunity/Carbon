namespace Oxide.Plugins;

using Facepunch;

public partial class Timers
{
	private static readonly object StartupTimerLock = new();
	private static readonly List<Timer> StartupTimers = [];
	private const int MaxStartupTimersPerFrame = 256;
	private const float StartupTimerDueTolerance = 0.001f;
	private const float MinimumStartupRepeatDelay = 0.001f;

	internal static float NormalizeStartupRepeatDelay(float delay)
	{
		return delay > MinimumStartupRepeatDelay ? delay : MinimumStartupRepeatDelay;
	}

	internal static void QueueStartupTimer(Timer timer)
	{
		lock (StartupTimerLock)
		{
			if (StartupTimers.Contains(timer))
			{
				return;
			}

			StartupTimers.Add(timer);
		}
	}

	internal static void RemoveStartupTimer(Timer timer)
	{
		lock (StartupTimerLock)
		{
			StartupTimers.Remove(timer);
		}
	}

	public static void UpdateStartupTimers()
	{
		if (Community.IsServerInitialized)
		{
			return;
		}

		FireDueStartupTimers(MaxStartupTimersPerFrame);
	}

	public static void FireDueStartupTimers(int maxTimers = int.MaxValue)
	{
		if (maxTimers <= 0)
		{
			return;
		}

		var now = UnityEngine.Time.realtimeSinceStartup;
		var timers = Pool.Get<List<Timer>>();

		try
		{
			lock (StartupTimerLock)
			{
				if (StartupTimers.Count == 0)
				{
					return;
				}

				for (var i = 0; i < StartupTimers.Count; i++)
				{
					var timer = StartupTimers[i];
					if (timer.Destroyed)
					{
						StartupTimers.RemoveAt(i);
						i--;
						continue;
					}

					if (timer.ExpiresAt - now > StartupTimerDueTolerance)
					{
						continue;
					}

					StartupTimers.RemoveAt(i);
					i--;
					timers.Add(timer);

					if (timers.Count >= maxTimers)
					{
						break;
					}
				}
			}

			for (var i = 0; i < timers.Count; i++)
			{
				var timer = timers[i];
				if (!timer.Destroyed)
				{
					timer.Callback?.Invoke();

					if (ShouldRequeueStartupTimer(timer))
					{
						timer.ExpiresAt = UnityEngine.Time.realtimeSinceStartup + timer.Delay;
						QueueStartupTimer(timer);
					}
				}
			}
		}
		finally
		{
			Pool.FreeUnmanaged(ref timers);
		}
	}

	public static void ConvertRemainingStartupTimersToInvokes()
	{
		var timers = Pool.Get<List<Timer>>();

		try
		{
			lock (StartupTimerLock)
			{
				for (var i = 0; i < StartupTimers.Count; i++)
				{
					timers.Add(StartupTimers[i]);
				}

				StartupTimers.Clear();
			}

			var now = UnityEngine.Time.realtimeSinceStartup;
			for (var i = 0; i < timers.Count; i++)
			{
				var timer = timers[i];
				if (timer.Destroyed || timer.Persistence == null || timer.Callback == null)
				{
					continue;
				}

				var remaining = Math.Max(0f, timer.ExpiresAt - now);
				if (timer.StartupRepeating)
				{
					timer.Persistence.InvokeRepeating(timer.Callback, remaining, timer.Delay);
				}
				else
				{
					timer.Persistence.Invoke(timer.Callback, remaining);
				}
			}
		}
		finally
		{
			Pool.FreeUnmanaged(ref timers);
		}
	}

	private static bool ShouldRequeueStartupTimer(Timer timer)
	{
		if (!timer.StartupRepeating || timer.Destroyed || Community.IsServerInitialized)
		{
			return false;
		}

		return timer.Repetitions <= 0 || timer.TimesTriggered < timer.Repetitions;
	}
}
