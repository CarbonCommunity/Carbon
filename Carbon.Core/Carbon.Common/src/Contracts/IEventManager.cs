using System;
using API.Events;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace API.Contracts;

public interface IEventManager
{
	public void Subscribe(CarbonEvent eventId, Action<EventArgs> callback);
	public void Trigger(CarbonEvent eventId, EventArgs args);
	public void Unsubscribe(CarbonEvent eventId, Action<EventArgs> callback);
}