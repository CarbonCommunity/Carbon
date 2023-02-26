using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

[AttributeUsage(AttributeTargets.Class)]
public class InfoAttribute : Attribute
{
	public string Title { get; }
	public string Author { get; }
	public Version Version { get; private set; }

	public InfoAttribute(string Title, string Author, string Version)
	{
		this.Title = Title;
		this.Author = Author;
		SetVersion(Version);
	}
	public InfoAttribute(string Title, string Author, double Version)
	{
		this.Title = Title;
		this.Author = Author;
		SetVersion(Version.ToString());
	}

	private void SetVersion(string version)
	{
		var list = (from part in version.Split('.')
					select (ushort)(ushort.TryParse(part, out ushort result) ? result : 0)).ToList();

		while (list.Count < 3)
			list.Add(0);

		if (list.Count > 3)
			throw new Exception($"Version {version} is invalid for {Title}, should be 'major.minor.patch'");

		Version = new Version(list[0], list[1], list[2]);
	}
}

[AttributeUsage(AttributeTargets.Class)]
public class DescriptionAttribute : Attribute
{
	public string Description { get; }

	public DescriptionAttribute(string description)
	{
		Description = description;
	}
}