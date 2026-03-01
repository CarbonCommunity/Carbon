using Carbon.Test;
using Facepunch;

namespace Carbon.Core;

public partial class CorePlugin
{
	[ConsoleCommand("test_plugin", "Executes a collection of tests found inside of the plugin, designed to ensure plugin logic integrity. Eg. c.test_plugin <plugin_name> [<channel|1>] [<delay|0.1>]")]
	[AuthLevel(2)]
	private void test_plugin(ConsoleSystem.Arg arg)
	{
		if (!arg.HasArgs())
		{
			arg.ReplyWith($"Syntax: c.test_plugin <plugin_name> [<channel|1>] [<delay|0.1>]");
			return;
		}

		var pluginName = arg.GetString(0);
		var channel = arg.GetInt(1, Integrations.DEFAULT_CHANNEL);
		var delay = arg.GetFloat(2, 0.1f);

		Integrations.Clear(channel);
		var plugin = ModLoader.Packages.FindPlugin(pluginName);
		if (plugin == null)
		{
			arg.ReplyWith($"Couldn't find that plugin");
			return;
		}

		plugin.CollectTests();
		plugin.NextFrame(() => Integrations.Run(delay, channel));
		arg.ReplyWith($"Collected {plugin.TestCount:n0} {plugin.TestCount.Plural("test", "tests")} for '{plugin}' and now running..");
	}

	[ConsoleCommand("test_collect", "Collects all available tests from all plugins and enabled modules currently loaded. Eg. c.test_collect [<channel|1>]")]
	[AuthLevel(2)]
	private void test_collect(ConsoleSystem.Arg arg)
	{
		var channel = arg.GetInt(0, Integrations.DEFAULT_CHANNEL);
		using var plugins = Pool.Get<PooledList<RustPlugin>>();
		ModLoader.Packages.GetAllHookables(plugins, true);

		var moduleTests = 0;
		var pluginTests = 0;
		foreach (var module in Community.Runtime.ModuleProcessor.Modules)
		{
			module.CollectTests();
			moduleTests += module.TestCount;
		}

		foreach (var plugin in plugins)
		{
			plugin.CollectTests(channel);
			pluginTests += plugin.TestCount;
		}

		arg.ReplyWith($"Collected {moduleTests:n0} module and {pluginTests:n0} plugin {(moduleTests + pluginTests).Plural("test", "tests")}. Run c.test_beds to display all or c.test_run to execute.");
	}

	[ConsoleCommand("test_beds", "Prints all currently queued up tests ready to be executed.")]
	[AuthLevel(2)]
	private void test_beds(ConsoleSystem.Arg arg)
	{
		using var table = new StringTable(string.Empty, "context", "tests", "channel");

		foreach (var bed in Integrations.Banks.Values.SelectMany(bank => bank))
		{
			table.AddRow(string.Empty, bed.Context, $"{bed.Count:n0}", bed.Channel.ToString());
		}

		arg.ReplyWith(table.ToStringMinimal());
	}

	[ConsoleCommand("test_run", "Executes all Test Beds that are currently queued up. Eg. c.test_run <channel|-1> [<delay|0.1>]")]
	[AuthLevel(2)]
	private void test_run(ConsoleSystem.Arg arg)
	{
		if (!arg.HasArgs(1))
		{
			arg.ReplyWith($"Syntax: c.test_run <channel|-1> [<delay|0.1>]");
			return;
		}

		var channel = arg.GetInt(0, -1);
		var delay = arg.GetFloat(1, 0.1f);

		if (delay < 0)
		{
			arg.ReplyWith("Delay must be above or equal to zero.");
			return;
		}

		Integrations.Run(delay, channel);
	}

	[ConsoleCommand("test_clear", "Clears all Test Beds that are currently queued up. Eg. c.test_clear [<channel|-1>]")]
	[AuthLevel(2)]
	private void test_clear(ConsoleSystem.Arg arg)
	{
		Integrations.Clear(arg.GetInt(0, -1));
	}
}
