using System;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace API.Hooks;

[Flags]
public enum HookFlags { None = 0, Static = 1, Patch = 2, Hidden = 4, IgnoreChecksum = 8 }
