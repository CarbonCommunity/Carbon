using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Logger = Doorstop.Utility.Logger;

namespace Carbon.Startup.Core;

/// <summary>
/// Reads the startup declarations (<see cref="StartupTaskAttribute"/>, <see cref="InjectFieldAttribute"/>)
/// of every package in carbon/managed/packages, straight from metadata so nothing gets loaded this early.
/// </summary>
public static class PackageManifests
{
	public struct Task
	{
		public StartupAction Action;
		public string Path;
		public string Target;
		public string Filter;
	}

	public static List<Task> Tasks { get; } = new();

	/// <summary>Fields to add to Rust types. Their type references resolve against the declaring package.</summary>
	public static List<(string TargetType, string FieldName, TypeReference FieldType)> Fields { get; } = new();

	public static string Folder => System.IO.Path.Combine(Defines.GetManagedFolder(), "packages");

	public static void Read()
	{
		Tasks.Clear();
		Fields.Clear();

		if (!Directory.Exists(Folder))
		{
			return;
		}

		foreach (var file in Directory.GetFiles(Folder, "*.dll"))
		{
			try
			{
				var assembly = AssemblyDefinition.ReadAssembly(new MemoryStream(File.ReadAllBytes(file)));

				foreach (var attribute in assembly.CustomAttributes)
				{
					switch (attribute.AttributeType.FullName)
					{
						case "Carbon.StartupTaskAttribute":
							Tasks.Add(new Task
							{
								Action = (StartupAction)Convert.ToInt32(attribute.ConstructorArguments[0].Value),
								Path = Resolve((string)attribute.ConstructorArguments[1].Value),
								Target = Resolve(GetProperty(attribute, nameof(StartupTaskAttribute.Target))),
								Filter = GetProperty(attribute, nameof(StartupTaskAttribute.Filter))
							});
							break;

						case "Carbon.InjectFieldAttribute":
							Fields.Add(((string)attribute.ConstructorArguments[0].Value,
								(string)attribute.ConstructorArguments[1].Value,
								(TypeReference)attribute.ConstructorArguments[2].Value));
							break;
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error($" Failed reading package manifest of '{System.IO.Path.GetFileName(file)}'", ex);
			}
		}
	}

	private static string GetProperty(CustomAttribute attribute, string name)
	{
		return attribute.Properties.FirstOrDefault(x => x.Name == name).Argument.Value as string;
	}

	private static string Resolve(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return path;
		}

		return System.IO.Path.GetFullPath(path
			.Replace("{rust_managed}", Defines.GetRustManagedFolder())
			.Replace("{rust}", Defines.GetRustRootFolder())
			.Replace("{carbon}", Defines.GetRootFolder())
			.Replace("{extensions}", Defines.GetExtensionsFolder())
			.Replace('/', System.IO.Path.DirectorySeparatorChar));
	}
}
