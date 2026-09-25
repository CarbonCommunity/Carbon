using System;

namespace Carbon.Events;

/// <summary>
/// Pooled event payload.
/// </summary>
public class CarbonEventArgs : EventArgs, Facepunch.Pool.IPooled
{
	public object Payload;

	public CarbonEventArgs()
	{

	}

	public CarbonEventArgs(object payload)
	{
		Payload = payload;
	}

	public void Init(object payload)
	{
		Payload = payload;
	}

	public virtual void EnterPool()
	{
		Payload = null;
	}

	public virtual void LeavePool()
	{

	}
}
