namespace Carbon.Components;

/// <summary>
/// Carbon-specific anonymously gathered analytics designed to help and inform us about Carbon features and their uses.
/// </summary>
public partial struct Analytics
{
	/// <summary>
	/// Metrics buffer. Gets cleared up every time it's being sent and reused.
	/// </summary>
	public static readonly Dictionary<string, object> Metrics = new();

	public static bool Enabled => Community.Runtime.Analytics.Enabled;

	public static Analytics Singleton = default;

	/// <summary>
	/// Records a metric with an ID and its respective value.
	/// </summary>
	/// <param name="key">Metric ID.</param>
	/// <param name="value">Metric value.</param>
	/// <returns></returns>
	public Analytics Include(string key, object value)
	{
		Metrics[key] = value;
		return this;
	}

	/// <summary>
	///
	/// </summary>
	/// <param name="eventName"></param>
	/// <returns></returns>
	public Analytics Submit(string eventName)
	{
		Community.Runtime.Analytics.LogEvents(eventName);
		Dispose();
		return this;
	}

	/// <summary>
	/// Current metric cleanup.
	/// </summary>
	public static void Dispose()
	{
		Metrics.Clear();
	}
}
