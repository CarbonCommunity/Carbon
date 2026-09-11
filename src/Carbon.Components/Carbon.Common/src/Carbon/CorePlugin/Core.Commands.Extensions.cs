namespace Carbon.Core;

public partial class CorePlugin
{
	[ConsoleCommand("extensions", "Prints a list of all currently loaded extensions.")]
	[AuthLevel(2)]
	private void Extensions(ConsoleSystem.Arg arg)
	{
		using var body = new StringTable("#", "extension", "type");
		var count = 1;

		foreach (var mod in Community.Runtime.AssemblyEx.Extensions.Loaded)
		{
			body.AddRow($"{count:n0}", Path.GetFileNameWithoutExtension(mod.Value.Key), mod.Key.FullName);
			count++;
		}

		arg.ReplyWith(body.Write(StringTable.FormatTypes.None));
	}
}
