namespace Carbon.Tests;

internal class AppSettings
{
	public required string WorkingDir { get; init; }
	public required string BranchName { get; init; }
	public required string CarbonDownloadZipUrl { get; init; }

	public AppSettings(string workingDir, string branchName, string carbonDownloadZipUrl)
	{
		WorkingDir = workingDir ?? throw new ArgumentException(null, nameof(workingDir));
		BranchName = branchName ?? throw new ArgumentException(null, nameof(branchName));
		CarbonDownloadZipUrl = carbonDownloadZipUrl ?? throw new ArgumentException(null, nameof(carbonDownloadZipUrl));
	}
}

internal class ForDebugSettings
{
	public const string SectionName = "ForDebug";

	public bool SkipRustServerIfPresent { get; init; }
	public bool SkipCarbonIfPresent { get; init; }
	public bool NoRustServerRun { get; init; }

	public ForDebugSettings(bool skipRustServerIfPresent = false, bool skipCarbonIfPresent = false, bool noRustServerRun = false)
	{
		SkipRustServerIfPresent = skipRustServerIfPresent;
		SkipCarbonIfPresent = skipCarbonIfPresent;
		NoRustServerRun = noRustServerRun;
	}
}

internal class TestOptOutSettings
{
	public const string SectionName = "Tests";

	public bool NoHooks { get; init; }
	public bool NoLogging { get; init; }
	public bool NoPermission { get; init; }
	public bool NoPermissionSqlMigration { get; init; }
	public bool NoProfiler { get; init; }
	public bool NoWebRequest { get; init; }

	public void GetCompilerSymbols(List<string> outSymbols)
	{
		if (NoHooks)
		{
			outSymbols.Add("TESTS_NO_HOOKS");
		}

		if (NoLogging)
		{
			outSymbols.Add("TESTS_NO_LOGGING");
		}

		if (NoPermission)
		{
			outSymbols.Add("TESTS_NO_PERMISSION");
		}

		if (NoPermissionSqlMigration)
		{
			outSymbols.Add("TESTS_NO_PERMISSION_SQL_MIGRATION");
		}

		if (NoProfiler)
		{
			outSymbols.Add("TESTS_NO_PROFILER");
		}

		if (NoWebRequest)
		{
			outSymbols.Add("TESTS_NO_WEBREQUEST");
		}
	}
}
