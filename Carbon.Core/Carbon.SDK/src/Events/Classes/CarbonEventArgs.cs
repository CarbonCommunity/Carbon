using System;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace API.Events;

public class CarbonEventArgs : EventArgs
{
	public object Payload { get; }

	public CarbonEventArgs(object payload)
	{
		Payload = payload;
	}
}
