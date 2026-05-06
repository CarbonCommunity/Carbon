namespace Carbon;

public partial class Community
{
	public CorePlugin Core { get; set; }
	public ModLoader.Package Plugins { get; set; }
	public ModLoader.Package ZipPlugins { get; set; }

	public virtual void ReloadPlugins(IEnumerable<string> except = null)
	{
		ModLoader.IsBatchComplete = false;
		ModLoader.ClearAllErrored();
		ModLoader.ClearAllRequirees();
	}

	public void ClearPlugins(bool all = false)
	{
		Runtime.ClearCommands(all);
		ModLoader.UnloadCarbonMods();
	}
}
