///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;

[AttributeUsage(AttributeTargets.Class)]
public class Hook : Attribute
{
	public string Name { get; set; }
	public Type ReturnType { get; set; }

	public Hook () { }
	public Hook (string name)
	{
		Name = name;
		ReturnType = typeof(void);
	}
	public Hook (string name, Type returnType = null)
	{
		Name = name;
		ReturnType = returnType ?? typeof(void);
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class Patch : Attribute
	{
		public Type Type { get; set; }
		public string Method { get; set; }
		public Type [] Parameters { get; set; }
		public bool UseProvidedParameters { get; set; } = false;

		public Patch () { }
		public Patch (Type type, string method)
		{
			Type = type;
			Method = method;
		}
		public Patch (Type type, string method, params Type [] parameters) : this(type, method)
		{
			Parameters = parameters;
		}
		public Patch (Type type, string method, bool useProvidedParameters, params Type [] parameters) : this(type, method, parameters)
		{
			UseProvidedParameters = useProvidedParameters;
		}
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class Require : Attribute
	{
		public string Hook { get; set; }

		public Require () { }
		public Require (string hook)
		{
			Hook = hook;
		}
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class Info : Attribute
	{
		public string Value { get; set; }

		public Info (string value)
		{
			Value = value;
		}
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class Parameter : Attribute
	{
		public string Name { get; set; }
		public Type Type { get; set; }

		public Parameter () { }
		public Parameter (string name, Type type = null)
		{
			Name = name;
			Type = type ?? typeof(object);
		}
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class Category : Attribute
	{
		public Enum Value { get; set; }

		public enum Enum
		{
			General,
			Entity,
			Firework,
			Item,
			Player,
			Structure,
			Resources,
			Team,
			Weapon,
			Network,
			Server,
			World
		}

		public Category (Enum @enum)
		{
			Value = @enum;
		}
	}
}

[AttributeUsage(AttributeTargets.Class)]
public class OxideHook : Hook
{
	public OxideHook () { }
	public OxideHook (string name) : base(name) { }
	public OxideHook (string name, Type returnType = null) : base(name, returnType) { }
}

[AttributeUsage(AttributeTargets.Class)]
public class CarbonHook : Hook
{
	public CarbonHook () { }
	public CarbonHook (string name) : base(name) { }
	public CarbonHook (string name, Type returnType = null) : base(name, returnType) { }
}
