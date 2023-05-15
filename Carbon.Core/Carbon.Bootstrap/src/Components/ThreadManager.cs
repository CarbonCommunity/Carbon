using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading;
using API.Abstracts;
using API.Assembly;
using API.Threads;
using Threads;
using Utility;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Components;
#pragma warning disable IDE0051

internal sealed class ThreadManager : CarbonBehaviour, IDisposable, IThreadManager
{
	private static readonly ConcurrentQueue<IThreadedJob> _workQueue = new();
	private static readonly Thread[] _threads = new Thread[4];

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
		for (int i = 0; i < _threads.Length; i++)
		{
			_threads[i] = CreateThread();
			_threads[i].Start();
		}

		InvokeRepeating(nameof(CheckThreads), 1f, 5f);
	}

	internal void OnDisable()
	{
		for (int i = 0; i < _threads.Length; i++)
		{
			_threads[i].Abort();
			_threads[i] = null;
		}

		CancelInvoke(nameof(CheckThreads));
	}

	internal void OnDestroy()
		=> Dispose();

	private static int ThreadID
	{ get => Thread.CurrentThread.ManagedThreadId; }

	private void CheckThreads()
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

	private Thread CreateThread(string name = "Carbon.Thread")
	{
		Thread thread = new Thread(new ThreadStart(ThreadManager.DoWork))
		{
			Name = name,
			Priority = ThreadPriority.Lowest
		};

		Logger.Debug($"Thread[{thread.ManagedThreadId}] created");
		return thread;
	}

	public void Clear()
	{
		for (int i = 0; i < _threads.Length; i++)
		{
			_threads[i].Abort();
			_threads[i] = null;
		}
	}

	public void Queue(IThreadedJob job)
	{
		_workQueue.Enqueue(job);
	}

	private static void DoWork()
	{
		try
		{
			while (true)
			{
				if (_workQueue.TryDequeue(out IThreadedJob job))
				{
					try
					{
						using (TimeMeasure.New($"Thread[{ThreadID}] '{job.FileName}'"))
						{
							job.Awake();
							job.DoWork();
							job.Destroy();
						}
					}
					catch (Exception e)
					{
						Logger.Error($"Thread[{ThreadID}] error: {e.Message}");
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
			Logger.Warn($"Thread[{ThreadID}] terminated");
		}
		catch (Exception e)
		{
			Logger.Error($"Thread[{ThreadID}] crashed", e);
		}
	}

	private bool _disposing;

	internal void Dispose(bool disposing)
	{
		if (!_disposing)
		{
			if (disposing)
			{
				Clear();
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

