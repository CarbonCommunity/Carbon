using System.Globalization;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Carbon.Tests.Services;

internal static class TestReport
{
	public const string TestResultMarker = "TEST_RESULT|";

	public const string HookInstallFailureMarker = "HOOK_INSTALL_FAILURE|";

	public const string ResultsEnvironmentVariable = "CARBON_TEST_RESULTS";

	private const int MaxMessageLength = 160;

	private record TestResult(string Suite, string Test, string Status, double DurationMs, string Message)
	{
		public bool Passed => Status == "Complete";
		public bool Skipped => Status == "Skipped";
		public bool Failed => !Passed && !Skipped;
	}

	private record HookFailure(string Kind, string Name, string Identifier, string Target, string Status);

	public static void Write(string output, int exitCode, ILogger logger)
	{
		var results = Parse(output, TestResultMarker, 5)
			.Select(x => new TestResult(x[0], x[1], x[2], double.TryParse(x[3], NumberStyles.Float, CultureInfo.InvariantCulture, out var ms) ? ms : 0, x[4]))
			.Where(x => x.Suite != "Cleanup")
			.ToList();
		var hooks = Parse(output, HookInstallFailureMarker, 5)
			.Select(x => new HookFailure(x[0], x[1], x[2], x[3], x[4]))
			.ToList();

		LogAndAnnotate(results, hooks, logger);

		var summaryPath = Environment.GetEnvironmentVariable("GITHUB_STEP_SUMMARY");

		if (!string.IsNullOrEmpty(summaryPath))
		{
			File.AppendAllText(summaryPath, BuildSummary(results, hooks, exitCode));
		}
	}

	private static List<string[]> Parse(string output, string marker, int fieldCount)
	{
		return output.Split('\n')
			.Select(line => (line: line.TrimEnd('\r'), index: line.IndexOf(marker, StringComparison.Ordinal)))
			.Where(x => x.index != -1)
			.Select(x => x.line[(x.index + marker.Length)..].Split('|', fieldCount))
			.Where(parts => parts.Length == fieldCount)
			.ToList();
	}

	private static void LogAndAnnotate(List<TestResult> results, List<HookFailure> hooks, ILogger logger)
	{
		var isGitHubActions = Environment.GetEnvironmentVariable("GITHUB_ACTIONS") == "true";

		foreach (var test in results.Where(x => x.Failed))
		{
			if (isGitHubActions)
			{
				Console.WriteLine($"::error title={EscapeProperty($"Test {test.Status.ToLower()} - {test.Suite}.{test.Test}")}::{EscapeData(test.Message)}");
			}
			else
			{
				logger.LogError("Test {Status}: {Suite}.{Test} - {Message}", test.Status, test.Suite, test.Test, test.Message);
			}
		}

		if (hooks.Count > 0)
		{
			logger.LogWarning("{Count} hook(s) failed to install (non-fatal)", hooks.Count);
		}

		foreach (var hook in hooks)
		{
			if (isGitHubActions)
			{
				Console.WriteLine($"::error title={EscapeProperty($"Hook failed to install ({hook.Kind})")}::{EscapeData($"{hook.Name} [{hook.Identifier}] on {hook.Target} - {hook.Status}")}");
			}
			else
			{
				logger.LogWarning("Hook failed to install ({Kind}): {Name} [{Id}] on {Target} - {Status}", hook.Kind, hook.Name, hook.Identifier, hook.Target, hook.Status);
			}
		}
	}

