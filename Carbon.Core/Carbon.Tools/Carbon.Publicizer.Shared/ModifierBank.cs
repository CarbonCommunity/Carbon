using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Carbon.Components;

public class ModifierBank : List<Modifier>
{
	public bool HasPlugin(string name)
	{
		for (int i = 0; i < Count; i++)
		{
			if (Path.GetFileNameWithoutExtension(this[i].Path).Equals(name, System.StringComparison.CurrentCulture))
			{
				return true;
			}
		}
		return false;
	}

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

