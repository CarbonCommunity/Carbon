namespace Carbon.Extensions;

public static class PathEx
{
#if WIN
	public const StringComparison PathComparison = StringComparison.OrdinalIgnoreCase;
#else
	public const StringComparison PathComparison = StringComparison.Ordinal;
#endif

	public static string NormalizePath(string path)
	{
		if (string.IsNullOrEmpty(path)) return string.Empty;

		var absolute = Path.GetFullPath(path);

		return IsRootDirectory(absolute) ? absolute : absolute.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
	}

	public static bool HasExtension(string path, string extension)
		=> !string.IsNullOrEmpty(path) && path.EndsWith(extension, PathComparison);

	public static bool Equals(string a, string b)
		=> string.Equals(a, b, PathComparison);

	/// <summary>Swaps any slash for the platform's directory separator.</summary>
	public static string CleanPath(string path)
		=> path?.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);

	/// <summary>Directory part of a path, tolerant to either slash. Null when there is none.</summary>
	public static string GetDirectoryName(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return null;
		}

		name = name.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
		var index = name.LastIndexOf(Path.DirectorySeparatorChar);
		return index < 0 ? null : name.Substring(0, index);
	}

	private static bool IsRootDirectory(string path)
	{
		if (path.Length == 1) return IsSeparator(path[0]);
		if (path.Length == 3) return path[1] == ':' && IsSeparator(path[2]);
		return false;
	}

	private static bool IsSeparator(char c)
		=> c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar;
}
