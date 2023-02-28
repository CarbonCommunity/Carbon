using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Carbon.Hooks;

namespace Carbon.Contracts
{
	public interface IHookManagerPublic
	{	
		int PatchesCount { get; }
		int StaticHooksCount { get; }
		int DynamicHooksCount { get; }

		IEnumerable<HookEx> Patches { get; }
		IEnumerable<HookEx> StaticHooks { get; }
		IEnumerable<HookEx> DynamicHooks { get; }

		IEnumerable<HookEx> InstalledPatches { get; }
		IEnumerable<HookEx> InstalledStaticHooks { get; }
		IEnumerable<HookEx> InstalledDynamicHooks { get; }

		void Subscribe(string hookName, string fileName);
		void Unsubscribe(string hookName, string fileName);

		bool IsHookLoaded(string hookName);
		int GetHookSubscriberCount(string identifier);
	}
}
