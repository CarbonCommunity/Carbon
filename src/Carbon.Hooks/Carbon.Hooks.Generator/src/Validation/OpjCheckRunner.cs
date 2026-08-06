using System.Text.Json;
using Carbon.Utility;

namespace Carbon.Validation;

/// <summary>
///     Entry point for the --check / --fix modes: runs the OPJ validation, prints a per-hook and
///     summary report, optionally applies the automatic fixes to a copy of the OPJ, and optionally
///     writes a JSON report. Exit code 0 means clean (or everything auto-fixed), 2 means at least
///     one hook needs human attention.
/// </summary>
internal static class OpjCheckRunner
{
	public const int ExitClean = 0;
	public const int ExitNeedsHuman = 2;

	public static int Run(CommandLineArguments arguments, Generation.PhaseTimings timings)
	{
		var project = timings.Measure("load project", () => Projects.Oxide.Oxide.Load(arguments.InputFile));
		var options = new OpjCheckOptions(arguments.ManagedFolder, ValidateOldManaged(arguments.OldManagedFolder));

		using var checker = new OpjChecker(options);
		var report = timings.Measure("check hooks", () => checker.Check(project));

		var fixedHooks = 0;
		string? fixOutput = null;
		if (arguments.Fix)
		{
			fixOutput = string.IsNullOrWhiteSpace(arguments.FixOutput)
				? Path.ChangeExtension(arguments.InputFile, ".fixed.opj")
				: arguments.FixOutput;

			timings.Measure("apply fixes", () =>
			{
				var fixer = new OpjFixer(arguments.InputFile);
				fixedHooks = fixer.Apply(report);
				fixer.Write(fixOutput);
			});
		}

		PrintReport(report, arguments.Fix);

		if (!string.IsNullOrWhiteSpace(arguments.CheckOutput))
		{
			timings.Measure("write report", () => WriteJsonReport(report, arguments, fixOutput));
		}

		PrintSummary(report, arguments.Fix, fixedHooks, fixOutput);

		if (arguments.Timings)
		{
			timings.Print();
		}

		var unappliedFixes = arguments.Fix && report.Hooks
			.SelectMany(x => x.Diagnostics)
			.Any(x => x.Severity == OpjIssueSeverity.AutoFixable && !x.Applied);

		return report.CountBySeverity(OpjIssueSeverity.NeedsHuman) > 0 || unappliedFixes ? ExitNeedsHuman : ExitClean;
	}

	private static string? ValidateOldManaged(string? oldManagedFolder)
	{
		if (string.IsNullOrWhiteSpace(oldManagedFolder))
		{
			return null;
		}

		if (!Directory.Exists(oldManagedFolder))
		{
			throw new Exception($"Old managed directory not found: {oldManagedFolder}");
		}

		return oldManagedFolder;
	}

	private static void PrintReport(OpjCheckReport report, bool fixesApplied)
	{
		foreach (var check in report.WithIssues.OrderByDescending(x => x.Severity))
		{
			var header = $"{check.HookName} [{check.TypeName}::{check.MethodName}]";

			foreach (var diagnostic in check.Diagnostics)
			{
				var line = $"{header} {diagnostic.Kind}: {diagnostic.Message}";
				switch (diagnostic.Severity)
				{
					case OpjIssueSeverity.NeedsHuman:
						Logger.Error($"[human] {line}");
						break;
					case OpjIssueSeverity.AutoFixable:
						Logger.Warning($"[{(diagnostic.Applied ? "fixed" : fixesApplied ? "fix-skipped" : "fixable")}] {line}");
						break;
					case OpjIssueSeverity.Warning:
						Logger.Warning($"[warn] {line}");
						break;
					default:
						Logger.Information($"[info] {line}");
						break;
				}

				foreach (var candidate in diagnostic.Candidates)
				{
					Logger.None($"    candidate: {candidate}");
				}

				foreach (var fix in diagnostic.Fixes)
				{
					if (diagnostic.Severity == OpjIssueSeverity.NeedsHuman)
					{
						Logger.None($"    proposal: {fix.Path} = {Render(fix.NewValue)}");
					}
				}
			}
		}
	}

