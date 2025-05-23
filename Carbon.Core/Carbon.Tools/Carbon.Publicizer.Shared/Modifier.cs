using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Carbon.Components;

public partial class Modifier
{
	public static readonly string DataType = "CarbonData";

	public static ModifierBank Read(string path)
	{
		return !File.Exists(path) ? null : JsonConvert.DeserializeObject<ModifierBank>(File.ReadAllText(path));
	}

	public string Assembly;
	public string Name;
	public List<Field> Fields = [];

	public Modifier WithAssembly(string value)
	{
		Assembly = value;
		return this;
	}

	public Modifier WithName(string value)
	{
		Name = value;
		return this;
	}

	public Modifier WithField(Field field)
	{
		Fields.Add(field);
		return this;
	}

	#region Helpers

	public bool Validate()
	{
		return !string.IsNullOrEmpty(Assembly) && !string.IsNullOrEmpty(Name);
	}

	public bool HasSavedFields()
	{
		for (int i = 0; i < Fields.Count; i++)
		{
			if (Fields[i].ShouldSave)
			{
				return true;
			}
		}
		return false;
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

	#endregion

	public class Field
	{
		public string Name;
		public string Type;
		public object DefaultValue;
		public bool IsStatic;
		public bool ShouldSave;

		public Field WithName(string value)
		{
			Name = value;
			return this;
		}

		public Field WithType(string value)
		{
			Type = value;
			return this;
		}

		public Field WithDefaultValue(object value)
		{
			DefaultValue = value;
			return this;
		}

		public Field WithStatic(bool wants)
		{
			IsStatic = wants;
			return this;
		}

		public Field WithSave(bool wants)
		{
			ShouldSave = wants;
			return this;
		}

		public bool Validate()
		{
			return !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Type);
		}
	}
}
