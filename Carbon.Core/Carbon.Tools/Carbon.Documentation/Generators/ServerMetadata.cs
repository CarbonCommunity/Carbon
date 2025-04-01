using System.Text;
using Newtonsoft.Json;

namespace Carbon.Documentation.Generators;

public static partial class ServerMetadata
{
	public static Structure.Entity[] Entities;
	public static Structure.Prefab[] Prefabs;
	public static Structure.Item[] Items;

	private static void Load()
	{
		var serverRoot = Path.Combine(Generator.Arguments.Docs, "..", "tools", "Server", "win", "server");
		Entities = JsonConvert.DeserializeObject<Structure.Entity[]>(File.ReadAllText(Path.Combine(serverRoot, "gen_entities.json")));
		Prefabs = JsonConvert.DeserializeObject<Structure.Prefab[]>(File.ReadAllText(Path.Combine(serverRoot, "gen_prefabs.json")));
		Items = JsonConvert.DeserializeObject<Structure.Item[]>(File.ReadAllText(Path.Combine(serverRoot, "gen_items.json")));
	}

	public static void Generate()
	{
		Load();

		var builder = new StringBuilder();
		Console.WriteLine($"Found {Entities.Length:n0} Rust entities");
		Console.WriteLine($"Found {Prefabs.Length:n0} Rust prefabs");
		Console.WriteLine($"Found {Items.Length:n0} Rust items");

		var groupedEntities = Entities.GroupBy(x => x.Type);
		var singularEntities = groupedEntities.Where(x => x.Count() == 1).SelectMany(x => x);
		{
			builder.Clear();
			builder.AppendLine($"# Singular Entities");
			builder.AppendLine($"| Entity Type, ID & Path |");
			builder.AppendLine($"| --- |");
			foreach (var prefab in singularEntities)
			{
				builder.AppendLine($"| <Badge type=\"info\" text=\"{prefab.Type}\"/> <Badge type=\"tip\" text=\"{prefab.ID}\"/> <br> {prefab.Path} |");
			}
			File.WriteAllText(Path.Combine(Generator.Arguments.Docs, "references", "entities", $"Singulars.md"), builder.ToString());
		}
		foreach (var entities in groupedEntities)
		{
			if (entities.Count() == 1)
			{
				continue;
			}

			builder.Clear();
			builder.AppendLine($"# {entities.Key}");
			builder.AppendLine($"| ID & Path |");
			builder.AppendLine($"| --- |");
			foreach (var prefab in entities)
			{
				builder.AppendLine($"| <Badge type=\"tip\" text=\"{prefab.ID}\"/> <br> {prefab.Path} |");
			}
			File.WriteAllText(Path.Combine(Generator.Arguments.Docs, "references", "entities", $"{entities.Key}.md"), builder.ToString());
		}
	}
}
