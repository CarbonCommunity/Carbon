using System;

namespace Carbon;

/// <summary>
/// Base contract of every Carbon addon assembly (components, extensions, module packages).
/// </summary>
public interface ICarbonAddon
{
	public void Awake(EventArgs args);
	public void OnLoaded(EventArgs args);
	public void OnUnloaded(EventArgs args);
}
