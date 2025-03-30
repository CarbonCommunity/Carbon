using CommandLine;

#pragma warning disable

public class CommandLineArguments
{
	[Option("oxide.hooks", Required = true, HelpText = "The Carbon.Hooks.Oxide DLL file")]
	public string OxideHooks { get; set; }
}
