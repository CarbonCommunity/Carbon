using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Carbon.Core;

public class Modifier
{
	public static List<Modifier> All = [];

	public static void Collect(string directory)
	{
		if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory))
		{
			return;
		}

		All.Clear();

		var files = Directory.GetFiles(directory);
		for (int i = 0; i < files.Length; i++)
		{
			All.AddRange(Read(files[i]));
		}

		if (All.Count > 0)
		{
			var invalidModifiers = 0;
			var invalidMembers = 0;
			for (int i = 0; i < All.Count; i++)
			{
				var modifier = All[i];
				if (!modifier.Validate())
				{
					invalidModifiers++;
					All.RemoveAt(i);
					i--;
					continue;
				}

				invalidMembers += modifier.GetInvalidMembers();
				modifier.ClearInvalidMembers();
			}
			Console.WriteLine($"Collected {All.Count:n0} modifiers and {All.Sum(x => x.Fields.Count):n0} field members");
		}
	}

	public static Modifier[] Read(string path)
	{
		return !File.Exists(path) ? null : JsonConvert.DeserializeObject<Modifier[]>(File.ReadAllText(path));
	}

	public string Assembly;
	public string Name;
	public List<Field> Fields;

	public bool Validate()
	{
		return !string.IsNullOrEmpty(Assembly) && !string.IsNullOrEmpty(Name);
	}

	public int GetInvalidMembers()
	{
		var invalidMembers = 0;
		for (int i = 0; i < Fields.Count; i++)
		{
			if (Fields[i].Validate())
			{
				continue;
			}
			invalidMembers++;
		}
		return invalidMembers;
	}

	public void ClearInvalidMembers()
	{
		for (int i = 0; i < Fields.Count; i++)
		{
			if (Fields[i].Validate())
			{
				continue;
			}
			Fields.RemoveAt(i);
			i--;
		}
	}

	public class Field
	{
		public string Name;
		public string Type;
		public object DefaultValue;
		public bool IsStatic;

		public bool Validate()
		{
			return !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Type);
		}
	}
}
