using System.Formats.Tar;
using System.IO.Compression;
using Microsoft.Extensions.Logging;

namespace Carbon.Profiler.Tests;

internal sealed class ProfilerInstaller
{
	private readonly HttpClient _httpClient;
	private readonly ILogger<ProfilerInstaller> _logger;

	public ProfilerInstaller(HttpClient httpClient, ILogger<ProfilerInstaller> logger)
	{
		_httpClient = httpClient;
		_logger = logger;
	}

	public async Task InstallAsync(string rustDir, AppSettings settings)
	{
		var harmonyModsDir = Path.Combine(rustDir, "HarmonyMods");
		if (Directory.Exists(harmonyModsDir))
		{
			Directory.Delete(harmonyModsDir, true);
		}

		Directory.CreateDirectory(harmonyModsDir);
		await InstallProfilerAsync(settings, harmonyModsDir);
		InstallHarness(settings, harmonyModsDir);
		MakeNativeFilesExecutable(harmonyModsDir);
	}

	private async Task InstallProfilerAsync(AppSettings settings, string harmonyModsDir)
	{
		if (!string.IsNullOrWhiteSpace(settings.ProfilerPath))
		{
			var profilerPath = Path.GetFullPath(settings.ProfilerPath);
			_logger.LogInformation("Installing profiler from path: {ProfilerPath}", profilerPath);

			if (Directory.Exists(profilerPath))
			{
				FileSystemUtils.CopyDirectory(profilerPath, harmonyModsDir);
			}
			else if (File.Exists(profilerPath))
			{
				await ExtractArchiveAsync(profilerPath, harmonyModsDir);
			}
			else
			{
				throw new FileNotFoundException("ProfilerPath does not exist", profilerPath);
			}

			return;
		}

		var downloadUrl = settings.ProfilerDownloadUrl!;
		_logger.LogInformation("Downloading profiler from: {ProfilerDownloadUrl}", downloadUrl);
		var archivePath = Path.Combine(Path.GetTempPath(), "Carbon.Profiler.Tests." + Path.GetFileName(new Uri(downloadUrl).AbsolutePath));
		await using (var stream = await _httpClient.GetStreamAsync(downloadUrl))
		await using (var file = File.Create(archivePath))
		{
			await stream.CopyToAsync(file);
		}

		await ExtractArchiveAsync(archivePath, harmonyModsDir);
	}

	private void InstallHarness(AppSettings settings, string harmonyModsDir)
	{
		var harnessPath = Path.GetFullPath(settings.HarnessPath);
		if (!File.Exists(harnessPath))
		{
			throw new FileNotFoundException("HarnessPath does not exist", harnessPath);
		}

		File.Copy(harnessPath, Path.Combine(harmonyModsDir, Path.GetFileName(harnessPath)), true);
		_logger.LogInformation("Installed harness: {HarnessPath}", harnessPath);
	}

	private static void MakeNativeFilesExecutable(string harmonyModsDir)
	{
		var nativeDir = Path.Combine(harmonyModsDir, "native");
		if (!Directory.Exists(nativeDir))
		{
			return;
		}

		foreach (var file in Directory.GetFiles(nativeDir))
		{
			FileSystemUtils.MakeExecutable(file);
		}
	}

	private static async Task ExtractArchiveAsync(string archivePath, string destination)
	{
		if (archivePath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
		{
			await ZipFile.ExtractToDirectoryAsync(archivePath, destination, true);
			return;
		}

		if (archivePath.EndsWith(".tar.gz", StringComparison.OrdinalIgnoreCase))
		{
			await using var file = File.OpenRead(archivePath);
			await using var gzip = new GZipStream(file, CompressionMode.Decompress);
			await TarFile.ExtractToDirectoryAsync(gzip, destination, true);
			return;
		}

		throw new NotSupportedException("Unsupported profiler archive: " + archivePath);
	}
}
