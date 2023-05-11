using System;
using System.Collections.Generic;
using API.Abstracts;
using API.Events;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Components;

internal sealed class EventManager : CarbonBehaviour, IEventManager
{
	private readonly Dictionary<CarbonEvent, Delegate> events = new();

	public void Subscribe(CarbonEvent eventId, Action<EventArgs> callback)
	{
		if (!events.ContainsKey(eventId)) events[eventId] = callback;
		else events[eventId] = Delegate.Combine(events[eventId], callback);
		Utility.Logger.Debug($"[{eventId}] New subscriptor '{callback.Target}' ('{callback.Method}')");
	}

	public void Trigger(CarbonEvent eventId, EventArgs args)
	{
#if DEBUG
		CarbonEventArgs parsed = args as CarbonEventArgs;
		string payload = (args == EventArgs.Empty) ? "empty payload" : $"{parsed.Payload}";
		Utility.Logger.Debug($"[{eventId}] {payload}");
#endif
		if (!events.ContainsKey(eventId)) return;
		Action<EventArgs> @event = events[eventId] as Action<EventArgs>;
		@event?.Invoke(args);
	}

	public void Unsubscribe(CarbonEvent eventId, Action<EventArgs> callback)
	{
		if (!events.ContainsKey(eventId)) return;
		events[eventId] = Delegate.Remove(events[eventId], callback);
		Utility.Logger.Debug($"[{eventId}] Remove subscription '{callback.Target}'");
	}
}
