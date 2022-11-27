using System.Collections.Generic;

///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

namespace Carbon.LoaderEx.Context;

internal sealed class Patterns
{
	// used mainly to skip any disk caches and for rudimentary validation of allowed assemblies
	internal static readonly string carbonNamePattern =
		@"(?i)^(carbon(?:\.(?:doorstop|hooks|loader))?)(_\w+)?$";

	// used mainly for not moving any carbon related files to the shared folders
	internal static readonly string carbonFileNamePattern =
		@"(?i)^carbon([\.-](doorstop|hooks|loader))?(.dll)$";

	// used to match assemblies compiled by carbon i.e. plugins
	internal static readonly string oxideCompiledAssembly =
		@"(?i)^(script\.)(.+)(\.[-\w]+)";

	internal static readonly List<string> refWhitelist = new List<string>
	{
		// Facepunch managed refs
		@"^Assembly-CSharp(-firstpass)?$",
		@"^Facepunch(.\w+(.\w+)+)?$",
		@"^Newtonsoft(.\w+)?$",
		@"^Rust(.\w+(.\w+)+)?$",
		@"^Unity(.\w+(.\w+)+)?$",
		@"^UnityEngine(.\w+)?$",

		// Carbon managed refs
		@"^Carbon(-\d+)?$",

		// System stuff
		@"^mscorlib$",
		@"^System.Drawing(.\w+)?$",
		@"^System.Core$",
		@"^System.Xml(.\w+)?$",
		@"^System$",
	};

	// used for resolving assembly names with random bits to a common name
	internal static readonly Dictionary<string, string> refTranslator = new Dictionary<string, string>
	{
		// special case: carbon random asm name
		{ @"^Carbon(-\d+)?$", "Carbon" },
		{ @"^Carbon.Hooks(-\d+)?$", "Carbon.Hooks" },

		// HarmonyLib v2
		{ @"^0Harmony$", "1Harmony" }
	};
}

