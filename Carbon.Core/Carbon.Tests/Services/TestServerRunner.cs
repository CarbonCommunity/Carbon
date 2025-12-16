using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Carbon.TestRunner.Services;

internal class TestServerRunner
{
	private readonly ProcessRunner _processRunner;
	private readonly ForDebugSettings forDebugSettings;
	private readonly ILogger<TestServerRunner> _logger;

	public TestServerRunner(ProcessRunner processRunner, IOptions<ForDebugSettings> forDebugOptions, ILogger<TestServerRunner> logger)
	{
		_processRunner = processRunner;
		forDebugSettings = forDebugOptions.Value;
		_logger = logger;
	}

	public async Task<bool> RunTesterServerAsync(ServerPaths paths)
	{
		var identifier = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

		var startInfo = new ProcessStartInfo
		{
			FileName = paths.RustExecutable,
			Arguments = $"-nographics -batchmode -logs -silent-crashes " +
			            $"-server.hostname THETESTER -server.identity thetester " +
			            $"-app.port 1- " +
			            $"-aimanager.nav_disable 1 " +
			            $"-disable-server-occlusion -disable-server-occlusion-rocks -disableconsolelog -skipload -noconsole " +
			            $"+server.level \"CraggyIsland\" -insercure " +
			            $"-testrunner-identifier {identifier} " +
			            $"-logfile -",
			RedirectStandardOutput = true,
			RedirectStandardError = true,
			UseShellExecute = false,
			CreateNoWindow = true,
			WorkingDirectory = paths.RustDirectory,
		};

		if (forDebugSettings.NoRustServerRun)
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
			};

			foreach (var (key, val) in envKeys)
			{
				startInfo.EnvironmentVariables[key] = val;
			}
		}

		_logger.LogInformation("Starting Rust server process");

		var result = await _processRunner.RunAsync("RustDedicated", startInfo, 600_000);
		var stdOut = result.StandardOutput;

		if (result.ExitCode != 0)
		{
			_logger.LogError($"Tester process did not exit with positive code[{result.ExitCode}]");
			return false;
		}

		if (stdOut.Contains($"{identifier} cancelled due to fatal status"))
		{
			_logger.LogError("Tester process failed some tests");
			return false;
		}

		_logger.LogInformation("No failed tests detected.");

		return true;
	}
}
