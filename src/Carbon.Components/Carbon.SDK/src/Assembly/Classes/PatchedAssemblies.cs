using System.Collections.Generic;

namespace API.Assembly;

public class PatchedAssemblies
{
	public static Dictionary<string, byte[]> AssemblyCache { get; set; } = new();
}
