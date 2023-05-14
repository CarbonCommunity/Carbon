using System;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Utility;

public struct TimeMeasure : IDisposable
{
	private int _timestamp;
	private string _name;

	public static TimeMeasure New(string name)
	{
		TimeMeasure result = default(TimeMeasure);
		result._timestamp = Environment.TickCount;
		result._name = name;
		return result;
	}

	public void Dispose()
	{
		int num = Environment.TickCount - _timestamp;
		Logger.Warn($"[PROFILER] {_name} took {num:0}ms");
	}
}
