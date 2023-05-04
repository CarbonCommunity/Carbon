extern alias MonoCecilStandalone;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Facepunch.Extend;
using MonoCecilStandalone::Mono.Cecil;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Utility;

public sealed class AssemblyValidator : MarshalByRefObject
{
	internal IReadOnlyList<string> Blacklist { get; set; }
	internal IReadOnlyList<string> Whitelist { get; set; }

	internal bool Validate(string file)
	{
		// no blacklist, no whitelist, no validation
		if (Blacklist == null && Whitelist == null) return true;

		try
		{
			byte[] raw = File.ReadAllBytes(file)
			?? throw new Exception($"Unable to read '{file}' from disk");

			using MemoryStream input = new MemoryStream(raw);
			AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(
				input, parameters: new ReaderParameters { InMemory = true });

			foreach (ModuleDefinition module in assembly.Modules)
			{
				foreach (AssemblyNameReference reference in module.AssemblyReferences)
				{
					if (Blacklist is not null)
					{
						foreach (string expr in Blacklist)
						{
							if (Regex.IsMatch(reference.Name, expr))
								throw new Exception($" >> Reference '{reference.Name}' not allowed by blacklisting");
						}
					}

					if (Whitelist is not null)
					{
						if (!Whitelist.Contains(reference.Name))
							throw new Exception($" >> Reference '{reference.Name}' not allowed by whitelisting");
					}
				}
			}
		}
		catch (System.Exception e)
		{
			Logger.Warn(e.Message);
			return false;
		}
		return true;
	}
}
