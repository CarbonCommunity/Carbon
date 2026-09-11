using Carbon.Publicizer;

#pragma warning disable

var input = args[0];
var carbonManaged = args.Length > 1 ? args[1] : null;
var patchableFiles = Directory.EnumerateFiles(input);

Config.Init(null!);
Patch.Init(null!, carbonManaged!, input);
foreach (var file in patchableFiles)
{
	try
	{
		var name = Path.GetFileName(file);
		var patch = BuiltInPatches.Current.FirstOrDefault(x => x.fileName.Equals(name));

		if (patch != null && patch.Execute())
		{
			patch.Write(file);
			Console.WriteLine($" Publicized '{Path.GetFileName(file)}'");
			continue;
		}

		if (!Config.Singleton.Publicizer.PublicizedAssemblies.Any(x => name.StartsWith(x, StringComparison.OrdinalIgnoreCase)))
		{
			continue;
		}

		patch = new Patch(Path.GetDirectoryName(file), name);
		if (patch.Execute())
		{
			patch.Write(file);
			Console.WriteLine($" Publicized '{Path.GetFileName(file)}'");
		}
	}
	catch (Exception ex)
	{
		Console.WriteLine($"Failed to patch ({ex.Message})\n{ex.StackTrace}");
	}
}
Patch.Uninit();
