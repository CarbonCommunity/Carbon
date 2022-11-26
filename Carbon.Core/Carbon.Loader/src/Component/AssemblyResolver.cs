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
using Carbon.LoaderEx.Common;
using Carbon.LoaderEx.Utility;

namespace Carbon.LoaderEx.Components;

public class AssemblyResolver : Singleton<AssemblyResolver>, IDisposable
{
	static AssemblyResolver() { }

	private readonly static string[] lookup =
	{
		Context.Directory.CarbonLib,
		Context.Directory.CarbonManaged,
		Context.Directory.GameManaged,
	};

	private List<CarbonReference> cachedReferences
		= new List<CarbonReference>();

	internal AssemblyResolver()
	{
		foreach (string fp in lookup)
		{
			Utility.Logger.Log($"Warming up assemblies from '{fp}'..");
			foreach (string file in Directory.EnumerateFiles(fp, "*.dll"))
				ResolveAssembly(Path.GetFileNameWithoutExtension(file));
		}
	}

	internal void RegisterDomain(AppDomain domain)
		=> domain.AssemblyResolve += ResolveAssembly;

	private Assembly ResolveAssembly(object sender, ResolveEventArgs args)
		=> ResolveAssembly(args.Name).assembly;

	public bool IsReferenceAllowed(Assembly assembly)
		=> IsReferenceAllowed(assembly.GetName().Name);

	public bool IsReferenceAllowed(string name)
	{
		return true;
		// foreach (string expr in Context.Patterns.refWhitelist)
		// 	if (Regex.IsMatch(name, expr))
		// 		return true;
		// Logger.Debug($"Reference: {name} is not white listed");
		// return false;
	}

	private CarbonReference ResolveAssembly(string name)
	{
		try
		{
			// static translator
			foreach (KeyValuePair<string, string> kvp in Context.Patterns.refTranslator)
			{
				if (!Regex.IsMatch(name, kvp.Key)) continue;
				string result = Regex.Replace(name, kvp.Key, kvp.Value);
				Logger.Debug($"Translated: input:{name} match:'{kvp.Key}' result:{result}");
				name = result;
				break;
			}

			// new carbon ref (ncr)
			CarbonReference ncr = new CarbonReference();
			ncr.LoadMetadata(info: new AssemblyName(name));

			// carbon asm files are an edge case
			Match match = Regex.Match(ncr.name, Context.Patterns.carbonNamePattern);
			if (match.Success)
			{
				switch (match.Groups[1].Value)
				{
					case "Carbon":
					case "Carbon.Doorstop":
						string p = Path.Combine(Context.Directory.CarbonManaged, $"{ncr.name}.dll");
						if (File.Exists(p) && ncr.LoadFromFile(p) != null)
						{
							Logger.Debug($"Resolved: {ncr.FileName} from disk (forced)");
							cachedReferences.Add(ncr);
							return ncr;
						}
						break;

					case "Carbon.Loader":
						// this is the only asm that will get it's name manipulated by Facepunch's
						// harmony loader, all this shit just to deal with it.. Thanks @Jake-Rich !
						if (ncr.LoadFromAppDomain(Program.assemblyName) != null)
						{
							Logger.Debug($"Resolved: Carbon.Loader from app domain (forced)");
							return ncr;
						}
						break;
				}

				Logger.Error("The asm name matched carbonNamePattern but it was not handled, this is a bug.");
				return null;
			}

			// cached carbon ref (ccr)
			CarbonReference ccr = cachedReferences.FirstOrDefault(
				item => item.name == ncr.name
			);

			if (ccr != null)
			{
				Logger.Debug($"Resolved: {ccr.FileName} from cache");
				return ccr;
			}

			foreach (string fp in lookup)
			{
				string p = Path.Combine(fp, $"{ncr.name}.dll");
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
		catch (System.Exception e)
		{
			Logger.Error($"ResolveAssembly Exception for '{name}'", e);
			return null;
		}
	}

	public CarbonReference GetAssembly(string name)
	{
		// the second check should be removed when whitelisting is in place
		if (!IsReferenceAllowed(name) || Regex.IsMatch(name, Context.Patterns.oxideCompiledAssembly)) return null;
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