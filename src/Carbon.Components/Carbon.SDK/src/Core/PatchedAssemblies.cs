using System.Collections.Generic;

namespace Carbon;

/// <summary>
/// Publicized Rust assemblies produced by Carbon.Startup, referenced by the plugin compiler.
/// </summary>
public class PatchedAssemblies
{
	public static Dictionary<string, byte[]> AssemblyCache { get; set; } = new();
}
