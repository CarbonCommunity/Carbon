using System;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace API.Assembly;

public interface IAddonCache
{
	public string File { get; }
	public ICarbonAddon Addon { get; }
}
