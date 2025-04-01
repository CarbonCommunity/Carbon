using System.Text;

namespace Carbon.Documentation.Generators;

public partial class ServerMetadata
{
	public static void GenerateItems()
	{
		Console.WriteLine($"Found {Items.Length:n0} Rust items");

		var groupedItems = Items.GroupBy(x => x.Category);
		foreach (var items in groupedItems)
		{
			Builder.Clear();
			Builder.AppendLine($"# {items.Key}");
			Builder.AppendLine($"Full list of all <Badge type=\"danger\" text=\"{items.Count():n0}\"/> items in the **{items.Key}** category.");
			Builder.AppendLine();
			Builder.AppendLine($"---");
			Builder.AppendLine($"<table>");
			foreach (var item in items)
			{
				Builder.AppendLine($"\t<tr {(item.Hidden ? "style=\"background-color:#300000;\"" : string.Empty)}>");
				{
					Builder.AppendLine($"\t\t<td style=\"width:15%;\">");
					{
						Builder.AppendLine($"\t\t\t<img src=\"https://carbonmod.gg/assets/media/items/{item.ShortName}.png\">");
					}
					Builder.AppendLine($"\t\t\t</td>");

					Builder.AppendLine($"\t\t<td>");
					{
						var flagsText = item.Flags.ToString();
						Builder.AppendLine($"\t<h5 id=\"{item.ShortName}\"><a href=\"{item.Category}#{item.ShortName}\"><Badge type=\"tip\" text=\"#\"/></a> {item.DisplayName} {(flagsText != "0" ? string.Join(' ', flagsText.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => $"<Badge type=\"tip\" text=\"{x}\"/>")) : string.Empty)}</h5> ");
						Builder.AppendLine($"\t<Badge type=\"info\" text=\"{item.Id}\"/> <Badge type=\"info\" text=\"{item.ShortName}\"/> <Badge type=\"warning\" text=\"x{item.Stack}\"/> {(item.Rarity != Structure.Rarity.None ? $"<Badge type=\"warning\" text=\"{item.Rarity}\"/>" : string.Empty)}{(item.Hidden ? $" <Badge type=\"danger\" text=\"Hidden\"/>" : string.Empty)}<br>");
						Builder.AppendLine($"\t{item.Description}");
					}
					Builder.AppendLine($"</td>");
				}
				Builder.AppendLine($"</tr>");
			}
			Builder.AppendLine($"</table>");
			File.WriteAllText(Path.Combine(Generator.Arguments.Docs, "references", "items", $"{items.Key}.md"), Builder.ToString());
		}
	}
}
