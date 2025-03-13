namespace Oxide.Core.Plugins;

/// <summary>
/// Granting this attribute to a sub-class in a plugin/module, will mark said HarmonyPatch
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class AutoPatchAttribute : Attribute
{
	/// <summary>
	/// If the patch fails, the plugin will become unloaded as this is an important patch.
	/// </summary>
	public bool IsRequired;

	/// <summary>
	/// The order which the patch should apply.
	/// </summary>
	public Orders Order;

	/// <summary>
	/// The local or static callback method name within the plugin which gets called when the patch is successful.
	/// </summary>
	public string PatchSuccessCallback;

	/// <summary>
	/// The local or static callback method name within the plugin which gets called when the patch is failed.
	/// </summary>
	public string PatchFailureCallback;

	public enum Orders
	{
		/// <summary>
		/// Execute patching as soon as the plugin runs Init().
		/// </summary>
		AfterPluginInit,

		/// <summary>
		/// Execute patching as soon as the plugin loads the config.
		/// </summary>
		AfterPluginLoad,

		/// <summary>
		/// Execute patching as soon as OnServerInitialized gets fired.
		/// </summary>
		AfterOnServerInitialized,

		/// <summary>
		/// Manual execution of all patches marked as Delayed.
		/// </summary>
		Delayed
	}
}
