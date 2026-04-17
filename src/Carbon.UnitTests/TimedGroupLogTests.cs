using System;
using System.IO;
using System.Text;
using Carbon.Tests;
using Xunit;

namespace Carbon.UnitTests;

public class TimedGroupLogTests
{
	/// <summary>
	/// Captures console output produced while executing <paramref name="action"/>.
	/// </summary>
	private static string CaptureConsoleOutput(Action action)
	{
		var original = Console.Out;
		using var sw = new StringWriter();
		Console.SetOut(sw);
		try
		{
			action();
		}
		finally
		{
			Console.SetOut(original);
		}

		return sw.ToString();
	}

	[Fact]
	public void Constructor_WritesGroupOpenToConsole()
	{
		var output = CaptureConsoleOutput(() =>
		{
			var log = new TimedGroupLog("MyGroup");
			log.Dispose();
		});

		Assert.Contains("::group::", output);
		Assert.Contains("MyGroup", output);
	}

	[Fact]
	public void Dispose_WritesEndGroupToConsole()
	{
		var output = CaptureConsoleOutput(() =>
		{
			var log = new TimedGroupLog("MyGroup");
			log.Dispose();
		});

		Assert.Contains("::endgroup::", output);
	}

	[Fact]
	public void Dispose_WritesCompletionMessageWithName()
	{
		const string groupName = "SomeOperation";
		var output = CaptureConsoleOutput(() =>
		{
			using var log = new TimedGroupLog(groupName);
		});

		Assert.Contains(groupName, output);
		// Completion line uses the check mark symbol or the name
		Assert.Contains("::endgroup::", output);
	}

	[Fact]
	public void UsingStatement_DisposeCalledAutomatically()
	{
		// If Dispose is called automatically, endgroup should appear
		var output = CaptureConsoleOutput(() =>
		{
			using (new TimedGroupLog("AutoDispose"))
			{
				// body – intentionally empty
			}
		});

		Assert.Contains("::endgroup::", output);
	}

	[Fact]
	public void Constructor_EmptyName_DoesNotThrow()
	{
		var ex = Record.Exception(() =>
		{
			var output = CaptureConsoleOutput(() =>
			{
				using var log = new TimedGroupLog(string.Empty);
			});
		});

		Assert.Null(ex);
	}

	[Fact]
	public void MultipleInstances_EachEmitOwnGroup()
	{
		var output = CaptureConsoleOutput(() =>
		{
			using (new TimedGroupLog("First")) { }
			using (new TimedGroupLog("Second")) { }
		});

		Assert.Contains("First", output);
		Assert.Contains("Second", output);
		// Two group/endgroup pairs
		Assert.Equal(2, CountOccurrences(output, "::group::"));
		Assert.Equal(2, CountOccurrences(output, "::endgroup::"));
	}

	private static int CountOccurrences(string text, string pattern)
	{
		var count = 0;
		var index = 0;
		while ((index = text.IndexOf(pattern, index, StringComparison.Ordinal)) != -1)
		{
			count++;
			index += pattern.Length;
		}

		return count;
	}
}
