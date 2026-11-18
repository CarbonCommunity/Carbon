namespace Carbon.Components;

/// <summary>
/// Measures the memory one hook call allocates, without what the hooks called from inside it allocate.
/// </summary>
public static class HookMemory
{
	public enum Modes : byte
	{
		/// <summary>Hook memory isn't tracked.</summary>
		None,

		/// <summary>
		/// Growth of the managed heap during the call. A collection during the call hides what the call
		/// allocated before it, and allocations of other threads are included.
		/// </summary>
		Estimate,

		/// <summary>Bytes the calling thread allocated, counted by the native profiler.</summary>
		Precise
	}

	public struct Scope
	{
		public Modes Mode;
		internal long heap;
		internal long nested;
	}

	// Heap growth of the estimated calls that ended inside the estimated call now running on this thread
	[ThreadStatic]
	private static long _nested;

	public static Scope Begin()
	{
		var config = Community.Runtime.Config.Debugging;
		var scope = default(Scope);

		if (!config.TrackHookMemory)
		{
			return scope;
		}

		// The native counters can only be switched from the main thread
		if (config.PreciseHookMemory && ThreadEx.IsOnMainThread() && MonoProfiler.BeginAllocationScope())
		{
			scope.Mode = Modes.Precise;
			return scope;
		}

		scope.Mode = Modes.Estimate;
		scope.nested = _nested;
		_nested = 0L;
		scope.heap = GC.GetTotalMemory(false);
		return scope;
	}

	/// <returns>The bytes the call allocated.</returns>
	public static long End(in Scope scope)
	{
		switch (scope.Mode)
		{
			case Modes.Precise:
				return MonoProfiler.EndAllocationScope();

			case Modes.Estimate:
			{
				// A heap that shrank was collected during the call: memory freed, not memory used
				var growth = Math.Max(GC.GetTotalMemory(false) - scope.heap, 0L);
				var own = Math.Max(growth - _nested, 0L);
				_nested = scope.nested + growth;
				return own;
			}

			default:
				return 0L;
		}
	}
}
