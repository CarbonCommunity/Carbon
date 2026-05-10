using System.Formats.Tar;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Carbon.Tests.Services;

internal class EnvironmentSetupService
{
	private readonly AppSettings _appSettings;
	private readonly ForDebugSettings _forDebugSettings;
	private readonly TestOptOutSettings _testOptOutSettings;
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
		IOptions<TestOptOutSettings> testOptOutOptions,
		DepotDownloaderService depotDownloaderService,
		ILogger<EnvironmentSetupService> logger,
		HttpClient httpClient
	)
	{
		_appSettings = appSettings.Value;
		_forDebugSettings = forDebugOptions.Value;
		_testOptOutSettings = testOptOutOptions.Value;
		_depotDownloaderService = depotDownloaderService;
		_logger = logger;
		_workingDirectory = PrepareWorkingDirectory();
		_httpClient = httpClient;
	}

	public async ValueTask<ServerPaths> PrepareEnvironmentAsync(ServerSettings settings)
	{
		const string rustIdentity = "thetester";
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
		await ApplyTestCompilerSymbolsAsync(rustDir);
		await CleanupServerState(rustDir, rustIdentity);
		await PrepareRustConfigFilesAsync(rustDir, rustIdentity);

		var rustExePath = Path.Combine(rustDir, RustDedicatedExecutable);
		_logger.LogInformation("Using Rust executable: {RustExecutable}", rustExePath);

		return new ServerPaths(rustDir, rustExePath);
	}

	private ValueTask CleanupServerState(string rustDir, string serverIdentity)
	{
		string[] paths =
		[
			Path.Combine(rustDir, "server", serverIdentity),
			Path.Combine(rustDir, "carbon", "data"),
		];

		foreach (var path in paths)
		{
			if (!Path.Exists(path))
			{
				continue;
			}

			_logger.LogInformation("Cleaning up old files in {Folder}", path);

			var dirInfo = new DirectoryInfo(path);
			foreach (var fileInfo in dirInfo.GetFiles())
			{
				_logger.LogInformation("Deleting {FileName}", fileInfo.Name);
				fileInfo.Delete();
			}
		}

		return ValueTask.CompletedTask;
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

	private async ValueTask ApplyTestCompilerSymbolsAsync(string rustDir)
	{
		var symbols = new List<string>();
		_testOptOutSettings.GetCompilerSymbols(symbols);

		if (symbols.Count == 0)
		{
			return;
		}

		var configPath = Path.Combine(rustDir, "carbon", "config.json");

		if (!File.Exists(configPath))
		{
			throw new FileNotFoundException($"Didn't find Carbon config file {configPath}");
		}

		var config = JsonNode.Parse(await File.ReadAllTextAsync(configPath))?.AsObject()
		             ?? throw new InvalidOperationException($"Invalid Carbon config JSON at {configPath}");

		var compiler = config["Compiler"]?.AsObject();
		if (compiler == null)
		{
			compiler = new JsonObject();
			config["Compiler"] = compiler;
		}

		var conditionalSymbols = compiler["ConditionalCompilationSymbols"]?.AsArray();
		if (conditionalSymbols == null)
		{
			conditionalSymbols = [];
			compiler["ConditionalCompilationSymbols"] = conditionalSymbols;
		}

		var existingSymbols = conditionalSymbols
			.Select(symbol => symbol?.GetValue<string>())
			.Where(symbol => !string.IsNullOrWhiteSpace(symbol))
			.ToHashSet(StringComparer.OrdinalIgnoreCase);

		foreach (var symbol in symbols)
		{
			if (existingSymbols.Add(symbol))
			{
				conditionalSymbols.Add(symbol);
			}
		}

		await File.WriteAllTextAsync(configPath, config.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
		_logger.LogInformation("Applied test compiler opt-out symbols: {Symbols}", string.Join(", ", symbols));
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
