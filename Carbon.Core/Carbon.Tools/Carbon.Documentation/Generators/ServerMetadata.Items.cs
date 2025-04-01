using System.Text;

namespace Carbon.Documentation.Generators;

public partial class ServerMetadata
{
	public static void GenerateItems()
	{
		Console.WriteLine($"Found {Items.Length:n0} Rust items");

		var groupedItems = Items.GroupBy(x => x.Category);

		Builder.Clear();
		Builder.AppendLine($"# All Items");
		Builder.AppendLine($"Full list of all <Badge type=\"danger\" text=\"{Items.Count():n0}\"/> items.");
		Builder.AppendLine();

		Builder.AppendLine($"## API");
		Builder.AppendLine($"Here's the API endpoint for you to use in your projects.");
		Builder.AppendLine($"<CarbonButton href=\"/Carbon.Documentation/rust/items.json\" text=\"Items API\" icon=\"buffer\" external=\"true\"/>");
		Builder.AppendLine();
		foreach (var items in groupedItems)
		{
			Builder.AppendLine($"## {items.Key}");
			Builder.AppendLine($"<table>");
			StoreItems(items);
			Builder.AppendLine($"</table>");
			Builder.AppendLine();
		}
		File.WriteAllText(Path.Combine(Generator.Arguments.Docs, "references", "items", $"0All.md"), Builder.ToString());

		foreach (var items in groupedItems)
		{
			Builder.Clear();
			Builder.AppendLine($"# {items.Key}");
			Builder.AppendLine($"Full list of all <Badge type=\"danger\" text=\"{items.Count():n0}\"/> items in the **{items.Key}** category.");
			Builder.AppendLine();
			Builder.AppendLine($"---");
			Builder.AppendLine($"<table>");
			StoreItems(items);
			Builder.AppendLine($"</table>");
			File.WriteAllText(Path.Combine(Generator.Arguments.Docs, "references", "items", $"{items.Key}.md"), Builder.ToString());
		}
	}

	private static void StoreItems(IEnumerable<Structure.Item> items)
	{
		foreach (var item in items)
		{
			Builder.AppendLine($"\t<tr {(item.Hidden ? "style=\"background-color:#300000;\"" : string.Empty)}>");
			{
				Builder.AppendLine($"\t\t<td style=\"width:15%;\">");
				{
					Builder.AppendLine($"\t\t\t<img src=\"https://carbonmod.gg/assets/media/items/{item.ShortName}.png\" onerror='this.src = \"https://carbonmod.gg/assets/media/missing.jpg\"'>");
				}
				Builder.AppendLine($"\t\t\t</td>");

				Builder.AppendLine($"\t\t<td>");
				{
					var flagsText = item.Flags.ToString();
					Builder.AppendLine($"\t<h5 id=\"{item.ShortName}\"><a href=\"#{item.ShortName}\"><Badge type=\"tip\" text=\"#\"/></a> {item.DisplayName} {(flagsText != "0" ? string.Join(' ', flagsText.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => $"<Badge type=\"tip\" text=\"{x}\"/>")) : string.Empty)}</h5> ");
					Builder.AppendLine($"\t<Badge type=\"info\" text=\"{item.Id}\"/> <Badge type=\"info\" text=\"{item.ShortName}\"/> <Badge type=\"warning\" text=\"x{item.Stack}\"/> {(item.Rarity != Structure.Rarity.None ? $"<Badge type=\"warning\" text=\"{item.Rarity}\"/>" : string.Empty)}{(item.Hidden ? $" <Badge type=\"danger\" text=\"Hidden\"/>" : string.Empty)}<br>");
					Builder.AppendLine($"\t{item.Description}");
				}
				Builder.AppendLine($"</td>");
			}
			Builder.AppendLine($"</tr>");
		}
	}
}
