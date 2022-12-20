using System;
using System.Collections.Generic;

/*
 *
 * Copyright (c) 2022 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.LoaderEx.Context;

internal sealed class Patterns
{
	internal static readonly StringComparison IgnoreCase
		= StringComparison.InvariantCultureIgnoreCase;

	internal static readonly string RenamedAssembly
		= @"(?i)^((?:\w+)(?:\.(?:\w+))?)_([0-9a-f]+)$";

	// used to rudimentary identify assembly/files that are part of carbon
	internal static readonly string CarbonManagedFile =
		@"(?i)^(carbon(?:\.(?:doorstop|hooks|loader))?)((_\w+)?(.dll)?)?$";

	internal static readonly IReadOnlyDictionary<string, string> refTranslator = new Dictionary<string, string>
	{
		{ @"^0Harmony$", "1Harmony" }
	};
}
