using System.Diagnostics;

namespace Carbon.Tests;

public class StopWatchGroupLog : IDisposable
{
	private readonly string _name;
	private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

	public StopWatchGroupLog(string name)
	{
		_name = name;

		// fix, as Microsoft Logger is async and not immediately prints to stdout,
		// so without it some _logger logs could end up inside group (although they were right before using group)
		Thread.Sleep(20);
		Console.WriteLine($"::group::{name}");
	}

	private void Stop()
	{
		_stopwatch.Stop();
		Thread.Sleep(20);
		Console.WriteLine("::endgroup::");
		Console.WriteLine($@"`{_name}` took {_stopwatch.Elapsed:mm\:ss\.ffff}");
	}

	public void Dispose()
	{
		Stop();
		GC.SuppressFinalize(this);
	}
}
