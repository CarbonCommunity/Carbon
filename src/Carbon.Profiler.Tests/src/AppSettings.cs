namespace Carbon.Profiler.Tests;

internal sealed class AppSettings
{
	public required string WorkingDir { get; init; }
	public string BranchName { get; init; } = "release";
	public string? ProfilerPath { get; init; }
	public string? ProfilerDownloadUrl { get; init; }
	public required string HarnessPath { get; init; }
	public bool SkipRustServerIfPresent { get; init; }
	public bool NoRustServerRun { get; init; }
	public int ServerTimeoutMs { get; init; } = 180_000;
}
