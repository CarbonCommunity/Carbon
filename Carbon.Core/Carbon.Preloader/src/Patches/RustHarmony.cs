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

internal sealed class RustHarmony : MarshalByRefObject
{
	private static readonly DefaultAssemblyResolver _resolver;
	private AssemblyDefinition _assembly;
	private string _filename;

	static RustHarmony()
	{
		_resolver = new DefaultAssemblyResolver();
		_resolver.AddSearchDirectory(Context.CarbonLib);
		_resolver.AddSearchDirectory(Context.CarbonModules);
		_resolver.AddSearchDirectory(Context.CarbonManaged);
		_resolver.AddSearchDirectory(Context.GameManaged);
	}

	public RustHarmony()
	{
		_filename = Path.Combine(Context.GameManaged, "Rust.Harmony.dll");

		if (!File.Exists(_filename))
			throw new Exception($"Assembly file '{_filename}' was not found");

		_assembly = AssemblyDefinition.ReadAssembly(_filename,
			parameters: new ReaderParameters { AssemblyResolver = _resolver });
	}

	internal void Patch()
	{
		try
		{
			Override_HarmonyLoader_Methods();
		}
		catch (System.Exception ex)
		{
			Logger.Error(ex.Message);
			throw ex;
		}
	}

	private void Override_HarmonyLoader_Methods()
	{
		TypeDefinition type = _assembly.MainModule.GetType("HarmonyLoader");
		string[] Items = { "LoadHarmonyMods", "TryLoadMod", "TryUnloadMod", "LoadAssembly", "UnloadMod" };

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