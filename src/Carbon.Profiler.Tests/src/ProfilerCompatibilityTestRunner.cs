using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Carbon.Profiler.Tests;

internal sealed class ProfilerCompatibilityTestRunner
{
	private static readonly string[] ForbiddenLogSnippets =
	[
		"Carbon.Profiler crashed",
		"Native protocol mismatch",
		"DllNotFoundException",
		"EntryPointNotFoundException",
	];

	private readonly RustServer _rustServer;
	private readonly ProfilerInstaller _profilerInstaller;
	private readonly ILogger _logger;

	public ProfilerCompatibilityTestRunner(RustServer rustServer, ProfilerInstaller profilerInstaller, ILogger logger)
	{
		_rustServer = rustServer;
		_profilerInstaller = profilerInstaller;
		_logger = logger;
	}

	public async Task<int> RunAsync(string workingDir, AppSettings settings)
	{
		string rustDir;
		using (new TimedGroupLog("Prepare Rust server"))
		{
			rustDir = await _rustServer.PrepareAsync(workingDir, settings);
		}

		using (new TimedGroupLog("Install profiler and harness"))
		{
			await _profilerInstaller.InstallAsync(rustDir, settings);
		}

		using (new TimedGroupLog("Prepare Rust config"))
		{
			_rustServer.PrepareConfig(rustDir);
		}

		if (settings.NoRustServerRun)
		{
			_logger.LogInformation("Skipping Rust server run due to NoRustServerRun");
			return 0;
		}

		ProcessResult result;
		using (new TimedGroupLog("Run Rust server compatibility harness"))
		{
			result = await _rustServer.RunAsync(rustDir, settings.ServerTimeoutMs);
		}

		await File.WriteAllTextAsync(Path.Combine(workingDir, $"rust-server-output-{settings.BranchName}.log"),
			result.StandardOutput + Environment.NewLine + result.StandardError);

		using (new TimedGroupLog("Validate compatibility result"))
		{
			return await ValidateResultAsync(rustDir, result);
		}
	}

	private async Task<int> ValidateResultAsync(string rustDir, ProcessResult result)
	{
		var output = result.StandardOutput + Environment.NewLine + result.StandardError;
		var resultPath = Path.Combine(rustDir, "HarmonyMods", "profiler-test-result.json");

		if (!File.Exists(resultPath))
		{
			_logger.LogError("Profiler harness did not write result file: {ResultPath}", resultPath);
			return -1;
		}

		var resultJson = await File.ReadAllTextAsync(resultPath);
		_logger.LogInformation("Harness result: {ResultJson}", resultJson);

		JsonDocument document;
		try
		{
			document = JsonDocument.Parse(resultJson);
		}
		catch (JsonException ex)
		{
			_logger.LogError(ex, "Harness result is not valid JSON");
			return -5;
		}

		using (document)
		{
			var root = document.RootElement;
			if (!root.TryGetProperty("success", out var successProperty) ||
			    (successProperty.ValueKind != JsonValueKind.True && successProperty.ValueKind != JsonValueKind.False))
			{
				_logger.LogError("Harness result does not contain a boolean success value");
				return -5;
			}

			if (!successProperty.GetBoolean())
			{
				var error = root.TryGetProperty("error", out var errorProperty) ? errorProperty.GetString() : null;
				_logger.LogError("Profiler harness failed: {Error}", error ?? "unknown error");
				return -6;
			}
		}

		if (result.ExitCode != 0)
		{
			_logger.LogError("Rust server exited with code {ExitCode}", result.ExitCode);
			return result.ExitCode;
		}

		foreach (var snippet in ForbiddenLogSnippets)
		{
			if (output.Contains(snippet, StringComparison.OrdinalIgnoreCase))
			{
				_logger.LogError("Detected forbidden log output: {Snippet}", snippet);
				return -2;
			}
		}

		if (!output.Contains("Carbon.Profiler initialized", StringComparison.OrdinalIgnoreCase))
		{
			_logger.LogError("Did not detect profiler initialization log");
			return -3;
		}

		_logger.LogInformation("Standalone profiler compatibility test passed");
		return 0;
	}
}
