using System;
using CommandLine;
using HarmonyLib;

Parser.Default.ParseArguments<CommandLineArguments>(args)
	.WithNotParsed(x => Environment.Exit(1))
	.WithParsed(x => Generator.Arguments = x);

Generator.Prewarm();
Generator.Generate();
