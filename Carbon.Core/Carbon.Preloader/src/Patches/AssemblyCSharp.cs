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

internal sealed class AssemblyCSharp : MarshalByRefObject
{
	private static readonly DefaultAssemblyResolver _resolver;
	private readonly Dictionary<string, string> _checksums = new();
	private readonly AssemblyDefinition _assembly;
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
			Add_Bootstrap_Tier0_Hook();
			Add_the_Fucking_IPlayer_shit();
			InjectCustomData();
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
			try
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
			catch (System.Exception e)
			{
				Logger.Debug($" - Patching failed: {e.Message}");
			}
		}
	}

	internal void Add_Bootstrap_Tier0_Hook()
	{
		try
		{
			AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(
				new MemoryStream(File.ReadAllBytes(Path.Combine(Context.CarbonManaged, "Carbon.Bootstrap.dll"))));

			TypeDefinition type1 = assembly.MainModule.GetType("Carbon", "Bootstrap")
				?? throw new Exception("Unable to get a type for 'Carbon.Bootstrap'");

			MethodDefinition method1 = type1.Methods.Single(x => x.Name == "Initialize")
				?? throw new Exception("Unable to get a method definition for 'Tier0'");

			TypeDefinition type2 = _assembly.MainModule.GetType("Bootstrap")
				?? throw new Exception("Unable to get a type for 'Bootstrap'");

			MethodDefinition method2 = type2.Methods.Single(x => x.Name == "Init_Tier0")
				?? throw new Exception("Unable to get a method definition for 'Init_Tier0'");

			ILProcessor processor = method2.Body.GetILProcessor();
			Instruction instruction = processor.Create(
				OpCodes.Call, _assembly.MainModule.ImportReference(method1));

			if (method2.Body.Instructions.Any(x => x.OpCode == OpCodes.Call
				&& x.Operand.ToString().Contains("Carbon.Bootstrap::Initialize"))) return;

			Logger.Debug($" - Patching Bootstrap.Init_Tier0");

			method2.Body.Instructions[method2.Body.Instructions.Count - 1]
				= instruction;

			method2.Body.Instructions.Insert(method2.Body.Instructions.Count,
				processor.Create(OpCodes.Ret));
			method2.Body.OptimizeMacros();
		}
		catch (System.Exception e)
		{
			Logger.Debug($" - Patching Bootstrap.Init_Tier0 failed: {e.Message}");
		}
	}

	internal void Add_the_Fucking_IPlayer_shit()
	{
		try
		{
			FieldDefinition iplayer = _assembly.MainModule.GetType("BasePlayer").Fields.FirstOrDefault(x => x.Name == "IPlayer");
			if (iplayer is not null) return;

			Logger.Debug($" - Patching BasePlayer.IPlayer");

			AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(
				new MemoryStream(File.ReadAllBytes(Path.Combine(Context.CarbonManaged, "Carbon.Common.dll"))));

			TypeDefinition type1 = assembly.MainModule.GetType("Oxide.Core.Libraries.Covalence", "IPlayer")
				?? throw new Exception("Unable to get a type for 'API.Contracts.IPlayer'");

			TypeDefinition type2 = _assembly.MainModule.GetType("BasePlayer")
				?? throw new Exception("Unable to get a type for 'BasePlayer'");

			type2.Fields.Add(item: new FieldDefinition("IPlayer",
				FieldAttributes.Public | FieldAttributes.NotSerialized, _assembly.MainModule.ImportReference(type1)));
		}
		catch (System.Exception e)
		{
			Logger.Debug($" - Patching BasePlayer.IPlayer failed: {e.Message}");
		}
	}

	private void InjectCustomData()
	{
		TypeDefinition baseEntityType = _assembly.MainModule.GetType("BaseEntity");
		//("Carbon.Components.CustomData.ICustomSerializable"
		if (baseEntityType.Fields.Any(x => x.Name == "additional_data_cache")) return;

		// interface

		AssemblyDefinition commonAsm = AssemblyDefinition.ReadAssembly(
			new MemoryStream(File.ReadAllBytes(Path.Combine(Context.CarbonManaged, "Carbon.Common.dll"))));

		AssemblyNameReference carbonCommonRef =
			_assembly.MainModule.AssemblyReferences.FirstOrDefault(a => a.Name == "Carbon.Common")
			?? new AssemblyNameReference(commonAsm.Name.Name, commonAsm.Name.Version);

		TypeReference seInterface = _assembly.MainModule.ImportReference(new TypeReference("Carbon.Contracts", "ICustomSerializable", _assembly.MainModule, carbonCommonRef));


		// fields

		TypeReference byteDictRef = _assembly.MainModule.ImportReference(typeof(Dictionary<string, byte[]>));

		TypeReference cacheDictRef = _assembly.MainModule.ImportReference(_assembly.MainModule.ImportReference(typeof(Dictionary<,>))
			.MakeGenericInstanceType(_assembly.MainModule.TypeSystem.String, seInterface));

		FieldDefinition cacheField =
			new FieldDefinition("additional_data_cache", FieldAttributes.Private, cacheDictRef);

		FieldDefinition rawField =
			new FieldDefinition("additional_data_raw", FieldAttributes.Private, byteDictRef);

		baseEntityType.Fields.Add(cacheField);

		baseEntityType.Fields.Add(rawField);

		// methods

		TypeDefinition customDataInternals = commonAsm.MainModule.GetType("Carbon.Components", "CustomDataInternals");


		GetAdditionalData();
		TryGetAdditionalData();
		GetOrCreateAdditionalData();
		
		HasAdditionalData();
		HasAnyAdditionalData();
		
		AddAdditionalData();
		
		ClearAdditionalData();
		DeleteAdditionalData();

		void HasAnyAdditionalData()
		{
			MethodDefinition method = new MethodDefinition("HasAnyAdditionalData", MethodAttributes.Public | MethodAttributes.HideBySig, _assembly.MainModule.TypeSystem.Boolean)
			{
				Body = { InitLocals = true }
			};

			ILProcessor IL = method.Body.GetILProcessor();
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldflda, cacheField);
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldflda, rawField);
			IL.Emit(OpCodes.Call, _assembly.MainModule.ImportReference(customDataInternals.Methods.First(x => x.Name == "HasAnyAdditionalData")));
			IL.Emit(OpCodes.Ret);

			baseEntityType.Methods.Add(method);
		}
		void HasAdditionalData()
		{
			MethodDefinition method = new MethodDefinition("HasAdditionalData", MethodAttributes.Public | MethodAttributes.HideBySig, _assembly.MainModule.TypeSystem.Boolean)
			{
				Body = { InitLocals = true }
			};
			method.Parameters.Add(new ParameterDefinition("name", ParameterAttributes.None, _assembly.MainModule.TypeSystem.String));

			ILProcessor IL = method.Body.GetILProcessor();
			IL.Emit(OpCodes.Ldarg_1);
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldflda, cacheField);
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldflda, rawField);
			IL.Emit(OpCodes.Call, _assembly.MainModule.ImportReference(customDataInternals.Methods.First(x => x.Name == "HasAdditionalData")));
			IL.Emit(OpCodes.Ret);

			baseEntityType.Methods.Add(method);
		}
		void GetAdditionalData()
		{
			MethodDefinition method = new MethodDefinition("GetAdditionalData", MethodAttributes.Public | MethodAttributes.HideBySig, _assembly.MainModule.TypeSystem.Void);
			GenericParameter arg0 = new Mono.Cecil.GenericParameter("T", method);
			method.GenericParameters.Add(arg0);
			arg0.HasDefaultConstructorConstraint = true;
			arg0.HasReferenceTypeConstraint = true;
			arg0.Constraints.Add(new GenericParameterConstraint(seInterface));
			method.ReturnType = arg0;
			method.Body.InitLocals = true;
			method.Parameters.Add(new ParameterDefinition("name", ParameterAttributes.None, _assembly.MainModule.TypeSystem.String));

			ILProcessor IL = method.Body.GetILProcessor();
			IL.Emit(OpCodes.Ldarg_1);
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldflda, cacheField);
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldflda, rawField);
			MethodReference call = _assembly.MainModule.ImportReference(customDataInternals.Methods.First(x => x.Name == "GetAdditionalData"));
			call.Parameters[1].ParameterType = baseEntityType;
			GenericInstanceMethod ge =
				new GenericInstanceMethod(call);
			ge.GenericArguments.Add(arg0);
			IL.Emit(OpCodes.Call, _assembly.MainModule.ImportReference(ge));
			IL.Emit(OpCodes.Ret);

			baseEntityType.Methods.Add(method);
		}
		void TryGetAdditionalData()
		{
			MethodDefinition method = new MethodDefinition("TryGetAdditionalData", MethodAttributes.Public | MethodAttributes.HideBySig, _assembly.MainModule.TypeSystem.Boolean);
			GenericParameter arg0 = new Mono.Cecil.GenericParameter("T", method);
			method.GenericParameters.Add(arg0);
			arg0.HasDefaultConstructorConstraint = true;
			arg0.HasReferenceTypeConstraint = true;
			arg0.Constraints.Add(new GenericParameterConstraint(seInterface));
			method.Body.InitLocals = true;
			method.Parameters.Add(new ParameterDefinition("name", ParameterAttributes.None, _assembly.MainModule.TypeSystem.String));
			method.Parameters.Add(new ParameterDefinition("ret", ParameterAttributes.Out, arg0.MakeByReferenceType()));

			ILProcessor IL = method.Body.GetILProcessor();
			IL.Emit(OpCodes.Ldarg_1);
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldarg_2);
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldflda, cacheField);
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldflda, rawField);
			MethodReference call = _assembly.MainModule.ImportReference(customDataInternals.Methods.First(x => x.Name == "TryGetAdditionalData"));
			call.Parameters[1].ParameterType = baseEntityType;
			GenericInstanceMethod ge =
				new GenericInstanceMethod(call);
			ge.GenericArguments.Add(arg0);
			IL.Emit(OpCodes.Call, _assembly.MainModule.ImportReference(ge));
			IL.Emit(OpCodes.Ret);

			baseEntityType.Methods.Add(method);
		}
		void GetOrCreateAdditionalData()
		{
			MethodDefinition method = new MethodDefinition("GetOrCreateAdditionalData", MethodAttributes.Public | MethodAttributes.HideBySig, _assembly.MainModule.TypeSystem.Void);
			GenericParameter arg0 = new Mono.Cecil.GenericParameter("T", method);
			method.GenericParameters.Add(arg0);
			arg0.HasDefaultConstructorConstraint = true;
			arg0.HasReferenceTypeConstraint = true;
			arg0.Constraints.Add(new GenericParameterConstraint(seInterface));
			method.ReturnType = arg0; ;
			method.Body.InitLocals = true;
			method.Parameters.Add(new ParameterDefinition("name", ParameterAttributes.None, _assembly.MainModule.TypeSystem.String));

			ILProcessor IL = method.Body.GetILProcessor();
			IL.Emit(OpCodes.Ldarg_1);
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldflda, cacheField);
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldflda, rawField);
			MethodReference call = _assembly.MainModule.ImportReference(customDataInternals.Methods.First(x => x.Name == "GetOrCreateAdditionalData"));
			call.Parameters[1].ParameterType = baseEntityType;
			GenericInstanceMethod ge =
				new GenericInstanceMethod(call);
			ge.GenericArguments.Add(arg0);
			IL.Emit(OpCodes.Call, _assembly.MainModule.ImportReference(ge));
			IL.Emit(OpCodes.Ret);

			baseEntityType.Methods.Add(method);
		}
		void AddAdditionalData()
		{
			MethodDefinition method = new MethodDefinition("AddAdditionalData", MethodAttributes.Public | MethodAttributes.HideBySig, _assembly.MainModule.TypeSystem.Void)
			{
				Body = { InitLocals = true }
			};
			method.Parameters.Add(new ParameterDefinition("name", ParameterAttributes.None, _assembly.MainModule.TypeSystem.String));
			method.Parameters.Add(new ParameterDefinition("instance", ParameterAttributes.None, seInterface));

			ILProcessor IL = method.Body.GetILProcessor();
			IL.Emit(OpCodes.Ldarg_1);
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldarg_2);
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldflda, cacheField);
			MethodReference call = _assembly.MainModule.ImportReference(customDataInternals.Methods.First(x => x.Name == "AddAdditionalData"));
			call.Parameters[1].ParameterType = baseEntityType;
			IL.Emit(OpCodes.Call, call);
			IL.Emit(OpCodes.Ret);

			baseEntityType.Methods.Add(method);
		}
		void DeleteAdditionalData()
		{
			MethodDefinition method = new MethodDefinition("DeleteAdditionalData", MethodAttributes.Public | MethodAttributes.HideBySig, _assembly.MainModule.TypeSystem.Boolean)
			{
				Body = { InitLocals = true }
			};
			method.Parameters.Add(new ParameterDefinition("name", ParameterAttributes.None, _assembly.MainModule.TypeSystem.String));

			ILProcessor IL = method.Body.GetILProcessor();
			IL.Emit(OpCodes.Ldarg_1);
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldflda, cacheField);
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldflda, rawField);
			IL.Emit(OpCodes.Call, _assembly.MainModule.ImportReference(customDataInternals.Methods.First(x => x.Name == "DeleteAdditionalData")));
			IL.Emit(OpCodes.Ret);

			baseEntityType.Methods.Add(method);
		}
		void ClearAdditionalData()
		{
			MethodDefinition method = new MethodDefinition("ClearAdditionalData", MethodAttributes.Public | MethodAttributes.HideBySig, _assembly.MainModule.TypeSystem.Void)
			{
				Body = { InitLocals = true }
			};
			method.Parameters.Add(new ParameterDefinition("name", ParameterAttributes.None, _assembly.MainModule.TypeSystem.String));

			ILProcessor IL = method.Body.GetILProcessor();
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldflda, cacheField);
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldflda, rawField);
			IL.Emit(OpCodes.Call, _assembly.MainModule.ImportReference(customDataInternals.Methods.First(x => x.Name == "ClearAdditionalData")));
			IL.Emit(OpCodes.Ret);

			baseEntityType.Methods.Add(method);
		}
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
