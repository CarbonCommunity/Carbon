using System.Diagnostics;
using System.Threading;
using Facepunch;
using Logger = Carbon.Logger;

namespace Carbon.Plugins;

public partial class TimerLibrary
{
	private static readonly object SchedulerLock = new();
	private static ScheduledEntry[] Heap = new ScheduledEntry[InitialHeapCapacity];
	private static int HeapCount;
	private static long HeapSequence;

	private const int InitialHeapCapacity = 1024;
	private const int MaxTimersPerFrame = 8192;
	private const int LivenessChecksPerFrame = 50;
	private const float MinimumRepeatDelay = 0.001f;

	private static bool ClockPrimed;
	private static bool ProcessingTimers;
	private static int LivenessIndex;

	private struct ScheduledEntry
	{
		public double At;
		public long Sequence;
		public TimerInstance Instance;
	}

	private static readonly double TimestampToSeconds = 1d / Stopwatch.Frequency;
	private static double ClockOffset = -Stopwatch.GetTimestamp() * TimestampToSeconds;

	static TimerLibrary()
	{
		try
		{
			if (ThreadEx.IsOnMainThread())
			{
				PrimeClock();
			}
		}
		catch
		{
		}
	}

	internal static void PrimeClock()
	{
		var realtime = UnityEngine.Time.realtimeSinceStartupAsDouble;
		var timestamp = Stopwatch.GetTimestamp();

		lock (SchedulerLock)
		{
			UpdateClock(realtime, timestamp);
		}
	}

	private static void UpdateClock(double realtime, long timestamp)
	{
		var offset = realtime - timestamp * TimestampToSeconds;

		if (!ClockPrimed)
		{
			ClockPrimed = true;
			var adjustment = offset - ClockOffset;

			for (var i = 0; i < HeapCount; i++)
			{
				Heap[i].At += adjustment;
				Heap[i].Instance.ExpiresAtDouble += adjustment;
			}
		}

		Volatile.Write(ref ClockOffset, offset);
	}

	internal static double CurrentTime => Stopwatch.GetTimestamp() * TimestampToSeconds + Volatile.Read(ref ClockOffset);

	internal static float NormalizeRepeatDelay(float delay)
	{
		return delay > MinimumRepeatDelay ? delay : MinimumRepeatDelay;
	}

	internal static void ScheduleIn(TimerInstance timer, float delay)
	{
		lock (SchedulerLock)
		{
			Schedule(timer, CurrentTime + delay);
		}
	}

	internal static void Schedule(TimerInstance timer, double at)
	{
		if (timer.Destroyed)
		{
			return;
		}

		if (double.IsNaN(at))
		{
			at = double.NegativeInfinity;
		}

		if (timer.HeapIndex >= 0)
		{
			RemoveAt(timer.HeapIndex);
		}

		timer.ExpiresAtDouble = at;

		Push(new ScheduledEntry
		{
			At = at,
			Sequence = ++HeapSequence,
			Instance = timer
		});
	}

	internal static void Unschedule(TimerInstance timer)
	{
		if (timer.HeapIndex < 0)
		{
			return;
		}

		RemoveAt(timer.HeapIndex);
		timer.HeapIndex = -1;
	}

	internal static void ProcessTimers(int maxTimers = MaxTimersPerFrame)
	{
		if (ProcessingTimers)
		{
			return;
		}

		var timers = (List<TimerInstance>)null;
		ProcessingTimers = true;

		try
		{
			var now = UnityEngine.Time.realtimeSinceStartupAsDouble;
			var timestamp = Stopwatch.GetTimestamp();
			bool hasDue;

			lock (SchedulerLock)
			{
				UpdateClock(now, timestamp);
				hasDue = HasDueTimers(now);
			}

			PurgeDeadTimers();

			if (!hasDue)
			{
				return;
			}

			timers = Pool.Get<List<TimerInstance>>();
			CollectDueTimers(timers, now, maxTimers);
			FireTimers(timers, now);
		}
		finally
		{
			ProcessingTimers = false;

			if (timers != null)
			{
				Pool.FreeUnmanaged(ref timers);
			}
		}
	}

	private static void PurgeDeadTimers()
	{
		var dead = (List<TimerInstance>)null;

		lock (SchedulerLock)
		{
			if (HeapCount == 0)
			{
				LivenessIndex = 0;
				return;
			}

			if (LivenessIndex >= HeapCount)
			{
				LivenessIndex = 0;
			}

			var end = Math.Min(LivenessIndex + LivenessChecksPerFrame, HeapCount);
			while (LivenessIndex < end)
			{
				var instance = Heap[LivenessIndex].Instance;
				if (instance.Destroyed || instance.Persistence == null || instance.Callback == null)
				{
					dead ??= Pool.Get<List<TimerInstance>>();
					dead.Add(instance);
				}
				LivenessIndex++;
			}
		}

		if (dead == null)
		{
			return;
		}

		for (var i = 0; i < dead.Count; i++)
		{
			dead[i].Destroy();
		}

		Pool.FreeUnmanaged(ref dead);
	}

