using System.Formats.Tar;
using System.IO.Compression;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Carbon.Tests.Services;

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

	public async ValueTask<ServerPaths> PrepareEnvironmentAsync(ServerSettings settings)
	{
		_logger.LogInformation("Preparing environment in working directory: {WorkingDirectory}", _workingDirectory);

		var rustDir = await PrepareServerAsync(settings.AppId, settings.Branch);
		_logger.LogInformation("Using Rust server directory: {RustDirectory}", rustDir);

		if (!_forDebugSettings.SkipCarbonIfPresent || !Directory.Exists(Path.Combine(rustDir, "carbon")))
		{
			await PrepareCarbonAsync(rustDir, _appSettings.CarbonDownloadZipUrl);
		}
		else
		{
			_logger.LogInformation("Skipped carbon installation due to debug setting");
		}

		await CopyCarbonWorkspaceAsync(rustDir);
		await PrepareRustConfigFilesAsync(rustDir, "thetester");

		var rustExePath = Path.Combine(rustDir, RustDedicatedExecutable);
		_logger.LogInformation("Using Rust executable: {RustExecutable}", rustExePath);

		return new ServerPaths(rustDir, rustExePath);
	}

	private async ValueTask<string> PrepareServerAsync(int appId, string branch)
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

	private async ValueTask PrepareCarbonAsync(string rustDir, string carbonUrl)
	{
		_logger.LogInformation("Preparing Carbon using: {CarbonUrl}", carbonUrl);

		using (new TimedGroupLog("EnvironmentSetupService PrepareCarbon - Downloading and Unpacking Carbon"))
		{
			Stream sourceStream;

			if (carbonUrl.StartsWith("file:", StringComparison.OrdinalIgnoreCase))
			{
				try
				{
					var uri = new Uri(carbonUrl);
					var localPath = uri.LocalPath;

					_logger.LogInformation("Reading from local file: {LocalPath}", localPath);

					sourceStream = new FileStream(localPath, FileMode.Open, FileAccess.Read, FileShare.Read);
				}
				catch (UriFormatException ex)
				{
					_logger.LogError(ex, "Invalid file URI format: {Url}", carbonUrl);
					throw;
				}
			}
			else
			{
				_logger.LogInformation("Downloading from: {Url}", carbonUrl);

				var response = await _httpClient.GetAsync(carbonUrl);
				response.EnsureSuccessStatusCode();

				var memoryStream = new MemoryStream();
				await response.Content.CopyToAsync(memoryStream);
				memoryStream.Position = 0;
				sourceStream = memoryStream;
			}

			try
			{
				Directory.CreateDirectory(rustDir);
				_logger.LogInformation("Unpacking archive into {RustDir}", rustDir);

				if (carbonUrl.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
				{
					await using var archive = new ZipArchive(sourceStream, ZipArchiveMode.Read);
					await archive.ExtractToDirectoryAsync(rustDir, true);
					_logger.LogInformation("Carbon .zip extracted");
				}
				else if (carbonUrl.EndsWith(".tar.gz", StringComparison.OrdinalIgnoreCase))
				{
					await using var gzipStream = new GZipStream(sourceStream, CompressionMode.Decompress);
					await TarFile.ExtractToDirectoryAsync(gzipStream, rustDir, true);
					_logger.LogInformation("Carbon .tar.gz extracted");
				}
				else
				{
					throw new NotSupportedException($"Unsupported file format: {carbonUrl}");
				}
			}
			finally
			{
				await sourceStream.DisposeAsync();
			}
		}
	}

	private ValueTask CopyCarbonWorkspaceAsync(string rustDir)
	{
		var runningDir = AppContext.BaseDirectory;
		var copyCarbonDir = Path.Combine(runningDir, "Static", "carbon");

		if (!Directory.Exists(copyCarbonDir))
		{
			throw new FileNotFoundException($"Didn't find path {copyCarbonDir}");
		}

		var targetCarbonDir = Path.Combine(rustDir, "carbon");
		Utils.Copy(copyCarbonDir, targetCarbonDir);
		return ValueTask.CompletedTask;
	}

	private static ValueTask PrepareRustConfigFilesAsync(string rustDir, string serverIdentity)
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

		return ValueTask.CompletedTask;
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
