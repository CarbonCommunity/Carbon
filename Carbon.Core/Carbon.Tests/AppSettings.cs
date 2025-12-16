namespace Carbon.Test;

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
	public bool NoRustServerRun { get; init; }

	public ForDebugSettings(bool? skipRustServerIfPresent, bool? noRustServerRun)
	{
		SkipRustServerIfPresent = skipRustServerIfPresent ?? throw new ArgumentException(null, nameof(skipRustServerIfPresent));
		NoRustServerRun = noRustServerRun ?? throw new ArgumentException(null, nameof(noRustServerRun));
	}
}
