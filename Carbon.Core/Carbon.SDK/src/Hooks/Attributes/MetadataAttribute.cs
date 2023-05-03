using System;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace API.Hooks;

[AttributeUsage(AttributeTargets.Class)]
public class MetadataAttribute : Attribute
{
	[AttributeUsage(AttributeTargets.Class)]
	public class Category : Attribute
	{
		public string Name
		{ get; }

		public Category(string name)
			=> Name = name;
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class Parameter : Attribute
	{
		public string Name
		{ get; }

		public Type Type
		{ get; }

		public bool Optional
		{ get; }

		public Parameter(string name, Type type, bool optional = false)
		{
			Name = name;
			Type = type;
			Optional = optional;
		}
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class Info : Attribute
	{
		public string Name
		{ get; }

		public Info(string name)
			=> Name = name;
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class Return : Attribute
	{
		public Type Type
		{ get; }

		public Return(Type type)
			=> Type = type;
	}
}
