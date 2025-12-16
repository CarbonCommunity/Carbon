using System.Diagnostics;
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
			            $"+server.level \"CraggyIsland\" +server.seed 1337 +server.worldsize 1000 -insercure " +
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

		string[] directVars = ["DOORSTOP_ENABLED", "DOORSTOP_TARGET_ASSEMBLY", "TERM"];
		foreach (var key in directVars)
		{
			var val = Environment.GetEnvironmentVariable(key);
			if (!string.IsNullOrEmpty(val))
			{
				startInfo.EnvironmentVariables[key] = val;
			}
		}

		var ldPreload = Environment.GetEnvironmentVariable("RUST_LD_PRELOAD");
		if (!string.IsNullOrEmpty(ldPreload))
		{
			startInfo.EnvironmentVariables["LD_PRELOAD"] = ldPreload;
		}

		var ldLibPath = Environment.GetEnvironmentVariable("RUST_LD_LIBRARY_PATH");
		if (!string.IsNullOrEmpty(ldLibPath))
		{
			startInfo.EnvironmentVariables["LD_LIBRARY_PATH"] = ldLibPath;
		}

		var result = await _processRunner.RunAsync("RustDedicated", startInfo, 600_000);
		var stdOut = result.StandardOutput;

		if (!stdOut.Contains($"{identifier} ENDED"))
		{
			_logger.LogError("Tester process did not output final signal `ENDED`");
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
