using System.Text;

namespace Carbon.Documentation.Generators;

public partial class ServerMetadata
{
	private static Dictionary<string, string> Categories = new()
	{
		["Autospawn"] = "assets/bundled/prefabs/autospawn",
		["World"] = "assets/bundled/prefabs/world",
		["Caves"] = "assets/bundled/prefabs/caves",
		["FX"] = "assets/bundled/prefabs/fx",
		["Modding"] = "assets/bundled/prefabs/modding",
		["Building"] = "assets/content/building",
		["Nature"] = "assets/content/nature",
		["Props"] = "assets/content/props",
		["Vehicles"] = "assets/content/vehicles",
		["Rust UI"] = "assets/plugins/rust.ui",
		["Clothes"] = "assets/prefabs/clothes",
		["Deployable"] = "assets/prefabs/deployable",
		["Food"] = "assets/prefabs/food",
		["Miscellaneous"] = "assets/prefabs/misc",
		["Missions"] = "assets/prefabs/missions",
		["Fish"] = "assets/prefabs/npc",
		["Plants"] = "assets/prefabs/plants",
		["Player"] = "assets/prefabs/player",
		["Resource"] = "assets/prefabs/resource",
		["Tea"] = "assets/prefabs/tea",
		["Tools"] = "assets/prefabs/tools",
		["Voice Audio"] = "assets/prefabs/voiceaudio",
		["Weapons"] = "assets/prefabs/weapons",
		["Rust AI"] = "assets/rust.ai",
		["Structures — Cargo Ship"] = "assets/content/structures/cargo_ship",
		["Structures — Coaling Tower"] = "assets/content/structures/coaling_tower",
		["Structures — Compound Gate"] = "assets/content/structures/compound_gate",
		["Structures — Concrete Slabs"] = "assets/content/structures/concrete_slabs",
		["Structures — Dirt Accum"] = "assets/content/structures/dirt_accum",
		["Structures — Dirt Mounds"] = "assets/content/structures/dirt_mounds",
		["Structures — Dredge"] = "assets/content/structures/dredge",
		["Structures — Excavator"] = "assets/content/structures/excavator",
		["Structures — Fences Walls"] = "assets/content/structures/fences_walls",
		["Structures — Gas Station"] = "assets/content/structures/gas_station",
		["Structures — Harbor"] = "assets/content/structures/harbor",
		["Structures — House Ruins"] = "assets/content/structures/house_ruins",
		["Structures — Industrial Doors"] = "assets/content/structures/industrial_doors_static",
		["Structures — Industrial"] = "assets/content/structures/industrial_structures",
		["Structures — Junkyard"] = "assets/content/structures/junkyard",
		["Structures — Launch Site"] = "assets/content/structures/launch_site",
		["Structures — Military Hangar"] = "assets/content/structures/military_hangar",
		["Structures — Military Tents"] = "assets/content/structures/military_tents",
		["Structures — Military Tunnels Signage"] = "assets/content/structures/military_tunnels_signage",
		["Structures — Oilrig"] = "assets/content/structures/oilrig",
		["Structures — Perimiter Walls"] = "assets/content/structures/perimeter_walls",
		["Structures — Pipelines"] = "assets/content/structures/pipelines",
		["Structures — Plywood Walls"] = "assets/content/structures/plywood_walls",
		["Structures — Portacabin Buildings"] = "assets/content/structures/portacabin_buildings",
		["Structures — Portacabin"] = "assets/content/structures/portacabins",
		["Structures — Power Substation"] = "assets/content/structures/power_substation",
		["Structures — Powerlines"] = "assets/content/structures/powerlines",
		["Structures — Underwater Labs"] = "assets/content/structures/underwater_labs",
		["Structures — Train Tunnels"] = "assets/content/structures/train_tunnels",
		["Structures — Timber Mine X-mas"] = "assets/content/structures/timber_mine_xmas",
		["Structures — Rocket Crate"] = "assets/content/structures/rocket_crane",
		["Structures — Roads"] = "assets/content/structures/roads",
		["Sounds"] = "assets/content/sound",
		["Building (Core)"] = "assets/prefabs/building",
		["Instruments"] = "assets/prefabs/instruments",
		["Locks"] = "assets/prefabs/locks",
		["Scenes"] = "assets/scenes",
		["Engine"] = "assets/prefabs/engine",
		["Component Items"] = "assets/prefabs/componentitems"
	};

	public static void GeneratePrefabs()
	{
		Console.WriteLine($"Found {Prefabs.Length:n0} Rust prefabs");

		var groupedPrefabs = Prefabs.GroupBy(x => Categories.FirstOrDefault(c => x.Path.StartsWith(c.Value, StringComparison.CurrentCultureIgnoreCase)).Key ?? "0Other");

		Builder.Clear();
		Builder.AppendLine($"# All Prefabs");
		Builder.AppendLine($"The following list contains information of a total of <Badge type=\"warning\" text=\"{Prefabs.Length:n0}\"/>\n prefabs.");
		Builder.AppendLine();
		Builder.AppendLine($"---");
		Builder.AppendLine($"| Entity Type, ID & Path |");
		Builder.AppendLine($"| --- |");
		foreach (var prefab in Prefabs)
		{
			Builder.AppendLine($"| <Badge type=\"info\" text=\"{(Categories.FirstOrDefault(c => prefab.Path.StartsWith(c.Value, StringComparison.CurrentCultureIgnoreCase)).Key ?? "Other")}\"/> <Badge type=\"tip\" text=\"{prefab.ID}\"/> <br> {prefab.Path} |");
		}
		File.WriteAllText(Path.Combine(Generator.Arguments.Docs, "references", "prefabs", "0All.md"), Builder.ToString());

		foreach (var entities in groupedPrefabs)
		{
			if (entities.Count() == 1)
			{
				continue;
			}

			Builder.Clear();
			Builder.AppendLine($"# {entities.Key}");
			Builder.AppendLine($"Full list of all <Badge type=\"warning\" text=\"{entities.Count():n0}\"/> prefabs.");
			Builder.AppendLine();
			Builder.AppendLine($"---");
			Builder.AppendLine($"| ID & Path |");
			Builder.AppendLine($"| --- |");
			foreach (var prefab in entities)
			{
				Builder.AppendLine($"| <Badge type=\"tip\" text=\"{prefab.ID}\"/> <br> {prefab.Path} |");
			}
			File.WriteAllText(Path.Combine(Generator.Arguments.Docs, "references", "prefabs", $"{entities.Key}.md"), Builder.ToString());
		}
	}
}
