namespace Carbon.Contracts;

public interface IScriptLoader : IDisposable
{
	List<IScript> Scripts { get; set; }
	void Clear();
	void Load();

	List<ISource> Sources { get; set; }
	bool IsCore { get; set; }
	bool IsExtension { get; set; }

	bool HasFinished { get; set; }
	bool HasRequires { get; set; }

	ModLoader.Package Mod { get; set; }
	IBaseProcessor.IProcess Process { get; set; }
	IBaseProcessor.IParser Parser { get; set; }
}
