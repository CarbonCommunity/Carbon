using System.Diagnostics;

namespace Carbon.Tests;

public class StopWatchGroupLog : IDisposable
{
	private readonly string _name;
	private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

	private const string Cyan = "\u001b[36;1m"; // Cyan + Bold
	private const string Green = "\u001b[32;1m"; // Green + Bold
	private const string Reset = "\u001b[0m"; // Reset to default

	public StopWatchGroupLog(string name)
	{
		_name = name;

		// fix, as Microsoft Logger is async and not immediately prints to stdout (should be flushed),
		// so without it some _logger logs could end up inside group (although they were right before using group)
		Thread.Sleep(20);

		Console.WriteLine($"::group::{Cyan}{name}{Reset}");
	}

	private void Stop()
	{
		_stopwatch.Stop();

		Thread.Sleep(20);
		Console.WriteLine("::endgroup::");
		Console.WriteLine($"{Green}✓ `{_name}` took {_stopwatch.Elapsed:mm\\:ss\\.ffff}{Reset}");
	}

	public void Dispose()
	{
		Stop();
		GC.SuppressFinalize(this);
	}
}
