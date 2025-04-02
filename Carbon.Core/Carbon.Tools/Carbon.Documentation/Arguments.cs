using CommandLine;

#pragma warning disable

public class CommandLineArguments
{
	[Option("carbon", Required = true, HelpText = "The carbon root directory")]
	public string Carbon { get; set; }

	[Option("rust", Required = true, HelpText = "The Rust (Windows) directory")]
	public string Rust { get; set; }

	[Option("docs", Required = true, HelpText = "The Docs output")]
	public string Docs { get; set; }
}
