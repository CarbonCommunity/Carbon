using System;

namespace API.Assembly;

public interface ICarbonComponent : ICarbonAddon
{
	public void OnEnable(EventArgs args);
	public void OnDisable(EventArgs args);
}
