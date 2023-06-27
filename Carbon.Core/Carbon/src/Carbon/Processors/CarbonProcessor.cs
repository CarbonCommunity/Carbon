using System;
using System.Collections.Generic;
using Carbon.Base;
using Carbon.Contracts;
using Carbon.Extensions;
using Facepunch;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Managers;

public class CarbonProcessor : BaseProcessor, ICarbonProcessor
{
	public override string Name => "Carbon Processor";

	public override void Start() { }
	public override void OnDestroy() { }
	public override void Dispose() { }

	public Queue<Action> OnFrameQueue { get; set; } = new Queue<Action>();

	public void FixedUpdate()
	{
		if (!Community.IsServerFullyInitializedCache || OnFrameQueue.Count <= 0) return;

		var count = OnFrameQueue.Count;

		for (int i = 0; i < count.Clamp(0, Community.Runtime.Config.FrameTickBufferSize); i++)
		{
			try
			{
				OnFrameQueue.Dequeue()?.Invoke();
			}
			catch (Exception exception)
			{
				Logger.Error($"Failed to execute OnFrame callback ({exception.Message})\n{exception.StackTrace}");
			}
		}

		 Logger.Debug($"Batch frame queue triggered {count:n0} callbacks", 5);
	}
}
