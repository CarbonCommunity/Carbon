using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using Carbon.Profiler;
using Facepunch;
using Facepunch.Extend;
using Newtonsoft.Json;

/*
 *
 * Copyright (c) 2024-2025 Carbon Community
 * Copyright (c) 2024-2025 Patrette
 * All rights reserved.
 *
 */

#pragma warning disable CS8500

namespace Carbon.Components;

/// <summary>
/// Carbon MonoProfiler tracking assembly, call, memory and GC information.
/// It directly interacts with Carbon's Native Rust-written library to start and stop tracking at a native level, making customized tracking as lightweight as it can get.
/// </summary>
[SuppressUnmanagedCodeSecurity]
public static unsafe partial class MonoProfiler
{
	public const string ProfileExtension = "cprf";
	public static readonly string DecimalFormat = "n0";

	public const ProfilerArgs AllFlags = AllNoTimingsFlags | ProfilerArgs.Timings;
	public const ProfilerArgs AllNoTimingsFlags = ProfilerArgs.Calls | ProfilerArgs.CallMemory
	                                                                 | ProfilerArgs.AdvancedMemory | ProfilerArgs.GCEvents;

	public static GCRecord GCStats;
	public static AssemblyOutput AssemblyRecords = new();
	public static CallOutput CallRecords = new();
	public static MemoryOutput MemoryRecords = new();
	public static RuntimeAssemblyBank AssemblyBank = new();
	public static ConcurrentDictionary<ModuleHandle, AssemblyNameEntry> AssemblyMap = new();
	public static Dictionary<IntPtr, string> ClassMap = new();
	public static Dictionary<IntPtr, string> MethodMap = new();
	public static TimeSpan DataProcessingTime;
	public static TimeSpan DurationTime;

	public static TimeSpan CurrentDurationTime => (_durationTimer?.Elapsed).GetValueOrDefault();

	private static Stopwatch _dataProcessTimer;
	private static Stopwatch _durationTimer;
	private static Action _profileTimer;
	private static Action _profileWarningTimer;

	public enum ProfilerResultCode : byte
	{
		OK = 0,
		InvalidArgs = 1,
		Aborted = 2,
		MainThreadOnly = 3,
		NotInitialized = 4,
		CorruptedState = 5,
		UnknownError = 6,
		Busy = 7,
		NoOp = 8
	}

	/// <summary>
	/// Assembly information provided by memory or call output data.
	/// </summary>
	public class AssemblyNameEntry
	{
		public string name;
		public string displayName;
		public string displayNameNonIncrement;
		public MonoProfilerConfig.ProfileTypes profileType;

		public string GetDisplayName(bool isCompared)
		{
			return isCompared ? displayNameNonIncrement : displayName;
		}
	}

	/// <summary>
	/// Assembly record dataset.
	/// </summary>
	public class AssemblyOutput : List<AssemblyRecord>
	{
		public bool AnyValidRecords => Count > 0;

		public AssemblyOutput Compare(AssemblyOutput other)
		{
			if (other == null)
			{
				return null;
			}

			var comparison = new AssemblyOutput();

			comparison.AddRange(
				from record in this
				let otherRecord = other.FirstOrDefault(x =>
					x.assembly_name.displayNameNonIncrement == record.assembly_name.displayNameNonIncrement)
				select new AssemblyRecord
			{
				assembly_name = record.assembly_name,
				total_time = AreRecordsValid(record, otherRecord) ? Sample.CompareValue(record.total_time, otherRecord.total_time) : default,
				total_time_percentage = AreRecordsValid(record, otherRecord) ? Sample.CompareValue(record.total_time_percentage, otherRecord.total_time_percentage) : default,
				total_exceptions = AreRecordsValid(record, otherRecord) ? Sample.CompareValue(record.total_exceptions, otherRecord.total_exceptions) : default,
				calls = AreRecordsValid(record, otherRecord) ? Sample.CompareValue(record.calls, otherRecord.calls) : default,
				alloc = AreRecordsValid(record, otherRecord) ? Sample.CompareValue(record.alloc, otherRecord.alloc) : default,
				comparison = new()
				{
					isCompared = true,
					total_time = AreRecordsValid(record, otherRecord) ? Sample.Compare(record.total_time, otherRecord.total_time) : Sample.Difference.None,
					total_exceptions = AreRecordsValid(record, otherRecord) ? Sample.Compare(record.total_exceptions, otherRecord.total_exceptions) : Sample.Difference.None,
					calls = AreRecordsValid(record, otherRecord) ? Sample.Compare(record.calls, otherRecord.calls) : Sample.Difference.None,
					alloc = AreRecordsValid(record, otherRecord) ? Sample.Compare(record.alloc, otherRecord.alloc) : Sample.Difference.None
				}
			});

			return comparison;
		}
		public static bool AreRecordsValid(AssemblyRecord recordA, AssemblyRecord recordB) =>
			recordA.IsValid && recordB.IsValid;

