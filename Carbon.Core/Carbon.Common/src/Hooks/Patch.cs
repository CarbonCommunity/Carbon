using System;
using API.Contracts;
using UnityEngine;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace API.Hooks;

public class Patch
{
	internal static readonly Lazy<IEventManager> _events = new(() =>
	{
		GameObject gameObject = GameObject.Find("Carbon")
			?? throw new Exception("Carbon GameObject not found");
		return gameObject?.GetComponent<IEventManager>();
	});

	protected static IEventManager Events { get => _events.Value; }
}
