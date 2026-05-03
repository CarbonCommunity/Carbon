using System.Reflection;
using Carbon.Generation;
using Carbon.Output;
using Carbon.Projects.Oxide;
using Carbon.Utility;
using CommandLine;
using CommandLineArguments = Carbon.Utility.CommandLineArguments;

namespace Carbon;

internal static class Program
{
	internal static CommandLineArguments Arguments = null!;

	internal static void Main(string[] args)
	{
		Parser.Default.ParseArguments<CommandLineArguments>(args)
			.WithNotParsed(_ => Environment.Exit(1))
			.WithParsed(x => Arguments = x);

		var validationMode = ParseValidationMode(Arguments.ValidationMode);
		var timings = new PhaseTimings();

		if (!Directory.Exists(Arguments.ManagedFolder))
		{
			throw new Exception($"Managed directory not found at '{Arguments.ManagedFolder}'");
		}

		AppDomain.CurrentDomain.AssemblyResolve += Utility.Program.AssemblyResolver;

		if (!Directory.Exists(Arguments.OutputFolder))
		{
			throw new Exception($"Output directory not found at '{Arguments.OutputFolder}'");
		}

		var writer = new HookahOutputWriter(Arguments.OutputFolder, Arguments.ManagedFolder, Arguments.Important, Arguments.Deterministic,
			Arguments is { FormatOutput: true, DisableOutputFormatting: false });
		timings.Measure("clean output", writer.CleanOutputFolder);

		var project = timings.Measure("load project", () => Oxide.Load(Arguments.InputFile));
		var gameProtocol = timings.Measure("resolve game protocol", ResolveGameProtocol);

		var generator = new HookahGenerator(new HookahGenerationOptions(Arguments.Jobs, validationMode, Arguments.Deterministic));
		var report = timings.Measure("generate hooks", () => generator.Generate(project));

		Console.WriteLine($">> types done:{report.SuccessfulHooks.Count} failed:{report.FailedHooks.Count} types");

		timings.Measure("write output", () => writer.Write(report, gameProtocol));
		if (!string.IsNullOrWhiteSpace(Arguments.SummaryOutput))
		{
			timings.Measure("write summary", () => HookahOutputWriter.WriteSummary(report, gameProtocol, Arguments.SummaryOutput));
		}

		if (Arguments.Timings)
		{
			timings.Print();
		}
	}

	private static string ResolveGameProtocol()
	{
		return (string?)Utility.Program.ResolveAssembly("Rust.Global")?.GetType("Rust.Protocol")
			       ?.GetProperty("printable", BindingFlags.Public | BindingFlags.Static)?.GetValue(null, null)
		       ?? "unknown";
	}

	private static ValidationMode ParseValidationMode(string value)
	{
		if (Enum.TryParse<ValidationMode>(value, true, out var mode))
		{
			return mode;
		}

		throw new ArgumentOutOfRangeException(nameof(value), value, "Validation mode must be warn, skip, or fail.");
	}
}