		public string ToTable()
		{
			var table = Pool.Get<TextTable>();
			table.Clear();
			table.AddColumns("assembly", "total time", "(%)", "calls", "exceptions", "allocations");

			foreach(AssemblyRecord record in this)
			{
				if (!AssemblyMap.TryGetValue(record.assembly_handle, out AssemblyNameEntry assemblyName))
				{
					continue;
				}

				table.AddRow($"{assemblyName.GetDisplayName(record.comparison.isCompared)}",
					record.total_time == 0 ? string.Empty : record.GetTotalTime(),
					record.total_time_percentage == 0 ? string.Empty : $"{record.total_time_percentage:0}%",
					record.calls == 0 ? string.Empty : record.calls.ToString(DecimalFormat),
					record.total_exceptions == 0 ? string.Empty : record.total_exceptions.ToString(DecimalFormat),
					record.alloc.FormatBytes(true));
			}

			var result = table.ToString();
			Pool.FreeUnsafe(ref table);
			return result;
		}
		public string ToCSV()
		{
			var builder = Pool.Get<StringBuilder>();
			builder.AppendLine("assembly," +
			                   "total time," +
			                   "(%)," +
			                   "calls," +
			                   "exceptions," +
			                   "allocations");

			foreach (AssemblyRecord record in this)
			{
				if (!AssemblyMap.TryGetValue(record.assembly_handle, out AssemblyNameEntry assemblyName))
				{
					continue;
				}

				builder.AppendLine($"{assemblyName.GetDisplayName(record.comparison.isCompared)}," +
				                   $"{record.GetTotalTime()}," +
				                   $"{record.total_time_percentage:0}%," +
				                   $"{record.calls.ToString(DecimalFormat)}," +
				                   $"{record.total_exceptions.ToString(DecimalFormat)}," +
				                   $"{record.alloc.FormatBytes(true)}");
			}

			string result = builder.ToString();
			Pool.FreeUnmanaged(ref builder);
			return result;
		}
		public string ToJson(bool indented)
		{
			return JsonConvert.SerializeObject(this, indented ? Formatting.Indented : Formatting.None);
		}
	}

	/// <summary>
	/// Call record dataset.
	/// </summary>
	public class CallOutput : List<CallRecord>
	{
		public bool AnyValidRecords => Count > 0;
		public bool Disabled;

		public CallOutput Compare(CallOutput other)
		{
			if (other == null)
			{
				return null;
			}

			var comparison = new CallOutput();

			comparison.AddRange(
				from record in this
				let otherRecord = other.FirstOrDefault(x =>
					x.assembly_name.displayNameNonIncrement == record.assembly_name.displayNameNonIncrement &&
				    x.method_name == record.method_name)
				select new CallRecord
			{
				assembly_name = record.assembly_name,
				method_name = record.method_name,

				total_time = AreRecordsValid(record, otherRecord) ? Sample.CompareValue(record.total_time, otherRecord.total_time) : default,
				total_time_percentage = AreRecordsValid(record, otherRecord) ? Sample.CompareValue(record.total_time_percentage, otherRecord.total_time_percentage) : default,
				own_time = AreRecordsValid(record, otherRecord) ? Sample.CompareValue(record.own_time, otherRecord.own_time) : default,
				own_time_percentage = AreRecordsValid(record, otherRecord) ? Sample.CompareValue(record.own_time_percentage, otherRecord.own_time_percentage) : default,
				calls = AreRecordsValid(record, otherRecord) ? Sample.CompareValue(record.calls, otherRecord.calls) : default,
				total_alloc = AreRecordsValid(record, otherRecord) ? Sample.CompareValue(record.total_alloc, otherRecord.total_alloc) : default,
				own_alloc = AreRecordsValid(record, otherRecord) ? Sample.CompareValue(record.own_alloc, otherRecord.own_alloc) : default,
				total_exceptions = AreRecordsValid(record, otherRecord) ? Sample.CompareValue(record.total_exceptions, otherRecord.total_exceptions) : default,
				own_exceptions = AreRecordsValid(record, otherRecord) ? Sample.CompareValue(record.own_exceptions, otherRecord.own_exceptions) : default,
				comparison = new()
				{
					isCompared = true,
					total_time = AreRecordsValid(record, otherRecord) ? Sample.Compare(record.total_time, otherRecord.total_time) : Sample.Difference.None,
					own_time = AreRecordsValid(record, otherRecord) ? Sample.Compare(record.own_time, otherRecord.own_time) : Sample.Difference.None,
					calls = AreRecordsValid(record, otherRecord) ? Sample.Compare(record.calls, otherRecord.calls) : Sample.Difference.None,
					total_alloc = AreRecordsValid(record, otherRecord) ? Sample.Compare(record.total_alloc, otherRecord.total_alloc) : Sample.Difference.None,
					own_alloc = AreRecordsValid(record, otherRecord) ? Sample.Compare(record.own_alloc, otherRecord.own_alloc) : Sample.Difference.None,
					total_exceptions = AreRecordsValid(record, otherRecord) ? Sample.Compare(record.total_exceptions, otherRecord.total_exceptions) : Sample.Difference.None,
					own_exceptions = AreRecordsValid(record, otherRecord) ? Sample.Compare(record.own_exceptions, otherRecord.own_exceptions) : Sample.Difference.None,
				}
			});

			return comparison;
		}
		public static bool AreRecordsValid(CallRecord recordA, CallRecord recordB) =>
			recordA.IsValid && recordB.IsValid;

