using System;
using API.Threads;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Threads;

public class ScriptParserJob : IThreadedJob
{
	private bool _processing = true;
	public bool IsDone { get => !_processing; }
	public bool IsProcessing { get => _processing; }


	public byte[] Output { get; private set; }
	public string FileName { get; set; }
	public string Input { get; set; }


	public void Awake()
	{
		throw new NotImplementedException();
	}

	public void Destroy()
	{
		throw new NotImplementedException();
	}

	public void DoWork()
	{
		throw new NotImplementedException();
	}
}