using System;
using System.Collections.Concurrent;
using System.Threading;
using API.Abstracts;
using API.Threads;
using Utility;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Components;
#pragma warning disable IDE0051

internal sealed class ThreadManager : CarbonBehaviour, IThreadManager, IDisposable
{
	private static readonly ConcurrentQueue<IThreadJob> _workQueue = new();
	private static readonly Thread[] _threads = new Thread[4];

	private static Thread _thread
	{ get => Thread.CurrentThread; }

	internal void Awake()
	{
		// by default disable the behaviour
		enabled = false;

#if EXPERIMENTAL
		Carbon.Bootstrap.Watcher.Watch(new WatchFolder
		{
			Extension = "*.cs",
			IncludeSubFolders = false,
			Directory = Path.Combine(Context.Carbon, "test"),

			OnFileCreated = (sender, file) =>
			{
				Threads.ScriptCompilerJob job = new();
				job.FileName = Path.GetFileName(file);
				Carbon.Bootstrap.Threads.Queue(job);
			},
		});
#endif
	}

	internal void OnEnable()
	{
		StartThreads();
		InvokeRepeating(nameof(Heartbeat), 1f, 5f);
	}

	internal void OnDisable()
	{
		AbortThreads();
		CancelInvoke(nameof(Heartbeat));
	}

	internal void OnDestroy()
	{
		Dispose();
	}

	private Thread CreateThread()
	{
		Thread thread = new Thread(new ThreadStart(ThreadManager.DoWork))
		{
			Name = "Carbon.Thread",
			Priority = ThreadPriority.Lowest
		};

		Logger.Debug($"A new thread created with ID {thread.ManagedThreadId}");
		return thread;
	}

	public void StartThreads()
	{
		for (int i = 0; i < _threads.Length; i++)
		{
			_threads[i] = CreateThread();
			_threads[i].Start();
		}
	}

	public void AbortThreads()
	{
		for (int i = 0; i < _threads.Length; i++)
		{
			_threads[i].Abort();
			_threads[i] = null;
		}
	}

	private void Heartbeat()
	{
		for (int i = 0; i < _threads.Length; i++)
		{
			if (_threads[i] is null || !_threads[i].IsAlive)
			{
				_threads[i] = CreateThread();
				_threads[i].Start();
			}
		}
	}

	public void Queue(IThreadJob job, Action<Result> callback)
	{
		job.MainThreadContext = SynchronizationContext.Current;
		job.Callback = callback;
		Queue(job);
	}

	public void Queue(IThreadJob job)
	{
		_workQueue.Enqueue(job);
	}

	private static void DoWork()
	{
		try
		{
			while (true)
			{
				if (_workQueue.TryDequeue(out IThreadJob job))
				{
					try
					{
						using (TimeMeasure.New($"Thread[{_thread.ManagedThreadId}] '{job}'"))
						{
							job.Awake();

							job.MainThreadContext.Post(d: new SendOrPostCallback((state) =>
							{
								job.Callback((Result)state);
							}), job.DoWork());

							job.Destroy();
						}
					}
					catch (Exception e)
					{
						Logger.Error($"Thread[{_thread.ManagedThreadId}] error: {e.Message}");
						continue;
					}
				}
				else
				{
					Thread.Sleep(100);
				}
			}
		}
		catch (ThreadAbortException)
		{
			Logger.Warn($"Thread[{_thread.ManagedThreadId}] terminated");
		}
		catch (Exception e)
		{
			Logger.Error($"Thread[{_thread.ManagedThreadId}] crashed", e);
		}
	}

	private bool _disposing;

	internal void Dispose(bool disposing)
	{
		if (!_disposing)
		{
			if (disposing)
			{
				AbortThreads();
			}
			_disposing = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}

