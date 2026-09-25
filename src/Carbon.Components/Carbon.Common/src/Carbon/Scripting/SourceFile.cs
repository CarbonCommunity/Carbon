namespace Carbon.Core;

/// <summary>
/// A single piece of plugin source code handed to the compiler.
/// </summary>
public class SourceFile
{
	/// <summary>The loadable unit this file belongs to (the .cs, .cszip or dev folder).</summary>
	public string ContextFilePath { get; set; }
	public string ContextFileName { get; set; }

	/// <summary>The actual file (or zip entry) this content came from.</summary>
	public string FilePath { get; set; }
	public string FileName { get; set; }

	/// <summary>Source code. Lazily read from <see cref="FilePath"/> when left empty.</summary>
	public string Content { get; set; }

	public static SourceFile FromFile(string path, string contextPath = null, string content = null)
	{
		contextPath ??= path;

		return new SourceFile
		{
			ContextFilePath = contextPath,
			ContextFileName = Path.GetFileName(contextPath),
			FilePath = path,
			FileName = Path.GetFileName(path),
			Content = content
		};
	}
}
