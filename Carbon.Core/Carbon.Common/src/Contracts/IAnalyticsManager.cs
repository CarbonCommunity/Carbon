using System;
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
	public string ClientID { get; }
	public void StartSession();
	public void LogEvent(string eventName, IDictionary<string, object> parameters);
}