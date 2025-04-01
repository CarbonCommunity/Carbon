using System.Text;
using Newtonsoft.Json;

namespace Carbon.Documentation.Generators;

public static partial class ServerMetadata
{
	private static StringBuilder Builder = new();

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

		GenerateEntities();
		GenerateItems();
	}
}