		public string ToTable()
		{
			var table = Pool.Get<TextTable>();
			table.Clear();
			table.AddColumns("assembly", "method", "total time", "(%)", "own time", "(%)", "calls", "total exceptions", "own exceptions", "total allocations", "own allocations");

			foreach (CallRecord record in this)
			{
				if (!AssemblyMap.TryGetValue(record.assembly_handle, out AssemblyNameEntry assemblyName))
				{
					continue;
				}

				table.AddRow($"{assemblyName.GetDisplayName(record.comparison.isCompared)}",
					record.method_name,
					record.total_time == 0 ? string.Empty : record.GetTotalTime(),
					record.total_time_percentage == 0 ? string.Empty : $"{record.total_time_percentage:0}%",
					record.own_time == 0 ? string.Empty : record.GetOwnTime(),
					record.own_time_percentage == 0 ? string.Empty : $"{record.own_time_percentage:0}%",
					record.calls == 0 ? string.Empty : record.calls.ToString(DecimalFormat),
					record.total_exceptions == 0 ? string.Empty : record.total_exceptions.ToString(DecimalFormat),
					record.own_exceptions == 0 ? string.Empty : record.own_exceptions.ToString(DecimalFormat),
					record.total_alloc == 0 ? string.Empty : record.total_alloc.FormatBytes(true),
					record.own_alloc == 0 ? string.Empty : record.own_alloc.FormatBytes(true));
			}

			var result = table.ToString();
			Pool.FreeUnsafe(ref table);
			return result;
		}
		public string ToCSV()
		{
			StringBuilder builder = Pool.Get<StringBuilder>();

			builder.AppendLine("assembly,method,total time,(%),own time,(%),calls,total exceptions,own exceptions,total allocations,own allocations");

			foreach (CallRecord record in this)
			{
				if (!AssemblyMap.TryGetValue(record.assembly_handle, out AssemblyNameEntry assemblyName))
				{
					continue;
				}

				builder.AppendLine($"{assemblyName.GetDisplayName(record.comparison.isCompared)}," +
				                   $"{record.method_name}," +
				                   $"{record.GetTotalTime()}," +
				                   $"{record.total_time_percentage:0}%," +
				                   $"{record.GetOwnTime()}," +
				                   $"{record.own_time_percentage:0}%," +
				                   $"{record.calls:n0}," +
				                   $"{record.total_exceptions.ToString(DecimalFormat)}," +
				                   $"{record.own_exceptions.ToString(DecimalFormat)}," +
				                   $"{record.total_alloc.FormatBytes(true)}," +
				                   $"{record.own_alloc.FormatBytes(true)}");
			}

			string result = builder.ToString();
			Pool.FreeUnmanaged(ref builder);
			return result;
		}
		public string ToJson(bool indented)
		{
			return JsonConvert.SerializeObject(this, indented ? Formatting.Indented : Formatting.None);
		}
	}

	/// <summary>
	/// Memory record dataset.
	/// </summary>
	public class MemoryOutput : List<MemoryRecord>
	{
		public MemoryOutput Compare(MemoryOutput other)
		{
			if (other == null)
			{
				return null;
			}

			var comparison = new MemoryOutput();

			comparison.AddRange(
				from record in this
				let otherRecord = other.FirstOrDefault(x =>
					x.assembly_name.displayNameNonIncrement == record.assembly_name.displayNameNonIncrement &&
					x.class_name == record.class_name)
				select new MemoryRecord
			{
				assembly_name = record.assembly_name,
				class_name = record.class_name,
				class_token = record.class_token,

				allocations = AreRecordsValid(record, otherRecord) ? Sample.CompareValue(record.allocations, otherRecord.allocations) : default,
				total_alloc_size = AreRecordsValid(record, otherRecord) ? Sample.CompareValue(record.total_alloc_size, otherRecord.total_alloc_size) : default,
				instance_size = AreRecordsValid(record, otherRecord) ? Sample.CompareValue(record.instance_size, otherRecord.instance_size) : default,
				comparison = new()
				{
					isCompared = true,
					allocations = AreRecordsValid(record, otherRecord) ? Sample.Compare(record.allocations, otherRecord.allocations) : Sample.Difference.None,
					total_alloc_size = AreRecordsValid(record, otherRecord) ? Sample.Compare(record.total_alloc_size, otherRecord.total_alloc_size) : Sample.Difference.None
				}
			});

			return comparison;
		}
		public static bool AreRecordsValid(MemoryRecord recordA, MemoryRecord recordB) =>
			recordA.IsValid && recordB.IsValid;

