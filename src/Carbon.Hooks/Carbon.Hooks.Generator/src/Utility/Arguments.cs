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

	[Option("output", Required = true, HelpText = "The output C# file location")]
	public required string OutputFolder { get; set; }

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
}
