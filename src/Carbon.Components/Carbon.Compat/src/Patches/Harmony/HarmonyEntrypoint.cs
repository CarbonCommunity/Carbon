using API.Events;
using Carbon.Compat.Converters;
using Carbon.Compat.Lib;
using HarmonyLib;
using FieldAttributes = AsmResolver.PE.DotNet.Metadata.Tables.Rows.FieldAttributes;
using MethodAttributes = AsmResolver.PE.DotNet.Metadata.Tables.Rows.MethodAttributes;

namespace Carbon.Compat.Patches.Harmony;

/*
 *
 * Copyright (c) 2023-2024 Patrette, under the GNU v3 license rights
 * Copyright (c) 2023-2024 Carbon Community, under the GNU v3 license rights
 *
 */

public class HarmonyEntrypoint : BaseHarmonyPatch
{
    public override void Apply(ModuleDefinition asm, ReferenceImporter importer, ref BaseConverter.Context context)
    {
	    if (context.NoEntrypoint)
		{
			return;
		}

        Guid guid = Guid.NewGuid();
        IEnumerable<TypeDefinition> entryPoints = asm.GetAllTypes().Where(x => x.Interfaces.Any(y=>y.Interface?.FullName == "IHarmonyModHooks"));

        CodeGenHelpers.GenerateEntrypoint(asm, importer, HarmonyStr, guid, out MethodDefinition load, out MethodDefinition unload, out TypeDefinition entryDef);

        load.CilMethodBody = new CilMethodBody(load);
        unload.CilMethodBody = new CilMethodBody(unload);
        unload.CilMethodBody.Instructions.Add(CilOpCodes.Ret);

        MethodDefinition postHookLoad = new MethodDefinition("postHookLoad", MethodAttributes.CompilerControlled, MethodSignature.CreateInstance(asm.CorLibTypeFactory.Void, importer.ImportTypeSignature(typeof(EventArgs))));
        postHookLoad.CilMethodBody = new CilMethodBody(postHookLoad);

        FieldDefinition loadedField = new FieldDefinition("loaded", FieldAttributes.PrivateScope, new FieldSignature(asm.CorLibTypeFactory.Boolean));
        int postHookIndex = 0;

        CodeGenHelpers.GenerateCarbonEventCall(load.CilMethodBody, importer, ref postHookIndex, CarbonEvent.HookValidatorRefreshed, postHookLoad, new CilInstruction(CilOpCodes.Ldarg_0));

        load.CilMethodBody.Instructions.Add(new CilInstruction(CilOpCodes.Ret));

        CilInstruction postHookRet = new CilInstruction(CilOpCodes.Ret);

        postHookLoad.CilMethodBody.Instructions.AddRange(new[]
        {
	        // load check
	        new CilInstruction(CilOpCodes.Ldarg_0),
	        new CilInstruction(CilOpCodes.Ldfld, loadedField),
	        new CilInstruction(CilOpCodes.Brtrue_S, postHookRet.CreateLabel()),
	        new CilInstruction(CilOpCodes.Ldarg_0),
	        new CilInstruction(CilOpCodes.Ldc_I4_1),
	        new CilInstruction(CilOpCodes.Stfld, loadedField),

	        // harmony patch all
	        new CilInstruction(CilOpCodes.Ldstr, $"__CCL:{asm.Assembly.Name}:{guid:N}"),
	        new CilInstruction(CilOpCodes.Newobj, importer.ImportMethod(AccessTools.Constructor(typeof(HarmonyLib.Harmony), [typeof(string)]))),
	        new CilInstruction(CilOpCodes.Callvirt, importer.ImportMethod(AccessTools.Method(typeof(HarmonyLib.Harmony), "PatchAll")))
        });

        postHookLoad.CilMethodBody.Instructions.Add(postHookRet);
        entryDef.Methods.Add(postHookLoad);
        entryDef.Fields.Add(loadedField);
    }
}
