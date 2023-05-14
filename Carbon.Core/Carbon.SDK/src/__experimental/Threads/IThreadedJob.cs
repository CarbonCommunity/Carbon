using System;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace API.Threads;

public interface IThreadedJob : IThreaded
{
	public string FileName { get; set; }
	public string Input { get; set; }
	public byte[] Output { get; }
}
