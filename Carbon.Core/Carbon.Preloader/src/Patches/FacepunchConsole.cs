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

internal sealed class FacepunchConsole : MarshalByRefObject
{
	private static readonly DefaultAssemblyResolver _resolver;
	private readonly Dictionary<string, string> _checksums = new();
	private readonly AssemblyDefinition _assembly;
	private string _filename;

	static FacepunchConsole()
	{
		_resolver = new DefaultAssemblyResolver();
		_resolver.AddSearchDirectory(Context.CarbonLib);
		_resolver.AddSearchDirectory(Context.CarbonModules);
		_resolver.AddSearchDirectory(Context.CarbonManaged);
		_resolver.AddSearchDirectory(Context.GameManaged);
	}

	public FacepunchConsole()
	{
		_filename = Path.Combine(Context.GameManaged, "Facepunch.Console.dll");

		if (!File.Exists(_filename))
			throw new Exception($"Assembly file '{_filename}' was not found");

		_assembly = AssemblyDefinition.ReadAssembly(_filename,
			parameters: new ReaderParameters { AssemblyResolver = _resolver });
	}

	internal bool IsPublic(string Type, string Property)
	{
		try
		{
			if (_assembly == null) throw new Exception("Loaded assembly is null");

			TypeDefinition t = _assembly.MainModule.GetType(Type);
			if (t == null) throw new Exception($"Unable to get type definition for '{Type}'");

			PropertyDefinition m = t.Properties.First(x => x.Name == Property);
			if (m == null) throw new Exception($"Unable to get property definition for '{Property}'");

			return m.SetMethod.IsPublic;
		}
		catch (System.Exception ex)
		{
			Logger.Error(ex.Message);
			throw ex;
		}
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
			Override_IndexAll_Setter();
			Override_Constructor_Modifier();
		}
		catch (System.Exception ex)
		{
			Logger.Error(ex.Message);
			throw ex;
		}
	}

	private void Override_IndexAll_Setter()
	{
		TypeDefinition type = _assembly.MainModule.GetType("ConsoleSystem/Index");
		string[] Items = { "All" };

		foreach (string Item in Items)
		{
			try
			{
				Logger.Debug($" - Patching {type.Name}.{Item}");

				PropertyDefinition method = type.Properties.Single(x => x.Name == Item);
				method.SetMethod.IsPublic = true;
			}
			catch (System.Exception e)
			{
				Logger.Debug($" - Patching failed: {e.Message}");
			}
		}
	}

	private void Override_Constructor_Modifier()
	{
		TypeDefinition type = _assembly.MainModule.GetType("ConsoleSystem/Arg");
		MethodDefinition ctor = type.GetConstructors().FirstOrDefault();

		try
		{
			ctor.IsPublic = true;
		}
		catch (Exception e)
		{
			Logger.Debug($" - Patching failed: {e.Message}");
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
