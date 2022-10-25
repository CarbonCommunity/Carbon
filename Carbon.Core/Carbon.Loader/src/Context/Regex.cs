

using System.Collections.Generic;
///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 
namespace Carbon.Context;

internal sealed class Regex
{
	internal static readonly string CarbonNameValidator =
		@"(?i)^carbon([\.-](doorstop|loader|unix))?(.dll)?$";

	internal static readonly List<string> refWhitelist = new List<string>
	{
		// Facepunch managed refs
		@"^Assembly-CSharp(-firstpass)?$",
		@"^Facepunch(.\w+(.\w+)+)?$",
		@"^Rust(.\w+(.\w+)+)?$",
		@"^System(.\w+(.\w+)+)?$",
		@"^Unity(.\w+(.\w+)+)?$",
		@"^UnityEngine(.\w+)?$",
		@"^mscorlib$",

		// Carbon managed refs
		@"^Carbon(-\d+)?$"
	};
}

