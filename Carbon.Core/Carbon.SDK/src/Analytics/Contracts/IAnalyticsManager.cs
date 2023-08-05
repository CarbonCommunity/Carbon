using System.Collections.Generic;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace API.Analytics;

public interface IAnalyticsManager
{
	public Dictionary<string, object> Segments { get; set; }

	public string Branch { get; }
	public string ClientID { get; }
	public string InformationalVersion { get; }
	public string Platform { get; }
	public string Protocol { get; }
	public string SessionID { get; }
	public string SystemID { get; }
	public string UserAgent { get; }
	public string Version { get; }

	public void SessionStart();
	public void LogEvent(string eventName);
	public void LogEvent(string eventName, IDictionary<string, object> segments = null, IDictionary<string, object> metrics = null);
}
