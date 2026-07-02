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

	private static bool IsRootDirectory(string path)
	{
		if (path.Length == 1) return IsSeparator(path[0]);
		if (path.Length == 3) return path[1] == ':' && IsSeparator(path[2]);
		return false;
	}

	private static bool IsSeparator(char c)
		=> c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar;
}
