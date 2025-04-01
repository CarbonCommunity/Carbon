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
					Builder.AppendLine($"\t\t<td style=\"width:20%;\">");
					{
						Builder.AppendLine($"\t\t\t<img src=\"https://carbonmod.gg/assets/media/items/{item.ShortName}.png\">");
					}
					Builder.AppendLine($"\t\t\t</td>");

					Builder.AppendLine($"\t\t<td>");
					{
						Builder.AppendLine($"\t\t<strong>{item.DisplayName}</strong><br>");
						Builder.AppendLine($"\t<code>{item.Id}</code> <code>{item.ShortName}</code> <code>Stack: {item.Stack}</code> <br> <code>Rarity: {item.Rarity}</code><br>");
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
