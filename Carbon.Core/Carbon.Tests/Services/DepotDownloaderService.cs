using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace Carbon.Tests.Services;

internal class DepotDownloaderService
{
	private static readonly string DepotDownloaderExecutable =
		$"DepotDownloader{(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ".exe" : "")}";

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

	public async Task RunDownloadAsync(string workingDirectory, DepotDownloadOptions options)
	{
		var filesToDownloadFile = Path.Combine(workingDirectory, "filelist.txt");

		const string regex =
			"""regex:^(?!.*/StreamingAssets/)(?!.*items/.*\.json$).*$""";

		await File.WriteAllTextAsync(filesToDownloadFile, regex);

		_logger.LogInformation("Starting download for appId: {AppId}, branch: {Branch}", options.AppId, options.Branch);

		var processStartInfo = new ProcessStartInfo
		{
			FileName = Path.Combine(workingDirectory, DepotDownloaderExecutable),
			Arguments =
				$"-app {options.AppId} -branch \"{options.Branch}\" -max-downloads 14 -filelist \"{filesToDownloadFile}\" -dir \"{options.OutputDir}\"",
			RedirectStandardOutput = true,
			RedirectStandardError = true,
			UseShellExecute = false,
			CreateNoWindow = true,
			WorkingDirectory = workingDirectory,
		};

		ProcessResult result;
		using (new StopWatchGroupLog("DepotDownloader RunDownload - Downloading Server"))
			result = await _processRunner.RunAsync("DepotDownloader", processStartInfo, 600_000);

		if (result.ExitCode != 0)
		{
			throw new Exception($"DepotDownloader exited with code: {result.ExitCode}");
		}
	}

	public async Task EnsureDepotDownloaderExistsAsync(string workingDirectory)
	{
		var executablePath = Path.Combine(workingDirectory, DepotDownloaderExecutable);

		if (File.Exists(executablePath))
		{
			_logger.LogInformation("DepotDownloader executable already exists");
			return;
		}

		_logger.LogInformation("DepotDownloader not found, downloading from {Link} ...", DepotDownloaderExecutableLink);

		using (new StopWatchGroupLog("DepotDownloader EnsureDepotDownloaderExists - Downloading it"))
		{
			await using var memoryStream = new MemoryStream();
			var response = await _httpClient.GetAsync(DepotDownloaderExecutableLink);
			response.EnsureSuccessStatusCode();
			await response.Content.CopyToAsync(memoryStream);

			memoryStream.Position = 0;

			await using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read);
			var entry = archive.Entries.FirstOrDefault(e => e.Name.Equals(DepotDownloaderExecutable, StringComparison.OrdinalIgnoreCase)) ??
			            throw new FileNotFoundException($"Could not find '{DepotDownloaderExecutable}' in the downloaded archive");

			await entry.ExtractToFileAsync(executablePath);

			Utils.MakeExecutableExecutable(executablePath);

			_logger.LogInformation("DepotDownloader download and extraction complete");
		}
	}
}

public record DepotDownloadOptions(int AppId, string Branch, string OutputDir);
