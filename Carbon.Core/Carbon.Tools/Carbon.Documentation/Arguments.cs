using CommandLine;

#pragma warning disable

public class CommandLineArguments
{
	[Option("carbon", Required = true, HelpText = "The carbon root directory")]
	public string Carbon { get; set; }
}