		public string ToTable()
		{
			var table = Pool.Get<TextTable>();
			table.Clear();
			table.AddColumns("assembly", "class", "allocations", "total allocation size", "instance size");

			foreach (MemoryRecord record in this)
			{
				if (!AssemblyMap.TryGetValue(record.assembly_handle, out AssemblyNameEntry assemblyName))
				{
					continue;
				}

				table.AddRow($"{assemblyName.GetDisplayName(record.comparison.isCompared)}",
					record.class_name,
					record.allocations == 0 ? string.Empty : record.allocations.ToString(DecimalFormat),
					record.total_alloc_size == 0 ? string.Empty : $"{record.total_alloc_size.FormatBytes(true)}",
					record.instance_size == 0 ? string.Empty : $"{record.instance_size.ToString(DecimalFormat)}b");
			}

			var result = table.ToString();
			Pool.FreeUnsafe(ref table);
			return result;
		}
		public string ToCSV()
		{
			StringBuilder builder = Pool.Get<StringBuilder>();
			builder.AppendLine("assembly,class,allocations,total allocation size,instance size");

			foreach (MemoryRecord record in this)
			{
				if (!AssemblyMap.TryGetValue(record.assembly_handle, out AssemblyNameEntry assemblyName))
				{
					continue;
				}

				builder.AppendLine($"{assemblyName.GetDisplayName(record.comparison.isCompared)}," +
				                   $"{record.class_name}," +
				                   $"{record.allocations.ToString(DecimalFormat)}," +
				                   $"{record.total_alloc_size.FormatBytes(true)}," +
				                   $"{record.instance_size.ToString(DecimalFormat)}b");
			}

			string result = builder.ToString();
			Pool.FreeUnmanaged(ref builder);
			return result;
		}
		public string ToJson(bool indented)
		{
			return JsonConvert.SerializeObject(this, indented ? Formatting.Indented : Formatting.None);
		}
	}

	/// <summary>
	/// Used to identify dynamically managed and processed assemblies (such as plugins and harmony mods) whenever they get hotloaded or assemblies with the same name get reloaded with changes.
	/// </summary>
	public class RuntimeAssemblyBank : ConcurrentDictionary<string, int>
	{
		public string Increment(string value)
		{
			return string.IsNullOrEmpty(value) ? string.Empty : $"{value} ({AddOrUpdate(value, 1, (_, arg) => arg + 1)})";
		}
	}

	/// <summary>
	/// Rust-returned dataset structure of a GC record, tracking total calls and time taken in the recording time period.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct GCRecord
	{
		public ulong calls;
		public ulong total_time;
		public Comparison comparison;

		public struct Comparison
		{
			public bool isCompared;
			public Sample.Difference calls_c;
			public Sample.Difference total_time_c;
		}

		public GCRecord Compare(GCRecord other)
		{
			GCRecord record = default;
			record.calls = Sample.CompareValue(calls, other.calls);
			record.total_time = Sample.CompareValue(total_time, other.total_time);

			record.comparison.isCompared = true;
			record.comparison.calls_c = Sample.Compare(record.calls, other.calls);
			record.comparison.total_time_c = Sample.Compare(record.total_time, other.total_time);
			return record;
		}

		public string ToTable()
		{
			var table = Pool.Get<TextTable>();
			table.Clear();
			table.AddColumns("calls", "total time");

			table.AddRow($" {calls:n0}", $"{GetTotalTime()}");

			var result = table.ToString();
			Pool.FreeUnsafe(ref table);
			return result;
		}
		public string ToCSV()
		{
			StringBuilder builder = Pool.Get<StringBuilder>();

			builder.AppendLine("Calls," + "Total Time");
			builder.AppendLine($"{calls}," + $"{GetTotalTime()}");

			string result = builder.ToString();
			Pool.FreeUnmanaged(ref builder);
			return result;
		}
		public string ToJson(bool indented)
		{
			return JsonConvert.SerializeObject(this, indented ? Formatting.Indented : Formatting.None);
		}

		// managed
		public double total_time_ms => total_time * 0.001f;

		private string total_time_ms_str;

		public string GetTotalTime() => total_time_ms_str ??= (total_time_ms < 10 ? $"{total_time:n0}μs" : $"{total_time_ms:n0}ms");
	}

