using System;
using API.Analytics;
using API.Assembly;
using API.Events;
using API.Hooks;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace API.Contracts;

public interface ICarbonInterface
{
	public IAnalyticsManager Analytics { get; }
	public IAssemblyManager Assembly { get; }
	public IDownloadManager Downloader { get; }
	public IEventManager Events { get; }
	public IPatchManager HookManager { get; }
}
