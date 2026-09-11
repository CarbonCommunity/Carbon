using System.Reflection;

namespace Carbon.Validation.Metadata;

/// <summary>
///     Lazily opens game assemblies from a managed folder for pure metadata inspection. Unlike the
///     reflection pipeline this never loads assemblies into the runtime, so an old and a new build
///     of the same assembly can be inspected side by side.
/// </summary>
internal sealed class MetadataAssemblySet(string folder) : IDisposable
{
	private readonly Dictionary<string, MetadataAssembly?> _byPath = new(StringComparer.OrdinalIgnoreCase);

	public string Folder { get; } = folder;

	/// <summary>
	///     Opens an assembly by file name ("Assembly-CSharp.dll") relative to the managed folder.
	/// </summary>
	public MetadataAssembly? Get(string assemblyFileName)
	{
		return GetByPath(Path.Combine(Folder, Path.GetFileName(assemblyFileName)));
	}

	/// <summary>
	///     Opens an assembly by full path. Used to map runtime-reflected modules (which may resolve
	///     from outside the managed folder) back to their metadata.
	/// </summary>
	public MetadataAssembly? GetByPath(string fullPath)
	{
		if (_byPath.TryGetValue(fullPath, out var cached))
		{
			return cached;
		}

		MetadataAssembly? assembly = null;
		try
		{
			if (File.Exists(fullPath))
			{
				assembly = new MetadataAssembly(fullPath);
			}
		}
		catch (Exception ex)
		{
			Utility.Logger.Warning($"failed to open '{fullPath}' for metadata inspection: {ex.Message}");
		}

		_byPath.Add(fullPath, assembly);
		return assembly;
	}

	/// <summary>
	///     Maps a runtime-resolved method back to its metadata definition so its IL can be inspected.
	/// </summary>
	public MetadataMethod? Resolve(MethodBase method)
	{
		var location = method.Module.Assembly.Location;
		if (string.IsNullOrEmpty(location))
		{
			return null;
		}

		return GetByPath(location)?.GetMethodByToken(method.MetadataToken);
	}

	public void Dispose()
	{
		foreach (var assembly in _byPath.Values)
		{
			assembly?.Dispose();
		}

		_byPath.Clear();
	}
}