	/// <summary>
	/// Rust-returned dataset structure of an Assembly record, tracking total time, time percentage, exceptions, call and allocations taken in the recording time period.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct AssemblyRecord
	{
		[JsonIgnore] public ModuleHandle assembly_handle;

		public ulong total_time;
		public double total_time_percentage;
		public ulong total_exceptions;
		public ulong calls;
		public ulong alloc;
		public Comparison comparison;
		public AssemblyNameEntry assembly_name;

		public struct Comparison
		{
			public bool isCompared;
			public Sample.Difference total_time;
			public Sample.Difference total_exceptions;
			public Sample.Difference calls;
			public Sample.Difference alloc;
		}

		public double total_time_ms => total_time * 0.001f;

		[JsonIgnore] public bool IsValid => assembly_name != null;

		private string total_time_ms_str;

		public string GetTotalTime() => total_time_ms_str ??= total_time_ms < 10 ? $"{total_time:n0}μs" : $"{total_time_ms:n0}ms";
	}

	/// <summary>
	/// Rust-returned dataset structure of a Memory record, tracking allocations, total allocation size, instance size and class token identifier taken in the recording time period.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct MemoryRecord
	{
		[JsonIgnore] public ModuleHandle assembly_handle;
		[JsonIgnore] public IntPtr class_handle;

		public ulong allocations;
		public ulong total_alloc_size;
		public uint instance_size;
		public uint class_token;
		public AssemblyNameEntry assembly_name;
		public string class_name;
		public Comparison comparison;

		public struct Comparison
		{
			public bool isCompared;
			public Sample.Difference allocations;
			public Sample.Difference total_alloc_size;
		}

		[JsonIgnore] public bool IsValid => assembly_name != null;
	}

	/// <summary>
	/// Rust-returned dataset structure of a Call record, tracking total time, its percentage, own time (+ percentage), calls, total and own allocations, total and own exceptions and assembly + method information taken in the recording time period.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct CallRecord
	{
		[JsonIgnore] public ModuleHandle assembly_handle;
		[JsonIgnore] public MonoMethod* method_handle;

		public ulong total_time;
		public double total_time_percentage;
		public ulong own_time;
		public double own_time_percentage;
		public ulong calls;
		public ulong total_alloc;
		public ulong own_alloc;
		public ulong total_exceptions;
		public ulong own_exceptions;
		public AssemblyNameEntry assembly_name;
		public string method_name;
		public Comparison comparison;

		public struct Comparison
		{
			public bool isCompared;
			public Sample.Difference total_time;
			public Sample.Difference own_time;
			public Sample.Difference calls;
			public Sample.Difference total_alloc;
			public Sample.Difference own_alloc;
			public Sample.Difference total_exceptions;
			public Sample.Difference own_exceptions;
		}

		public double total_time_ms => total_time * 0.001f;
		public double own_time_ms => own_time * 0.001f;

		[JsonIgnore] public bool IsValid => assembly_name != null;

		private string total_time_ms_str;
		private string own_time_ms_str;

		public string GetTotalTime() => total_time_ms_str ??= total_time_ms < 10 ? $"{total_time:n0}μs" : $"{total_time_ms:n0}ms";
		public string GetOwnTime() => own_time_ms_str ??= own_time_ms < 10 ? $"{own_time:n0}μs" : $"{own_time_ms:n0}ms";
	}

	/// <summary>
	/// Don't touch it.
	/// </summary>
	[StructLayout(LayoutKind.Explicit)]
	public struct MonoImageUnion
	{
		[FieldOffset(0)]
		public ModuleHandle handle;
		[FieldOffset(0)]
		public MonoImage* ptr;
	}

	/// <summary>
	/// Don't touch it.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public readonly struct MonoImage
	{
		public readonly int ref_count;
		public readonly void* storage;

		/* Aliases storage->raw_data when storage is non-NULL. Otherwise NULL. */
		public readonly byte* raw_data;
		public readonly uint raw_data_len;

		public static ModuleHandle image_to_handle(MonoImage* image)
		{
			return new MonoImageUnion { ptr = image }.handle;
		}
		public static MonoImage* handle_to_image(ModuleHandle handle)
		{
			return new MonoImageUnion { handle = handle }.ptr;
		}
	}

	/// <summary>
	/// Don't touch it.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public readonly struct MonoMethod
	{
		public readonly ushort flags;
		public readonly ushort iflags;
		public readonly uint token;
		public readonly void* klass;
		public readonly void* signature;
		public readonly byte* name;
	}

	[Flags]
	public enum ProfilerArgs : ushort
	{
		None = 0,
		Abort = 1 << 0,
		CallMemory = 1 << 1,
		AdvancedMemory = 1 << 2,
		Timings = 1 << 3,
		Calls = 1 << 4,
		FastResume = 1 << 5, // Pass this when you're toggling the profiler multiple times on the same frame
		GCEvents = 1 << 6,
		StackWalkAllocations = 1 << 7
	}

