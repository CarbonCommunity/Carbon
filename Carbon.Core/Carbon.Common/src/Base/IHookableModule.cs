using System.Collections.Generic;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Base.Interfaces;

public interface IHookableModule
{
	Dictionary<uint, List<BaseHookable.CachedHook>> HookCache { get; }
}
