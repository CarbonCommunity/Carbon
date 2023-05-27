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
	public void Clear();
	public void Queue(IThreadedJob job);
}
