using Carbon.Tests.Services;
using Xunit;

namespace Carbon.UnitTests;

/// <summary>
/// Tests for the public record types that act as value objects / DTOs.
/// </summary>
public class RecordTypeTests
{
	// ── ProcessResult ─────────────────────────────────────────────────────────

	[Fact]
	public void ProcessResult_Properties_AreSetCorrectly()
	{
		var result = new ProcessResult(0, "some output");

		Assert.Equal(0, result.ExitCode);
		Assert.Equal("some output", result.StandardOutput);
	}

	[Fact]
	public void ProcessResult_NonZeroExitCode_IsPreserved()
	{
		var result = new ProcessResult(-1, "error output");

		Assert.Equal(-1, result.ExitCode);
		Assert.Equal("error output", result.StandardOutput);
	}

	[Fact]
	public void ProcessResult_Equality_SameValues_AreEqual()
	{
		var a = new ProcessResult(0, "output");
		var b = new ProcessResult(0, "output");

		Assert.Equal(a, b);
	}

	[Fact]
	public void ProcessResult_Equality_DifferentExitCode_AreNotEqual()
	{
		var a = new ProcessResult(0, "output");
		var b = new ProcessResult(1, "output");

		Assert.NotEqual(a, b);
	}

	[Fact]
	public void ProcessResult_Equality_DifferentOutput_AreNotEqual()
	{
		var a = new ProcessResult(0, "output a");
		var b = new ProcessResult(0, "output b");

		Assert.NotEqual(a, b);
	}

	[Fact]
	public void ProcessResult_EmptyOutput_IsAllowed()
	{
		var result = new ProcessResult(0, string.Empty);

		Assert.Equal(string.Empty, result.StandardOutput);
	}

	// ── ServerPaths ───────────────────────────────────────────────────────────

	[Fact]
	public void ServerPaths_Properties_AreSetCorrectly()
	{
		var paths = new ServerPaths("/rust/dir", "/rust/dir/RustDedicated");

		Assert.Equal("/rust/dir", paths.RustDirectory);
		Assert.Equal("/rust/dir/RustDedicated", paths.RustExecutable);
	}

	[Fact]
	public void ServerPaths_Equality_SameValues_AreEqual()
	{
		var a = new ServerPaths("/rust/dir", "/rust/dir/RustDedicated");
		var b = new ServerPaths("/rust/dir", "/rust/dir/RustDedicated");

		Assert.Equal(a, b);
	}

	[Fact]
	public void ServerPaths_Equality_DifferentDirectory_AreNotEqual()
	{
		var a = new ServerPaths("/rust/dir1", "/rust/dir1/RustDedicated");
		var b = new ServerPaths("/rust/dir2", "/rust/dir2/RustDedicated");

		Assert.NotEqual(a, b);
	}

	// ── ServerSettings ────────────────────────────────────────────────────────

	[Fact]
	public void ServerSettings_Properties_AreSetCorrectly()
	{
		var settings = new ServerSettings(258550, "public");

		Assert.Equal(258550, settings.AppId);
		Assert.Equal("public", settings.Branch);
	}

	[Fact]
	public void ServerSettings_Equality_SameValues_AreEqual()
	{
		var a = new ServerSettings(258550, "public");
		var b = new ServerSettings(258550, "public");

		Assert.Equal(a, b);
	}

	[Fact]
	public void ServerSettings_Equality_DifferentBranch_AreNotEqual()
	{
		var a = new ServerSettings(258550, "public");
		var b = new ServerSettings(258550, "staging");

		Assert.NotEqual(a, b);
	}

	[Fact]
	public void ServerSettings_Equality_DifferentAppId_AreNotEqual()
	{
		var a = new ServerSettings(258550, "public");
		var b = new ServerSettings(1, "public");

		Assert.NotEqual(a, b);
	}

	// ── DepotDownloadOptions ──────────────────────────────────────────────────

	[Fact]
	public void DepotDownloadOptions_Properties_AreSetCorrectly()
	{
		var options = new DepotDownloadOptions(258550, "public", "/output/dir");

		Assert.Equal(258550, options.AppId);
		Assert.Equal("public", options.Branch);
		Assert.Equal("/output/dir", options.OutputDir);
	}

	[Fact]
	public void DepotDownloadOptions_Equality_SameValues_AreEqual()
	{
		var a = new DepotDownloadOptions(258550, "public", "/output/dir");
		var b = new DepotDownloadOptions(258550, "public", "/output/dir");

		Assert.Equal(a, b);
	}

	[Fact]
	public void DepotDownloadOptions_Equality_DifferentOutputDir_AreNotEqual()
	{
		var a = new DepotDownloadOptions(258550, "public", "/dir/a");
		var b = new DepotDownloadOptions(258550, "public", "/dir/b");

		Assert.NotEqual(a, b);
	}
}
