using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Carbon.Base;
using Carbon.Hooks;
using Oxide.Core;
using Oxide.Plugins;

namespace Carbon.Contracts
{
	public interface IScript : IDisposable
	{
		Assembly Assembly { get; set; }
		Type Type { get; set; }

		string Name { get; set; }
		string Author { get; set; }
		Version Version { get; set; }
		string Description { get; set; }
		string Source { get; set; }
		IScriptLoader Loader { get; set; }
		RustPlugin Instance { get; set; }
		bool IsCore { get; set; }
	}
}
