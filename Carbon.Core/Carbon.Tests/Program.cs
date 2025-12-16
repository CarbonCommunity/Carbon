using Carbon.TestRunner.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Carbon.TestRunner;

internal abstract class Program
{
	private static async Task<int> Main(string[] _)
	{
		var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

		var config = new ConfigurationBuilder().AddEnvironmentVariables().Build();

		var appSettings = config.Get<AppSettings>()
		                  ?? throw new Exception("App settings configuration missing");
		var debugSettings = config.GetSection(ForDebugSettings.SectionName).Get<ForDebugSettings>()
		                    ?? throw new Exception("Debug options are missing");

		var httpClient = new HttpClient();
		var processLifetimeManager = new ProcessLifetimeManager(loggerFactory.CreateLogger<ProcessLifetimeManager>());
		var processRunner = new ProcessRunner(processLifetimeManager);

		var depotDownloader = new DepotDownloaderService(
			processRunner,
			httpClient,
			loggerFactory.CreateLogger<DepotDownloaderService>());

		var envSetup = new EnvironmentSetupService(
			new OptionsWrapper<AppSettings>(appSettings),
			new OptionsWrapper<ForDebugSettings>(debugSettings),
			depotDownloader,
			loggerFactory.CreateLogger<EnvironmentSetupService>(),
			httpClient);

		var testServerRunner = new TestServerRunner(
			processRunner,
			new OptionsWrapper<ForDebugSettings>(debugSettings),
			loggerFactory.CreateLogger<TestServerRunner>());

		var serverPaths = await envSetup.PrepareEnvironmentAsync(new ServerSettings(258550, appSettings.BranchName));
		var allTestsSucceeded = await testServerRunner.RunTesterServerAsync(serverPaths);

		if (!allTestsSucceeded)
		{
			return -666;
		}

		return 0;
	}
}
