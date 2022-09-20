using System;
using CommandLine;

namespace Carbon.Patch
{
	public class CommandLineOptions
	{
		[Option('p', "path", Required = true)]
		public string Path { get; set; }

	}
}