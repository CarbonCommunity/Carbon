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
			            $"+server.seed 1337 +server.worldsize 1000 -insercure " +
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

		return true;
	}
}
