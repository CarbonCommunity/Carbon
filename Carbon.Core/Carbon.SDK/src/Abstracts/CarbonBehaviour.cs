using System;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace API.Abstracts;

public abstract class CarbonBehaviour : UnityEngine.MonoBehaviour
{
	public CarbonBehaviour()
	{
		// we can't use logger here until we migrate it to a component.
		Console.WriteLine($"A new instance of '{this}' created");
	}
}
