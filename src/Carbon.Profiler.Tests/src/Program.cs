using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Carbon.Profiler.Tests;

internal static class Program
{
	private static async Task<int> Main()
	{
		Log.Logger = new LoggerConfiguration()
			.MinimumLevel.Information()
			.Enrich.FromLogContext()
			.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
			.CreateLogger();

		try
		{
			using var loggerFactory = LoggerFactory.Create(builder => builder.AddSerilog(Log.Logger));
			var logger = loggerFactory.CreateLogger("Carbon.Profiler.Tests");
			var config = new ConfigurationBuilder().AddEnvironmentVariables().Build();
			var settings = config.Get<AppSettings>() ?? throw new InvalidOperationException("App settings configuration missing");

			if (string.IsNullOrWhiteSpace(settings.ProfilerPath) && string.IsNullOrWhiteSpace(settings.ProfilerDownloadUrl))
			{
				throw new InvalidOperationException("Set ProfilerPath or ProfilerDownloadUrl");
			}

			var workingDir = Path.GetFullPath(settings.WorkingDir);
			Directory.CreateDirectory(workingDir);
			logger.LogInformation("Using working directory: {WorkingDirectory}", workingDir);

			using var httpClient = new HttpClient();
			var processRunner = new ProcessRunner();
			var depotDownloader = new DepotDownloaderService(processRunner, httpClient, loggerFactory.CreateLogger<DepotDownloaderService>());
			var rustServer = new RustServer(processRunner, depotDownloader, loggerFactory.CreateLogger<RustServer>());
			var profilerInstaller = new ProfilerInstaller(httpClient, loggerFactory.CreateLogger<ProfilerInstaller>());
			var testRunner = new ProfilerCompatibilityTestRunner(rustServer, profilerInstaller, logger);

			return await testRunner.RunAsync(workingDir, settings);
		}
		finally
		{
			await Log.CloseAndFlushAsync();
		}
	}
}
