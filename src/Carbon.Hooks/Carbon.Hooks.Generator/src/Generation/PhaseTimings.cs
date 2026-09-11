using System.Diagnostics;

namespace Carbon.Generation;

internal sealed class PhaseTimings
{
	private readonly Stopwatch _total = Stopwatch.StartNew();
	private readonly List<(string Name, long Milliseconds)> _phases = [];

	public T Measure<T>(string name, Func<T> action)
	{
		var stopwatch = Stopwatch.StartNew();
		try
		{
			return action();
		}
		finally
		{
			stopwatch.Stop();
			_phases.Add((name, stopwatch.ElapsedMilliseconds));
		}
	}

	public void Measure(string name, Action action)
	{
		Measure(name, () =>
		{
			action();
			return true;
		});
	}

	public void Print()
	{
		_total.Stop();
		Console.WriteLine(">> timings:");
		for (var i = 0; i < _phases.Count; i++)
		{
			var (name, milliseconds) = _phases[i];
			Console.WriteLine($">>   {name}: {milliseconds} ms");
		}

		Console.WriteLine($">>   total: {_total.ElapsedMilliseconds} ms");
	}
}
