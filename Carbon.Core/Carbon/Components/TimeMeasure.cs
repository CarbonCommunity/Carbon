///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;

namespace Carbon.Core
{
	public struct TimeMeasure : IDisposable
	{
#if DEBUG
		internal string _name;
		internal string _warn;
		internal int _miliseconds;
		internal RealTimeSince _timeSince;
#endif

		public static TimeMeasure New(string name, int miliseconds = 75, string warn = null)
		{
#if DEBUG
			var result = default(TimeMeasure);
			result._name = name;
			result._warn = warn;
			result._miliseconds = miliseconds;
			result._timeSince = 0f;
			return result;
#else
			return default;
#endif
		}

		public void Dispose()
		{
#if DEBUG
			var num = (float)_timeSince * 1000f;

			if (num >= _miliseconds)
			{
				Carbon.Logger.WarnFormat(
					" {0} took {1:0}ms [abv {2}]{3}",
					_name, num, _miliseconds, string.IsNullOrEmpty(_warn) ? "" : (": " + _warn)
				);
			}
#endif
		}
	}
}
