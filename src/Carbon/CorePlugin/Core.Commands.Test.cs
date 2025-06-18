using Carbon.Test;

namespace Carbon.Core;

public partial class CorePlugin
{
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
		var delay = arg.GetFloat(0, 0.1f);

		if (delay < 0)
		{
			arg.ReplyWith("Delay must be above zero.");
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
