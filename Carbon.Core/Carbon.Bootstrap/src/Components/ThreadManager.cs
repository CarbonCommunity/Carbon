using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using API.Abstracts;
using API.Assembly;
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
				// IThreadedJob job = new();
				// job.FileName = Path.GetFileName(file);
				// Carbon.Bootstrap.Threads.Execute(job);
			},
		});
#endif
	}

	internal void OnEnable()
	{
		for (int i = 0; i < _threads.Length; i++)
		{
			_threads[i] = new Thread(new ThreadStart(Worker));
			_threads[i].Start();
		}
	}

	internal void OnDisable()
	{
		for (int i = 0; i < _threads.Length; i++)
		{
			_threads[i].Abort();
			_threads[i] = null;
		}
	}

	internal void OnDestroy()
		=> Dispose();

	private static int ThreadID
	{ get => Thread.CurrentThread.ManagedThreadId; }

	public void Clear()
	{
		foreach (Thread thread in _threads)
		{
			thread.Abort();
		}
	}

	public void Execute(IThreadedJob job)
	{
		_workQueue.Enqueue(job);
	}

	private static void Worker()
	{
		try
		{
			Logger.Log($"Thread[{ThreadID}] started");

			while (true)
			{
				try
				{
					if (_workQueue.TryDequeue(out IThreadedJob job))
					{
						Logger.Log($"Thread[{ThreadID}] processing '{job.FileName}'");
						job.Initialize();
						job.Process();
						Logger.Log($"Thread[{ThreadID}] finished '{job.FileName}': {job.Result}");
					}
					else
					{
						Thread.Sleep(100);
					}
				}
				catch (System.Exception e)
				{
					Logger.Error($"Thread[{ThreadID}] error: {e.Message}");
					continue;
				}
			}
		}
		catch (ThreadAbortException)
		{
			Logger.Log($"Thread[{ThreadID}] terminated");
		}
	}

	private bool _disposing;

	internal void Dispose(bool disposing)
	{
		if (!_disposing)
		{
			if (disposing)
			{
				foreach (Thread thread in _threads)
				{
					thread.Abort();
				}
			}

			// no unmanaged resources
			_disposing = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}

