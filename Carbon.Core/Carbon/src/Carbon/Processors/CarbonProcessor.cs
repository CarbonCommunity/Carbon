using System;
using System.Collections.Generic;
using Carbon.Base;
using Carbon.Contracts;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Processors;

public class CarbonProcessor : BaseProcessor, ICarbonProcessor
{
	public override void Start() { }
	public override void OnDestroy() { }
	public override void Dispose() { }

	public Queue<Action> OnFrameQueue { get; set; } = new Queue<Action>();

	public void Update()
	{
		if (OnFrameQueue.Count <= 0) return;

		try
		{
			OnFrameQueue.Dequeue()?.Invoke();
		}
		catch (Exception exception)
		{
			Logger.Error($"Failed to execute OnFrame callback ({exception.Message})\n{exception.StackTrace}");
		}
	}
}
