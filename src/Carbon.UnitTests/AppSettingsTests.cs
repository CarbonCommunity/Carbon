using System;
using Xunit;

namespace Carbon.UnitTests;

public class AppSettingsTests
{
	// ── AppSettings ───────────────────────────────────────────────────────────

	[Fact]
	public void AppSettings_ValidArguments_SetsProperties()
	{
		var settings = new Tests.AppSettings("C:\\work", "public", "https://example.com/carbon.zip");

		Assert.Equal("C:\\work", settings.WorkingDir);
		Assert.Equal("public", settings.BranchName);
		Assert.Equal("https://example.com/carbon.zip", settings.CarbonDownloadZipUrl);
	}

	[Fact]
	public void AppSettings_NullWorkingDir_ThrowsArgumentException()
	{
		Assert.Throws<ArgumentException>(() =>
			new Tests.AppSettings(null!, "public", "https://example.com/carbon.zip"));
	}

	[Fact]
	public void AppSettings_NullBranchName_ThrowsArgumentException()
	{
		Assert.Throws<ArgumentException>(() =>
			new Tests.AppSettings("C:\\work", null!, "https://example.com/carbon.zip"));
	}

	[Fact]
	public void AppSettings_NullCarbonDownloadZipUrl_ThrowsArgumentException()
	{
		Assert.Throws<ArgumentException>(() =>
			new Tests.AppSettings("C:\\work", "public", null!));
	}

	[Fact]
	public void AppSettings_EmptyStrings_DoesNotThrow()
	{
		// Empty strings are not null so should not throw
		var ex = Record.Exception(() => new Tests.AppSettings("", "", ""));
		Assert.Null(ex);
	}

	// ── ForDebugSettings ─────────────────────────────────────────────────────

	[Fact]
	public void ForDebugSettings_DefaultConstructor_AllFalse()
	{
		var settings = new Tests.ForDebugSettings();

		Assert.False(settings.SkipRustServerIfPresent);
		Assert.False(settings.SkipCarbonIfPresent);
		Assert.False(settings.NoRustServerRun);
	}

	[Fact]
	public void ForDebugSettings_AllTrue_SetsAllTrue()
	{
		var settings = new Tests.ForDebugSettings(
			skipRustServerIfPresent: true,
			skipCarbonIfPresent: true,
			noRustServerRun: true);

		Assert.True(settings.SkipRustServerIfPresent);
		Assert.True(settings.SkipCarbonIfPresent);
		Assert.True(settings.NoRustServerRun);
	}

	[Fact]
	public void ForDebugSettings_PartialTrue_SetsCorrectly()
	{
		var settings = new Tests.ForDebugSettings(skipRustServerIfPresent: true);

		Assert.True(settings.SkipRustServerIfPresent);
		Assert.False(settings.SkipCarbonIfPresent);
		Assert.False(settings.NoRustServerRun);
	}

	[Fact]
	public void ForDebugSettings_SectionName_IsForDebug()
	{
		Assert.Equal("ForDebug", Tests.ForDebugSettings.SectionName);
	}
}
