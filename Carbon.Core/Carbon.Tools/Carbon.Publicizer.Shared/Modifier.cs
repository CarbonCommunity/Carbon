using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Mono.Cecil;
using Newtonsoft.Json;

namespace Carbon.Core;

public class Modifier
{
	public static List<Modifier> All = [];

	public static void Collect(string directory)
	{
		if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory))
		{
			return;
		}

		All.Clear();

		var files = Directory.GetFiles(directory);
		for (int i = 0; i < files.Length; i++)
		{
			All.AddRange(Read(files[i]));
		}

		if (All.Count > 0)
		{
			var invalidModifiers = 0;
			var invalidMembers = 0;
			for (int i = 0; i < All.Count; i++)
			{
				var modifier = All[i];
				if (!modifier.Validate())
				{
					invalidModifiers++;
					All.RemoveAt(i);
					i--;
					continue;
				}

				invalidMembers += modifier.GetInvalidMembers();
				modifier.ClearInvalidMembers();
			}
		}
	}

	public static Modifier[] Read(string path)
	{
		return !File.Exists(path) ? null : JsonConvert.DeserializeObject<Modifier[]>(File.ReadAllText(path));
	}

	public string Assembly;
	public string Name;
	public List<Field> Fields;

	public bool Validate()
	{
		return !string.IsNullOrEmpty(Assembly) && !string.IsNullOrEmpty(Name);
	}

	public bool HasSavedFields()
	{
		for (int i = 0; i < Fields.Count; i++)
		{
			if (Fields[i].ShouldSave)
			{
				return true;
			}
		}
		return false;
	}

	public int GetInvalidMembers()
	{
		var invalidMembers = 0;
		for (int i = 0; i < Fields.Count; i++)
		{
			if (Fields[i].Validate())
			{
				continue;
			}
			invalidMembers++;
		}
		return invalidMembers;
	}

	public void ClearInvalidMembers()
	{
		for (int i = 0; i < Fields.Count; i++)
		{
			if (Fields[i].Validate())
			{
				continue;
			}
			Fields.RemoveAt(i);
			i--;
		}
	}

	public static void ApplyModifiers(string assemblyFileName, AssemblyDefinition assembly, ref int modifiers, ref int members)
	{
		var name = Path.GetFileNameWithoutExtension(assemblyFileName);
		for (int i = 0; i < All.Count; i++)
		{
			var modifier = All[i];
			if (!modifier.Assembly.Equals(name, StringComparison.CurrentCultureIgnoreCase))
			{
				continue;
			}

			ApplySavedModifiersImpl(assembly, modifier, ref modifiers, ref members);
			ApplyModifiersImpl(assembly, modifier, null, ref modifiers, ref members);
		}
	}

	private static void ApplyModifiersImpl(AssemblyDefinition assembly, Modifier modifier, TypeDefinition? type, ref int modifiers, ref int members)
	{
		try
		{
			var saveType = type != null;
			type ??= GetTypeDefinition(assembly, modifier.Name);

			if (type == null)
			{
				Console.WriteLine($" Couldn't find type for modifier: {modifier.Name} [{modifier.Assembly}]");
				return;
			}

			modifiers++;
			for (int i = 0; i < modifier.Fields.Count; i++)
			{
				var field = modifier.Fields[i];
				if ((saveType && !field.ShouldSave) || (!saveType && field.ShouldSave))
				{
					continue;
				}

				var fieldType = GetTypeReference(assembly, field.Type);

				if (fieldType == null)
				{
					Console.WriteLine($" Couldn't find field type for modifier: {field.Name} in {modifier.Name} [{modifier.Assembly}]");
					continue;
				}

				if (type.Fields.FirstOrDefault(x => x.Name.Equals(field.Name)) != null ||
				    type.Properties.FirstOrDefault(x => x.Name.Equals(field.Name)) != null)
				{
					Console.WriteLine($" Couldn't create field for modifier: {field.Name} in {modifier.Name} [{modifier.Assembly}] as a member with the same name already exists");
					continue;
				}

				var newField = new FieldDefinition(field.Name, FieldAttributes.NotSerialized, assembly.MainModule.ImportReference(fieldType))
				{
					IsStatic = field.IsStatic,
					Constant = field.DefaultValue
				};
				type.Fields.Add(newField);
				members++;
			}
		}
		catch(Exception ex)
		{
			Console.WriteLine($" Failed applying modifier: {modifier.Name} [{modifier.Assembly}] ({ex.Message})\n{ex.StackTrace}");
		}
	}

	public static void ApplySavedModifiersImpl(AssemblyDefinition assembly, Modifier modifier, ref int modifiers, ref int members)
	{
		if (!modifier.HasSavedFields())
		{
			return;
		}

		try
		{
			var type = GetTypeDefinition(assembly, modifier.Name);

			if (type == null)
			{
				return;
			}

			var newType = new TypeDefinition(type.Namespace, "CarbonSerialized", TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable);
			type.NestedTypes.Add(newType);

			ApplyModifiersImpl(assembly, modifier, newType, ref modifiers, ref members);
		}
		catch (Exception ex)
		{
			Console.WriteLine($" Failed applying saved modifier: {modifier.Name} [{modifier.Assembly}] ({ex.Message})\n{ex.StackTrace}");
		}
	}

	private static TypeDefinition? GetTypeDefinition(AssemblyDefinition assembly, string name)
	{
		var type = assembly.MainModule.GetType(name);
		if (type != null)
		{
			return type;
		}

		foreach (var nameReference in assembly.MainModule.AssemblyReferences)
		{
			try
			{
				var resolvedAsm = assembly.MainModule.AssemblyResolver.Resolve(nameReference);
				var externalType = resolvedAsm.MainModule.GetType(name);
				if (externalType != null)
				{
					return externalType;
				}
			}
			catch { }
		}

		return null;
	}

	private static TypeReference GetTypeReference(AssemblyDefinition assembly, string fullName)
	{
		if (!fullName.Contains('`') || !fullName.Contains('['))
		{
			return TryResolveSimple(assembly, fullName);
		}

		var bracketStart = fullName.IndexOf('[');
		var baseName = fullName.Substring(0, bracketStart);

		var argsString = fullName.Substring(bracketStart + 1, fullName.Length - bracketStart - 2);
		var argNames = SplitGenericArgs(argsString);

		var openType = GetTypeDefinition(assembly, baseName);

		var genericInstance = new GenericInstanceType(openType);
		foreach (var arg in argNames)
		{
			genericInstance.GenericArguments.Add(GetTypeReference(assembly, arg));
		}

		return genericInstance;
	}

	private static List<string> SplitGenericArgs(string input)
	{
		var args = new List<string>();
		var depth = 0;
		var lastSplit = 0;
		for (int i = 0; i < input.Length; i++)
		{
			switch (input[i])
			{
				case '[':
					depth++;
					break;
				case ']':
					depth--;
					break;
				case ',' when depth == 0:
					args.Add(input.Substring(lastSplit, i - lastSplit).Trim());
					lastSplit = i + 1;
					break;
			}
		}
		args.Add(input.Substring(lastSplit).Trim());
		return args;
	}

	private static TypeReference TryResolveSimple(AssemblyDefinition assembly, string name)
	{
		var typeRef = assembly.MainModule.GetType(name);
		if (typeRef != null)
		{
			return typeRef;
		}

		foreach (var asmRef in assembly.MainModule.AssemblyReferences)
		{
			try
			{
				var asm = assembly.MainModule.AssemblyResolver.Resolve(asmRef);
				var found = asm.MainModule.Types.FirstOrDefault(t => t.FullName == name);
				if (found != null)
				{
					return assembly.MainModule.ImportReference(found);
				}
			}
			catch { } // Ignore
		}
		return null;
	}

	public class Field
	{
		public string Name;
		public string Type;
		public object DefaultValue;
		public bool IsStatic;
		public bool ShouldSave;

		public bool Validate()
		{
			return !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Type);
		}
	}
}
