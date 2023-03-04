using System;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace API.Hooks;

[AttributeUsage(AttributeTargets.Class)]
public class HookAttribute : Attribute
{
	[AttributeUsage(AttributeTargets.Class)]
	public class Patch : Attribute
	{
		public string Name
		{ get; }

		public string FullName
		{ get; }

		public Type Target
		{ get; }

		public string Method
		{ get; }

		public Type[] MethodArgs
		{ get; }

		/// <summary>
		/// This should be the most used patch declaration decorator.
		/// Use one of the other only for specific purposes.
		/// </summary>
		public Patch(string name, string fullName, Type target, string method, Type[] args) : this(name, fullName, target, method)
		 => MethodArgs = args;

		/// <summary>
		/// Short version of the standard patch declaration decorator.
		/// Use one of the other only for specific purposes.
		/// </summary>
		public Patch(string name, string fullName, Type target, string method)
		{
			FullName = fullName;
			Method = method;
			Name = name;
			Target = target;
		}


		/// <summary>
		/// To be used to facilitate patching of generic methods
		/// </summary>
		public Patch(string name, string fullName, Type target)
		{
			Name = name;
			Target = target;
			FullName = fullName;
		}
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class Options : Attribute
	{
		public HookFlags Value
		{ get; }

		public Options(HookFlags value)
			=> Value = value;
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class Identifier : Attribute
	{
		public string Value
		{ get; }

		public Identifier(string value)
			=> Value = value;
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class Dependencies : Attribute
	{
		public string[] Value
		{ get; }

		public Dependencies(string[] value)
			=> Value = value;
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class Checksum : Attribute
	{
		public string Value
		{ get; }

		public Checksum(string value)
			=> Value = value;
	}
}

[AttributeUsage(AttributeTargets.Class)]
public class MetadataAttribute : Attribute
{
	[AttributeUsage(AttributeTargets.Class)]
	public class Parameter : Attribute
	{
		public string Name { get; }

		public Parameter(string name) { Name = name; }
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class Info : Attribute
	{
		public string Name { get; }

		public Info(string name) { Name = name; }
	}
}
