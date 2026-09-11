using System;
using System.Collections;
using UnityEngine;

namespace Carbon.Profiler.Tests.Harness;

internal sealed partial class Runner : MonoBehaviour
{
	private IEnumerator Start()
	{
		RunnerLog.Info("Harness started");

		var waitForCommands = WaitForCommands();
		while (true)
		{
			bool moveNext;
			try
			{
				moveNext = waitForCommands.MoveNext();
			}
			catch (Exception ex)
			{
				Fail(ex);
				yield break;
			}

			if (!moveNext)
			{
				break;
			}

			yield return waitForCommands.Current;
		}

		try
		{
			ValidateProfilerState();
			ValidateProfilerConfig();
			ClearProfiles();
		}
		catch (Exception ex)
		{
			Fail(ex);
			yield break;
		}

		RunnerLog.Info("Starting short profile");
		ConsoleSystem.Run(ConsoleSystem.Option.Server, "carbon.profile 1 -c -t");

		var deadline = Time.realtimeSinceStartup + 15f;
		while (IsRecording())
		{
			if (Time.realtimeSinceStartup > deadline)
			{
				Fail("Profiler did not stop recording within timeout");
				yield break;
			}

			yield return new WaitForSeconds(0.25f);
		}

		yield return new WaitForSeconds(1f);

		try
		{
			ValidateProfilerState();
			ConsoleSystem.Run(ConsoleSystem.Option.Server, "carbon.export_profile -j");
			ValidateExport();
			Pass();
		}
		catch (Exception ex)
		{
			Fail(ex);
		}
	}
}