	public static bool Enabled { get; }
	public static bool IsRecording { get; private set; }
	public static bool Crashed { get; }

	public static bool IsCleared => AssemblyRecords.Count == 0 && CallRecords.Count == 0;

	static MonoProfiler()
	{
		try
		{
			ulong np = carbon_get_protocol();
			if (np != NATIVE_PROTOCOL)
			{
				UnityEngine.Debug.LogError($"Native protocol mismatch (native) {np} != (managed) {NATIVE_PROTOCOL}");
				Enabled = false;
				Crashed = true;
				return;
			}

			ProfilerCallbacks callbacks = new();
			profiler_register_callbacks(&callbacks);
			Enabled = profiler_is_enabled();
			carbon_init_logger(&native_logger);
		}
		catch (Exception ex)
		{
			Crashed = true; // TODO: print an error when running commands if this is true
			UnityEngine.Debug.LogError($"NativeInitFailure {ex}");
		}
	}

	private static void native_logger(int level, int verbosity, byte* data, int length, LogSource source)
	{
		UnityEngine.Debug.Log($"[{source}] {Encoding.UTF8.GetString(data, length)}");
	}
	private static void native_string_cb(string* target, byte* ptr, int len)
	{
		*target = Encoding.UTF8.GetString(ptr, len);
	}

	private static void memcpy_array_cb<T>(T[]* target, T* src, ulong len)
	{
		T[] data = new T[len];
		*target = data;
		ulong bytes = len * (uint)sizeof(T);
		fixed (T* dst = data)
		{
			Buffer.MemoryCopy(src,dst, bytes, bytes);
		}
	}
	private static void native_iter<T>(List<T>* data, ulong length, IntPtr iter, delegate*<IntPtr, out T, bool> cb) where T: struct
	{
		if (*data == null)
		{
			*data = new((int)length);
		}
		else if (length > (ulong)data->Capacity)
		{
			data->Capacity = (int)length;
		}
		while (cb(iter, out T inst))
		{
			data->Add(inst);
		}
	}

	/// <summary>
	/// Clears currently collected and Rust-returned information.
	/// </summary>
	public static void Clear()
	{
		AssemblyRecords.Clear();
		CallRecords.Clear();
		MemoryRecords.Clear();
		DurationTime = default;
		GCStats = default;
	}
	public static void ToggleProfilingTimed(float duration, ProfilerArgs args = AllFlags, Action<ProfilerArgs> onTimerEnded = null, bool logging = true)
	{
		if (Crashed)
		{
			UnityEngine.Debug.LogError($"CarbonNative did not properly initialize. Please report to the developers.");
			return;
		}

		if (_profileTimer != null)
		{
			HarmonyProfiler.Runner.CancelInvoke(_profileTimer);
			_profileTimer = null;
		}
		if (_profileWarningTimer != null)
		{
			HarmonyProfiler.Runner.CancelInvoke(_profileWarningTimer);
			_profileWarningTimer = null;
		}

		if (!ToggleProfiling(args, logging).GetValueOrDefault())
		{
			if (logging)
			{
				PrintWarn();
			}
		}

		if (duration >= 1f && IsRecording)
		{
			if (logging)
			{
				UnityEngine.Debug.LogWarning($"[MonoProfiler] Profiling duration {((long)duration).FormatSeconds()}..");
			}

			HarmonyProfiler.Runner.Invoke(_profileTimer = () =>
			{
				if (!IsRecording)
				{
					return;
				}

				ToggleProfiling(args, logging).GetValueOrDefault();

				if (logging)
				{
					PrintWarn();
				}

				onTimerEnded?.Invoke(args);

				Clear();
			}, duration);
		}
		else if(IsRecording && logging)
		{
			HarmonyProfiler.Runner.Invoke(_profileWarningTimer = () =>
			{
				UnityEngine.Debug.LogWarning($" Reminder: You've been profiling for {CurrentDurationTime.TotalSeconds}s");
			}, 60 * 5);
		}

		return;

		static void PrintWarn()
		{
			var table = Pool.Get<TextTable>();
			table.Clear();
			table.AddColumns(" duration", "processing", "assemblies", "calls");

			table.AddRow(
				$" {DurationTime.TotalSeconds}s",
				$"{DataProcessingTime.TotalMilliseconds:0}ms",
				AssemblyRecords.Count.ToString(),
				CallRecords.Count.ToString());

			UnityEngine.Debug.LogWarning(table.ToString());
			Pool.FreeUnsafe(ref table);
		}
	}
	public static bool? ToggleProfiling(ProfilerArgs args = AllFlags, bool logging = true)
	{
		if (!Enabled)
		{
			UnityEngine.Debug.Log("Profiler disabled");
			return null;
		}

		bool state;
		AssemblyRecords.Clear();
		CallRecords.Clear();
		MemoryRecords.Clear();
		List<AssemblyRecord> assemblyOutput = AssemblyRecords;
		List<CallRecord> callOutput = CallRecords;
		List<MemoryRecord> memoryOutput = MemoryRecords;
		GCRecord gcOutput = default;

		if (IsRecording)
		{
			_dataProcessTimer = Pool.Get<Stopwatch>();
			_dataProcessTimer.Start();
		}

		ProfilerResultCode result = profiler_toggle(args, &state, &gcOutput, &assemblyOutput, &callOutput, &memoryOutput);

		if (result == ProfilerResultCode.Aborted)
		{
			// Handle abort;
			if (logging)
			{
				UnityEngine.Debug.LogWarning("[MonoProfiler] Profiler aborted");
			}
			IsRecording = false;
			return false;
		}

		if (!state)
		{
			DataProcessingTime = _dataProcessTimer?.Elapsed ?? TimeSpan.Zero;
			if (_dataProcessTimer != null)
			{
				Pool.FreeUnmanaged(ref _dataProcessTimer);
			}
		}

		if (result != ProfilerResultCode.OK)
		{
			UnityEngine.Debug.LogError($"[MonoProfiler] Failed to toggle profiler: {result}");
			return null;
		}

		if (assemblyOutput is { Count: > 0 })
		{
			MapAssemblyRecords(assemblyOutput);
		}
		if (callOutput is { Count: > 0 })
		{
			MapCallRecords(callOutput);
		}
		if (memoryOutput is { Count: > 0 })
		{
			MapMemoryRecords(memoryOutput);
		}

		GCStats = gcOutput;

		CallRecords.Disabled = callOutput.Count == 0;

		IsRecording = state;

		if (state)
		{
			if (logging)
			{
				UnityEngine.Debug.LogWarning($"[MonoProfiler] Started recording..");
			}

			_durationTimer = Pool.Get<Stopwatch>();
			_durationTimer.Start();
		}
		else
		{
			if (logging)
			{
				UnityEngine.Debug.LogWarning($"[MonoProfiler] Recording ended");
			}

			DurationTime = _durationTimer.Elapsed;
			Pool.FreeUnmanaged(ref _durationTimer);
		}

		return state;
	}

