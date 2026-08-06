namespace Oxide.Plugins;

public class Timer
{
	private Core.Libraries.Timer.TimerInstance instance;

	public Timer(Core.Libraries.Timer.TimerInstance instance)
	{
		this.instance = instance;
	}

	public int Repetitions => instance.Repetitions;

	public float Delay => instance.Delay;

	public Action Callback => instance.Callback;

	public bool Destroyed => instance.Destroyed;

	public Plugin Owner => instance.Plugin;

	public void Reset(float delay = -1, int repetitions = 1) => instance.Reset(delay, repetitions);

	public void Destroy() => instance.Destroy();

	public void DestroyToPool() => instance.DestroyToPool();
}

public class PluginTimers
{
	private Core.Libraries.Timer timer;
	private Plugin plugin;

	public PluginTimers(Plugin plugin)
	{
		this.plugin = plugin;
		timer = new Core.Libraries.Timer(plugin);
	}

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
