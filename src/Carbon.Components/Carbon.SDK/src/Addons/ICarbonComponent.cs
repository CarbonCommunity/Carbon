using System;

namespace Carbon;

/// <summary>
/// Core components shipped with Carbon, and packages in carbon/managed/packages.
/// </summary>
public interface ICarbonComponent : ICarbonAddon
{
	public void OnEnable(EventArgs args);
	public void OnDisable(EventArgs args);
}
