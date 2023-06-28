using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using API.Threads;
using Microsoft.CodeAnalysis;
using Utility;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Threads;

public class ScriptParserJob : IThreadJob
{
	private bool _processing = true;
	public bool IsDone { get => !_processing; }
	public bool IsProcessing { get => _processing; }


	public SynchronizationContext MainThreadContext { get; set; }
	public Action<Result> Callback { get; set; }
	public object Input { get; set; }


	private string _contents = null;
	private Result _result = new();


	public void Awake()
	{
		string path = Path.Combine(Context.Carbon, "test", (string)Input);

		if (File.Exists(path))
		{
			_contents = File.ReadAllText(path);
			return;
		}

		throw new FileNotFoundException();
	}

	public void Destroy()
	{
		throw new NotImplementedException();
	}

	public Result DoWork()
	{
		_result.Output = _contents
			.Replace("PluginTimers", "Timers")
			.Replace("using Harmony;", "using HarmonyLib;")
			.Replace("HarmonyInstance.Create", "new Harmony")
			.Replace("HarmonyInstance", "Harmony");

		return _result;
	}

	public override string ToString()
	{
		return $"Script parser ({(string)Input})";
	}
}