	private static bool HasDueTimers(double now)
	{
		return HeapCount > 0 && Heap[0].At <= now;
	}

	private static void CollectDueTimers(List<TimerInstance> timers, double now, int maxTimers)
	{
		lock (SchedulerLock)
		{
			while (timers.Count < maxTimers && HasDueTimers(now))
			{
				var timer = Heap[0].Instance;
				timer.DueAt = Heap[0].At;
				RemoveAt(0);
				timer.HeapIndex = -1;

				if (timer.Destroyed || timer.Persistence == null || timer.Callback == null)
				{
					timer.Destroyed = true;
					timer.Callback = null;
					timer.OwnerTimers?.UntrackTimer(timer);
					continue;
				}

				timer.CollectedGeneration = timer.Generation;
				timers.Add(timer);
			}
		}
	}

	private static void FireTimers(List<TimerInstance> timers, double now)
	{
		for (var i = 0; i < timers.Count; i++)
		{
			FireTimer(timers[i], now);
		}
	}

	private static void FireTimer(TimerInstance timer, double now)
	{
		var generation = timer.CollectedGeneration;

		try
		{
			FireCollectedTimer(timer, generation, now);
		}
		catch (Exception ex)
		{
			lock (SchedulerLock)
			{
				if (!timer.Destroyed && timer.Generation == generation)
				{
					timer.Destroy();
				}
			}

			try
			{
				Logger.Error($"Failed processing a timer of {timer.Delay}s in '{timer.Plugin?.ToPrettyString() ?? "unknown plugin"}'", ex);
			}
			catch
			{
			}
		}
	}

	private static void FireCollectedTimer(TimerInstance timer, int generation, double now)
	{
		if (timer.Destroyed || timer.Generation != generation)
		{
			return;
		}

		if (timer.Persistence == null)
		{
			timer.Destroy();
			return;
		}

		try
		{
			timer.Activity?.Invoke();
		}
		catch (Exception ex)
		{
			Logger.Error($"Timer of {timer.Delay}s has failed in '{timer.Plugin?.ToPrettyString() ?? "unknown plugin"}' [callback]", ex);
			timer.Destroy();
		}

		lock (SchedulerLock)
		{
			if (timer.Destroyed || timer.Generation != generation)
			{
				return;
			}

			timer.TimesTriggered++;

			if (ShouldRequeue(timer))
			{
				var delay = (double)NormalizeRepeatDelay(timer.Delay);
				var next = now <= timer.DueAt
					? timer.DueAt + delay
					: now + delay - ((now - timer.DueAt) % delay);
				Schedule(timer, next);
			}
			else
			{
				timer.Destroy();
			}
		}
	}

	private static bool ShouldRequeue(TimerInstance timer)
	{
		if (!timer.Repeating || timer.Destroyed || timer.Persistence == null)
		{
			return false;
		}

		return timer.Repetitions <= 0 || timer.TimesTriggered < timer.Repetitions;
	}

	private static void Push(ScheduledEntry entry)
	{
		if (HeapCount == Heap.Length)
		{
			Array.Resize(ref Heap, Heap.Length << 1);
		}

		Heap[HeapCount] = entry;
		SiftUp(HeapCount);
		HeapCount++;
	}

	private static void RemoveAt(int index)
	{
		HeapCount--;

		if (index == HeapCount)
		{
			Heap[index] = default;
			return;
		}

		Heap[index] = Heap[HeapCount];
		Heap[HeapCount] = default;

		var instance = Heap[index].Instance;
		instance.HeapIndex = index;

		SiftDown(index);

		if (instance.HeapIndex == index)
		{
			SiftUp(index);
		}
	}

	private static void SiftUp(int index)
	{
		var entry = Heap[index];

		while (index > 0)
		{
			var parent = (index - 1) >> 1;
			if (!IsBefore(in entry, in Heap[parent]))
			{
				break;
			}

			Heap[index] = Heap[parent];
			Heap[index].Instance.HeapIndex = index;
			index = parent;
		}

		Heap[index] = entry;
		entry.Instance.HeapIndex = index;
	}

	private static void SiftDown(int index)
	{
		var entry = Heap[index];

		while (true)
		{
			var child = (index << 1) + 1;
			if (child >= HeapCount)
			{
				break;
			}

			if (child + 1 < HeapCount && IsBefore(in Heap[child + 1], in Heap[child]))
			{
				child++;
			}

			if (!IsBefore(in Heap[child], in entry))
			{
				break;
			}

			Heap[index] = Heap[child];
			Heap[index].Instance.HeapIndex = index;
			index = child;
		}

		Heap[index] = entry;
		entry.Instance.HeapIndex = index;
	}

	private static bool IsBefore(in ScheduledEntry a, in ScheduledEntry b)
	{
		if (a.At != b.At)
		{
			return a.At < b.At;
		}

		return a.Sequence < b.Sequence;
	}
}
