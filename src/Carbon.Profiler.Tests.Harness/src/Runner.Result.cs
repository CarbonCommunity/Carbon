using System;
using System.IO;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Carbon.Profiler.Tests.Harness;

internal sealed partial class Runner
{
	private readonly string _resultPath = Path.Combine(RunnerPaths.ModPath, "profiler-test-result.json");

	private void Pass()
	{
		WriteResult(true, null);
		RunnerLog.Info("PASS");
		Quit(0);
	}

	private void Fail(Exception ex)
	{
		Fail(ex.ToString());
	}

	private void Fail(string message)
	{
		WriteResult(false, message);
		RunnerLog.Error("FAIL: " + message);
		Quit(1);
	}

	private void WriteResult(bool success, string error)
	{
		File.WriteAllText(_resultPath, new JObject
		{
			["success"] = success,
			["error"] = error,
			["utc"] = DateTime.UtcNow.ToString("O"),
		}.ToString());
	}

	private static void Quit(int exitCode)
	{
		Application.Quit(exitCode);

#if WIN
		ExitProcess((uint)exitCode);
#else
		exit(exitCode);
#endif
	}

#if WIN
	[DllImport("kernel32.dll")]
	private static extern void ExitProcess(uint uExitCode);
#else
	[DllImport("libc")]
	private static extern void exit(int status);
#endif
}
