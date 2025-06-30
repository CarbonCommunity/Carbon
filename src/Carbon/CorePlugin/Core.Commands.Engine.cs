using System.Text;
using Facepunch;

namespace Carbon.Core;

public partial class CorePlugin
{
	[ConsoleCommand("shutdown", "Completely unloads Carbon from the game, rendering it fully vanilla. WARNING: This is for testing purposes only.")]
	[AuthLevel(2)]
	private void Shutdown(ConsoleSystem.Arg arg)
	{
		Community.Runtime.Uninitialize();
	}

	[ConsoleCommand("help", "Returns a brief introduction to Carbon.")]
	[AuthLevel(2)]
	private void Help(ConsoleSystem.Arg arg)
	{
		arg.ReplyWith($"To get started, run the `c.find c.` to list all Carbon commands.\n" +
		              $"To list all currently loaded plugins, execute `c.plugins`.\n" +
		              $"For more information, please visit https://carbonmod.gg or join the Discord server at https://discord.gg/carbonmod\n" +
		              $"You're currently running {Community.Runtime.Analytics.Version}.");
	}

	private void HarmonyMods(ConsoleSystem.Arg arg)
	{
		using var table = new StringTable("name", "hooks", "methods");
		foreach (var mod in HarmonyLoader.loadedMods)
		{
			var harmonyInstance = mod.Harmony.harmonyObject as HarmonyLib.Harmony;
			table.AddRow(mod.Name, mod.Hooks.Count.ToString("n0"), harmonyInstance.GetPatchedMethods().Count().ToString("n0"));
		}
		arg.ReplyWith(table.ToStringMinimal());
	}

	private void SayAs(ConsoleSystem.Arg arg)
	{
		if (!arg.HasArgs(4))
		{
			arg.ReplyWith("Syntax: sayas \"<username>\" \"<steamid>\" \"<color>\" \"<message>\"");
			return;
		}
		var username = arg.GetString(0, "SERVER");
		var steamId = arg.GetULong(1);
		var color = arg.GetString(2, "#eee");
		var message = arg.GetString(3);
		ConVar.Chat.Broadcast(message, username, color, steamId);
	}

	[ConsoleCommand("version", "Version information of the Carbon build and Rust.")]
	private void VersionCall(ConsoleSystem.Arg arg)
	{
		var analytics = Community.Runtime.Analytics;

		if (arg.IsServerside)
		{
			arg.ReplyWith($"Carbon" +
#if MINIMAL
					$" Minimal" +
#endif
				$" {analytics.Version}/{analytics.Platform}/{analytics.Protocol} [{Build.Git.Branch}] [{Build.Git.Tag}] on Rust {BuildInfo.Current.Build.Number}/{Rust.Protocol.printable} ({BuildInfo.Current.BuildDate})");
		}
		else
		{
			arg.ReplyWith($"Carbon" +
#if MINIMAL
					$" Minimal" +
#endif
				$" <color=#d14419>{analytics.Version}/{analytics.Platform}/{analytics.Protocol}</color> [{Build.Git.Branch}] [{Build.Git.Tag}] on Rust <color=#d14419>{BuildInfo.Current.Build.Number}/{Rust.Protocol.printable}</color> ({BuildInfo.Current.BuildDate}).");

		}
	}

	[ConsoleCommand("build", "Information about the currently running Carbon build.")]
	[AuthLevel(2)]
	private void BuildCall(ConsoleSystem.Arg arg)
	{
		arg.ReplyWith(Community.Runtime.Analytics.InformationalVersion);
	}

	[ConsoleCommand("protocol", "Protocol information used by the hook system of the Carbon build.")]
	[AuthLevel(2)]
	private void Protocol(ConsoleSystem.Arg arg)
	{
		arg.ReplyWith(Community.Runtime.Analytics.Protocol);
	}

	[ConsoleCommand("commit", "Information about the Git commit of this build.")]
	[AuthLevel(2)]
	private void Commit(ConsoleSystem.Arg arg)
	{
		var builder = Pool.Get<StringBuilder>();
		var added = Build.Git.Changes.Count(x => x.Type == Build.Git.AssetChange.ChangeTypes.Added);
		var modified = Build.Git.Changes.Count(x => x.Type == Build.Git.AssetChange.ChangeTypes.Modified);
		var deleted = Build.Git.Changes.Count(x => x.Type == Build.Git.AssetChange.ChangeTypes.Deleted);

		builder.AppendLine($"  Branch:  {Build.Git.Branch}");
		builder.AppendLine($"  Author:  {Build.Git.Author}");
		builder.AppendLine($" Comment:  {Build.Git.Comment}");
		builder.AppendLine($"    Date:  {Build.Git.Date}");
		builder.AppendLine($"     Tag:  {Build.Git.Tag}");
		builder.AppendLine($"    Hash:  {Build.Git.HashShort} ({Build.Git.HashLong})");
		builder.AppendLine($"     Url:  {Build.Git.Url}");
		builder.AppendLine($"   Debug:  {Build.IsDebug}");
		builder.AppendLine($" Changes:  {added} added, {modified} modified, {deleted} deleted");

		arg.ReplyWith(builder.ToString());
		Pool.FreeUnmanaged(ref builder);
	}

	[ConsoleCommand("whymodded", "Prints an intricate list of all the reasons why the server is set to modded and solutions to fix it.")]
	[AuthLevel(2)]
	private void WhyModded(ConsoleSystem.Arg arg)
	{
		using var table = new StringTable("reason", "type", "quick fix");

		if (Community.Runtime.Config.IsModded)
		{
			table.AddRow($"IsModded option is true", "Config", "c.modding 0");
		}

		foreach (var hookable in Community.Runtime.ModuleProcessor.Modules)
		{
			if (hookable is BaseModule { ForceModded: true } module && module.IsEnabled())
			{
				table.AddRow($"{module.Name} is enabled", "Module", $"c.setmodule \"{module.Name}\" 0");
			}
		}

		foreach (var auto in CarbonAuto.AutoCache.Where(auto => auto.Value.Variable.ForceModded && auto.Value.IsChanged()))
		{
			table.AddRow($"'{auto.Value.Variable.DisplayName}' is changed", "Carbon Auto", $"{auto.Key} -1");
		}

		arg.ReplyWith($"{table.ToStringMinimal()}\nTo apply all the changes necessary to be listed under the Community section, run 'c.gocommunity'.");
	}

	[ConsoleCommand("gocommunity", "Executes a variety of changes necessary to set the server viable for the Community section. Run 'c.whymodded' to see what will be changed.")]
	[AuthLevel(2)]
	private void GoCommunity(ConsoleSystem.Arg arg)
	{
		var changes = 0;

		if (Community.Runtime.Config.IsModded)
		{
			Community.Runtime.Config.IsModded = false;
			changes++;
		}

		foreach (var hookable in Community.Runtime.ModuleProcessor.Modules)
		{
			if (hookable is BaseModule { ForceModded: true } module && module.IsEnabled())
			{
				module.SetEnabled(false);
				changes++;
			}
		}

		foreach (var auto in CarbonAuto.AutoCache.Where(auto => auto.Value.Variable.ForceModded && auto.Value.IsChanged()))
		{
			auto.Value.SetValue(-1);
			changes++;
		}

		arg.ReplyWith($"Applied {changes:n0} {changes.Plural("change", "changes")} to ensure that the server is no longer modded and fit for the Community tab.");
	}

	[CommandVar("lang", "Current server language for Carbon and plugins loaded.")]
	[AuthLevel(2)]
	private string Lang
	{
		get => lang.GetServerLanguage();
		set => lang.SetServerLanguage(value);
	}
}
