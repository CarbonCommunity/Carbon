using System;
using System.Collections.Generic;
using Carbon.Base;
using Carbon.Contracts;
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

	internal List<Action> _onFrameQueueBuffer = new();

	public override void Start() { }
	public override void OnDestroy() { }
	public override void Dispose() { }

	public Queue<Action> OnFrameQueue { get; set; } = new Queue<Action>();

	public void Update()
	{
		if (OnFrameQueue.Count <= 0) return;

		_onFrameQueueBuffer.AddRange(OnFrameQueue);

		foreach (var callback in _onFrameQueueBuffer)
		{
			try
			{
				callback?.Invoke();
			}
			catch (Exception exception)
			{
				Logger.Error($"Failed to execute OnFrame callback ({exception.Message})\n{exception.StackTrace}");
			}
		}

		_onFrameQueueBuffer.Clear();
		OnFrameQueue.Clear();
	}
}
