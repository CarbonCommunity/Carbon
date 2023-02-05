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

namespace Patches.Cecil;

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

		try
		{
			foreach (TypeDefinition Type in _assembly.MainModule.Types)
			{
				if (Type.IsNested)
				{
					Type.IsNestedPublic = true;
				}
				else
				{
					Type.IsPublic = true;
				}

				foreach (MethodDefinition Method in Type.Methods)
				{
					Method.IsPublic = true;
				}

				foreach (FieldDefinition Field in Type.Fields)
				{
					// Prevent publicize auto-generated fields
					if (Type.Events.Any(x => x.Name == Field.Name)) continue;

					if (ctor != null && !Field.IsPublic && !Field.CustomAttributes.Any(a => a.AttributeType.FullName == "UnityEngine.SerializeField"))
					{
						Field.IsNotSerialized = true;
						Field.CustomAttributes.Add(item: new CustomAttribute(ctor));
					}

					Field.IsPublic = true;
				}
			}
		}
		catch (System.Exception ex)
		{
			Logger.Error(ex.Message);
			throw ex;
		}
	}

	internal void Cleanup()
	{
		try
		{
			Override_Harmony_Methods();
			Remove_RustHarmony_Reference();
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

			MethodDefinition method = type.Methods.First(x => x.Name == Item);
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

		_assembly.MainModule.AssemblyReferences.Remove(
			_assembly.MainModule.AssemblyReferences.Single(x => x.Name == "Rust.Harmony")
		);
	}

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