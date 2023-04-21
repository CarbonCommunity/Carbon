using System;
using System.Collections.Generic;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace API.Assembly;

public interface IAddonCache
{
	public ICarbonAddon Addon { get; }
	public IReadOnlyList<Type> Types { get; }
	public string File { get; }
}
