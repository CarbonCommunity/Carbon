using AsmResolver.DotNet;
using Carbon.Compat;

namespace Carbon.Compat;

/*
 *
 * Copyright (c) 2023-2024 Patrette, under the GNU v3 license rights
 * Copyright (c) 2023-2024 Carbon Community, under the GNU v3 license rights
 *
 */

public static class OxideHelpers
{
	public static readonly AssemblyReference OxideAssembly = new("Carbon.Oxide", new Version(0, 0, 0, 0));

	/// <summary>True for references to Oxide's own assemblies (Oxide.Core, Oxide.Rust..), excluding third-party extensions.</summary>
	public static bool IsOxideASM(AssemblyReference aref)
	{
		return aref != null && aref.Name.StartsWith("Oxide.") && !aref.Name.ToLower().StartsWith("oxide.ext.");
	}

	private static System.Reflection.Assembly _newtonsoft;

	private static System.Reflection.Assembly Newtonsoft
	{
		get
		{
			_newtonsoft ??= AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => assembly.GetName().Name == "Newtonsoft.Json");
			return _newtonsoft;
		}
	}

	public static IResolutionScope GetNewtonsoftScope(TypeReference type, ReferenceImporter importer)
	{
		// Oxide ships a patched Newtonsoft.Json 8.0.0.0 carrying converters the official 13.0.0.0 build
		// Rust uses doesn't have. This package mirrors those under their original namespace, so anything
		// the real assembly can't provide gets pointed here instead of at a type that would only fail to
		// resolve once the plugin or extension actually runs.
		if (Newtonsoft == null || Newtonsoft.GetType(type.FullName) != null)
		{
			return CompatManager.Newtonsoft.ImportWith(importer);
		}

		if (typeof(OxideHelpers).Assembly.GetType(type.FullName) == null)
		{
			Logger.Warn($"Oxide type '{type.FullName}' is missing from Newtonsoft.Json {Newtonsoft.GetName().Version} and Carbon has no equivalent for it");
		}

		return OxideAssembly.ImportWith(importer);
	}
}
