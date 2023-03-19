using System;
using System.Collections;
using System.Collections.Generic;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace API.Contracts;

public interface IAnalyticsManager
{
	public string Platform { get; }
	public string Branch { get; }
	public string Version { get; }
	public string InformationalVersion { get; }


	public string ClientID { get; }
	public string SessionID { get; }


	public void Keepalive();
	public void StartSession();
	public void LogEvent(string eventName);
	public void LogEvent(string eventName, IDictionary<string, object> parameters);
}