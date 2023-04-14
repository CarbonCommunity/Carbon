using System;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace API.Commands;

[Flags]
public enum CommandFlags
{
	None,
	Hidden,
	Protected
}
