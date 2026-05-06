using Carbon.Test;
using Facepunch;

namespace Carbon.Core;

public partial class CorePlugin
{
	[ConsoleCommand("devdump", "Creates a zip package in the temporary directory of the Carbon folder with useful information (output log & profile snapshot). Syntax: c.devdump [logfile] [duration]")]
	[AuthLevel(2)]
	private void DevDumpSnapshot(ConsoleSystem.Arg arg)
	{
		var file = Path.Combine(Defines.GetTempFolder(), $"devdump-{RandomEx.GetRandomString(5)}.zip");
		var dump = Pool.Get<DevDump>();

		var includeLog = arg.GetBool(0, true);
		var duration = arg.GetFloat(arg.HasArgs(2) ? 1 : 0, 5).Clamp(1f, 20);
		dump.Init(includeLog);
		if (duration > 0)
		{
			dump.Export(duration, file, () =>
			{
				Logger.Log($"Exported developer dump at '{file}' with a {duration} seconds profile recording.");
				Pool.Free(ref dump);
			});
		}
		else
		{
			dump.Export(file);
			Pool.Free(ref dump);
			arg.ReplyWith($"Exported developer dump at '{file}'");
		}
	}
}
