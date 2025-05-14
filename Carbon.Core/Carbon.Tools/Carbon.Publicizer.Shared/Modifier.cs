using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Carbon.Publicizer;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Newtonsoft.Json;

namespace Carbon.Core;

public class Modifier
{
	public static readonly string DataType = "CarbonData";
	public static readonly string StoredModifiers = "StoredModifiers";

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

	private static uint ManifestHash(string str)
	{
		return string.IsNullOrEmpty(str) ? 0 : BitConverter.ToUInt32(new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(str)), 0);
	}

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

	private static void ApplySavedModifiersImpl(AssemblyDefinition assembly, Modifier modifier, ref int modifiers, ref int members)
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

			var baseDataType = Patch.common.MainModule.GetType("Carbon.Components", "StoredModifiers").NestedTypes[0];
			var dataType = type.NestedTypes.FirstOrDefault(x => x.Name.Equals(DataType, StringComparison.CurrentCulture)) ?? new TypeDefinition(type.Namespace, DataType, TypeAttributes.NestedPublic | TypeAttributes.Class, assembly.MainModule.ImportReference(typeof(object)));
			if (!type.NestedTypes.Contains(dataType))
			{
				type.NestedTypes.Add(dataType);

				var module = assembly.MainModule;
				var runtimeModelType = module.ImportReference(typeof(ProtoBuf.Meta.RuntimeTypeModel));
				var typeModelType = module.ImportReference(typeof(ProtoBuf.Meta.TypeModel));
				var metaTypeType = module.ImportReference(typeof(ProtoBuf.Meta.MetaType));

				var defaultProperty = new MethodReference("get_Default", typeModelType, runtimeModelType) { HasThis = false };
				var itemMethod = new MethodReference("get_Item", metaTypeType, typeModelType) { HasThis = true, Parameters = { new ParameterDefinition(module.ImportReference(typeof(Type))) } };
				var addSubTypeMethod = new MethodReference("AddSubType", module.TypeSystem.Void, metaTypeType)
					{ HasThis = true, Parameters = { new ParameterDefinition(module.TypeSystem.Int32), new ParameterDefinition(module.ImportReference(typeof(Type))) } };

				var cctor = new MethodDefinition(".cctor", MethodAttributes.Private | MethodAttributes.Static | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
					module.TypeSystem.Void);

				var il = cctor.Body.GetILProcessor();

				// RuntimeTypeModel.Default
				il.Append(il.Create(OpCodes.Call, defaultProperty));

				// [Default][typeof(Data)]
				il.Append(il.Create(OpCodes.Ldtoken, assembly.MainModule.ImportReference(baseDataType)));
				il.Append(il.Create(OpCodes.Call, module.ImportReference(typeof(Type).GetMethod("GetTypeFromHandle"))));
				il.Append(il.Create(OpCodes.Callvirt, itemMethod));

				// .AddSubType(100, typeof(ThisType))
				var tagId = unchecked((int)ManifestHash(dataType.FullName)) & 0x0FFFFFFF; // Clamp to 28-bit range
				il.Append(il.Create(OpCodes.Ldc_I4, tagId));
				il.Append(il.Create(OpCodes.Ldtoken, dataType));
				il.Append(il.Create(OpCodes.Call, module.ImportReference(typeof(Type).GetMethod("GetTypeFromHandle"))));
				il.Append(il.Create(OpCodes.Callvirt, addSubTypeMethod));
				il.Append(il.Create(OpCodes.Ret));

				dataType.Methods.Add(cctor);
			}
			ApplyModifiersImpl(assembly, modifier, dataType, ref modifiers, ref members);
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
		var typeRef = assembly.MainModule.GetType(name, true);
		if (typeRef != null)
		{
			return typeRef;
		}

		foreach (var asmRef in assembly.MainModule.AssemblyReferences)
		{
			try
			{
				var asm = assembly.MainModule.AssemblyResolver.Resolve(asmRef);
				var found = asm.MainModule.Types.FirstOrDefault(t => t.FullName.Equals(name));
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
