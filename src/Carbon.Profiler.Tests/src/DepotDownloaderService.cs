using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace Carbon.Profiler.Tests;

internal sealed class DepotDownloaderService
{
	private static readonly string DepotDownloaderExecutable =
		$"DepotDownloader{(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ".exe" : string.Empty)}";

	private static readonly string DepotDownloaderExecutableLink =
		$"https://github.com/SteamRE/DepotDownloader/releases/latest/download/DepotDownloader-{(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "windows" : "linux")}-x64.zip";

	private readonly ProcessRunner _processRunner;
	private readonly HttpClient _httpClient;
	private readonly ILogger<DepotDownloaderService> _logger;

	public DepotDownloaderService(ProcessRunner processRunner, HttpClient httpClient, ILogger<DepotDownloaderService> logger)
	{
		_processRunner = processRunner;
		_httpClient = httpClient;
		_logger = logger;
	}

	public async Task EnsureDepotDownloaderExistsAsync(string workingDirectory)
	{
		var executablePath = Path.Combine(workingDirectory, DepotDownloaderExecutable);
		if (File.Exists(executablePath))
		{
			_logger.LogInformation("DepotDownloader executable already exists");
			return;
		}

		_logger.LogInformation("Downloading DepotDownloader from {Link}", DepotDownloaderExecutableLink);
		await using var memoryStream = new MemoryStream();
		var response = await _httpClient.GetAsync(DepotDownloaderExecutableLink);
		response.EnsureSuccessStatusCode();
		await response.Content.CopyToAsync(memoryStream);
		memoryStream.Position = 0;

		await using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read);
		var entry = archive.Entries.FirstOrDefault(e => e.Name.Equals(DepotDownloaderExecutable, StringComparison.OrdinalIgnoreCase)) ??
		            throw new FileNotFoundException($"Could not find '{DepotDownloaderExecutable}' in the downloaded archive");

		await entry.ExtractToFileAsync(executablePath, true);
		MakeExecutable(executablePath);
	}

	public async Task RunDownloadAsync(string workingDirectory, DepotDownloadOptions options)
	{
		var filesToDownloadFile = Path.Combine(workingDirectory, "filelist.txt");
		await File.WriteAllTextAsync(filesToDownloadFile, "regex:^(?!.*/StreamingAssets/)(?!.*items/.*\\.json$).*$");

		var startInfo = new ProcessStartInfo
		{
			FileName = Path.Combine(workingDirectory, DepotDownloaderExecutable),
			Arguments = $"-app {options.AppId} -branch \"{options.Branch}\" -max-downloads 14 -filelist \"{filesToDownloadFile}\" -dir \"{options.OutputDir}\"",
			RedirectStandardOutput = true,
			RedirectStandardError = true,
			UseShellExecute = false,
			CreateNoWindow = true,
			WorkingDirectory = workingDirectory,
		};

		_logger.LogInformation("Downloading Rust server app {AppId}, branch {Branch}", options.AppId, options.Branch);
		var result = await _processRunner.RunAsync("DepotDownloader", startInfo, 600_000);
		if (result.ExitCode != 0)
		{
			throw new InvalidOperationException($"DepotDownloader exited with code {result.ExitCode}");
		}
	}

	private static void MakeExecutable(string filePath)
	{
		FileSystemUtils.MakeExecutable(filePath);
	}
}

internal sealed record DepotDownloadOptions(int AppId, string Branch, string OutputDir);
