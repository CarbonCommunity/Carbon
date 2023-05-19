using System;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace API.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class Metadata : Attribute
{
	[AttributeUsage(AttributeTargets.Class)]
	public class Product : Attribute
	{
		public string Title { get; private set; }
		public string Author { get; private set; }
		public Version Version { get; private set; }

		public Product(string title, string author, string version)
		{
			Title = title;
			Author = author; ;
			Version = new Version(version);
		}
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class Description : Attribute
	{
		public string Value { get; private set; }

		public Description(string description)
		{
			Value = description;
		}
	}
}
