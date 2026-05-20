using Logger = Carbon.Logger;

namespace Oxide.Plugins;

public partial class Timers : Library
{
	public Plugin Plugin { get; }
	internal List<Timer> _timers { get; set; } = [];

	public Timers() { }
	public Timers(Plugin plugin)
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

	internal void TrackTimer(Timer timer)
	{
		timer.OwnerTimers = this;
		_timers ??= [];

		if (_timers.Contains(timer))
		{
			return;
		}

		_timers.Add(timer);
	}
	internal void UntrackTimer(Timer timer)
	{
		if (timer.OwnerTimers != this)
		{
			return;
		}

		_timers?.Remove(timer);
	}

	public Timer In(float time, Action action)
	{
		if (!IsValid())
		{
			return null;
		}

		var timer = new Timer(Persistence, action, Plugin);
		TrackTimer(timer);
		timer.Repetitions = 1;
		var activity = new Action(() =>
		{
			try
			{
				var callback = timer.Callback;
				action?.Invoke();
				if (timer.Destroyed || timer.Callback != callback)
				{
					return;
				}

				timer.TimesTriggered++;
				timer.Destroy();
			}
			catch (Exception ex)
			{
				Logger.Error($"Timer of {time}s has failed in '{Plugin.ToPrettyString()}' [callback]", ex);
				timer.Destroy();
			}
		});

		timer.Delay = time;
		timer.Callback = activity;

		if (Community.IsServerInitialized)
		{
			Persistence.Invoke(activity, time);
		}
		else
		{
			timer.ExpiresAt = UnityEngine.Time.realtimeSinceStartup + time;
			QueueStartupTimer(timer);
		}

		return timer;
	}
	public Timer Once(float time, Action action)
	{
		return In(time, action);
	}
	public Timer Every(float time, Action action)
	{
		if (!IsValid())
		{
			return null;
		}

		var timer = new Timer(Persistence, action, Plugin);
		TrackTimer(timer);
		var activity = new Action(() =>
		{
			try
			{
				var callback = timer.Callback;
				action?.Invoke();
				if (timer.Destroyed || timer.Callback != callback)
				{
					return;
				}

				timer.TimesTriggered++;
			}
			catch (Exception ex)
			{
				Logger.Error($"Timer of {time}s has failed in '{Plugin.ToPrettyString()}' [callback]", ex);
				timer.Destroy();
			}
		});

		timer.Delay = time;
		timer.Repetitions = 0;
		timer.StartupRepeating = true;
		timer.Callback = activity;

		if (Community.IsServerInitialized)
		{
			Persistence.InvokeRepeating(activity, time, time);
		}
		else
		{
			timer.Delay = NormalizeStartupRepeatDelay(time);
			timer.ExpiresAt = UnityEngine.Time.realtimeSinceStartup + timer.Delay;
			QueueStartupTimer(timer);
		}

		return timer;
	}
	public Timer Repeat(float time, int times, Action action)
	{
		if (!IsValid()) return null;

		var timer = new Timer(Persistence, action, Plugin);
		TrackTimer(timer);
		var activity = new Action(() =>
		{
			try
			{
				var callback = timer.Callback;
				action?.Invoke();
				if (timer.Destroyed || timer.Callback != callback)
				{
					return;
				}

				timer.TimesTriggered++;

				if (times <= 0 || timer.TimesTriggered < times) return;
				timer.Destroy();
			}
			catch (Exception ex)
			{
				Logger.Error($"Timer of {time}s has failed in '{Plugin.ToPrettyString()}' [callback]", ex);
				timer.Destroy();
			}
		});

		timer.Delay = time;
		timer.Repetitions = times;
		timer.StartupRepeating = times != 1;
		timer.Callback = activity;

		if (Community.IsServerInitialized)
		{
			Persistence.InvokeRepeating(activity, time, time);
		}
		else if (timer.StartupRepeating)
		{
			timer.Delay = NormalizeStartupRepeatDelay(time);
			timer.ExpiresAt = UnityEngine.Time.realtimeSinceStartup + timer.Delay;
			QueueStartupTimer(timer);
		}
		else
		{
			timer.ExpiresAt = UnityEngine.Time.realtimeSinceStartup + time;
			QueueStartupTimer(timer);
		}

		return timer;
	}
	public void Destroy(ref Timer timer)
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