	private static void MapAssemblyRecords(List<AssemblyRecord> records)
	{
		for (int i = 0; i < records.Count; i++)
		{
			AssemblyRecord entry = records[i];

			if (AssemblyMap.TryGetValue(entry.assembly_handle, out AssemblyNameEntry asmName))
			{
				entry.assembly_name = asmName;
			}
			else
			{
				string name;
				get_image_name(&name, entry.assembly_handle);
				if (name == null) throw new NullReferenceException();
				asmName = new AssemblyNameEntry
				{
					name = name, displayName = name, displayNameNonIncrement = name, profileType = MonoProfilerConfig.ProfileTypes.Assembly
				};
				AssemblyMap[entry.assembly_handle] = asmName;
				entry.assembly_name = asmName;
			}

			records[i] = entry;
		}
	}
	private static void MapMemoryRecords(List<MemoryRecord> records)
	{
		for (int i = 0; i < records.Count; i++)
		{
			MemoryRecord entry = records[i];

			if (ClassMap.TryGetValue(entry.class_handle, out string className))
			{
				entry.class_name = className;
			}
			else
			{
				get_class_name(&className, entry.class_handle);
				if (className == null) throw new NullReferenceException();
				ClassMap[entry.class_handle] = className;
				entry.class_name = className;
			}

			if (AssemblyMap.TryGetValue(entry.assembly_handle, out AssemblyNameEntry asmName))
			{
				entry.assembly_name = asmName;
			}
			else
			{
				string name;
				get_image_name(&name, entry.assembly_handle);
				if (name == null) throw new NullReferenceException();
				asmName = new AssemblyNameEntry
				{
					name = name, displayName = name, displayNameNonIncrement = name, profileType = MonoProfilerConfig.ProfileTypes.Assembly
				};
				AssemblyMap[entry.assembly_handle] = asmName;
				entry.assembly_name = asmName;
			}

			records[i] = entry;
		}
	}
	private static void MapCallRecords(List<CallRecord> records)
	{
		var temp = Pool.Get<Dictionary<string, CallRecord>>();

		for (int i = 0; i < records.Count; i++)
		{
			CallRecord entry = records[i];

			if (MethodMap.TryGetValue((IntPtr)entry.method_handle, out string methName))
			{
				entry.method_name = methName;
			}
			else
			{
				get_method_name(&methName, entry.method_handle);
				MethodMap[(IntPtr)entry.method_handle] = methName ?? throw new NullReferenceException();
				entry.method_name = methName;
			}

			if (AssemblyMap.TryGetValue(entry.assembly_handle, out AssemblyNameEntry asmName))
			{
				entry.assembly_name = asmName;
			}
			else
			{
				string name;
				get_image_name(&name, entry.assembly_handle);
				if (name == null) throw new NullReferenceException();
				asmName = new AssemblyNameEntry
				{
					name = name, displayName = name, displayNameNonIncrement = name, profileType = MonoProfilerConfig.ProfileTypes.Assembly
				};
				AssemblyMap[entry.assembly_handle] = asmName;
				entry.assembly_name = asmName;
			}

			if (temp.TryGetValue(entry.method_name, out var existingRecord))
			{
				existingRecord.total_time += entry.total_time;
				existingRecord.total_time_percentage += entry.total_time_percentage;
				existingRecord.own_time += entry.own_time;
				existingRecord.own_time_percentage += entry.own_time_percentage;
				existingRecord.calls += entry.calls;
				existingRecord.total_alloc += entry.total_alloc;
				existingRecord.own_alloc += entry.own_alloc;
				existingRecord.total_exceptions += entry.total_exceptions;
				existingRecord.own_exceptions += entry.own_exceptions;
				temp[entry.method_name] = existingRecord;
			}
			else
			{
				temp[entry.method_name] = entry;
			}
		}

		records.Clear();
		records.AddRange(temp.Values);

		Pool.FreeUnmanaged(ref temp);
	}

