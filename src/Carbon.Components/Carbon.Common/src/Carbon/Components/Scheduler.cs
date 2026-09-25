namespace Carbon.Components;

/// <summary>
/// Main-thread dispatcher. Runs queued callbacks on the next frame and ticks plugin timers.
/// Thread-safe, so worker threads can use it to get back onto the main thread.
/// </summary>
public class Scheduler : FacepunchBehaviour
{
	private List<Action> _current = new();
	private List<Action> _previous = new();
	private readonly object _lock = new();

	/// <summary>Fired every LateUpdate, after the frame queue ran. Timers hook into this.</summary>
	public event Action OnLateUpdate;

	/// <summary>Queues <paramref name="callback"/> for the next frame.</summary>
	public void NextFrame(Action callback)
	{
		if (callback == null)
		{
			return;
		}

		lock (_lock)
		{
			_current.Add(callback);
		}
	}

	public void Update()
	{
		if (_current.Count == 0)
		{
			return;
		}

		// Swap buffers so callbacks can queue more work without mutating the list we're iterating
		List<Action> queue;
		lock (_lock)
		{
			queue = _current;
			_current = _previous;
			_previous = queue;
		}

		for (var i = 0; i < queue.Count; i++)
		{
			try
			{
				queue[i]();
			}
			catch (Exception exception)
			{
				Logger.Error("Failed to execute next-frame callback", exception.InnerException ?? exception);
			}
		}

		queue.Clear();
	}

	public void Awake()
	{
		TimerLibrary.PrimeClock();
	}

	public void LateUpdate()
	{
		TimerLibrary.ProcessTimers();

		try
		{
			OnLateUpdate?.Invoke();
		}
		catch (Exception exception)
		{
			Logger.Error("Failed to execute late-update callback", exception);
		}
	}
}
