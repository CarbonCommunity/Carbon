using System;

namespace API.Hooks;

[AttributeUsage(AttributeTargets.Class)]
public class MetadataAttribute : Attribute
{
	[AttributeUsage(AttributeTargets.Class)]
	public class Category : Attribute
	{
		public string Name
		{ get; }

		public Category(string name) => Name = name;
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class Assembly : Attribute
	{
		public string Name
		{ get; }

		public Assembly(string name) => Name = name;
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class Parameter : Attribute
	{
		public string Name
		{ get; }

		public string Type
		{ get; }

		public bool Optional
		{ get; }

		public Parameter(string name, string type, bool optional = false)
		{
			Name = name;
			Type = type;
			Optional = optional;
		}

		public Parameter(string name, Type type, bool optional = false)
		{
			Name = name;
			Type = type.FullName;
			Optional = optional;
		}
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class Info : Attribute
	{
		public string Name
		{ get; }

		public Info(string name) => Name = name;
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class Return : Attribute
	{
		public Type Type
		{ get; }

		public Return(Type type) => Type = type;
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class OxideCompatible : Attribute;
}
