///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
///
using System;
using Carbon.Interfaces;

namespace Carbon.Patterns;

internal abstract class RepeatingBehaviour : CarbonBehaviour, IBase, IDisposable
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