using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using API.Logger;
using Facepunch;
using UnityEngine;
using ILogger = API.Logger.ILogger;

namespace Carbon.Test;

public static partial class Integrations
{
	public const int DEFAULT_CHANNEL = 1;

	public static ILogger Logger;
	public static readonly Stopwatch Stopwatch = new();
	public static readonly Dictionary<int, Queue<TestBank>> Banks = [];
	public static Action OnFatalTestFailure;
	public static ExitCodes ExitCode;

	public enum ExitCodes
	{
		Ok = 0,
		FatalFailure = -1
	}

	private static bool _isRunning;

	public static bool IsRunning() => _isRunning;

	public static TestBank Get(string context, Type type, object target = null, int channel = DEFAULT_CHANNEL)
	{
		var bed = (TestBank)null;

		foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
		{
			var test = method.GetCustomAttribute<Test>();

			if (test == null || (channel != -1 && test.Channel != channel))
			{
				continue;
			}

			(bed ??= new TestBank(channel, context)).AddTest((target ??= Activator.CreateInstance(type)), type, method, test);
		}

		return bed;
	}

	/// <summary>
	/// Manual addition of a Test bank to be ran when executed
	/// </summary>
	/// <param name="bank"></param>
	public static void EnqueueBed(TestBank bank)
	{
		if (!Banks.TryGetValue(bank.Channel, out var queue))
		{
			Banks[bank.Channel] = queue = new Queue<TestBank>();
		}
		queue.Enqueue(bank);
	}

	/// <summary>
	/// Executes all available Test banks for a specific channel (or all channels when "channel" is -1).
	/// </summary>
	/// <param name="delay">Starting delay of the test run</param>
	/// <param name="channel">Test channel the run should run on</param>
	public static void Run(float delay, int channel)
	{
		ServerMgr.Instance.StartCoroutine(RunRoutine(delay, channel));
	}

	public static IEnumerator RunRoutine(float delay, int channel)
	{
		if (_isRunning)
		{
			yield break;
		}

		_isRunning = true;

		var banks = Pool.Get<List<TestBank>>();

		switch (channel)
		{
			case -1:
			{
				foreach (var queue in Banks.Values)
				{
					while (queue.Count != 0)
					{
						banks.Add(queue.Dequeue());
					}
				}
				break;
			}
			default:
			{
				if (Banks.TryGetValue(channel, out var queue))
				{
					while (queue.Count != 0)
					{
						banks.Add(queue.Dequeue());
					}
				}
				break;
			}
		}

		var anyTestsFailedFatally = false;
		for (int i = 0; i < banks.Count; i++)
		{
			var bank = banks[i];
			yield return RunBankRoutine(delay, bank);
			if (bank.AnyTestsFailedFatally())
			{
				anyTestsFailedFatally = true;
				break;
			}
		}

		Pool.FreeUnmanaged(ref banks);

		_isRunning = false;

		try
		{
			if (anyTestsFailedFatally)
			{
				ExitCode = ExitCodes.FatalFailure;
				OnFatalTestFailure?.Invoke();
			}
		}
		catch (Exception ex)
		{
			Logger.Console("Fatal test failure callback error", Severity.Error, ex);
		}
	}

	public static IEnumerator RunBankRoutine(float delay, TestBank bank)
	{
		var completed = 0;

		Logger.Console($"initialized testbed - context: {bank.Context}");

		for(int i = 0; i < bank.Count; i++)
		{
			Stopwatch.Restart();

			var test = bank[i];
			test.Run();

			while (test.IsRunning)
			{
				test.SetDuration(Stopwatch.Elapsed);
				test.RunCheck();
				yield return null;
			}

			if (test.HasFailedFatally())
			{
				Logger.Console($"cancelled due to fatal status - context: {bank.Context}", Severity.Error);
				break;
			}

			completed++;
			ExitCode = ExitCodes.Ok;

			if (delay > 0)
			{
				yield return CoroutineEx.waitForSecondsRealtime(delay);
			}
			else
			{
				yield return null;
			}
		}

		Logger.Console($"completed {completed:n0} out of {bank.Count:n0} {(bank.Count == 1 ? "test" : "tests")} - context: {bank.Context}");

		yield return null;
	}

	public static void Clear(int channel)
	{
		switch (channel)
		{
			case -1:
				Banks.Clear();
				break;
			default:
				Banks.Remove(channel);
				break;
		}
	}

	public class TestBank(int channel, string context) : List<Test>
	{
		public int Channel = channel;
		public string Context = context;

		public void AddTest(object target, Type type, MethodInfo method, Test test)
		{
			test.Setup(target, type, method);
			Add(test);
		}

		public bool AnyTestsFailedFatally()
		{
			for (int i = 0; i < Count; i++)
			{
				var test = this[i];
				if (test.HasFailedFatally())
				{
					return true;
				}
			}
			return false;
		}
	}

	public interface ITestable
	{
		void CollectTests(int channel);
	}
}
