using System;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace API.Plugins;

[AttributeUsage(AttributeTargets.Class)]
public class PluginAttribute : Attribute
{
	[AttributeUsage(AttributeTargets.Class)]
	public class Info : Attribute
	{
		public string Title { get; }
		public string Author { get; }
		public Version Version { get; private set; }

		public Info(string title, string author, string version)
		{
			Title = title;
			Author = author; ;
			Version = new Version(version);
		}
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class Description : Attribute
	{
		public string Value { get; }

		public Description(string description)
		{
			Value = description;
		}
	}
}