	private static void PrintSummary(OpjCheckReport report, bool fix, int fixedHooks, string? fixOutput)
	{
		var needsHuman = report.CountBySeverity(OpjIssueSeverity.NeedsHuman);
		var autoFixable = report.CountBySeverity(OpjIssueSeverity.AutoFixable);
		var warnings = report.CountBySeverity(OpjIssueSeverity.Warning);
		var infos = report.CountBySeverity(OpjIssueSeverity.Info);

		Logger.None(string.Empty);
		Logger.None($">> opj check: {report.Hooks.Count} hooks | {report.CleanCount} clean | {infos} notes | {warnings} warnings | "
			+ (fix ? $"{fixedHooks} auto-fixed" : $"{autoFixable} auto-fixable") + $" | {needsHuman} need human input");

		if (!fix && report.AutoFixableDiagnostics > 0)
		{
			Logger.None($">> re-run with --fix to apply the {report.AutoFixableDiagnostics} automatic fix(es)");
		}

		if (fix)
		{
			var skipped = report.Hooks.SelectMany(x => x.Diagnostics)
				.Count(x => x.Severity == OpjIssueSeverity.AutoFixable && !x.Applied);
			if (skipped > 0)
			{
				Logger.Warning($">> {skipped} fix(es) could not be applied; see the log above");
			}
		}

		if (fix && fixOutput != null)
		{
			Logger.None($">> fixed OPJ written to '{fixOutput}'");
		}
	}

	private static void WriteJsonReport(OpjCheckReport report, CommandLineArguments arguments, string? fixOutput)
	{
		var payload = new
		{
			input = Path.GetFullPath(arguments.InputFile),
			managed = Path.GetFullPath(arguments.ManagedFolder),
			oldManaged = arguments.OldManagedFolder == null ? null : Path.GetFullPath(arguments.OldManagedFolder),
			fixOutput,
			totals = new
			{
				hooks = report.Hooks.Count,
				clean = report.CleanCount,
				info = report.CountBySeverity(OpjIssueSeverity.Info),
				warning = report.CountBySeverity(OpjIssueSeverity.Warning),
				autoFixable = report.CountBySeverity(OpjIssueSeverity.AutoFixable),
				needsHuman = report.CountBySeverity(OpjIssueSeverity.NeedsHuman),
			},
			hooks = report.WithIssues.Select(check => new
			{
				manifest = check.AssemblyName,
				action = check.Action,
				name = check.Name,
				hookName = check.HookName,
				typeName = check.TypeName,
				method = check.MethodName,
				severity = check.Severity.ToString(),
				diagnostics = check.Diagnostics.Select(diagnostic => new
				{
					kind = diagnostic.Kind.ToString(),
					severity = diagnostic.Severity.ToString(),
					message = diagnostic.Message,
					applied = diagnostic.Applied,
					fixes = diagnostic.Fixes.Select(fix => new
					{
						path = fix.Path,
						oldValue = fix.OldValue,
						newValue = fix.NewValue,
					}).ToArray(),
					candidates = diagnostic.Candidates.ToArray(),
				}).ToArray(),
			}).ToArray(),
		};

		var directory = Path.GetDirectoryName(Path.GetFullPath(arguments.CheckOutput!));
		if (!string.IsNullOrEmpty(directory))
		{
			Directory.CreateDirectory(directory);
		}

		File.WriteAllText(arguments.CheckOutput!, JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true }));
	}

	private static string Render(object? value)
	{
		return value switch
		{
			null => "null",
			string[] array => "[" + string.Join(", ", array) + "]",
			_ => value.ToString() ?? "null",
		};
	}
}
