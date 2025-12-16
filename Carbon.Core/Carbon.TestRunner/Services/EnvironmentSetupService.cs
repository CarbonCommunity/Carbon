using System.Formats.Tar;
using System.IO.Compression;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Carbon.TestRunner.Services;

internal class EnvironmentSetupService
{
	private readonly AppSettings _appSettings;
	private readonly ForDebugSettings _forDebugSettings;
	private readonly DepotDownloaderService _depotDownloaderService;
	private readonly ILogger<EnvironmentSetupService> _logger;
	private readonly string _workingDirectory;
	private readonly HttpClient _httpClient;

	private const string RustInstanceFolder = "RustInstance";

	private static readonly string RustDedicatedExecutable =
		$"RustDedicated{(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ".exe" : "")}";

	public EnvironmentSetupService(
		IOptions<AppSettings> appSettings,
		IOptions<ForDebugSettings> forDebugOptions,
		DepotDownloaderService depotDownloaderService,
		ILogger<EnvironmentSetupService> logger,
		HttpClient httpClient
	)
	{
		_appSettings = appSettings.Value;
		_forDebugSettings = forDebugOptions.Value;
		_depotDownloaderService = depotDownloaderService;
		_logger = logger;
		_workingDirectory = PrepareWorkingDirectory();
		_httpClient = httpClient;
	}

	public async Task<ServerPaths> PrepareEnvironmentAsync(ServerSettings settings)
	{
		_logger.LogInformation("Preparing environment in working directory: {WorkingDirectory}", _workingDirectory);

		var rustDir = await PrepareServerAsync(settings.AppId, settings.Branch);
		_logger.LogInformation("Using Rust server directory: {RustDirectory}", rustDir);

		await PrepareCarbonAsync(rustDir, _appSettings.CarbonDownloadZipUrl);
		await CopyPluginsAsync(rustDir);
		await PrepareRustConfigFilesAsync(rustDir, "thetester");

		var rustExePath = Path.Combine(rustDir, RustDedicatedExecutable);
		_logger.LogInformation("Using Rust executable: {RustExecutable}", rustExePath);

		return new ServerPaths(rustDir, rustExePath);
	}

	private async Task<string> PrepareServerAsync(int appId, string branch)
	{
		var rustInstancePath = Path.Combine(_workingDirectory, RustInstanceFolder);
		var rustDedicatedExe = Path.Combine(rustInstancePath, RustDedicatedExecutable);

		if (File.Exists(rustDedicatedExe) && _forDebugSettings.SkipRustServerIfPresent)
		{
			_logger.LogInformation("Found RustDedicated executable and skipping download based on configuration.");
			return rustInstancePath;
		}

		await _depotDownloaderService.EnsureDepotDownloaderExistsAsync(_workingDirectory);
		var downloadOptions = new DepotDownloadOptions(appId, branch, rustInstancePath);
		await _depotDownloaderService.RunDownloadAsync(_workingDirectory, downloadOptions);

		Utils.MakeExecutableExecutable(rustDedicatedExe);

		return rustInstancePath;
	}

	private async Task PrepareCarbonAsync(string rustDir, string carbonUrl)
	{
		_logger.LogInformation("Downloading Carbon from {CarbonUrl}", carbonUrl);

		await using var memoryStream = new MemoryStream();
		var response = await _httpClient.GetAsync(carbonUrl);
		response.EnsureSuccessStatusCode();
		await response.Content.CopyToAsync(memoryStream);

		memoryStream.Position = 0;

		Directory.CreateDirectory(rustDir);

		_logger.LogInformation("Unpacking archive into {RustDir}", rustDir);

		if (carbonUrl.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
		{
			await using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read);
			await archive.ExtractToDirectoryAsync(rustDir, true);
			_logger.LogInformation("Carbon .zip extracted to {Path}", rustDir);
		}
		else if (carbonUrl.EndsWith(".tar.gz", StringComparison.OrdinalIgnoreCase))
		{
			await using var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress);
			await TarFile.ExtractToDirectoryAsync(gzipStream, rustDir, true);
			_logger.LogInformation("Carbon .tar.gz extracted to {Path}", rustDir);
		}
		else
		{
			throw new NotSupportedException($"Unsupported file format for URL: {carbonUrl}");
		}
	}

	private static Task CopyPluginsAsync(string rustDir)
	{
		var runningDir = AppContext.BaseDirectory;
		var copyPluginsDir = Path.Combine(runningDir, "Static", "plugins");

		if (!Directory.Exists(copyPluginsDir))
		{
			throw new FileNotFoundException($"Didn't find path {copyPluginsDir}");
		}

		var rustPluginsDir = Path.Combine(rustDir, "carbon", "plugins");
		Directory.CreateDirectory(rustPluginsDir);

		var copyPluginsDirInfo = new DirectoryInfo(copyPluginsDir);

		foreach (var fileInfo in copyPluginsDirInfo.GetFiles())
		{
			var target = Path.Combine(rustPluginsDir, fileInfo.Name);
			fileInfo.CopyTo(target, true);
		}

		return Task.CompletedTask;
	}

	private static Task PrepareRustConfigFilesAsync(string rustDir, string serverIdentity)
	{
		var runningDir = AppContext.BaseDirectory;
		var copyCfgFile = Path.Combine(runningDir, "Static", "server.cfg");

		if (!File.Exists(copyCfgFile))
		{
			throw new FileNotFoundException($"Didn't find rust server cfg file {copyCfgFile}");
		}

		var cfgDir = Path.Combine(rustDir, "server", serverIdentity, "cfg");

		Directory.CreateDirectory(cfgDir);

		var targetCfgFile = Path.Combine(cfgDir, "server.cfg");

		File.Copy(copyCfgFile, targetCfgFile, true);

		return Task.CompletedTask;
	}

	private string PrepareWorkingDirectory()
	{
		var rawWorkingDir = _appSettings.WorkingDir;

		_logger.LogInformation("Using raw working dir: {RawWorkingDir}", rawWorkingDir);

		var workingDir = Path.GetFullPath(rawWorkingDir);
		Directory.CreateDirectory(workingDir);

		return workingDir;
	}
}

public record ServerPaths(string RustDirectory, string RustExecutable);

public record ServerSettings(int AppId, string Branch);
