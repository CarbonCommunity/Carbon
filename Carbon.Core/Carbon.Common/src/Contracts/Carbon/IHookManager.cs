using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Carbon.Hooks;

namespace Carbon.Contracts
{
	public interface IHookManager
	{
		IEnumerable<IHook> LoadedPatches { get; }
		IEnumerable<IHook> LoadedStaticHooks { get; }
		IEnumerable<IHook> LoadedDynamicHooks { get; }

		IEnumerable<IHook> InstalledPatches { get; }
		IEnumerable<IHook> InstalledStaticHooks { get; }
		IEnumerable<IHook> InstalledDynamicHooks { get; }

		void Subscribe(string hookName, string fileName);
		void Unsubscribe(string hookName, string fileName);

		bool IsHookLoaded(string hookName);
		int GetHookSubscriberCount(string identifier);
	}
}
