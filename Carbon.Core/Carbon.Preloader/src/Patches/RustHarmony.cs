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
#pragma warning disable IDE0051

internal sealed class RustHarmony : MarshalByRefObject
{
	private static readonly DefaultAssemblyResolver _resolver;
	private readonly AssemblyDefinition _assembly;
	private readonly string _filename;

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
			//Nuke_them_all();
			Clean_HarmonyLoader_Methods();
			Clean_HarmonyLoader_HarmonyMod();
			Remove_Harmony_Reference();
		}
		catch (System.Exception ex)
		{
			Logger.Error(ex.Message);
			throw ex;
		}
	}

	private void Clean_HarmonyLoader_Methods()
	{
		TypeDefinition type = _assembly.MainModule.GetType("HarmonyLoader");
		string[] Items = { "LoadHarmonyMods", "TryLoadMod", "TryUnloadMod", "LoadAssembly", "UnloadMod" };

		foreach (string Item in Items)
		{
			try
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
			catch (System.Exception e)
			{
				Logger.Debug($" - Patching failed: {e.Message}");
			}
		}
	}

	private void Clean_HarmonyLoader_HarmonyMod()
	{
		TypeDefinition parent = _assembly.MainModule.GetType("HarmonyLoader");
		TypeDefinition child = parent.NestedTypes.First(x => x.Name == "HarmonyMod");

		string[] Items = { "Harmony" };

		foreach (string Item in Items)
		{
			try
			{
				PropertyDefinition prop = child.Properties.FirstOrDefault(x => x.Name == Item);
				if (prop is null) return;

				Logger.Debug($" - Patching {prop}");

				child.Methods.Remove(prop.GetMethod);
				child.Methods.Remove(prop.SetMethod);
				child.Properties.Remove(prop);

				FieldDefinition backingField = child.Fields.First(x => x.Name == $"<{Item}>k__BackingField");
				child.Fields.Remove(backingField);
			}
			catch (System.Exception e)
			{
				Logger.Debug($" - Patching failed: {e.Message}");
			}
		}
	}

	private void Remove_Harmony_Reference()
	{
		try
		{
			AssemblyNameReference harmony =
				_assembly.MainModule.AssemblyReferences.FirstOrDefault(x => x.Name == "0Harmony");

			if (harmony is null) return;
			_assembly.MainModule.AssemblyReferences.Remove(harmony);
			Logger.Debug($" - Remove '0Harmony' reference");
		}
		catch (System.Exception e)
		{
			Logger.Debug($" - Remove reference failed: {e.Message}");
		}
	}

	private void Nuke_them_all()
	{
		string[] whitelist = {
			"HarmonyModInfo",
			"IHarmonyModHooks",
			"OnHarmonyModLoadedArgs",
			"OnHarmonyModUnloadedArgs"
		};

		foreach (TypeDefinition type in _assembly.MainModule.GetTypes())
		{
			if (!whitelist.Contains(type.Name))
				_assembly.MainModule.Types.Remove(type);
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