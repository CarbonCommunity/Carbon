namespace Carbon.Plugins;

/// <summary>
/// Base of the shared plugin libraries (permissions, lang, timers..).
/// </summary>
public class Library : IDisposable
{
	/// <summary>Global libraries are shared by every plugin instead of being created per plugin.</summary>
	public virtual bool IsGlobal { get; }

	public Exception LastException { get; protected set; }

	public static implicit operator bool(Library library)
	{
		return library != null;
	}
	public static bool operator !(Library library)
	{
		return !library;
	}

	public virtual void Dispose() { }
	public virtual void Shutdown() { }
}
