using System.Text;
using Newtonsoft.Json;

namespace Carbon.Documentation.Generators;

public static class Changelogs
{
	private static StringBuilder Builder = new();

	public static Structure.Changelog[] Logs;

	public static void Load()
	{
		Logs = JsonConvert.DeserializeObject<Structure.Changelog[]>(File.ReadAllText(Path.Combine(Generator.Arguments.Docs, "public", "rust", "changelogs.json")));
		Console.WriteLine($"Loaded up {Logs.Length:n0} changelogs.");
	}

	public static void Generate()
	{
		Load();

		var latestUpdate = Logs[0];
		Builder.AppendLine($"# Release Notes");
		Builder.AppendLine($"## Latest Update");
		Builder.AppendLine($"Latest production release build changelog based on the [production branch](https://github.com/CarbonCommunity/Carbon.Core/tree/production).");
		Builder.AppendLine();
		Builder.AppendLine($"<Badge type=\"info\" text=\"Current Version: {latestUpdate.Version}\" style=\"text-align:center; width:178px\" /> <br>");
		Builder.AppendLine($"<CarbonButton href=\"https://github.com/CarbonCommunity/Carbon.Core/releases/tag/production_build\" text=\"Download Latest\" icon=\"github\" external/> <br>");
		Builder.AppendLine($"<Badge type=\"info\" text=\"{latestUpdate.Date}\" style=\"text-align:center; width:178px\" />");
		Builder.AppendLine($"## Current Changes");
		Builder.AppendLine();
		WriteChanges(latestUpdate.Changes);
		Builder.AppendLine();
		Builder.AppendLine($"## Older Changes");
		Builder.AppendLine($"The following are archived changes of older releases of Carbon. Publicly, we only ship the very latest version of Carbon as older ones are out of support.");
		Builder.AppendLine();
		for (int i = 1; i < Logs.Length; i++)
		{
			var log = Logs[i];
			Builder.AppendLine($":::details {log.Version} <CarbonBadge type=\"date\" text=\"Released on {log.Date}\" />");
			Builder.AppendLine();
			WriteChanges(log.Changes);
			Builder.AppendLine();
			Builder.AppendLine($":::");
		}

		File.WriteAllText(Path.Combine(Generator.Arguments.Docs, "release-notes.md"), Builder.ToString());
	}

	private static void WriteChanges(Structure.Changelog.Change[] changes)
	{
		WriteLogs("add", changes.Where(x => x.Type == Structure.Changelog.ChangeTypes.Added).OrderBy(x => x.Message));
		WriteLogs("fix", changes.Where(x => x.Type == Structure.Changelog.ChangeTypes.Fixed).OrderBy(x => x.Message));
		WriteLogs("update",changes.Where(x => x.Type == Structure.Changelog.ChangeTypes.Updated).OrderBy(x => x.Message));
		WriteLogs("misc",changes.Where(x => x.Type == Structure.Changelog.ChangeTypes.Miscellaneous).OrderBy(x => x.Message));
		WriteLogs("remove",changes.Where(x => x.Type == Structure.Changelog.ChangeTypes.Removed).OrderBy(x => x.Message));

		static void WriteLogs(string type, IOrderedEnumerable<Structure.Changelog.Change> changes)
		{
			foreach (var change in changes)
			{
				var formattedMessage = (change.Message.EndsWith(".") ? change.Message : change.Message + ".").Replace("\"", "'");
				Builder.AppendLine($"<CarbonChange type=\"{type}\" text=\"{formattedMessage}\" style=\"font-size:15px\" />");
			}
		}
	}
}
