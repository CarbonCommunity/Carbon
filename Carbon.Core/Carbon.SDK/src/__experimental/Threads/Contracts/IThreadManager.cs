using System;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace API.Threads;

public interface IThreadManager
{
	public void Queue(IThreadJob job);
	public void Queue(IThreadJob job, Action<Result> callback);
}
