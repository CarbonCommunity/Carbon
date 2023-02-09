using System;
using System.Collections.Generic;
using API.Contracts;
using API.Events;
using UnityEngine;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Components;

internal sealed class EventManager : MonoBehaviour, IEventManager
{
	private readonly Dictionary<CarbonEvent, Delegate> events
		= new Dictionary<CarbonEvent, Delegate>();

	internal EventManager()
	{
		Utility.Logger.Log($"{this} ctor");
	}

	private void Awake()
	{
		Utility.Logger.Log($"{this} awake");
	}

	private void Update()
	{
	}

	public void Subscribe(CarbonEvent eventId, Action<EventArgs> callback)
	{
		if (!events.ContainsKey(eventId)) events[eventId] = callback;
		else events[eventId] = Delegate.Combine(events[eventId], callback);
		Utility.Logger.Debug($"[{eventId}] New subscriptor '{callback}'");
	}

	public void Trigger(CarbonEvent eventId, EventArgs args)
	{
		if (!events.ContainsKey(eventId)) return;
		Utility.Logger.Debug($"[{eventId}] triggered");

		Action<EventArgs> @event = events[eventId] as Action<EventArgs>;
		@event?.Invoke(args);
	}

	public void Unsubscribe(CarbonEvent eventId, Action<EventArgs> callback)
	{
		if (!events.ContainsKey(eventId)) return;
		events[eventId] = Delegate.Remove(events[eventId], callback);
		Utility.Logger.Debug($"[{eventId}] Remove subscription '{callback}'");
	}
}
