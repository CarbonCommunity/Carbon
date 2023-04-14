using System;
using API.Events;
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
		GameObject gameObject = GameObject.Find("Carbon");

		return gameObject == null
			? throw new Exception("Carbon GameObject not found")
			: gameObject.GetComponent<IEventManager>();
	});

	protected static IEventManager Events { get => _events.Value; }
}
