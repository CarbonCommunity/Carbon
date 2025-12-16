using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Carbon.Test.Services;

internal class ProcessLifetimeManager : IDisposable
{
	private readonly ConcurrentDictionary<int, Process> _runningProcesses = new();
	private readonly ILogger<ProcessLifetimeManager> _logger;

	public ProcessLifetimeManager(ILogger<ProcessLifetimeManager> logger)
	{
		_logger = logger;
		AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
		Console.CancelKeyPress += OnCancelKeyPress;
	}

	public void RegisterProcess(Process process)
	{
		if (process.HasExited)
		{
			return;
		}

		_runningProcesses.TryAdd(process.Id, process);
	}

	public void UnregisterProcess(Process process)
	{
		if (_runningProcesses.TryRemove(process.Id, out _))
		{
		}
	}

	private void OnProcessExit(object? sender, EventArgs e)
	{
		_logger.LogInformation("ProcessExit event triggered. Cleaning up child processes...");
		CleanupAllProcesses();
	}

	private void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs e)
	{
		_logger.LogInformation("CancelKeyPress event triggered. Cleaning up child processes...");
		e.Cancel = true;
		CleanupAllProcesses();
	}

	private void CleanupAllProcesses()
	{
		foreach (var (_, process) in _runningProcesses)
		{
			CleanupProcess(process);
		}
	}

	private void CleanupProcess(Process process)
	{
		try
		{
			if (process.HasExited)
			{
				return;
			}

			_logger.LogInformation("Attempting to kill process tree with ID: {ProcessId} ({ProcessName})", process.Id, process.ProcessName);
			process.Kill(true);
			process.WaitForExit();
			_logger.LogInformation("Process {ProcessId} ({ProcessName}) successfully terminated.", process.Id, process.ProcessName);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while terminating process {ProcessId}.", process.Id);
		}
	}

	public void Dispose()
	{
		AppDomain.CurrentDomain.ProcessExit -= OnProcessExit;
		Console.CancelKeyPress -= OnCancelKeyPress;
		CleanupAllProcesses();
		GC.SuppressFinalize(this);
	}
}
