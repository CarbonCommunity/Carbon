using CommandLine;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Carbon.Utility;

// ReSharper disable once ClassNeverInstantiated.Global
internal class CommandLineArguments
{
	[Option("input", Required = true, HelpText = "The Oxide project file (opj) to be read")]
	public required string InputFile { get; set; }

	[Option("managed", Required = true, HelpText = "The managed folder where the game libraries can be found")]
	public required string ManagedFolder { get; set; }

	[Option("output", Required = false, HelpText = "The output C# file location (required unless running --check/--fix)")]
	public string? OutputFolder { get; set; }

	[Option("important", Default = false, HelpText = "It's a very important patch")]
	public bool Important { get; set; }

	[Option("validation-mode", Default = "warn", HelpText = "Validation mode: warn, skip, fail")]
	public string ValidationMode { get; set; } = "warn";

	[Option("deterministic", Default = false, HelpText = "Emit stable generated output for regression tests")]
	public bool Deterministic { get; set; }

	[Option("jobs", Default = 0, HelpText = "Maximum concurrent hook generation jobs. Defaults to processor count")]
	public int Jobs { get; set; }

	[Option("timings", Default = true, HelpText = "Print generation phase timings")]
	public bool Timings { get; set; }

	[Option("format-output", Default = true, HelpText = "Format generated source with Roslyn before writing files")]
	public bool FormatOutput { get; set; }

	[Option("no-format-output", Default = false, HelpText = "Write generated source without Roslyn formatting")]
	public bool DisableOutputFormatting { get; set; }

	[Option("summary-output", Required = false, HelpText = "Optional path to write a JSON generation summary")]
	public string? SummaryOutput { get; set; }

	[Option("check", Default = false, HelpText = "Validate the OPJ hooks against the managed folder instead of generating hooks")]
	public bool Check { get; set; }

	[Option("fix", Default = false, HelpText = "Implies --check; applies the automatic fixes and writes an updated OPJ")]
	public bool Fix { get; set; }

	[Option("fix-output", Required = false, HelpText = "Path for the fixed OPJ; defaults to '<input>.fixed.opj' next to the input")]
	public string? FixOutput { get; set; }

	[Option("old-managed", Required = false, HelpText = "Managed folder of the previous game build, used to re-anchor drifted injection indexes")]
	public string? OldManagedFolder { get; set; }

	[Option("check-output", Required = false, HelpText = "Optional path to write a JSON validation report")]
	public string? CheckOutput { get; set; }
}
