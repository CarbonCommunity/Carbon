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
	private readonly static string[] lookup =
	{
		Context.Directory.GameManaged,
		Context.Directory.CarbonLib,
		Context.Directory.CarbonManaged,
	};

	private readonly static Dictionary<string, string> translation = new Dictionary<string, string>
	{
		{ @"^Carbon(-\d+)?$", "Carbon" }, // special case: carbon random asm name
		{ @"^0Harmony$", "1Harmony" }
	};

	private List<CarbonReference> cachedReferences
		= new List<CarbonReference>();

	internal AssemblyResolver()
	{
		string bp = Context.Directory.CarbonManaged;
		Utility.Logger.Log($"Warming up assemblies from '{bp}'..");

		foreach (string file in Directory.EnumerateFiles(bp, "*.dll"))
			ResolveAssembly(Path.GetFileNameWithoutExtension(file));
	}

	internal void RegisterDomain(AppDomain domain)
	{
		domain.AssemblyResolve += ResolveAssembly;
		//domain.AssemblyLoad += LoadAssembly;
	}

	public bool IsReferenceAllowed(Assembly assembly)
		=> IsReferenceAllowed(assembly.GetName().Name);

	public bool IsReferenceAllowed(string name)
	{
		return true;
		foreach (string expr in Context.Regex.refWhitelist)
			if (Regex.IsMatch(name, expr))
				return true;
		Logger.Debug($"Reference: {name} is not white listed");
		return false;
	}

	private Assembly ResolveAssembly(object sender, ResolveEventArgs args)
		=> ResolveAssembly(args.Name).assembly ?? null;

	private CarbonReference ResolveAssembly(string name)
	{
		foreach (KeyValuePair<string, string> kvp in translation)
		{
			if (!Regex.IsMatch(name, kvp.Key)) continue;
			string result = Regex.Replace(name, kvp.Key, kvp.Value);
			Logger.Debug($"Translation match:'{kvp.Key}' input:{name} result:{result}");
			name = result;
			break;
		}

		// new carbon ref (ncr)
		CarbonReference ncr = new CarbonReference();
		ncr.LoadMetadata(info: new AssemblyName(name));

		// cached carbon ref (ccr)
		CarbonReference ccr = cachedReferences.FirstOrDefault(
			item => item.name == ncr.name
		);

		if (ccr != null)
		{
			Logger.Debug($"Resolved: {ccr.FileName} from cache");
			return ccr;
		}

		foreach (string bp in lookup)
		{
			string p = Path.Combine(bp, $"{ncr.name}.dll");
			if (File.Exists(p) && ncr.LoadFromFile(p) != null)
			{
				Logger.Debug($"Resolved: {ncr.FileName} from disk");
				cachedReferences.Add(ncr);
				return ncr;
			}
		}

		Logger.Warn($"Unresolved: {ncr.fullName}");
		ncr = default;
		return null;
	}

	public CarbonReference GetAssembly(string name)
	{
		if (!IsReferenceAllowed(name)) return null;
		CarbonReference asm = ResolveAssembly(name);
		if (asm == null || asm.assembly.IsDynamic) return null;
		return asm;
	}

	public void Dispose()
	{
		cachedReferences.Clear();
		cachedReferences = default;
	}
}