using System.Diagnostics;
using System.Text;

namespace Carbon.TestRunner.Services;

internal class ProcessRunner
{
	private readonly ProcessLifetimeManager _processLifetimeManager;

	public ProcessRunner(ProcessLifetimeManager processLifetimeManager)
	{
		_processLifetimeManager = processLifetimeManager;
	}

	public async Task<ProcessResult> RunAsync(string processName, ProcessStartInfo startInfo, int timeoutAfterMs = -1)
	{
		using var process = new Process();
		process.StartInfo = startInfo;
		var outputBuilder = new StringBuilder();
		var errorBuilder = new StringBuilder();

		using var cts = new CancellationTokenSource();
		if (timeoutAfterMs != -1)
		{
			cts.CancelAfter(timeoutAfterMs);
		}

		process.OutputDataReceived += (_, args) =>
		{
			if (args.Data is null)
			{
				return;
			}

			outputBuilder.AppendLine(args.Data);
			Console.WriteLine($"[{processName}] {args.Data}");
		};

		process.ErrorDataReceived += (_, args) =>
		{
			if (args.Data is null)
			{
				return;
			}

			errorBuilder.AppendLine(args.Data);
			Console.WriteLine($"[{processName} ERR] {args.Data}");
		};

		process.Start();
		_processLifetimeManager.RegisterProcess(process);

		try
		{
			process.BeginOutputReadLine();
			process.BeginErrorReadLine();

			await process.WaitForExitAsync(cts.Token);

			return new ProcessResult(process.ExitCode, outputBuilder.ToString());
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
		finally
		{
			_processLifetimeManager.UnregisterProcess(process);
		}
	}
}

public record ProcessResult(int ExitCode, string StandardOutput);
