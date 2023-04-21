using System;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace API.Assembly;

public class CarbonAddon
{
	public string File { get; set; }
	public ICarbonAddon Addon { get; set; }
}
