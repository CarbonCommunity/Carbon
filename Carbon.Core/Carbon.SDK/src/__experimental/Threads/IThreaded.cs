using System;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace API.Threads;

public interface IThreaded
{
	public void Awake();
	public void DoWork();
	public void Destroy();

	public bool IsDone { get; }
	public bool IsProcessing { get; }
}
