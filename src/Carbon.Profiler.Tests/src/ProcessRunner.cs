using System.Diagnostics;
using System.Text;

namespace Carbon.Profiler.Tests;

internal sealed class ProcessRunner
{
	public async Task<ProcessResult> RunAsync(string processName, ProcessStartInfo startInfo, int timeoutAfterMs)
	{
		using var process = new Process();
		process.StartInfo = startInfo;
		var outputBuilder = new StringBuilder();
		var errorBuilder = new StringBuilder();
		var outputLock = new Lock();
		var outputClosed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
		var errorClosed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

		using var cts = new CancellationTokenSource(timeoutAfterMs);

		process.OutputDataReceived += (_, args) =>
		{
			if (args.Data == null)
			{
				outputClosed.TrySetResult();
				return;
			}

			lock (outputLock)
				outputBuilder.AppendLine(args.Data);

			Console.WriteLine($"[{processName}] {args.Data}");
		};

		process.ErrorDataReceived += (_, args) =>
		{
			if (args.Data == null)
			{
				errorClosed.TrySetResult();
				return;
			}

			lock (outputLock)
				errorBuilder.AppendLine(args.Data);

			Console.WriteLine($"[{processName} ERR] {args.Data}");
		};

		process.Start();
		process.BeginOutputReadLine();
		process.BeginErrorReadLine();

		try
		{
			await process.WaitForExitAsync(cts.Token);
			await Task.WhenAll(outputClosed.Task, errorClosed.Task);

			lock (outputLock)
				return new ProcessResult(process.ExitCode, outputBuilder.ToString(), errorBuilder.ToString());
		}
		catch (OperationCanceledException)
		{
			try
			{
				process.Kill(true);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[{processName}] Failed to force kill on timeout: {ex.Message}");
			}

			throw new TimeoutException($"The process '{processName}' exceeded the timeout of {timeoutAfterMs}ms.");
		}
	}
}

internal sealed record ProcessResult(int ExitCode, string StandardOutput, string StandardError);
