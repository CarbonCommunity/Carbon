using Carbon.Components;
using Carbon.Extensions;
using Carbon.Test;

namespace Carbon.Plugins;

public partial class Tests
{
	public class Profiler
	{
		public const MonoProfiler.ProfilerArgs Args = MonoProfiler.AllFlags;
		public MonoProfiler.Sample Sample1;
		public MonoProfiler.Sample Sample2;
		public byte[] Data;

		[Integrations.Test.Assert(CancelOnFail = true)]
		public void validate(Integrations.Test.Assert test)
		{
			test.IsTrue(MonoProfiler.Enabled, "MonoProfiler.Enabled");
			test.IsFalse(MonoProfiler.Crashed, "MonoProfiler.Crashed");
		}

		[Integrations.Test.Assert(Timeout = 10_000)]
		public async void profile_start_sample1(Integrations.Test.Assert test)
		{
			test.IsTrue(MonoProfiler.ToggleProfiling(Args, false).GetValueOrDefault(),
				"MonoProfiler.ToggleProfiling(Args, false)");
			test.Log("SAMPLE-1: Recording started");

			await AsyncEx.WaitForSeconds(4);
			test.Complete();
		}

		[Integrations.Test.Assert]
		public void profile_stop_sample1(Integrations.Test.Assert test)
		{
			test.IsFalse(MonoProfiler.ToggleProfiling(Args, false).GetValueOrDefault(),
				"MonoProfiler.ToggleProfiling(Args, false)");
			test.Log("SAMPLE-1: Recording stopped");
		}

		[Integrations.Test.Assert]
		public void profile_resampling_sample1(Integrations.Test.Assert test)
		{
			Sample1.Resample();
			test.Log("SAMPLE-1: Sample.Resample()");

			test.Log(
				$"SAMPLE-1: Assemblies = {Sample1.Assemblies.Count} | Calls = {Sample1.Calls.Count} | Memory = {Sample1.Memory.Count}");
		}

		[Integrations.Test.Assert(Timeout = 10_000)]
		public async void profile_start_sample2(Integrations.Test.Assert test)
		{
			test.IsTrue(MonoProfiler.ToggleProfiling(Args, false).GetValueOrDefault(),
				"MonoProfiler.ToggleProfiling(Args, false)");
			test.Log("SAMPLE-2: Recording started");

			await AsyncEx.WaitForSeconds(4);
			test.Complete();
		}

		[Integrations.Test.Assert]
		public void profile_stop_sample2(Integrations.Test.Assert test)
		{
			test.IsFalse(MonoProfiler.ToggleProfiling(Args, false).GetValueOrDefault(),
				"MonoProfiler.ToggleProfiling(Args, false)");
			test.Log("SAMPLE-2: Recording stopped");
		}

		[Integrations.Test.Assert]
		public void profile_resampling_sample2(Integrations.Test.Assert test)
		{
			Sample2.Resample();
			test.Log("SAMPLE-2: Sample.Resample()");

			test.Log(
				$"SAMPLE-2: Assemblies = {Sample2.Assemblies.Count} | Calls = {Sample2.Calls.Count} | Memory = {Sample2.Memory.Count}");
		}

		[Integrations.Test.Assert]
		public void profile_compare(Integrations.Test.Assert test)
		{
			var compare = Sample1.Compare(Sample2);

			test.Log(
				$"COMPARE: Assemblies = {compare.Assemblies.Count} | Calls = {compare.Calls.Count} | Memory = {compare.Memory.Count}");
		}

		[Integrations.Test.Assert]
		public void profile_save(Integrations.Test.Assert test)
		{
			Data = MonoProfiler.SerializeSample(Sample1);
			test.Log($"Sample saved file size: {ByteEx.Format(Data.Length).ToUpper()}");
		}

		[Integrations.Test.Assert]
		public void profile_load(Integrations.Test.Assert test)
		{
			var sample = MonoProfiler.DeserializeSample(Data);
			test.Log(
				$"Loaded sample: Assemblies = {sample.Assemblies.Count} | Calls = {sample.Calls.Count} | Memory = {sample.Memory.Count}");
		}
	}
}
