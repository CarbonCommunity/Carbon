namespace Carbon.Core;

public partial class CorePlugin
{
	[ConsoleCommand("harmonyload", "Loads a mod from 'carbon/harmony'. The equivalent of Rust's `harmony.load` that's been stripped away under framework management.")]
	[AuthLevel(2)]
	private void HarmonyLoad(ConsoleSystem.Arg args)
	{
		var mod = args.GetString(0);

		if (string.IsNullOrEmpty(mod))
		{
			args.ReplyWith($"Please provide a HarmonyMod name.");
			return;
		}

		foreach (var ext in Community.Runtime.AssemblyEx.HarmonyMods.Loaded)
		{
			var folder = Path.GetDirectoryName(ext.Value.Key);
			var file = Path.GetFileNameWithoutExtension(ext.Value.Key);

			if (folder.Equals(Defines.GetHarmonyFolder(), StringComparison.InvariantCultureIgnoreCase) &&
			    file.Equals(mod, StringComparison.InvariantCultureIgnoreCase))
			{
				Community.Runtime.AssemblyEx.HarmonyMods.Unload(ext.Value.Key, "Command");
				Community.Runtime.AssemblyEx.HarmonyMods.Load(ext.Value.Key, "Command");
				return;
			}
		}

		Community.Runtime.AssemblyEx.HarmonyMods.Load(Path.Combine(Defines.GetHarmonyFolder(), $"{mod}.dll"), "Command");
	}

	[ConsoleCommand("harmonyunload", "Unloads a mod from 'carbon/harmony'. The equivalent of Rust's `harmony.unload` that's been stripped away under framework management.")]
	[AuthLevel(2)]
	private void HarmonyUnload(ConsoleSystem.Arg args)
	{
		var mod = args.GetString(0);

		if (string.IsNullOrEmpty(mod))
		{
			args.ReplyWith($"Please provide a HarmonyMod name.");
			return;
		}

		foreach (var ext in Community.Runtime.AssemblyEx.HarmonyMods.Loaded)
		{
			var folder = Path.GetDirectoryName(ext.Value.Key);
			var file = Path.GetFileNameWithoutExtension(ext.Value.Key);

			if (folder.Equals(Defines.GetHarmonyFolder(), StringComparison.InvariantCultureIgnoreCase) &&
			    file.Equals(mod, StringComparison.InvariantCultureIgnoreCase))
			{
				Community.Runtime.AssemblyEx.HarmonyMods.Unload(ext.Value.Key, "Command");
				break;
			}
		}
	}

	[ConsoleCommand("harmonymods", "Prints all currently loaded and processed HarmonyMods.")]
	[AuthLevel(2)]
	private void HarmonyMods(ConsoleSystem.Arg args)
	{
		using var table = new StringTable($"HarmonyMod ({Harmony.ModHooks.Count:n0})", "Version", "Assembly");

		foreach (var mod in Assemblies.Harmony)
		{
			var name = mod.Value.CurrentAssembly.GetName();
			table.AddRow(mod.Key, name.Version, name.Name);
		}

		args.ReplyWith($"{table.ToStringMinimal()}");
	}
}
