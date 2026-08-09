using Facepunch;
using Logger = Carbon.Logger;

namespace Oxide.Core.Libraries;

public partial class Timer : Library
{
	public Plugin Plugin { get; }
	internal HashSet<TimerInstance> _timers { get; set; } = [];

	public Timer() { }
	public Timer(Plugin plugin)
	{
		Plugin = plugin;
	}

	public bool IsValid()
	{
		return Plugin != null && Plugin.persistence != null;
	}
	public void Clear()
	{
		DestroyAll();
	}

	public Plugin.Persistence Persistence => Plugin.persistence;

	internal void TrackTimer(TimerInstance timer)
	{
		timer.OwnerTimers = this;
		_timers ??= [];

		lock (SchedulerLock)
		{
			_timers.Add(timer);
		}
	}
	internal void UntrackTimer(TimerInstance timer)
	{
		if (timer.OwnerTimers != this || _timers == null)
		{
			return;
		}

		lock (SchedulerLock)
		{
			_timers.Remove(timer);
		}
	}

	public TimerInstance In(float time, Action action, Plugin plugin = null)
	{
		if (!IsValid())
		{
			return null;
		}

		var timer = new TimerInstance(Persistence, action, plugin ?? Plugin);
		timer.Delay = time;
		timer.Repetitions = 1;
		timer.Callback = action;
		timer.Tracking = ResolveTracking(action);

		TrackTimer(timer);
		Schedule(timer, CurrentTime + time);

		return timer;
	}
	public TimerInstance Once(float time, Action action, Plugin plugin = null)
	{
		return In(time, action, plugin);
	}
	public TimerInstance Every(float time, Action action, Plugin plugin = null)
	{
		if (!IsValid())
		{
			return null;
		}

		var timer = new TimerInstance(Persistence, action, plugin ?? Plugin);
		timer.Delay = time;
		timer.Repetitions = 0;
		timer.Repeating = true;
		timer.Callback = action;
		timer.Tracking = ResolveTracking(action);

		TrackTimer(timer);
		Schedule(timer, CurrentTime + NormalizeRepeatDelay(time));

		return timer;
	}
	public TimerInstance Repeat(float time, int times, Action action, Plugin plugin = null)
	{
		if (!IsValid()) return null;

		var timer = new TimerInstance(Persistence, action, plugin ?? Plugin);
		timer.Delay = time;
		timer.Repetitions = times;
		timer.Repeating = times != 1;
		timer.Callback = action;
		timer.Tracking = ResolveTracking(action);

		TrackTimer(timer);
		Schedule(timer, CurrentTime + (timer.Repeating ? NormalizeRepeatDelay(time) : time));

		return timer;
	}
	public void Destroy(ref TimerInstance timer)
	{
		if (timer != null)
		{
			timer.Destroy();
		}

		timer = null;
	}
	public void DestroyAll()
	{
		if (_timers == null)
		{
			return;
		}

		var timers = Pool.Get<List<TimerInstance>>();

		try
		{
			lock (SchedulerLock)
			{
				if (_timers.Count == 0)
				{
					return;
				}

				timers.AddRange(_timers);
			}

			for (var i = 0; i < timers.Count; i++)
			{
				timers[i].Destroy();
			}
		}
		finally
		{
			Pool.FreeUnmanaged(ref timers);
		}
	}

	public class TimerInstance : IDisposable
	{
		public Plugin Plugin { get; set; }
		internal Timer OwnerTimers { get; set; }

		public Action Activity { get; set; }
		public Action Callback { get; set; }
		public Plugin.Persistence Persistence { get; set; }
		public int Repetitions { get; set; }
		public float Delay { get; set; }
		public float ExpiresAt
		{
			get => (float)ExpiresAtDouble;
			set => ExpiresAtDouble = value;
		}
		public bool Repeating { get; set; }
		public int TimesTriggered { get; set; }
		public bool Destroyed { get; set; }
		public bool Scheduled => HeapIndex >= 0;

		internal double ExpiresAtDouble;
		internal double DueAt;
		internal int HeapIndex = -1;
		internal int Generation;
		internal int CollectedGeneration;
		internal InvokeTrackingData Tracking;

		public TimerInstance() { }
		public TimerInstance(Plugin.Persistence persistence, Action activity, Plugin plugin = null)
		{
			Persistence = persistence;
			Activity = activity;
			Plugin = plugin;
		}

		public void Reset(float delay = -1f, int repetitions = 1)
		{
			if (Persistence == null)
			{
				Logger.Warn($"Cannot restart a timer for '{Plugin?.ToPrettyString() ?? "unknown plugin"}' because persistence is null.");
				return;
			}

			lock (SchedulerLock)
			{
				TimesTriggered = 0;
				Repetitions = repetitions;
				Repeating = repetitions != 1;

				if (delay < 0)
				{
					delay = Delay;
				}
				else
				{
					Delay = delay;
				}

				Timer.Unschedule(this);

				Generation++;
				Destroyed = false;
				Callback = Activity;
				OwnerTimers?.TrackTimer(this);
				Tracking ??= Timer.ResolveTracking(Activity);

				Timer.Schedule(this, Timer.CurrentTime + (Repeating ? Timer.NormalizeRepeatDelay(delay) : delay));
			}
		}
		public bool Destroy()
		{
			lock (SchedulerLock)
			{
				var wasDestroyed = Destroyed;
				Destroyed = true;

				Generation++;
				Timer.Unschedule(this);
				OwnerTimers?.UntrackTimer(this);
				Callback = null;

				return !wasDestroyed;
			}
		}
		public void DestroyToPool()
		{
			Destroy();
		}
		public void Dispose()
		{
			Destroy();
		}
	}
}
