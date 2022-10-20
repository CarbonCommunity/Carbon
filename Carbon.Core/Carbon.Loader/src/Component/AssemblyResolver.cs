///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
///
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Carbon.Common;
using Carbon.Utility;

namespace Carbon.Components;

internal class AssemblyResolver : Singleton<AssemblyResolver>, IDisposable
{
	private static string[] lookup =
	{
		Context.Directory.CarbonLib,
		Context.Directory.GameManaged
	};

	private static List<CarbonReference> cachedReferences
		= new List<CarbonReference>();

	internal void RegisterDomain(AppDomain domain)
	{
		domain.AssemblyResolve += ResolveAssembly;
#if DEBUG
		domain.AssemblyLoad += LoadAssembly;
#endif
	}

	private static Assembly ResolveAssembly(object sender, ResolveEventArgs args)
	{
		// new carbon ref (ncr)
		CarbonReference ncr = new CarbonReference();
		ncr.LoadMetadata(info: new AssemblyName(args.Name));

		// cached carbon ref (ccr)
		CarbonReference ccr = cachedReferences.FirstOrDefault(
			item => item.name == ncr.name
		);

		if (ccr != null)
		{
			Logger.Log($"Resolved: {ccr.FileName} from cache");
			return ccr.assembly;
		}

		foreach (string bp in lookup)
		{
			string p = Path.Combine(bp, $"{ncr.name}.dll");
			if (File.Exists(p) && ncr.LoadFromFile(p) != null)
			{
				Logger.Log($"Resolved: {ncr.FileName} from disk");
				cachedReferences.Add(ncr);
				return ncr.assembly;
			}
		}

		Logger.Warn($"Unresolved: {ncr.fullName}");
		return null;
	}

#if DEBUG
	private static void LoadAssembly(object sender, AssemblyLoadEventArgs args)
	{
		Utility.Logger.Log($"Load: {args.LoadedAssembly.GetName().Name}");
	}
#endif

	public void Dispose()
	{
		cachedReferences.Clear();
		cachedReferences = default;
	}
}