using System;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Utility;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Patches;

internal sealed class AssemblyCSharp : MarshalByRefObject
{
	private static readonly DefaultAssemblyResolver _resolver;
	private AssemblyDefinition _assembly;
	private string _filename;

	static AssemblyCSharp()
	{
		_resolver = new DefaultAssemblyResolver();
		_resolver.AddSearchDirectory(Context.CarbonLib);
		_resolver.AddSearchDirectory(Context.CarbonModules);
		_resolver.AddSearchDirectory(Context.CarbonManaged);
		_resolver.AddSearchDirectory(Context.GameManaged);
	}

	public AssemblyCSharp()
	{
		_filename = Path.Combine(Context.GameManaged, "Assembly-CSharp.dll");

		if (!File.Exists(_filename))
			throw new Exception($"Assembly file '{_filename}' was not found");

		_assembly = AssemblyDefinition.ReadAssembly(_filename,
			parameters: new ReaderParameters { AssemblyResolver = _resolver });
	}

	internal bool IsPublic(string Type, string Method)
	{
		try
		{
			if (_assembly == null) throw new Exception("Loaded assembly is null");

			TypeDefinition t = _assembly.MainModule.Types.First(x => x.Name == Type);
			if (t == null) throw new Exception($"Unable to get type definition for '{Type}'");

			MethodDefinition m = t.Methods.First(x => x.Name == Method);
			if (m == null) throw new Exception($"Unable to get method definition for '{Method}'");

			return m.IsPublic;
		}
		catch (System.Exception ex)
		{
			Logger.Error(ex.Message);
			throw ex;
		}
	}

	internal void Publicize()
	{
		if (_assembly == null) throw new Exception("Loaded assembly is null");

		Logger.Debug($" - Publicize assembly");

		AssemblyNameReference scope =
			_assembly.MainModule.AssemblyReferences.OrderByDescending(a => a.Version).FirstOrDefault(a => a.Name == "mscorlib");

		MethodReference ctor = new MethodReference(".ctor", _assembly.MainModule.TypeSystem.Void,
			declaringType: new TypeReference("System", "NonSerializedAttribute", _assembly.MainModule, scope))
		{ HasThis = true };

		foreach (TypeDefinition type in _assembly.MainModule.Types)
			Publicize(type, ctor);
	}

	internal static void Publicize(TypeDefinition type, MethodReference ctor)
	{
		try
		{
			if (Blacklist.IsBlacklisted(type.Name))
			{
				Logger.Warn($"Excluded '{type.Name}' due to blacklisting");
				return;
			}

			if (type.IsNested)
			{
				type.IsNestedPublic = true;
			}
			else
			{
				type.IsPublic = true;
			}

			foreach (MethodDefinition Method in type.Methods)
			{
				if (Blacklist.IsBlacklisted($"{type.Name}.{Method.Name}"))
				{
					Logger.Warn($"Excluded '{type.Name}.{Method.Name}' due to blacklisting");
					continue;
				}

				Method.IsPublic = true;
			}

			foreach (FieldDefinition Field in type.Fields)
			{
				if (Blacklist.IsBlacklisted($"{type.Name}.{Field.Name}"))
				{
					Logger.Warn($"Excluded '{type.Name}.{Field.Name}' due to blacklisting");
					continue;
				}

				// Prevent publicize auto-generated fields
				if (type.Events.Any(x => x.Name == Field.Name)) continue;

				if (ctor != null && !Field.IsPublic && !Field.CustomAttributes.Any(a => a.AttributeType.FullName == "UnityEngine.SerializeField"))
				{
					Field.IsNotSerialized = true;
					Field.CustomAttributes.Add(item: new CustomAttribute(ctor));
				}

				Field.IsPublic = true;
			}
		}
		catch (System.Exception ex)
		{
			Logger.Error(ex.Message);
			throw ex;
		}

		foreach (TypeDefinition childType in type.NestedTypes)
			Publicize(childType, ctor);
	}

	internal void Patch()
	{
		try
		{
			Override_Harmony_Methods();
			Remove_RustHarmony_Reference();
			Add_Bootstrap_Tier0_Hook();
			Add_the_Fucking_IPlayer_shit();
		}
		catch (System.Exception ex)
		{
			Logger.Error(ex.Message);
			throw ex;
		}
	}

	private void Override_Harmony_Methods()
	{
		TypeDefinition type = _assembly.MainModule.GetType("ConVar.Harmony");
		string[] Items = { "Load", "Unload" };

		foreach (string Item in Items)
		{
			Logger.Debug($" - Patching {type.Name}.{Item}");

			MethodDefinition method = type.Methods.Single(x => x.Name == Item);
			ILProcessor processor = method.Body.GetILProcessor();

			method.Body.Variables.Clear();
			method.Body.Instructions.Clear();
			method.Body.ExceptionHandlers.Clear();

			switch (method.ReturnType.FullName)
			{
				case "System.Void":
					break;

				case "System.Boolean":
					processor.Append(processor.Create(OpCodes.Ldc_I4_0));
					break;

				default:
					processor.Append(processor.Create(OpCodes.Ldnull));
					break;
			}

			processor.Append(processor.Create(OpCodes.Ret));
		}
	}

