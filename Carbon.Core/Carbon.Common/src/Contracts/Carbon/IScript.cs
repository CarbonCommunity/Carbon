namespace Carbon.Contracts;

public interface IScript : IDisposable
{
	Assembly Assembly { get; set; }
	Type Type { get; set; }

	string Name { get; set; }
	string Author { get; set; }
	VersionNumber Version { get; set; }
	string Description { get; set; }
	string Source { get; set; }
	IScriptLoader Loader { get; set; }
	RustPlugin Instance { get; set; }
	bool IsCore { get; set; }
}
