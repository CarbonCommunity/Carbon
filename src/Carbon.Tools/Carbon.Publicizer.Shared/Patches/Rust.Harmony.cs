using System;
using System.IO;
using System.Linq;
using Mono.Cecil.Cil;

namespace Carbon.Publicizer;

#pragma warning disable

public class RustHarmony() : Patch(RustManagedDirectory, "Rust.Harmony.dll")
{
	public override bool Execute()
	{
		if (!base.Execute()) return false;

		try
		{
			PatchLoadHarmonyMods();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			return false;
		}

		return true;
	}

	private void PatchLoadHarmonyMods()
	{
		var harmonyLoader = assembly.MainModule.GetType("HarmonyLoader");
		var method = harmonyLoader.Methods.FirstOrDefault(x => x.Name == "LoadHarmonyMods");
		var getSwitchType = facepunchSystem.MainModule.GetType("Facepunch.CommandLine")?.Methods.FirstOrDefault(x => x.Name == "GetSwitch");

		if (getSwitchType == null || method is null || !method.HasBody)
		{
			return;
		}

		var tryingToLoadAssemblyLogMethod = harmonyLoader.NestedTypes.FirstOrDefault(x => x.FullName == "HarmonyLoader/<>c").Methods.FirstOrDefault(x => x.Name == "<LoadHarmonyMods>b__12_0").Body.Instructions;
		for (int i = 0; i < 5; i++)
		{
			tryingToLoadAssemblyLogMethod.RemoveAt(0);
		}

		var switchReference = assembly.MainModule.ImportReference(getSwitchType);
		var combineReference = assembly.MainModule.ImportReference(typeof(Path).GetMethod("Combine", [typeof(string), typeof(string)]));

		const int offset = 21;
		method.Body.Instructions.RemoveAt(offset);
		method.Body.Instructions.RemoveAt(offset);
		method.Body.Instructions.RemoveAt(offset);

		method.Body.Instructions.Insert(offset, Instruction.Create(OpCodes.Ldstr, "-harmonydir"));
		method.Body.Instructions.Insert(offset + 1, Instruction.Create(OpCodes.Ldloc_0));
		method.Body.Instructions.Insert(offset + 2, Instruction.Create(OpCodes.Ldstr, "HarmonyMods"));
		method.Body.Instructions.Insert(offset + 3, Instruction.Create(OpCodes.Call, combineReference));
		method.Body.Instructions.Insert(offset + 4, Instruction.Create(OpCodes.Call, switchReference));
	}
}
