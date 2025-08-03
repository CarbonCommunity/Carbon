using Facepunch;

namespace Oxide.Plugins;

public class Timers : Library
{
    public Plugin Plugin { get; }

    private List<Timer> _timers = Pool.Get<List<Timer>>();

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
            return;

        Pool.Free(ref _timers, true);

        _timers = Pool.Get<List<Timer>>();
    }

    public Plugin.Persistence Persistence => Plugin.persistence;

    public Timer In(float time, Action action)
    {
        if (!IsValid()) return null;

        var timer = Pool.Get<Timer>();
        timer.Init(Persistence, action, Plugin);
        timer.SetupOnce(time);
        _timers.Add(timer);
        return timer;
    }

    public Timer Once(float time, Action action) => In(time, action);

    public Timer Every(float time, Action action)
    {
        if (!IsValid()) return null;

        var timer = Pool.Get<Timer>();
        timer.Init(Persistence, action, Plugin);
        timer.SetupRepeat(time);
        _timers.Add(timer);
        return timer;
    }

    public Timer Repeat(float time, int times, Action action)
    {
        if (!IsValid()) return null;

        var timer = Pool.Get<Timer>();
        timer.Init(Persistence, action, Plugin);
        timer.SetupRepeatCount(time, times);
        _timers.Add(timer);
        return timer;
    }

    public void Destroy(ref Timer timer)
    {
	    timer?.Destroy();
        timer = null;
    }

    public void DestroyAll()
    {
        foreach (var timer in _timers)
            timer.Destroy();

        _timers.Clear();
    }
}

public class Timer : IDisposable, Pool.IPooled
{
    public Plugin Plugin { get; private set; }
    public Action Activity { get; private set; }
    public Action Callback { get; private set; }
    public Plugin.Persistence Persistence { get; private set; }
    public int Repetitions { get; private set; }
    public float Delay { get; private set; }
    public int TimesTriggered { get; private set; }
    public bool Destroyed { get; private set; }

    public Timer() { }
    public Timer(Plugin.Persistence persistence, Action activity, Plugin plugin = null)
    {
        Init(persistence, activity, plugin);
    }
    public void Init(Plugin.Persistence persistence, Action activity, Plugin plugin = null)
    {
        Persistence = persistence;
        Activity = activity;
        Plugin = plugin;
        TimesTriggered = 0;
        Destroyed = false;
        Callback = null;
        Repetitions = 0;
        Delay = 0f;
    }

    public void SetupOnce(float time)
    {
        Delay = time;
        Repetitions = 1;
        Callback = () =>
        {
            try
            {
                Activity?.Invoke();
                TimesTriggered++;
            }
            catch (Exception ex)
            {
                Carbon.Logger.Error($"Timer of {Delay}s has failed in '{Plugin?.ToPrettyString()}' [callback]", ex);
            }
            Destroy();
        };
        Persistence?.Invoke(Callback, Delay);
    }

    public void SetupRepeat(float time)
    {
        Delay = time;
        Repetitions = 0;
        Callback = () =>
        {
            try
            {
                Activity?.Invoke();
                TimesTriggered++;
            }
            catch (Exception ex)
            {
	            Carbon.Logger.Error($"Timer of {Delay}s has failed in '{Plugin?.ToPrettyString()}' [callback]", ex);
                Destroy();
            }
        };
        Persistence?.InvokeRepeating(Callback, Delay, Delay);
    }

    public void SetupRepeatCount(float time, int count)
    {
        Delay = time;
        Repetitions = count;
        Callback = () =>
        {
            try
            {
                Activity?.Invoke();
                TimesTriggered++;
                if (TimesTriggered >= Repetitions)
                    Destroy();
            }
            catch (Exception ex)
            {
	            Carbon.Logger.Error($"Timer of {Delay}s has failed in '{Plugin?.ToPrettyString()}' [callback]", ex);
                Destroy();
            }
        };
        Persistence?.InvokeRepeating(Callback, Delay, Delay);
    }

    public void Reset(float delay = -1f, int repetitions = 1)
    {
	    TimesTriggered = 0;
	    Repetitions = repetitions;

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
		    Carbon.Logger.Warn($"You cannot restart a timer that has been destroyed.");
		    return;
	    }

	    if (Persistence != null)
	    {
		    Persistence.CancelInvoke(Callback);
		    Persistence.CancelInvokeFixedTime(Callback);
	    }

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
				    Carbon.Logger.Error($"Timer of {delay}s has failed in '{Plugin.ToPrettyString()}' [callback]", ex);
			    }

			    Destroy();
		    };

		    Persistence.Invoke(Callback, delay);
	    }
	    else
	    {
		    Callback = () =>
		    {
			    try
			    {
				    Activity?.Invoke();
				    TimesTriggered++;

				    if (TimesTriggered >= Repetitions)
				    {
					    Dispose();
				    }
			    }
			    catch (Exception ex)
			    {
				    Carbon.Logger.Error($"Timer of {delay}s has failed in '{Plugin.ToPrettyString()}' [callback]", ex);
				    Destroy();
			    }
		    };

		    Persistence.InvokeRepeating(Callback, delay, delay);
	    }
    }

    public void Destroy()
    {
	    if (Destroyed)
	    {
		    return;
	    }
        Destroyed = true;

        Persistence?.CancelInvoke(Callback);
        Persistence?.CancelInvokeFixedTime(Callback);

        Activity = null;
        Callback = null;
        Plugin = null;
        Persistence = null;

        Timer self = this;
        Pool.Free(ref self);
    }

    public void DestroyToPool() => Destroy();

    public void Dispose() => Destroy();

    public void EnterPool()
    {
        Activity = null;
        Callback = null;
        Plugin = null;
        Persistence = null;
        TimesTriggered = 0;
        Repetitions = 0;
        Delay = 0f;
        Destroyed = false;
    }

    public void LeavePool()
    {

    }
}
