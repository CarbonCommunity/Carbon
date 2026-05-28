using System.Diagnostics;
using Serilog;

namespace Carbon.Profiler.Tests;

internal readonly struct TimedGroupLog : IDisposable
{
	private readonly string _name;
	private readonly Stopwatch _stopwatch;
	private readonly bool _groupsEnabled;

	private const string Cyan = "\u001b[36;1m"; // Cyan + Bold
	private const string Green = "\u001b[32;1m"; // Green + Bold
	private const string Reset = "\u001b[0m"; // Reset to default

	public TimedGroupLog(string name)
	{
		_name = name;
		_stopwatch = Stopwatch.StartNew();
		_groupsEnabled = IsEnabled();

		if (_groupsEnabled)
		{
			Thread.Sleep(20);
			Console.WriteLine($"::group::{Cyan}{name}{Reset}");
		}
	}

	public void Dispose()
	{
		_stopwatch.Stop();

		if (_groupsEnabled)
		{
			Thread.Sleep(20);
			Console.WriteLine("::endgroup::");
		}


		if (_groupsEnabled)
		{
			Console.WriteLine($@"{Green}✓ {_name} took {_stopwatch.Elapsed:mm\:ss\.ffff} {Reset}");
		}
		else
		{
			Log.Information("{GroupName} took {Elapsed}", _name, _stopwatch.Elapsed.ToString(@"mm\:ss\.ffff"));
		}
	}

	private static bool IsEnabled()
	{
		return IsTruthy(Environment.GetEnvironmentVariable("CarbonProfilerTestGroups")) ||
		       IsTruthy(Environment.GetEnvironmentVariable("CARBON_PROFILER_TEST_GROUPS"));
	}

	private static bool IsTruthy(string? value)
	{
		return value != null &&
		       (value.Equals("true", StringComparison.OrdinalIgnoreCase) ||
		        value.Equals("1", StringComparison.OrdinalIgnoreCase) ||
		        value.Equals("yes", StringComparison.OrdinalIgnoreCase));
	}
}
