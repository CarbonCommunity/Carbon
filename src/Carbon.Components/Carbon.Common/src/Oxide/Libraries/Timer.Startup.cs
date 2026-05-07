namespace Oxide.Plugins;

using Facepunch;

public partial class Timers
{
	private static readonly object StartupTimerLock = new();
	private static readonly List<Timer> StartupTimers = [];
	private const int MaxStartupTimersPerFrame = 256;
	private const float StartupTimerDueTolerance = 0.001f;

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

				for (var i = StartupTimers.Count - 1; i >= 0; i--)
				{
					var timer = StartupTimers[i];
					if (timer.Destroyed)
					{
						StartupTimers.RemoveAt(i);
						continue;
					}

					if (timer.ExpiresAt - now > StartupTimerDueTolerance)
					{
						continue;
					}

					StartupTimers.RemoveAt(i);
					timers.Add(timer);

					if (timers.Count >= maxTimers)
					{
						break;
					}
				}
			}

			timers.Reverse();

			for (var i = 0; i < timers.Count; i++)
			{
				var timer = timers[i];
				if (!timer.Destroyed)
				{
					timer.Callback?.Invoke();
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
				if (timer.Repetitions > 1)
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
}