	public static bool TryStartProfileFor(MonoProfilerConfig.ProfileTypes profileType, Assembly assembly, string value, bool incremental = false)
	{
		if (!MonoProfilerConfig.Instance.IsWhitelisted(profileType, value))
		{
			return false;
		}

		return ProfileAssembly(assembly, value, incremental, profileType);
	}
	public static bool ProfileAssembly(Assembly assembly, string assemblyName, bool incremental, MonoProfilerConfig.ProfileTypes profileType)
	{
		if (!Enabled)
		{
			return false;
		}

		var incrementedValue = assemblyName;

		if (incremental)
		{
			incrementedValue = AssemblyBank.Increment(assemblyName);
		}

		ModuleHandle handle = assembly.ManifestModule.ModuleHandle;

		AssemblyMap[handle] = new AssemblyNameEntry
		{
			name = assembly.GetName().Name,
			displayName = incrementedValue,
			displayNameNonIncrement = assemblyName,
			profileType = profileType
		};

		register_profiler_assembly(handle);

		return true;
	}

	#region PInvokes

	[StructLayout(LayoutKind.Sequential)]
	struct ProfilerCallbacks
	{
		private delegate*<string*, byte*, int, void> string_marshal;
		private delegate*<byte[]*, byte*, ulong, void> bytes_marshal;
		private delegate*<List<AssemblyRecord>*, ulong, IntPtr, delegate*<IntPtr, out AssemblyRecord, bool>, void> basic_iter;
		private delegate*<List<CallRecord>*, ulong, IntPtr, delegate*<IntPtr, out CallRecord, bool>, void> advanced_iter;
		private delegate*<List<MemoryRecord>*, ulong, IntPtr, delegate*<IntPtr, out MemoryRecord, bool>, void> memory_iter;

		public ProfilerCallbacks()
		{
			string_marshal = &native_string_cb;
			bytes_marshal = &memcpy_array_cb;
			basic_iter = &native_iter;
			advanced_iter = &native_iter;
			memory_iter = &native_iter;
		}
	}

	public enum LogSource : uint
	{
		Native,
		Profiler
	}

	[DllImport("CarbonNative")]
	private static extern void profiler_register_callbacks(ProfilerCallbacks* callbacks);

	[DllImport("CarbonNative")]
	private static extern void register_profiler_assembly(ModuleHandle handle);

	[DllImport("CarbonNative")]
	private static extern bool profiler_is_enabled();

	[DllImport("CarbonNative")]
	private static extern void carbon_init_logger(delegate*<int, int, byte*, int, LogSource, void> logger);

	[DllImport("CarbonNative")]
	private static extern ulong carbon_get_protocol();

	[DllImport("CarbonNative")]
	private static extern void get_image_name(string* str, ModuleHandle handle);

	[DllImport("CarbonNative")]
	private static extern void get_class_name(string* str, IntPtr handle);

	[DllImport("CarbonNative")]
	private static extern void get_method_name(string* str, MonoMethod* handle);

	[DllImport("CarbonNative")]
	private static extern ProfilerResultCode profiler_toggle(
		ProfilerArgs args,
		bool* state,
		GCRecord* gc_out,
		List<AssemblyRecord>* basic_out,
		List<CallRecord>* advanced_out,
		List<MemoryRecord>* mem_out
		);

	#endregion
}
