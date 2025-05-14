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
			InjectIPlayer();
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

	private void InjectIPlayer()
	{
		var iplayer = assembly.MainModule.GetType("BasePlayer").Fields.FirstOrDefault(x => x.Name == "IPlayer");

		if (iplayer is not null)
		{
			return;
		}

		try
		{
			var iPlayerType = common.MainModule.GetType("Oxide.Core.Libraries.Covalence", "IPlayer") ?? throw new Exception("Unable to get a type for 'API.Contracts.IPlayer'");
			var basePlayerType = assembly.MainModule.GetType("BasePlayer") ?? throw new Exception("Unable to get a type for 'BasePlayer'");
			basePlayerType.Fields.Add(item: new FieldDefinition("IPlayer", FieldAttributes.Public | FieldAttributes.NotSerialized, assembly.MainModule.ImportReference(iPlayerType)));
		}
		catch { }
	}
}
