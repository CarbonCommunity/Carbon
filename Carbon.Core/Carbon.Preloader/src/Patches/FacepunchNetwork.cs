using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;
using Utility;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Patches;

internal sealed class FacepunchNetwork : MarshalByRefObject
{
	private static readonly DefaultAssemblyResolver _resolver;
	private readonly Dictionary<string, string> _checksums = new();
	private readonly AssemblyDefinition _assembly;
	private string _filename;

	static FacepunchNetwork()
	{
		_resolver = new DefaultAssemblyResolver();
		_resolver.AddSearchDirectory(Context.CarbonLib);
		_resolver.AddSearchDirectory(Context.CarbonModules);
		_resolver.AddSearchDirectory(Context.CarbonManaged);
		_resolver.AddSearchDirectory(Context.GameManaged);
	}

	public FacepunchNetwork()
	{
		_filename = Path.Combine(Context.GameManaged, "Facepunch.Network.dll");

		if (!File.Exists(_filename))
			throw new Exception($"Assembly file '{_filename}' was not found");

		_assembly = AssemblyDefinition.ReadAssembly(_filename,
			parameters: new ReaderParameters { AssemblyResolver = _resolver });
	}

	internal bool IsPublic(string Type, string Field)
	{
		try
		{
			if (_assembly == null) throw new Exception("Loaded assembly is null");

			TypeDefinition t = _assembly.MainModule.Types.First(x => x.Name == Type);
			if (t == null) throw new Exception($"Unable to get type definition for '{Type}'");

			FieldDefinition f = t.Fields.First(x => x.Name == Field);
			if (f == null) throw new Exception($"Unable to get field definition for '{Field}'");

			return f.IsPublic;
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

	public static string GetMethodMSILHash(MethodDefinition method)
	{
		try
		{
			ILProcessor processor = method.Body.GetILProcessor();
			Collection<Instruction> instructions = processor.Body.Instructions;

			byte[] raw = new byte[instructions.Count * sizeof(int)];

			for (int i = 0; i < instructions.Count; i++)
			{
				Instruction instruction = instructions[i];
				int opcodeValue = (int)instruction.OpCode.Value;

				raw[i * sizeof(int) + 0] = (byte)(opcodeValue & 0xFF);
				raw[i * sizeof(int) + 1] = (byte)((opcodeValue >> 8) & 0xFF);
				raw[i * sizeof(int) + 2] = (byte)((opcodeValue >> 16) & 0xFF);
				raw[i * sizeof(int) + 3] = (byte)((opcodeValue >> 24) & 0xFF);
			}

			return Crypto.md5(raw);
		}
		catch (System.Exception)
		{
			return null;
		}
	}

	public static string GetMethodSignature(MethodDefinition method)
	{
		string methodName = method.Name;
		string typeName = method.DeclaringType.FullName.Replace("+", ".");
		string parameterList = string.Join(",", method.Parameters.Select(p => p.ParameterType.FullName));

		if (method.HasGenericParameters)
		{
			string genericList = string.Join(",", method.GenericParameters.Select(p => p.FullName));
			methodName += $"<{genericList}>";
		}

		return $"{typeName}::{methodName}({parameterList})";
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
