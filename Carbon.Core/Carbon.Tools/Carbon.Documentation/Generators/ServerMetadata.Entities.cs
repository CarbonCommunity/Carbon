using System.Text;

namespace Carbon.Documentation.Generators;

public partial class ServerMetadata
{
	public static void GenerateEntities()
	{
		Console.WriteLine($"Found {Entities.Length:n0} Rust entities");

		var groupedEntities = Entities.GroupBy(x => x.Type);
		var singularEntities = groupedEntities.Where(x => x.Count() == 1).SelectMany(x => x);
		{
			Builder.Clear();
			Builder.AppendLine($"# Singular Entities");
			Builder.AppendLine($"The following list contains information of a total of <Badge type=\"warning\" text=\"{singularEntities.Count():n0}\"/>\n entities with a singular prefab of that entity type.");
			Builder.AppendLine();
			Builder.AppendLine($"---");
			Builder.AppendLine($"| Entity Type, ID & Path |");
			Builder.AppendLine($"| --- |");
			foreach (var prefab in singularEntities)
			{
				Builder.AppendLine($"| <Badge type=\"info\" text=\"{prefab.Type}\"/> <Badge type=\"tip\" text=\"{prefab.ID}\"/> <br> {prefab.Path} |");
			}
			File.WriteAllText(Path.Combine(Generator.Arguments.Docs, "references", "entities", "0Singulars.md"), Builder.ToString());
		}

		Builder.Clear();
		Builder.AppendLine($"# All Entities");
		Builder.AppendLine($"The following list contains information of a total of <Badge type=\"warning\" text=\"{Entities.Count():n0}\"/>\n entities.");
		Builder.AppendLine();
		Builder.AppendLine($"## API");
		Builder.AppendLine($"Here's the API endpoint for you to use in your projects.");
		Builder.AppendLine($"<CarbonButton href=\"/Carbon.Documentation/rust/entities.json\" text=\"Entities API\" icon=\"gtk\" external=\"true\"/>");
		Builder.AppendLine();
		Builder.AppendLine($"---");
		Builder.AppendLine($"| Entity Type, ID & Path |");
		Builder.AppendLine($"| --- |");
		foreach (var prefab in Entities)
		{
			var isSingular = singularEntities.Contains(prefab);
			Builder.AppendLine($"| {(isSingular ? string.Empty : $"<a href='{prefab.Type}'>")}<Badge type=\"info\" text=\"{prefab.Type}\"/>{(isSingular ? string.Empty : "</a>")} <Badge type=\"tip\" text=\"{prefab.ID}\"/> <br> {prefab.Path} |");
		}
		File.WriteAllText(Path.Combine(Generator.Arguments.Docs, "references", "entities", "0All.md"), Builder.ToString());

		foreach (var entities in groupedEntities)
		{
			if (entities.Count() == 1)
			{
				continue;
			}

			Builder.Clear();
			Builder.AppendLine($"# {entities.Key}");
			Builder.AppendLine($"Full list of all <Badge type=\"warning\" text=\"{entities.Count():n0}\"/> entity prefabs with the **{entities.Key}** entity type.");
			Builder.AppendLine();
			Builder.AppendLine($"---");
			Builder.AppendLine($"| ID & Path |");
			Builder.AppendLine($"| --- |");
			foreach (var prefab in entities)
			{
				Builder.AppendLine($"| <Badge type=\"tip\" text=\"{prefab.ID}\"/> <br> {prefab.Path} |");
			}
			File.WriteAllText(Path.Combine(Generator.Arguments.Docs, "references", "entities", $"{entities.Key}.md"), Builder.ToString());
		}
	}
}
