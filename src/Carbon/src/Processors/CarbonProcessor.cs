using System;
using System.Collections.Generic;
using API.Commands;
using Carbon.Base;
using Carbon.Contracts;

namespace Carbon.Managers;

public class CarbonProcessor : BaseProcessor, ICarbonProcessor
{
	public override string Name => "Carbon Processor";

	public override void OnDestroy() { }
	public override void Dispose() { }

	public List<Action> CurrentFrameQueue { get; set; } = new();
	public List<Action> PreviousFrameQueue { get; set; } = new();
	public object CurrentFrameLock { get; set; } = new();

	public override void Start()
	{
		Community.Runtime.CommandManager.RegisterCommand(new Command.RCon
		{
			Name = "avgfps",
			Help = "Displays the server's average FPS.",
			Callback = arg =>
			{
				arg.ReplyWith($"{Performance.report.frameRateAverage:0}");
			}
		}, out _);
		Community.Runtime.CommandManager.RegisterCommand(new Command.ClientConsole
		{
			Name = "avgfps",
			Help = "Displays the server's average FPS.",
			Callback = arg =>
			{
				arg.ReplyWith($"{Performance.report.frameRateAverage:0}");
			},
			Auth = new Command.Authentication
			{
				AuthLevel = 2
			}
		}, out _);
	}
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
				Logger.Error($"Failed to execute OnFrame callback", exception.InnerException ?? exception);
			}
		}

		queueList.Clear();
	}
}