	private static string BuildSummary(List<TestResult> results, List<HookFailure> hooks, int exitCode)
	{
		var summary = new StringBuilder();

		var passed = results.Count(x => x.Passed);
		var failed = results.Count(x => x.Failed);
		var skipped = results.Count(x => x.Skipped);
		var duration = FormatDuration(results.Sum(x => x.DurationMs));

		if (results.Count == 0)
		{
			summary.AppendLine("### 🧪 No test results were reported");
		}
		else if (failed == 0 && skipped == 0)
		{
			summary.AppendLine($"### 🧪 ✅ All {results.Count} tests passed · {duration}");
		}
		else
		{
			var counts = new List<string> { $"✅ {passed} passed" };

			if (failed > 0)
			{
				counts.Add($"❌ {failed} failed");
			}

			if (skipped > 0)
			{
				counts.Add($"⏭️ {skipped} skipped");
			}

			summary.AppendLine($"### 🧪 {string.Join(" · ", counts)} · {results.Count} tests · {duration}");
		}

		if (exitCode != 0)
		{
			summary.AppendLine().AppendLine($"**Server exited with code `{exitCode}`.**");
		}

		if (results.Count > 0)
		{
			summary.AppendLine()
				.AppendLine("| | Suite | Passed | Failed | Skipped | Time |")
				.AppendLine("| :-: | --- | --: | --: | --: | --: |");

			foreach (var suite in results.GroupBy(x => x.Suite))
			{
				var suiteFailed = suite.Count(x => x.Failed);
				var suiteSkipped = suite.Count(x => x.Skipped);
				var icon = suiteFailed > 0 ? "❌" : suiteSkipped > 0 ? "⏭️" : "✅";

				summary.AppendLine($"| {icon} | {suite.Key} | {suite.Count(x => x.Passed)}/{suite.Count()} | {Count(suiteFailed)} | {Count(suiteSkipped)} | {FormatDuration(suite.Sum(x => x.DurationMs))} |");
			}
		}

		if (failed > 0)
		{
			summary.AppendLine()
				.AppendLine("#### ❌ Failures")
				.AppendLine()
				.AppendLine("| Test | Status | Time | Details |")
				.AppendLine("| --- | --- | --: | --- |");

			foreach (var test in results.Where(x => x.Failed))
			{
				summary.AppendLine($"| `{test.Suite}.{test.Test}` | {StatusIcon(test.Status)} {test.Status} | {FormatDuration(test.DurationMs)} | {Cell(test.Message)} |");
			}
		}

		if (hooks.Count > 0)
		{
			summary.AppendLine()
				.AppendLine($"#### ⚠️ {hooks.Count} {(hooks.Count == 1 ? "hook" : "hooks")} failed to install")
				.AppendLine()
				.AppendLine("_Reported only, doesn't fail the run._")
				.AppendLine()
				.AppendLine("| Kind | Hook | Target | Status | Identifier |")
				.AppendLine("| --- | --- | --- | --- | --- |");

			foreach (var hook in hooks)
			{
				summary.AppendLine($"| {hook.Kind} | {Cell(hook.Name)} | `{Cell(hook.Target)}` | {hook.Status} | `{hook.Identifier}` |");
			}
		}

		if (results.Count > 0)
		{
			summary.AppendLine()
				.AppendLine($"<details><summary>All {results.Count} tests</summary>")
				.AppendLine()
				.AppendLine("| | Suite | Test | Time |")
				.AppendLine("| :-: | --- | --- | --: |");

			foreach (var test in results)
			{
				summary.AppendLine($"| {StatusIcon(test.Status)} | {test.Suite} | `{test.Test}` | {(test.Skipped ? "–" : FormatDuration(test.DurationMs))} |");
			}

			summary.AppendLine().AppendLine("</details>");
		}

		return summary.AppendLine().ToString();
	}

	private static string StatusIcon(string status)
	{
		return status switch
		{
			"Complete" => "✅",
			"Skipped" => "⏭️",
			"Timeout" => "⏱️",
			_ => "❌"
		};
	}

	private static string Count(int value)
	{
		return value == 0 ? "–" : value.ToString();
	}

	private static string FormatDuration(double ms)
	{
		return ms switch
		{
			< 0.9995 => $"{ms * 1000:0}µs",
			< 999.5 => $"{ms:0}ms",
			< 60_000 => $"{ms / 1000:0.0}s",
			_ => $"{(int)(ms / 60_000)}m {ms / 1000 % 60:0}s"
		};
	}

	private static string Cell(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			return "–";
		}

		value = value.Trim();

		if (value.Length > MaxMessageLength)
		{
			value = value[..MaxMessageLength] + "…";
		}

		return value.Replace("|", "\\|");
	}

	private static string EscapeData(string value)
	{
		return (value ?? string.Empty).Replace("%", "%25").Replace("\r", "%0D").Replace("\n", "%0A");
	}

	private static string EscapeProperty(string value)
	{
		return EscapeData(value).Replace(":", "%3A").Replace(",", "%2C");
	}
}