	internal void Remove_RustHarmony_Reference()
	{
		Logger.Debug($" - Remove reference to Rust.Harmony");

		if (_assembly.MainModule.AssemblyReferences.Any(x => x.Name == "Rust.Harmony"))
		{
			_assembly.MainModule.AssemblyReferences.Remove(
				_assembly.MainModule.AssemblyReferences.Single(x => x.Name == "Rust.Harmony")
			);
		}
	}

	internal void Add_Bootstrap_Tier0_Hook()
	{
		Logger.Debug($" - Patching Bootstrap.Init_Tier0");

		AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(
			stream: new MemoryStream(File.ReadAllBytes(Path.Combine(Context.CarbonManaged, "Carbon.Bootstrap.dll"))));

		TypeDefinition type1 = assembly.MainModule.GetType("Carbon", "Bootstrap");
		if (type1 == null) throw new Exception("Unable to get a type for 'Carbon.Bootstrap'");

		MethodDefinition method1 = type1.Methods.Single(x => x.Name == "Initialize");
		if (method1 == null) throw new Exception("Unable to get a method definition for 'Tier0'");

		TypeDefinition type2 = _assembly.MainModule.GetType("Bootstrap");
		if (type2 == null) throw new Exception("Unable to get a type for 'Bootstrap'");

		MethodDefinition method2 = type2.Methods.Single(x => x.Name == "Init_Tier0");
		if (method2 == null) throw new Exception("Unable to get a method definition for 'Init_Tier0'");

		ILProcessor processor = method2.Body.GetILProcessor();

		Instruction instruction = processor.Create(
			OpCodes.Call, method2.Module.ImportReference(method1));

		Instruction code = method2.Body.Instructions[method2.Body.Instructions.Count - 2];
		if (code.OpCode == instruction.OpCode && code.Operand.ToString().Contains("Tier0"))
			return;

		method2.Body.Instructions[method2.Body.Instructions.Count - 1]
			= instruction;

		method2.Body.Instructions.Insert(method2.Body.Instructions.Count,
			processor.Create(OpCodes.Ret));

		_assembly.MainModule.AssemblyReferences.Add(assembly.Name);
	}

	internal void Add_the_Fucking_IPlayer_shit()
	{
		Logger.Debug($" - Patching BasePlayer.IPlayer");

		AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(
			stream: new MemoryStream(File.ReadAllBytes(Path.Combine(Context.CarbonManaged, "Carbon.API.dll"))));

		TypeDefinition type1 = assembly.MainModule.GetType("Oxide.Core.Libraries.Covalence", "IPlayer");
		if (type1 == null) throw new Exception("Unable to get a type for 'API.Contracts.IPlayer'");

		_assembly.MainModule.AssemblyReferences.Add(assembly.Name);
		_assembly.MainModule.GetType("BasePlayer").Fields.Add(item: new FieldDefinition("IPlayer",
			FieldAttributes.Public | FieldAttributes.NotSerialized, _assembly.MainModule.ImportReference(type1)));
	}

	// internal void Add_Bootstrap_StartupShared_Hook()
	// {
	// 	Logger.Debug($" - Patching Bootstrap.StartupShared");

	// 	AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(
	// 		stream: new MemoryStream(File.ReadAllBytes(Path.Combine(Context.CarbonManaged, "Carbon.Bootstrap.dll"))));

	// 	TypeDefinition t_type = assembly.MainModule.GetType("Hooks", "Bootstrap");
	// 	if (t_type == null) throw new Exception("Unable to get a type for 'Hooks.Bootstrap'");

	// 	MethodDefinition t_method = t_type.Methods.Single(x => x.Name == "StartupShared");
	// 	if (t_method == null) throw new Exception("Unable to get a method definition for 'StartupShared'");

	// 	TypeDefinition type = _assembly.MainModule.GetType("Bootstrap");
	// 	if (type == null) throw new Exception("Unable to get a type for 'Bootstrap'");

	// 	MethodDefinition method = type.Methods.Single(x => x.Name == "StartupShared");
	// 	if (method == null) throw new Exception("Unable to get a method definition for 'StartupShared'");

	// 	ILProcessor processor = method.Body.GetILProcessor();

	// 	Instruction instruction = processor.Create(
	// 		OpCodes.Call, method.Module.ImportReference(t_method));

	// 	Instruction code = method.Body.Instructions[1];
	// 	if (code.OpCode == instruction.OpCode && code.Operand.ToString().Contains("StartupShared"))
	// 		return;

	// 	_assembly.MainModule.AssemblyReferences.Add(assembly.Name);
	// 	method.Body.Instructions.Insert(1, instruction);
	// }

	internal void Write()
	{
		try
		{
			Logger.Log(" - Validating changes in-memory");

			using MemoryStream memoryStream = new MemoryStream();
			_assembly.Write(memoryStream);
			memoryStream.Position = 0;
			_assembly.Dispose();

			Logger.Log(" - Writing changes to disk");

			using FileStream outputStream = File.Open(_filename, FileMode.Create); //  + ".new.dll"
			memoryStream.CopyTo(outputStream);
		}
		catch (System.Exception ex)
		{
			Logger.Error(ex.Message);
			throw ex;
		}
	}
}
