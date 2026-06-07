using System.Collections.Concurrent;
using System.Text;
using System.Threading;

namespace Carbon.InternalCallHookGeneration;

/// <summary>
/// Thread-safe pool of reusable <see cref="StringBuilder"/> instances.
/// Source generator code runs in the Roslyn compiler context where syntax providers
/// and source-production callbacks may execute in parallel, so all pool operations
/// must be safe for concurrent access from multiple threads.
/// </summary>
internal static class StringBuilderPool
{
	private const int InitialCapacity = 1024;
	private const int MaxRetainedCapacity = 32 * 1024;
	private const int MaxPoolSize = 64;

	private static readonly ConcurrentQueue<StringBuilder> Pool = new();
	private static int _count;

	/// <summary>
	/// Rents a cleared <see cref="StringBuilder"/> from the pool, or allocates a new
	/// one if the pool is empty. Always returns a usable instance.
	/// </summary>
	public static StringBuilder Rent()
	{
		if (Pool.TryDequeue(out var builder))
		{
			Interlocked.Decrement(ref _count);
			return builder;
		}

		return new StringBuilder(InitialCapacity);
	}

	/// <summary>
	/// Materializes the builder content and returns it to the pool in a single call.
	/// Prefer this over manual <see cref="Return"/> to avoid forgetting to release.
	/// </summary>
	public static string ToStringAndReturn(ref StringBuilder builder)
	{
		var result = builder.ToString();
		Return(ref builder);
		return result;
	}

	/// <summary>
	/// Returns a <see cref="StringBuilder"/> to the pool. Oversized builders and
	/// instances beyond the pool size cap are dropped to avoid unbounded memory growth.
	/// Safe to call concurrently from multiple threads.
	/// </summary>
	public static void Return(ref StringBuilder builder)
	{
		if (builder is null || builder.Capacity > MaxRetainedCapacity)
		{
			return;
		}

		builder.Clear();

		// Reserve a slot before enqueueing. If the pool is already at capacity,
		// release the reservation and drop the builder.
		if (Interlocked.Increment(ref _count) > MaxPoolSize)
		{
			Interlocked.Decrement(ref _count);
			return;
		}

		Pool.Enqueue(builder);
		builder = null;
	}
}
