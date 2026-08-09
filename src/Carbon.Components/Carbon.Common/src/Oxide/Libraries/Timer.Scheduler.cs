using System.Diagnostics;
using Facepunch;
using Logger = Carbon.Logger;

namespace Oxide.Core.Libraries;

public partial class Timer
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

	private sealed class ClockSample
	{
		public double Realtime;
		public long Timestamp;
	}

	private static volatile ClockSample Clock = new()
	{
		Realtime = 0d,
		Timestamp = Stopwatch.GetTimestamp()
	};

	static Timer()
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
		var sample = new ClockSample
		{
			Realtime = UnityEngine.Time.realtimeSinceStartupAsDouble,
			Timestamp = Stopwatch.GetTimestamp()
		};

		lock (SchedulerLock)
		{
			if (!ClockPrimed)
			{
				RebaseDeadlines(sample);
			}

			Clock = sample;
		}
	}

	private static void RebaseDeadlines(ClockSample sample)
	{
		ClockPrimed = true;

		var clock = Clock;
		var offset = sample.Realtime - (clock.Realtime + (sample.Timestamp - clock.Timestamp) / (double)Stopwatch.Frequency);

		if (offset == 0)
		{
			return;
		}

		for (var i = 0; i < HeapCount; i++)
		{
			Heap[i].At += offset;
			Heap[i].Instance.ExpiresAtDouble += offset;
		}
	}

	internal static double CurrentTime
	{
		get
		{
			var clock = Clock;
			return clock.Realtime + (Stopwatch.GetTimestamp() - clock.Timestamp) / (double)Stopwatch.Frequency;
		}
	}

	internal static float NormalizeRepeatDelay(float delay)
	{
		return delay > MinimumRepeatDelay ? delay : MinimumRepeatDelay;
	}

	internal static InvokeTrackingData ResolveTracking(Action action)
	{
		return ThreadEx.IsOnMainThread() ? InvokeProfiler.update.GetTrackingData(new InvokeTrackingKey(action)) : null;
	}

	internal static void Schedule(TimerInstance timer, double at, bool requeue = false)
	{
		if (double.IsNaN(at))
		{
			at = double.NegativeInfinity;
		}

		lock (SchedulerLock)
		{
			if (timer.Destroyed)
			{
				return;
			}

			if (timer.HeapIndex >= 0)
			{
				RemoveAt(timer.HeapIndex);
			}
			else if (timer.Tracking != null)
			{
				timer.Tracking.InvokeCount++;
			}

			if (!requeue)
			{
				InvokeProfiler.update.addCount++;
			}

			timer.ExpiresAtDouble = at;

			Push(new ScheduledEntry
			{
				At = at,
				Sequence = ++HeapSequence,
				Instance = timer
			});
		}
	}

	internal static void Unschedule(TimerInstance timer)
	{
		lock (SchedulerLock)
		{
			if (timer.HeapIndex < 0)
			{
				return;
			}

			RemoveAt(timer.HeapIndex);
			timer.HeapIndex = -1;

			InvokeProfiler.update.deletedCount++;

			if (timer.Tracking != null)
			{
				timer.Tracking.InvokeCount--;
			}
		}
	}

	internal static void ProcessTimers(int maxTimers = MaxTimersPerFrame)
	{
		if (ProcessingTimers)
		{
			return;
		}

		var sample = new ClockSample
		{
			Realtime = UnityEngine.Time.realtimeSinceStartupAsDouble,
			Timestamp = Stopwatch.GetTimestamp()
		};
		var now = sample.Realtime;
		var hasDue = false;

		lock (SchedulerLock)
		{
			if (!ClockPrimed)
			{
				RebaseDeadlines(sample);
			}

			Clock = sample;
			hasDue = HasDueTimers(now);
		}

		PurgeDeadTimers();

		if (!hasDue)
		{
			return;
		}

		var timers = (List<TimerInstance>)null;
		ProcessingTimers = true;

		try
		{
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

				if (timer.Tracking != null)
				{
					timer.Tracking.InvokeCount--;
				}

				if (timer.Destroyed || timer.Persistence == null || timer.Callback == null)
				{
					InvokeProfiler.update.deletedCount++;
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
		var profiler = InvokeProfiler.update;
		var trackExecution = profiler.mode > 1;

		for (var i = 0; i < timers.Count; i++)
		{
			FireTimer(timers[i], profiler, trackExecution, now);
		}
	}

	private static void FireTimer(TimerInstance timer, InvokeProfiler profiler, bool trackExecution, double now)
	{
		var generation = timer.CollectedGeneration;

		try
		{
			FireCollectedTimer(timer, generation, profiler, trackExecution, now);
		}
		catch (Exception ex)
		{
			lock (SchedulerLock)
			{
				if (!timer.Destroyed && timer.Generation == generation)
				{
					profiler.deletedCount++;
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

	private static void FireCollectedTimer(TimerInstance timer, int generation, InvokeProfiler profiler, bool trackExecution, double now)
	{
		if (timer.Destroyed || timer.Generation != generation)
		{
			profiler.deletedCount++;
			return;
		}

		if (timer.Persistence == null)
		{
			profiler.deletedCount++;
			timer.Destroy();
			return;
		}

		var activity = timer.Activity;
		timer.Tracking ??= ResolveTracking(activity);

		try
		{
			if (trackExecution && timer.Tracking != null)
			{
				var started = Stopwatch.GetTimestamp();
				try
				{
					activity?.Invoke();
				}
				finally
				{
					var elapsed = Stopwatch.GetTimestamp() - started;
					timer.Tracking.ExecutionTime += TimeSpan.FromSeconds(elapsed / (double)Stopwatch.Frequency);
					timer.Tracking.Calls++;
				}
			}
			else
			{
				activity?.Invoke();
			}
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
				profiler.deletedCount++;
				return;
			}

			timer.TimesTriggered++;

			if (ShouldRequeue(timer))
			{
				var delay = (double)NormalizeRepeatDelay(timer.Delay);
				var next = now <= timer.DueAt
					? timer.DueAt + delay
					: now + delay - ((now - timer.DueAt) % delay);
				if (double.IsNaN(next) || double.IsInfinity(next))
				{
					next = now + delay;
				}
				Schedule(timer, next, requeue: true);
			}
			else
			{
				profiler.deletedCount++;
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