		while (_timers.Count > 0)
		{
			var timer = _timers[^1];
			_timers.RemoveAt(_timers.Count - 1);

			timer.Destroy();
		}
	}
}

public class Timer : IDisposable
{
	public Plugin Plugin { get; set; }
	internal Timers OwnerTimers { get; set; }

	public Action Activity { get; set; }
	public Action Callback { get; set; }
	public Plugin.Persistence Persistence { get; set; }
	public int Repetitions { get; set; }
	public float Delay { get; set; }
	public float ExpiresAt { get; set; }
	public bool StartupRepeating { get; set; }
	public int TimesTriggered { get; set; }
	public bool Destroyed { get; set; }

	public Timer() { }
	public Timer(Plugin.Persistence persistence, Action activity, Plugin plugin = null)
	{
		Persistence = persistence;
		Activity = activity;
		Plugin = plugin;
	}

	public void Reset(float delay = -1f, int repetitions = 1)
	{
		TimesTriggered = 0;
		Repetitions = repetitions;
		StartupRepeating = repetitions != 1;

		if (delay < 0)
		{
			delay = Delay;
		}
		else
		{
			Delay = delay;
		}

		if (Persistence == null)
		{
			Logger.Warn($"Cannot restart a timer for '{Plugin?.ToPrettyString() ?? "unknown plugin"}' because persistence is null.");
			return;
		}

		Timers.RemoveStartupTimer(this);

		if (Callback != null)
		{
			Persistence.CancelInvoke(Callback);
			Persistence.CancelInvokeFixedTime(Callback);
		}

		Destroyed = false;
		OwnerTimers?.TrackTimer(this);

		if (Repetitions == 1)
		{
			Action callback = null;
			callback = () =>
			{
				try
				{
					Activity?.Invoke();
					if (Destroyed || Callback != callback)
					{
						return;
					}

					TimesTriggered++;
				}
				catch (Exception ex)
				{
					Logger.Error($"Timer of {delay}s has failed in '{Plugin.ToPrettyString()}' [callback]", ex);
					Destroy();
					return;
				}

				Destroy();
			};
			Callback = callback;

			if (Community.IsServerInitialized)
			{
				Persistence.Invoke(Callback, delay);
			}
			else
			{
				ExpiresAt = UnityEngine.Time.realtimeSinceStartup + delay;
				Timers.QueueStartupTimer(this);
			}
		}
		else
		{
			if (!Community.IsServerInitialized)
			{
				delay = Timers.NormalizeStartupRepeatDelay(delay);
				Delay = delay;
			}

			Action callback = null;
			callback = () =>
			{
				try
				{
					Activity?.Invoke();
					if (Destroyed || Callback != callback)
					{
						return;
					}

					TimesTriggered++;

					if (Repetitions > 0 && TimesTriggered >= Repetitions)
					{
						Destroy();
					}
				}
				catch (Exception ex)
				{
					Logger.Error($"Timer of {delay}s has failed in '{Plugin.ToPrettyString()}' [callback]", ex);
					Destroy();
				}
			};
			Callback = callback;

			if (Community.IsServerInitialized)
			{
				Persistence.InvokeRepeating(Callback, delay, delay);
			}
			else
			{
				ExpiresAt = UnityEngine.Time.realtimeSinceStartup + delay;
				Timers.QueueStartupTimer(this);
			}
		}
	}
	public bool Destroy()
	{
		var wasDestroyed = Destroyed;
		Destroyed = true;

		Timers.RemoveStartupTimer(this);
		OwnerTimers?.UntrackTimer(this);

		if (Callback != null)
		{
			Persistence?.CancelInvoke(Callback);
			Callback = null;
		}

		return !wasDestroyed;
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
