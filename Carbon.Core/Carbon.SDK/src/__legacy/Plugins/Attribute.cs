using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Oxide.Core;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

//namespace API.Plugins;

[AttributeUsage(AttributeTargets.Class)]
[MeansImplicitUse]
public class InfoAttribute : Attribute
{
	public string Title { get; }
	public string Author { get; }
	public VersionNumber Version { get; private set; }
	public int ResourceId { get; set; }

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
					select (ushort)(ushort.TryParse(part, out var result) ? result : 0)).ToList();
		while (list.Count < 3) list.Add(0);

		// if (list.Count > 3)
		// {
		// 	Debug.LogWarning("Version `" + version + "` is invalid for " + Title + ", should be `major.minor.patch`");
		// }

		Version = new VersionNumber(list[0], list[1], list[2]);
	}
}

[AttributeUsage(AttributeTargets.Class)]
[MeansImplicitUse]
public class DescriptionAttribute : Attribute
{
	public string Description { get; }

	public DescriptionAttribute(string description)
	{
		Description = description;
	}
}

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Field)]
[MeansImplicitUse]
public class PluginReferenceAttribute : Attribute
{
	public string Name { get; set; }

	public FieldInfo Field { get; set; }

	public PluginReferenceAttribute(string name = null)
	{
		Name = name;
	}
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
[MeansImplicitUse]
public class CommandAttribute : Attribute
{
	public string[] Names { get; } = new string[1];

	public CommandAttribute(string name)
	{
		Names[0] = name;
	}

	public CommandAttribute(params string[] commands)
	{
		Names = commands;
	}
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
[MeansImplicitUse]
public class ChatCommandAttribute : Attribute
{
	public string Name { get; }
	public string Help { get; }

	public ChatCommandAttribute(string name, string help = null)
	{
		Name = name;
		Help = help;
	}
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
[MeansImplicitUse]
public class ConsoleCommandAttribute : Attribute
{
	public string Name { get; }
	public string Help { get; }

	public ConsoleCommandAttribute(string name)
	{
		Name = name;
	}

	public ConsoleCommandAttribute(string name, string help)
	{
		Name = name;
		Help = help;
	}
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
[MeansImplicitUse]
public class RConCommandAttribute : Attribute
{
	public string Name { get; }
	public string Help { get; }

	public RConCommandAttribute(string name, string help = null)
	{
		Name = name;
		Help = help;
	}
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
[MeansImplicitUse]
public class ProtectedCommandAttribute : Attribute
{
	public string Name { get; }
	public string Help { get; }

	public ProtectedCommandAttribute(string name)
	{
		Name = name;
	}

	public ProtectedCommandAttribute(string name, string help)
	{
		Name = name;
		Help = help;
	}
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
[MeansImplicitUse]
public class CommandVarAttribute : Attribute
{
	public string Name { get; }
	public string Help { get; }
	public bool Protected { get; set; }
	public bool Saved { get; }

	public CommandVarAttribute(string name, string help = null, bool saved = false)
	{
		Name = name;
		Saved = saved;
		Help = help;
	}
	public CommandVarAttribute(string name, bool @protected, string help = null, bool save = false)
	{
		Name = name;
		Saved = save;
		Help = help;
		Protected = @protected;
	}
}

[AttributeUsage(AttributeTargets.Method)]
[MeansImplicitUse]
public class HookMethodAttribute : Attribute
{
	public string Name { get; set; }
	public MethodInfo Method { get; set; }

	public HookMethodAttribute() { }
	public HookMethodAttribute(string name)
	{
		Name = name;
	}
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
[MeansImplicitUse]
public class PermissionAttribute : Attribute
{
	public string Name { get; }

	public PermissionAttribute(string permission)
	{
		Name = permission;
	}
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
[MeansImplicitUse]
public class GroupAttribute : Attribute
{
	public string Name { get; }

	public GroupAttribute(string group)
	{
		Name = group;
	}
}

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
[MeansImplicitUse]
public class AuthLevelAttribute : Attribute
{
	public int AuthLevel { get; }

	public AuthLevelAttribute(int level)
	{
		AuthLevel = level;
	}
}

[AttributeUsage(AttributeTargets.Method)]
[MeansImplicitUse]
public class CooldownAttribute : Attribute
{
	public int Miliseconds { get; } = 0;

	public CooldownAttribute(int miliseconds)
	{
		Miliseconds = miliseconds;
	}
}

[AttributeUsage(AttributeTargets.Class)]
[MeansImplicitUse]
public class HotloadableAttribute : Attribute
{

}

[AttributeUsage(AttributeTargets.Class)]
[MeansImplicitUse]
public class Conditional : Attribute
{
	public string Symbol { get;set; }

	public Conditional() { }
	public Conditional(string symbol)
	{
		Symbol = symbol;
	}
}
