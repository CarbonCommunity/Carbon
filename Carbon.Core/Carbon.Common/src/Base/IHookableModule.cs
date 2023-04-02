using System;
using System.Collections.Generic;
using System.Reflection;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Base.Interfaces;

public interface IHookableModule
{
	Dictionary<string, List<BaseHookable.CachedHook>> HookCache { get; }
}
