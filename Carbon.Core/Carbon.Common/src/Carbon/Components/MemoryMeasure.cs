/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Components;

public struct MemoryMeasure : IDisposable
{
#if DEBUG
	internal string _name;
	internal string _warn;
	internal long _threshold;
	internal long _currentValue;
	internal bool _formatted;
#endif

	public const string Format = "{0}{1}";

	public static MemoryMeasure New(string name, long threshold = 1024, string warn = null, bool formatted = true)
	{
#if DEBUG
		var result = default(MemoryMeasure);
		result._name = name;
		result._warn = warn;
		result._threshold = threshold;
		result._currentValue = GC.GetTotalMemory(false);
		result._formatted = formatted;
		return result;
#else
		return default;
#endif
	}

	public void Dispose()
	{
#if DEBUG
		var difference = GC.GetTotalMemory(false) - _currentValue;

		if (difference > _threshold)
		{
			Carbon.Logger.Warn(
				$" {_name} increased memory to {(_formatted ? ByteEx.Format(difference, stringFormat: Format).ToLower() : $"{difference}b")} [abv {(_formatted ? ByteEx.Format(_threshold, stringFormat: Format).ToLower() : $"{_threshold}b")}]{(string.IsNullOrEmpty(_warn) ? "" : (": " + _warn))}");
		}
#endif
	}
}
