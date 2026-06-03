using System;
using System.Collections.Generic;
using ProtoBuf;
using UnityEngine;

/*
 *
 * Copyright (c) 2023 Patrette, under the GNU v3 license rights
 * Copyright (c) 2023-2024 Carbon Community, under the GNU v3 license rights
 *
 */

namespace Carbon.Components;

public partial class MonoProfiler
{
	public class TimelineRecording
	{
		public StatusTypes Status;
		public float Rate;
		public float Duration;
		public Timeline Timeline = new();
		public ProfilerArgs Args = AllFlags;

		public Action<Sample> OnSample;
		public Action<bool> OnStopped;

		private DateTime _timeSinceStart;

		public enum StatusTypes
		{
			None,
			Running,
			Discarded,
			Completed
		}
		public double CurrentDuration => (DateTime.Now - _timeSinceStart).TotalSeconds;

		public bool IsRecording() => Status == StatusTypes.Running;
		public bool IsDiscarded() => Status == StatusTypes.Discarded;
		public bool IsClear() => Timeline.Count == 0;

		private Sample Record(AssemblyOutput assemblies, CallOutput calls, MemoryOutput memory, GCRecord gc)
		{
			var snapshot = Sample.Create();
			snapshot.Assemblies.AddRange(assemblies);
			snapshot.Calls.AddRange(calls);
			snapshot.Memory.AddRange(memory);
			snapshot.GC = gc;

			Record(snapshot);
			return snapshot;
		}
		private void Record(Sample snapshot)
		{
			Timeline.Add(DateTime.Now, snapshot);
		}
		private void Clear()
		{
			foreach (var timeline in Timeline)
			{
				timeline.Value.Clear();
			}

			Timeline.Clear();
			Duration = 0;
			Rate = 0;
		}

		public TimelineRecording Start(float rate, float duration, ProfilerArgs args, Action<bool> onStopped)
		{
			if (Status == StatusTypes.Running)
			{
				UnityEngine.Debug.LogWarning("Timeline is already recording.");
				return this;
			}

			Rate = rate;
			Duration = duration;
			Args = args | ProfilerArgs.FastResume;
			OnStopped = onStopped;

			if (MonoProfiler.IsRecording)
			{
				ToggleProfiling(ProfilerArgs.Abort);
			}

			_timeSinceStart = DateTime.Now;
			Status = StatusTypes.Running;

			UnityEngine.Debug.LogWarning("Started timeline recording..");

			Recurse(this);

			static void Recurse(TimelineRecording recording)
			{
				ToggleProfilingTimed(recording.Rate, recording.Args, _ =>
				{
					var snapshot = recording.Record(AssemblyRecords, CallRecords, MemoryRecords, GCStats);
					recording.OnSample?.Invoke(snapshot);

					if (recording.CurrentDuration >= recording.Duration)
					{
						recording.Stop();
						return;
					}

					Recurse(recording);
				}, false);
			}

			return this;
		}
		public void Stop(bool discard = false)
		{
			if (MonoProfiler.IsRecording)
			{
				var snapshot = Record(AssemblyRecords, CallRecords, MemoryRecords, GCStats);
				OnSample?.Invoke(snapshot);
			}

			UnityEngine.Debug.LogWarning($"Ended timeline recording.{(discard ? " Discarded." : string.Empty)}");

			if (discard)
			{
				Discard();
			}
			else
			{
				Status = StatusTypes.Completed;
			}

			OnStopped?.Invoke(discard);
		}
		public void Discard()
		{
			if (MonoProfiler.IsRecording)
			{
				ToggleProfiling(ProfilerArgs.Abort, false);
			}

			Clear();
			Status = StatusTypes.Discarded;
		}

		public static TimelineRecording Create(float rate, float duration, ProfilerArgs args, Action<bool> onStopped)
		{
			return new TimelineRecording().Start(rate, duration, args, onStopped);
		}
	}

	public class Timeline : Dictionary<DateTime, Sample>;
}
