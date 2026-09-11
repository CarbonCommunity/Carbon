using System;
using API.Events;
using UnityEngine;

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
