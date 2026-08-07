using System.Linq;
using System.Text;
using Facepunch;
using Newtonsoft.Json;
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
	public struct Sample
	{
		public double Duration;
		public bool IsCompared;
		public AssemblyOutput Assemblies;
		public CallOutput Calls;
		public MemoryOutput Memory;
		public GCRecord GC;
		public SampleComparison Comparison;

		public struct SampleComparison
		{
			public Difference Duration;
		}

		public static Sample Create() => new()
		{
			Duration = 0,
			Assemblies = new(),
			Calls = new(),
			Memory = new(),
			GC = default
		};
		public static Sample Load(byte[] data)
		{
			return DeserializeSample(data);
		}

		[JsonIgnore] public bool FromDisk;
		[JsonIgnore] public bool IsCleared => Assemblies == null || !Assemblies.Any();

		public Sample Compare(Sample other)
		{
			Sample sample = default;
			sample.FromDisk = true;
			sample.Duration = CompareValue(Duration, other.Duration);
			sample.Comparison.Duration = Compare(Duration, other.Duration);
			sample.Assemblies = Assemblies.Compare(other.Assemblies);
			sample.Calls = Calls.Compare(other.Calls);
			sample.Memory = Memory.Compare(other.Memory);
			sample.GC = GC.Compare(other.GC);
			sample.IsCompared = true;
			return sample;
		}

		public void Resample()
		{
			Clear();

			FromDisk = false;
			IsCompared = false;
			Duration = DurationTime.TotalSeconds;
			Comparison = default;
			Assemblies.AddRange(AssemblyRecords);
			Calls.AddRange(CallRecords);
			Memory.AddRange(MemoryRecords);
			GC = GCStats;
		}
		public void Clear()
		{
			IsCompared = false;
			Duration = default;
			Comparison = default;
			FromDisk = false;
			Assemblies ??= new();
			Calls ??= new();
			Memory ??= new();

			Assemblies.Clear();
			Calls.Clear();
			Memory.Clear();
			GC = default;
		}

		public enum Difference
		{
			None,
			ValueHigher,
			ValueEqual,
			ValueLower
		}

		public string ToTable()
		{
			var builder = Pool.Get<StringBuilder>();
			builder.AppendLine(Assemblies.ToTable());
			builder.AppendLine(Calls.ToTable());
			builder.AppendLine(Memory.ToTable());
			builder.AppendLine(GC.ToTable());

			var result = builder.ToString();
			Pool.FreeUnmanaged(ref builder);
			return result;
		}
		public string ToCSV()
		{
			var builder = Pool.Get<StringBuilder>();
			builder.AppendLine(Assemblies.ToCSV());
			builder.AppendLine(Calls.ToCSV());
			builder.AppendLine(Memory.ToCSV());
			builder.AppendLine(GC.ToCSV());

			var result = builder.ToString();
			Pool.FreeUnmanaged(ref builder);
			return result;
		}
		public string ToJson(bool indented)
		{
			return JsonConvert.SerializeObject(this, indented ? Formatting.Indented : Formatting.None);
		}
		public byte[] ToProto()
		{
			return SerializeSample(this);
		}

		public static Difference Compare(ulong a, ulong b)
		{
			if (a == b)
			{
				return Difference.ValueEqual;
			}

			return a > b ? Difference.ValueHigher : Difference.ValueLower;
		}
		public static Difference Compare(uint a, uint b)
		{
			if (a == b)
			{
				return Difference.ValueEqual;
			}

			return a > b ? Difference.ValueHigher : Difference.ValueLower;
		}
		public static Difference Compare(double a, double b)
		{
			if (a == b)
			{
				return Difference.ValueEqual;
			}

			return a > b ? Difference.ValueHigher : Difference.ValueLower;
		}

		public const string ValueHigherStr = "<color=#ff370a>\u2191</color>";
		public const string ValueLowerStr = "<color=#91ff0a>\u2193</color>";
		public const string ValueEqualStr = "<color=#fff30a>—</color>";

		public static string GetDifferenceString(Difference difference)
		{
			return difference switch
			{
				Difference.ValueHigher => ValueHigherStr,
				Difference.ValueEqual => ValueEqualStr,
				Difference.ValueLower => ValueLowerStr,
				_ => string.Empty
			};
		}

		public static ulong CompareValue(ulong a, ulong b)
		{
			return Max(a, b) - Min(a, b);
		}
		public static uint CompareValue(uint a, uint b)
		{
			return Max(a, b) - Min(a, b);
		}
		public static double CompareValue(double a, double b)
		{
			return Max(a, b) - Min(a, b);
		}

		private static ulong Max(ulong a, ulong b)
		{
			return a > b ? a : b;
		}
		private static ulong Min(ulong a, ulong b)
		{
			return a < b ? a : b;
		}
		private static uint Max(uint a, uint b)
		{
			return a > b ? a : b;
		}
		private static uint Min(uint a, uint b)
		{
			return a < b ? a : b;
		}
		private static double Max(double a, double b)
		{
			return a > b ? a : b;
		}
		private static double Min(double a, double b)
		{
			return a < b ? a : b;
		}
	}
}
