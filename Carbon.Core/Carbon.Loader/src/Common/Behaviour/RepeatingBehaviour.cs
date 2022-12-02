///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
///
using System;

namespace Carbon.LoaderEx.Common;

internal abstract class RepeatingBehaviour : CarbonBehaviour, IDisposable
{
	internal float tickRate = 10f;

	internal virtual void Awake()
	{
		Initialize();
	}

	public abstract void Initialize();

	internal virtual void OnEnable()
	{
		InvokeRepeating(Do, tickRate, tickRate);
	}

	internal abstract void Do();

	internal virtual void OnDisable()
	{
		CancelInvoke(Do);
	}

	internal virtual void OnDestroy()
	{
		CancelInvoke(Do);
		Dispose();
	}

	public abstract void Dispose();
}
