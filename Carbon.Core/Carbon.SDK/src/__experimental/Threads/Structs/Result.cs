using System.Collections.Generic;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace API.Threads;

public struct Result
{
	public List<Problem> Diagnostics { get; set; }
	public object Output { get; set; }
}
