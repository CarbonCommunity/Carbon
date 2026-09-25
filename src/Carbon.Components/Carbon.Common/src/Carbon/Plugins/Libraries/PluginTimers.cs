namespace Carbon.Plugins;

/// <summary>
/// Handle to a scheduled timer.
/// </summary>
public class Timer
{
	private TimerLibrary.TimerInstance instance;

	public Timer(TimerLibrary.TimerInstance instance)
	{
		this.instance = instance;
	}

	public int Repetitions => instance.Repetitions;

	public float Delay => instance.Delay;

	public Action Callback => instance.Callback;

	public bool Destroyed => instance.Destroyed;

	public bool Scheduled => instance.Scheduled;

	public Plugin Owner => instance.Plugin;

	public int TimesTriggered => instance.TimesTriggered;

	public Plugin.Persistence Persistence => instance.Persistence;

	public void Reset(float delay = -1, int repetitions = 1) => instance.Reset(delay, repetitions);

	public void Destroy() => instance.Destroy();

	public void DestroyToPool() => instance.DestroyToPool();
}

/// <summary>
/// The "timer" library of a plugin: Once, Every, Repeat.
/// </summary>
public class PluginTimers
{
	private TimerLibrary timer;
	private Plugin plugin;

	public PluginTimers(Plugin plugin)
	{
		this.plugin = plugin;
		timer = new TimerLibrary(plugin);
	}

	public TimerLibrary Library => timer;

	public Timer Once(float seconds, Action callback)
	{
		return new Timer(timer.Once(seconds, callback, plugin));
	}

	public Timer In(float seconds, Action callback)
	{
		return new Timer(timer.Once(seconds, callback, plugin));
	}

	public Timer Every(float interval, Action callback)
	{
		return new Timer(timer.Repeat(interval, -1, callback, plugin));
	}

	public Timer Repeat(float interval, int repeats, Action callback)
	{
		return new Timer(timer.Repeat(interval, repeats, callback, plugin));
	}

	public void Destroy(ref Timer timer)
	{
		timer?.DestroyToPool();
		timer = null;
	}

	public void Clear()
	{
		timer.Clear();
	}
}
