using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Carbon.Components;

public class ModifierBank : List<Modifier>
{
	public ModifierBank WithModifier(Modifier modifier)
	{
		Add(modifier);
		return this;
	}

	public string ToJson(Formatting formatting = Formatting.Indented)
	{
		return JsonConvert.SerializeObject(this, formatting);
	}

	public void ToFile(string path, Formatting formatting = Formatting.Indented)
	{
		File.WriteAllText(path, ToJson(formatting));
	}
}

