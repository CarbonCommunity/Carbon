using Logger = Carbon.Logger;

namespace Oxide.Plugins;

public partial class Timers : Library
{
	public Plugin Plugin { get; }
	internal List<Timer> _timers { get; set; } = new();

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
		if (_timers == null)
		{
			return;
		}

		foreach (var timer in _timers)
		{
			timer.Destroy();
		}

		_timers.Clear();
		_timers = null;
	}

	public Plugin.Persistence Persistence => Plugin.persistence;

	public Timer In(float time, Action action)
	{
		if (!IsValid())
		{
			return null;
		}

		var timer = new Timer(Persistence, action, Plugin);
		_timers.Add(timer);
		timer.Repetitions = 1;
		var activity = new Action(() =>
		{
			try
			{
				action?.Invoke();
				timer.TimesTriggered++;
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
		_timers.Add(timer);
		var activity = new Action(() =>
		{
			try
			{
				action?.Invoke();
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
		_timers.Add(timer);
		var activity = new Action(() =>
		{
			try
			{
				action?.Invoke();
				timer.TimesTriggered++;

				if (times <= 0 || timer.TimesTriggered < times) return;
				if (Persistence == null) return;
				Persistence.CancelInvoke(timer.Callback);
				Persistence.CancelInvokeFixedTime(timer.Callback);
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
		foreach (var timer in _timers)
		{
			timer.Destroy();
		}

		_timers.Clear();
	}
}

public class Timer : IDisposable
{
	public Plugin Plugin { get; set; }

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

		if (Destroyed)
		{
			Logger.Warn($"You cannot restart a timer that has been destroyed.");
			return;
		}

		if (Persistence == null)
		{
			Logger.Warn($"Cannot restart a timer for '{Plugin?.ToPrettyString() ?? "unknown plugin"}' because persistence is null.");
			return;
		}

		Persistence.CancelInvoke(Callback);
		Persistence.CancelInvokeFixedTime(Callback);

		if (Repetitions == 1)
		{
			Callback = () =>
			{
				try
				{
					Activity?.Invoke();
					TimesTriggered++;
				}
				catch (Exception ex)
				{
					Logger.Error($"Timer of {delay}s has failed in '{Plugin.ToPrettyString()}' [callback]", ex);
				}

				Destroy();
			};

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

			Callback = () =>
			{
				try
				{
					Activity?.Invoke();
					TimesTriggered++;

					if (Repetitions > 0 && TimesTriggered >= Repetitions)
					{
						Dispose();
					}
				}
				catch (Exception ex)
				{
					Logger.Error($"Timer of {delay}s has failed in '{Plugin.ToPrettyString()}' [callback]", ex);
					Destroy();
				}
			};

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
		if (Destroyed) return false;
		Destroyed = true;

		if (Persistence != null)
		{
			Persistence.CancelInvoke(Callback);
		}

		Timers.RemoveStartupTimer(this);

		if (Callback != null)
		{
			Callback = null;
		}

		return true;
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
