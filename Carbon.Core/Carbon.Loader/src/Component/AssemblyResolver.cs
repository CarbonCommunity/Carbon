///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
///
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Carbon.Common;
using Carbon.Utility;

namespace Carbon.Components;

public class AssemblyResolver : Singleton<AssemblyResolver>, IDisposable
{
	private static string[] lookup =
	{
		Context.Directory.GameManaged,
		Context.Directory.CarbonLib,
		Context.Directory.CarbonManaged,
	};

	private static List<CarbonReference> cachedReferences
		= new List<CarbonReference>();

	public static float LastCacheUpdate
	{
		get;
		private set;
	}

	internal void RegisterDomain(AppDomain domain)
	{
		domain.AssemblyResolve += ResolveAssembly;
#if DEBUG
		domain.AssemblyLoad += LoadAssembly;
#endif
	}

	private static Assembly ResolveAssembly(object sender, ResolveEventArgs args)
		=> GetAssembly(args.Name).assembly ?? null;

#if DEBUG
	private static void LoadAssembly(object sender, AssemblyLoadEventArgs args)
	{
		Utility.Logger.Log($"Load: {args.LoadedAssembly.GetName().Name}");
	}
#endif

	public static bool IsReferenceAllowed(string name)
	{
		foreach (string expr in Context.Regex.refWhitelist)
			if (Regex.IsMatch(name, expr))
				return true;
		Logger.Warn($"name:{name} is not allowed");
		return false;
	}

	public static bool IsReferenceAllowed(Assembly assembly)
		=> IsReferenceAllowed(assembly.GetName().Name);

	public static CarbonReference GetAssembly(string name)
	{
		// special case: carbon random asm name
		if (Regex.IsMatch(name, @"^Carbon(-\d+)?$"))
			name = "Carbon";

		// new carbon ref (ncr)
		CarbonReference ncr = new CarbonReference();
		ncr.LoadMetadata(info: new AssemblyName(name));

		// cached carbon ref (ccr)
		CarbonReference ccr = cachedReferences.FirstOrDefault(
			item => item.name == ncr.name
		);

		if (ccr != null)
		{
			Logger.Log($"Resolved: {ccr.FileName} from cache");
			return ccr;
		}

		foreach (string bp in lookup)
		{
			string p = Path.Combine(bp, $"{ncr.name}.dll");
			if (File.Exists(p) && ncr.LoadFromFile(p) != null)
			{
				Logger.Log($"Resolved: {ncr.FileName} from disk");
				LastCacheUpdate = UnityEngine.Time.realtimeSinceStartup;
				cachedReferences.Add(ncr);
				return ncr;
			}
		}

		Logger.Warn($"Unresolved: {ncr.fullName}");
		ncr = default;
		return null;
	}

	public void Dispose()
	{
		cachedReferences.Clear();
		cachedReferences = default;
	}
}