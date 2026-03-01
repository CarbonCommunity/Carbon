using System.Collections.Generic;

namespace API.Analytics;

public interface IAnalyticsManager
{
	public Dictionary<string, object> Segments { get; set; }

	public bool Enabled { get; }
	public string Branch { get; }
	public string ClientID { get; }
	public string InformationalVersion { get; }
	public string Platform { get; }
	public string Protocol { get; }
	public string SessionID { get; }
	public string SystemID { get; }
	public string UserAgent { get; }
	public string Version { get; }
    public bool HasNewIdentifier { get; }

    public void SessionStart();
	public void LogEvent(string eventName);
	public void LogEvents(string eventName);
}
