namespace API.Assembly;

public interface IExtensionManager : IAddonManager
{
	public enum ExtensionTypes
	{
		Default,
		Extension,
		HarmonyMod
	}
}
