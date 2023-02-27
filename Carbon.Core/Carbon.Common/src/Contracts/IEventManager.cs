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
	/// <summary>
	/// Subscribe to an event.
	/// </summary>
	public void Subscribe(CarbonEvent eventId, Action<EventArgs> callback);

	/// <summary>
	/// Trigger an event.
	/// </summary>
	public void Trigger(CarbonEvent eventId, EventArgs args);

	/// <summary>
	/// Unsubscribe from an event.
	/// </summary>
	public void Unsubscribe(CarbonEvent eventId, Action<EventArgs> callback);
}