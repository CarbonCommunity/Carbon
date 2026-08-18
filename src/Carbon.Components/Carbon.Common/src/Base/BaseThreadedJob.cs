namespace Carbon.Base;

public class BaseThreadedJob : IDisposable
{
	internal bool _isDone = false;
	internal object _handle = new();
	internal Task _task = null;

	private CancellationTokenSource cancellationToken;
	private volatile bool _isAborted;

	internal object _abortHandle = new();

	public CancellationToken CancellationToken => cancellationToken?.Token ?? CancellationToken.None;
	public bool IsAborted => _isAborted;

	public bool IsDone
	{
		get
		{
			bool temp;
			lock (_handle)
			{
				temp = _isDone;
			}
			return temp;
		}
		set
		{
			lock (_handle)
			{
				_isDone = value;
			}
		}
	}

	public virtual void Start()
	{
		if (IsAborted)
		{
			IsDone = true;
			return;
		}

		if (Community.IsServerInitialized)
		{
			cancellationToken = new CancellationTokenSource();
			_task = Task.Factory.StartNew(Run, cancellationToken.Token);
		}
		else
		{
			Run();
		}
	}
	public virtual void Abort()
	{
		lock (_abortHandle)
		{
			_isAborted = true;
		}

		cancellationToken?.Cancel();
	}

	public virtual void ThreadFunction() { }
	public virtual void OnFinished() { }

	public virtual bool Update()
	{
		if (IsDone)
		{
			OnFinished();
			return true;
		}
		return false;
	}
	public IEnumerator WaitFor()
	{
		while (!Update())
		{
			yield return null;
		}
	}
	private void Run()
	{
		ThreadFunction();
		IsDone = true;
	}

	public virtual void Dispose() { }
}
