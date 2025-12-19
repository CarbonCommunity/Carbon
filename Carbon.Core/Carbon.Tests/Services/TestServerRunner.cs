using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Carbon.Tests.Services;

internal class TestServerRunner
{
	private readonly ProcessRunner _processRunner;
	private readonly ForDebugSettings _forDebugSettings;
	private readonly ILogger<TestServerRunner> _logger;

	public TestServerRunner(ProcessRunner processRunner, IOptions<ForDebugSettings> forDebugOptions, ILogger<TestServerRunner> logger)
	{
		_processRunner = processRunner;
		_forDebugSettings = forDebugOptions.Value;
		_logger = logger;
	}

	public async Task<bool> RunTesterServerAsync(ServerPaths paths)
	{
		var startInfo = new ProcessStartInfo
		{
			FileName = paths.RustExecutable,
			Arguments = $"-nographics -batchmode -logs -silent-crashes " +
			            $"-server.hostname THETESTER -server.identity thetester " +
			            $"-app.port 1- " +
			            $"-aimanager.nav_disable 1 " +
			            $"-disable-server-occlusion -disable-server-occlusion-rocks -disableconsolelog -skipload -noconsole " +
			            $"+server.level \"CraggyIsland\" -insecure " +
			            $"-logfile -",
			RedirectStandardOutput = true,
			RedirectStandardError = true,
			UseShellExecute = false,
			CreateNoWindow = true,
			WorkingDirectory = paths.RustDirectory,
		};

		if (_forDebugSettings.NoRustServerRun)
		{
			_logger.LogInformation("Skipped rust server run based on configuration");
			return true;
		}

		if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
		{
			var rustDirAbsolute = Path.GetFullPath(paths.RustDirectory);

			var envKeys = new Dictionary<string, string>
			{
				{ "TERM", "xterm" },
				{ "DOORSTOP_ENABLED", "1" },
				{ "DOORSTOP_TARGET_ASSEMBLY", Path.Combine(rustDirAbsolute, "carbon/managed/Carbon.Preloader.dll") },
				{ "LD_PRELOAD", Path.Combine(rustDirAbsolute, "libdoorstop.so") },
				{ "LD_LIBRARY_PATH", $"{rustDirAbsolute}:{Path.Combine(rustDirAbsolute, "RustDedicated_Data/Plugins/x86_64")}" },
				// { "DOORSTOP_MONO_DEBUG_ENABLED", "1" },
				// { "DOORSTOP_MONO_DEBUG_ADDRESS", "127.0.0.1:5337" },
				// { "DOORSTOP_MONO_DEBUG_SUSPEND", "1" },
			};

			foreach (var (key, val) in envKeys)
			{
				startInfo.EnvironmentVariables[key] = val;
			}
		}

		_logger.LogInformation("Starting Rust server process");

		ProcessResult result;
		using (new StopWatchGroupLog("TestServerRunner RunTesterServer - Running Server Process"))
			result = await _processRunner.RunAsync("RustDedicated", startInfo, 600_000);

		if (result.ExitCode != 0)
		{
			_logger.LogError("Tester process did not exit with positive code - exit code {ResultExitCode}", result.ExitCode);
			return false;
		}

		_logger.LogInformation("No failed tests detected - exit code {ResultExitCode}", result.ExitCode);

		return true;
	}
}
