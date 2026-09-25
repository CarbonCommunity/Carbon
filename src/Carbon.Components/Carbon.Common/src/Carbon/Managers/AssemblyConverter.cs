namespace Carbon.Managers;

/// <summary>What kind of addon an assembly is being loaded as.</summary>
public enum AddonType
{
	Default,
	Extension,
	HarmonyMod
}

public enum ConversionResult
{
	Success,
	Fail,
	Skip
}

/// <summary>
/// Rewrites an assembly before it gets loaded, e.g. to remap references to another framework.
/// Register instances in <see cref="AssemblyManager.Converters"/>.
/// </summary>
public abstract class AssemblyConverter
{
	public abstract string Name { get; }

	public abstract bool Handles(AddonType type);

	/// <summary>Replace <paramref name="raw"/> with the converted bytes. Return <see cref="ConversionResult.Fail"/> to block loading.</summary>
	public abstract ConversionResult Convert(string file, AddonType type, ref byte[] raw);
}

/// <summary>An assembly loaded by Carbon, with the raw bytes it was loaded from.</summary>
public class CachedAssembly
{
	public string Name { get; internal set; }
	public byte[] Raw { get; internal set; }
	public Assembly Assembly { get; internal set; }
}
