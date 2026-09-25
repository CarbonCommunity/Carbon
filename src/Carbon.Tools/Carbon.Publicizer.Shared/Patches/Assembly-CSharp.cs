using System;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace Carbon.Publicizer;

#pragma warning disable

public class AssemblyCSharp() : Patch(RustManagedDirectory, "Assembly-CSharp.dll")
{
	public override bool Execute()
	{
		if (!base.Execute()) return false;

		try
		{
			InjectBootstrap();
			InjectPackageFields();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			return false;
		}

		return true;
	}

	private void InjectBootstrap()
	{
		if (bootstrap == null)
		{
			return;
		}

		var type1 = bootstrap.MainModule.GetType("Carbon", "Bootstrap") ?? throw new Exception("Unable to get a type for 'Carbon.Bootstrap'");
		var method1 = type1.Methods.Single(x => x.Name == "Initialize") ?? throw new Exception("Unable to get a method definition for 'Tier0'");
		var type2 = assembly.MainModule.GetType("Bootstrap") ?? throw new Exception("Unable to get a type for 'Bootstrap'");
		var method2 = type2.Methods.Single(x => x.Name == "Init_Tier0") ?? throw new Exception("Unable to get a method definition for 'Init_Tier0'");

		if (method2.Body.Instructions.Any(x => x.OpCode == OpCodes.Call && x.Operand.ToString().Contains("Carbon.Bootstrap::Initialize")))
		{
			return;
		}

		var processor = method2.Body.GetILProcessor();
		var instruction = processor.Create( OpCodes.Call, assembly.MainModule.ImportReference(method1));

		method2.Body.Instructions[method2.Body.Instructions.Count - 1] = instruction;
		method2.Body.Instructions.Insert(method2.Body.Instructions.Count, processor.Create(OpCodes.Ret));
		method2.Body.OptimizeMacros();
	}

	/// <summary>Adds the fields declared by packages through [InjectField].</summary>
	private void InjectPackageFields()
	{
		foreach (var (targetType, fieldName, fieldType) in InjectedFields)
		{
			try
			{
				var type = assembly.MainModule.GetType(targetType) ?? throw new Exception($"Unable to get a type for '{targetType}'");

				if (type.Fields.Any(x => x.Name == fieldName))
				{
					continue;
				}

				type.Fields.Add(new FieldDefinition(fieldName, Mono.Cecil.FieldAttributes.Public | Mono.Cecil.FieldAttributes.NotSerialized, assembly.MainModule.ImportReference(fieldType)));
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Failed injecting field '{targetType}.{fieldName}': {ex.Message}");
			}
		}
	}
}
