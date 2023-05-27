using System;
using System.Collections.Generic;
using Carbon.Core;

namespace Carbon.Contracts;

public interface IScriptLoader : IDisposable
{
	List<IScript> Scripts { get; set; }
	void Clear();
	void Load();

	string File { get; set; }
	string Source { get; set; }
	bool IsCore { get; set; }
	bool IsExtension { get; set; }

	bool HasFinished { get; set; }
	bool HasRequires { get; set; }

	ModLoader.ModPackage Mod { get; set; }
	IBaseProcessor.IInstance Instance { get; set; }
	IBaseProcessor.IParser Parser { get; set; }
}
