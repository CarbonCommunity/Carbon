namespace Carbon.Tests;

internal class AppSettings
{
	public required string WorkingDir { get; init; }
	public required string BranchName { get; init; }
	public required string CarbonDownloadZipUrl { get; init; }

	[System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
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
