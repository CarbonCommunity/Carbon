using System;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace API.Threads;

public interface IThreadedJob
{
	public string FileName { get; set; }
	public string Source { get; set; }
	public byte[] Output { get; }

	public bool Result { get; }
	public bool IsDone { get; }

	public void Initialize();
	public void Process();
}
