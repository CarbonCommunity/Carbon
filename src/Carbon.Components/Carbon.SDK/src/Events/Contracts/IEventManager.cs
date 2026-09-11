using System;

namespace API.Events;

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

	/// <summary>
	/// Reset an event.
	/// </summary>
	public void Reset(CarbonEvent eventId);
}
