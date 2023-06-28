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

	public List<Action> CurrentFrameQueue { get; set; } = new();
	public List<Action> PreviousFrameQueue { get; set; } = new();
	public object CurrentFrameLock { get; set; } = new();

	public void Update()
	{
		if (CurrentFrameQueue.Count <= 0) return;

		var lockObject = CurrentFrameLock;
		var queueList = (List<Action>)null;

		lock (lockObject)
		{
			queueList = CurrentFrameQueue;
			CurrentFrameQueue = PreviousFrameQueue;
			PreviousFrameQueue = queueList;
		}

		for (int i = 0; i < queueList.Count; i++)
		{
			try
			{
				queueList[i]();
			}
			catch (Exception exception)
			{
				Logger.Error($"Failed to execute OnFrame callback ({exception.Message})\n{exception.StackTrace}");
			}
		}

		queueList.Clear();
	}
}
