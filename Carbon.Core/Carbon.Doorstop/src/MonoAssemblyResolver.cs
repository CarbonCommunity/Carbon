using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Cecil;

///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

namespace Carbon.Utility;

internal sealed class MonoAssemblyResolver : BaseAssemblyResolver
{
	private static HashSet<AssemblyDefinition> cachedAssemblies
		= new HashSet<AssemblyDefinition>();

	private DefaultAssemblyResolver defaultResolver;

	public MonoAssemblyResolver()
	{
		defaultResolver = new DefaultAssemblyResolver();
	}

	public override AssemblyDefinition Resolve(AssemblyNameReference name)
	{
		AssemblyDefinition assembly = null;
		try
		{
			assembly = defaultResolver.Resolve(name);
			return assembly;
		}
		catch (AssemblyResolutionException)
		{
			string location;
			AssemblyName assemblyName = new AssemblyName(name.Name);

			string fileName = $"{assemblyName.Name}.dll";

			AssemblyDefinition cached = cachedAssemblies.FirstOrDefault(
					item => item.FullName == assemblyName.FullName);
			if (cached != null) return cached;

			location = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileName(fileName));
			if (File.Exists(location)) assembly = AssemblyDefinition.ReadAssembly(location);

			location = Path.Combine(Context.Managed, Path.GetFileName(fileName));
			if (File.Exists(location)) assembly = AssemblyDefinition.ReadAssembly(location);

			if (assembly != null)
			{
				if (cachedAssemblies.Add(assembly))
					Utility.Logger.Log($">> MonoAssemblyResolver {fileName}");
			}
			return assembly;
		}
	}
}
