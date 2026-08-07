using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace Carbon.Profiler.Tests;

internal sealed class RustServer
{
	private const int RustAppId = 258550;
	private const string InstanceFolderPrefix = "rust-instance";
	private const string ServerIdentity = "profilertest";

	private const string ServerConfig =
		"""
		bear.population "0"
		bike.motorbikemonumentpopulation "0"
		bike.pedalmonumentpopulation "0"
		bike.pedalroadsidepopulation "0"
		boar.population "0"
		chicken.population "0"
		halloween.murdererpopulation "0"
		halloween.scarecrowpopulation "0"
		spawn.population_cap_rate "0"
		spawn.respawn_populations "0"
		halloweendungeon.population "0"
		hotairballoon.population "0"
		metaldetectorsource.population "0"
		minicopter.population "0"
		modularcar.population "0"
		motorrowboat.population "0"
		polarbear.population "0"
		rhib.rhibpopulation "0"
		ridablehorse.population "0"
		crocodile.population "0"
		panther.population "0"
		tiger.population "0"
		scraptransporthelicopter.population "0"
		snakehazard.population "0"
		stag.population "0"
		traincar.population "0"
		wolf.population "0"
		xmasdungeon.xmaspopulation "0"
		zombie.population "0"
		server.autouploadmap "0"
		server.autouploadmapimages "0"
		server.tickrate "2"
		server.corpses "0"
		server.events "0"
		""";

	private static readonly string RustDedicatedExecutable = $"RustDedicated{(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ".exe" : string.Empty)}";

	private readonly ProcessRunner _processRunner;
	private readonly DepotDownloaderService _depotDownloader;
	private readonly ILogger<RustServer> _logger;

	public RustServer(ProcessRunner processRunner, DepotDownloaderService depotDownloader, ILogger<RustServer> logger)
	{
		_processRunner = processRunner;
		_depotDownloader = depotDownloader;
		_logger = logger;
	}

	public async Task<string> PrepareAsync(string workingDir, AppSettings settings)
	{
		var rustDir = GetInstancePath(workingDir, settings.BranchName);
		var rustDedicatedExe = Path.Combine(rustDir, RustDedicatedExecutable);

		if (File.Exists(rustDedicatedExe) && settings.SkipRustServerIfPresent)
		{
			_logger.LogInformation("Found RustDedicated executable and skipping download: {RustDir}", rustDir);
			return rustDir;
		}

		await _depotDownloader.EnsureDepotDownloaderExistsAsync(workingDir);
		await _depotDownloader.RunDownloadAsync(workingDir, new DepotDownloadOptions(RustAppId, settings.BranchName, rustDir));
		FileSystemUtils.MakeExecutable(rustDedicatedExe);

		return rustDir;
	}

	public void PrepareConfig(string rustDir)
	{
		var cfgDir = Path.Combine(rustDir, "server", ServerIdentity, "cfg");
		Directory.CreateDirectory(cfgDir);
		File.WriteAllText(Path.Combine(cfgDir, "server.cfg"), ServerConfig + Environment.NewLine);
		_logger.LogInformation("Prepared server config at {CfgDir}", cfgDir);
	}

	public async Task<ProcessResult> RunAsync(string rustDir, int timeoutMs)
	{
		var rustExePath = Path.Combine(rustDir, RustDedicatedExecutable);
		var startInfo = new ProcessStartInfo
		{
			FileName = rustExePath,
			Arguments = "-nographics -batchmode -logs -silent-crashes " +
			            "-server.hostname PROFILERTEST -server.identity " + ServerIdentity + " " +
			            "-app.port 1- " +
			            "-aimanager.nav_disable 1 " +
			            "-disable-server-occlusion -disable-server-occlusion-rocks -disableconsolelog -skipload -noconsole " +
			            "+server.level \"CraggyIsland\" -insecure " +
			            "-logfile -",
			RedirectStandardOutput = true,
			RedirectStandardError = true,
			UseShellExecute = false,
			CreateNoWindow = true,
			WorkingDirectory = rustDir,
		};

		if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
		{
			startInfo.EnvironmentVariables["TERM"] = "xterm";
			startInfo.EnvironmentVariables["LD_LIBRARY_PATH"] = rustDir + ":" + Path.Combine(rustDir, "RustDedicated_Data", "Plugins", "x86_64");
		}

		_logger.LogInformation("Starting Rust server: {RustExePath}", rustExePath);
		return await _processRunner.RunAsync("RustDedicated", startInfo, timeoutMs);
	}

	private static string GetInstancePath(string workingDir, string branchName)
	{
		var safeBranchName = string.Join('-', branchName.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));
		return Path.Combine(workingDir, $"{InstanceFolderPrefix}-{safeBranchName}");
	}
}
