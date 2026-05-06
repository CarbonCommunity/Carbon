using System;
using System.Collections.Generic;

namespace API.Hooks
{
	public interface IPatchManager
	{
		IEnumerable<IHook> LoadedPatches { get; }
		IEnumerable<IHook> LoadedStaticHooks { get; }
		IEnumerable<IHook> LoadedDynamicHooks { get; }

		IEnumerable<IHook> InstalledPatches { get; }
		IEnumerable<IHook> InstalledStaticHooks { get; }
		IEnumerable<IHook> InstalledDynamicHooks { get; }

		void ForceUpdateHooks();
		void Fetch();

		void Subscribe(string hookName, string fileName);
		void Unsubscribe(string hookName, string fileName);
		void UnsubscribeAll(string hookName);

		bool IsHook(string hookName);
		bool IsHookLoaded(string hookName);
		int GetHookSubscriberCount(string hookName);
		IEnumerable<string> GetHookSubscribers(string hookName);
		bool AnySubscribers(string hookName);

		public void LoadHooksFromType(Type type);
	}
}
