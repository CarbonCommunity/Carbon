using System;
using System.Collections.Generic;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

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

		void Subscribe(string hookName, string fileName);
		void Unsubscribe(string hookName, string fileName);

		bool IsHookLoaded(string hookName);
		int GetHookSubscriberCount(string identifier);

		public void LoadHooksFromType(Type type);
	}
}
