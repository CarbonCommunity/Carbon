using System.Text;
using Facepunch;

namespace Carbon.Core;

public partial class CorePlugin
{
	public bool ClientEnabledCheck()
	{
		if (!Community.Runtime.ClientConfig.Enabled)
		{
			Logger.Warn($" Carbon Client is not enabled. You may not execute that command.");
			return false;
		}

		return true;
	}

	[ConsoleCommand("client.loadconfig", "Loads Carbon Client config from file.")]
	[AuthLevel(2)]
	private void ClientLoadConfig(ConsoleSystem.Arg arg)
	{
		if (Community.Runtime == null) return;

		Community.Runtime.LoadClientConfig();

		arg.ReplyWith("Loaded Carbon Client config.");
	}

	[ConsoleCommand("client.saveconfig", "Saves Carbon Client config to file.")]
	[AuthLevel(2)]
	private void ClientSaveConfig(ConsoleSystem.Arg arg)
	{
		if (Community.Runtime == null) return;

		Community.Runtime.SaveClientConfig();

		arg.ReplyWith("Saved Carbon Client config.");
	}

	[CommandVar("client.enabled", "Enable this if the server is Carbon client-enabled server. [Only applies on server restart]")]
	[AuthLevel(2)]
	private bool ClientEnabled
	{
		get { return Community.Runtime.ClientConfig.Enabled; }
		set
		{
			Community.Runtime.ClientConfig.Enabled = value;
			Community.Runtime.SaveClientConfig();

			if (value)
			{
				ConVar.Server.secure = false;
			}
		}
	}

	[ConsoleCommand("client.addons", "Prints a list of the currently available addons.")]
	[AuthLevel(2)]
	private void ClientAddons(ConsoleSystem.Arg args)
	{
		var builder = Pool.Get<StringBuilder>();

		builder.AppendLine($"Client Addons ({Community.Runtime.ClientConfig.Addons.Count:n0})");

		foreach (var addon in Community.Runtime.ClientConfig.Addons)
		{
			builder.AppendLine($" {addon.Url} [{(addon.Enabled ? "ENABLED" : "DISABLED")}]");
		}

		args.ReplyWith(builder.ToString());

		Pool.FreeUnmanaged(ref builder);
	}

	[ConsoleCommand("client.stats", "Prints a list of useful statistic information of Carbon Client performance.")]
	[AuthLevel(2)]
	private void ClientStats(ConsoleSystem.Arg args)
	{
		using var table = new StringTable("Addons", "Assets", "Spawnable Prefabs", "Prefabs (Custom)", "Prefabs (Rust)", "Entities");

		table.AddRow(
			Community.Runtime.CarbonClient.AddonCount.ToString("n0"),
			Community.Runtime.CarbonClient.AssetCount.ToString("n0"),
			Community.Runtime.CarbonClient.SpawnablePrefabsCount.ToString("n0"),
			Community.Runtime.CarbonClient.PrefabsCount.ToString("n0"),
			Community.Runtime.CarbonClient.RustPrefabsCount.ToString("n0"),
			Community.Runtime.CarbonClient.EntityCount.ToString("n0"));

		args.ReplyWith(table.ToStringMinimal());
	}

	[ConsoleCommand("client.addons_reinstall", "Unloads all currently loaded addons then reloads them relative to the config changes. (Options: async [bool|true])")]
	[AuthLevel(2)]
	private void ClientAddonsReinstall(ConsoleSystem.Arg args)
	{
		if (!ClientEnabledCheck())
		{
			return;
		}

		ReloadCarbonClientAddons(args.GetBool(0, true));
	}

	[ConsoleCommand("client.addons_uninstall", "Unloads all currently loaded addons then reloads them relative to the config changes.")]
	[AuthLevel(2)]
	private void ClientAddonsUninstall(ConsoleSystem.Arg args)
	{
		if (!ClientEnabledCheck())
		{
			return;
		}

		Community.Runtime.CarbonClient.UninstallAddons();
	}

	public static void ReloadCarbonClientAddons(bool async = false)
	{
		Community.Runtime.CarbonClient.UninstallAddons();

		if (async)
		{
			Community.Runtime.CarbonClient.InstallAddonsAsync(Community.Runtime.ClientConfig.AddonCache);
		}
		else
		{
			Community.Runtime.CarbonClient.InstallAddons(Community.Runtime.ClientConfig.AddonCache);
		}
	}
}
