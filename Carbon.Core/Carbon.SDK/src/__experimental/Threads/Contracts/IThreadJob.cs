using System;
using System.Threading;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace API.Threads;

public interface IThreadJob
{
	public void Awake();
	public Result DoWork();
	public void Destroy();

	public SynchronizationContext MainThreadContext { get; set; }
	public Action<Result> Callback { get; set; }
	public object Input { get; set; }

	public bool IsProcessing { get; }
	public bool IsDone { get; }
}
