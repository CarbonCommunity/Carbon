using System;
using System.Linq;
using HarmonyLib;

namespace API.Hooks;

[AttributeUsage(AttributeTargets.Class)]
public class HookAttribute : Attribute
{
	[AttributeUsage(AttributeTargets.Class)]
	public class Patch : Attribute
	{
		public string Name { get; }

		public string FullName { get; }

		public string Target { get; }

		public string Method { get; }

		public string[] MethodArgs { get; }

		public MethodType MethodType { get; }

		public Patch(string name, string fullName) : this(name, fullName, (string)null, null)
		{

		}

		/// <summary>
		/// This should be the most used patch declaration decorator.
		/// Use one of the other only for specific purposes.
		/// </summary>
		public Patch(string name, string fullName, string target, string method, string[] args) : this(name, fullName, target, method, MethodType.Normal)
			=> MethodArgs = args;
		public Patch(string name, string fullName, string target, string method, string[] args, MethodType type) : this(name, fullName, target, method, type)
			=> MethodArgs = args;
		public Patch(string name, string fullName, Type target, string method, Type[] args) : this(name, fullName, target.FullName, method, MethodType.Normal)
			=> MethodArgs = args == null ? [] : [.. args.Select(x => x.FullName)];
		public Patch(string name, string fullName, Type target, string method, Type[] args, MethodType type) : this(name, fullName, target.FullName, method, type)
			=> MethodArgs = args == null ? [] : [.. args.Select(x => x.FullName)];

		/// <summary>
		/// Short version of the standard patch declaration decorator.
		/// Use one of the other only for specific purposes.
		/// </summary>
		public Patch(string name, string fullName, string target, string method)
		{
			FullName = fullName;
			Method = method;
			Name = name;
			Target = target;
			MethodType = MethodType.Normal;
		}
		public Patch(string name, string fullName, string target, string method, MethodType type)
		{
			FullName = fullName;
			Method = method;
			Name = name;
			Target = target;
			MethodType = type;
		}
		public Patch(string name, string fullName, Type target, string method)
		{
			FullName = fullName;
			Method = method;
			Name = name;
			Target = target.FullName;
			MethodType = MethodType.Normal;
		}
		public Patch(string name, string fullName, Type target, string method, MethodType type)
		{
			FullName = fullName;
			Method = method;
			Name = name;
			Target = target.FullName;
			MethodType = type;
		}

		/// <summary>
		/// To be used to facilitate patching of generic methods
		/// </summary>
		public Patch(string name, string fullName, string target)
		{
			Name = name;
			Target = target;
			FullName = fullName;
			MethodType = MethodType.Normal;
		}
		public Patch(string name, string fullName, string target, MethodType type)
		{
			Name = name;
			Target = target;
			FullName = fullName;
			MethodType = type;
		}
		public Patch(string name, string fullName, Type target)
		{
			Name = name;
			Target = target.FullName;
			FullName = fullName;
			MethodType = MethodType.Normal;
		}
		public Patch(string name, string fullName, Type target, MethodType type)
		{
			Name = name;
			Target = target.FullName;
			FullName = fullName;
			MethodType = type;
		}
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class Options : Attribute
	{
		public HookFlags Value { get; }

		public Options(HookFlags value) => Value = value;
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class Identifier : Attribute
	{
		public string Value { get; }

		public Identifier(string value) => Value = value;
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class Dependencies : Attribute
	{
		public string[] Value { get; }

		public Dependencies(string[] value) => Value = value;
	}
}
