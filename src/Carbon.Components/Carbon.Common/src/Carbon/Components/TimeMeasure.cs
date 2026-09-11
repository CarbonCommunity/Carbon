using System.Diagnostics;
using Facepunch;

namespace Carbon.Components;

public struct TimeMeasure : IDisposable
{
#if DEBUG
	internal string _name;
	internal string _warn;
	internal int _miliseconds;
	internal Stopwatch _watch;
#endif

	public static TimeMeasure New(string name, int miliseconds = 100, string warn = null)
	{
#if DEBUG
		var result = default(TimeMeasure);
		result._name = name;
		result._warn = warn;
		result._miliseconds = miliseconds;
		result._watch = Pool.Get<Stopwatch>();
		result._watch.Start();
		return result;
#else
		return default;
#endif
	}

	public void Dispose()
	{
#if DEBUG
		var milliseconds = _watch.ElapsedMilliseconds;

		if (milliseconds > _miliseconds)
		{
			Carbon.Logger.Warn(
				$" {_name} took {milliseconds:0}ms{(string.IsNullOrEmpty(_warn) ? "" : (": " + _warn))}");
		}

		_watch.Reset();
		Pool.FreeUnsafe(ref _watch);
#endif
	}
}
