using System;

namespace API.Assembly;

public interface ICarbonAddon
{
	public void Awake(EventArgs args);
	public void OnLoaded(EventArgs args);
	public void OnUnloaded(EventArgs args);
}
