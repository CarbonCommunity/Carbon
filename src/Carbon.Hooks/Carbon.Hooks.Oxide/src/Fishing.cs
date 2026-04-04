using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Cryptography;
using API.Hooks;
using Carbon;
using HarmonyLib;
using Oxide.Core;
using UnityEngine;

// Copyright (c) 2022-2024 Carbon Community
// All rights reserved

// Using game protocol 2622.282.1
// Auto generated at 2026-03-02 11:41:10

namespace Carbon.Hooks;
#pragma warning disable IDE0051
#pragma warning disable IDE0060

public partial class Category_Fishing
{
    public partial class Fishing_BaseFishingRod
    {
        [HookAttribute.Patch("OnFishingStopped", "OnFishingStopped", "BaseFishingRod", "Server_Cancel", ["BaseFishingRod/FailReason"])]
        [HookAttribute.Identifier("2425fe66f0f541e4befbb155a48ee4c4")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "BaseFishingRod")]
        [MetadataAttribute.Parameter("reason", "BaseFishingRod+FailReason")]
        [MetadataAttribute.Category("Fishing")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Fishing_BaseFishingRod_2425fe66f0f541e4befbb155a48ee4c4 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 71)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)197690385)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True BaseFishingRod 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BaseFishingRod+FailReason 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Box typeof(BaseFishingRod.FailReason) False  
                    yield return new CodeInstruction(OpCodes.Box, typeof(BaseFishingRod.FailReason));
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object), typeof(object), }));
                    // hook call end

                    // return behaviour start
                    /* ReturnBehavior.Continue */
                    // AddYieldInstruction: Pop  True  
                    yield return new CodeInstruction(OpCodes.Pop);
                    // return behaviour end

                    yield return instruction;
                }
            }
        }
    }
}

public partial class Category_Fishing
{
    public partial class Fishing_BaseFishingRod
    {
        [HookAttribute.Patch("OnFishingRodCast", "OnFishingRodCast", "BaseFishingRod", "Server_RequestCast", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("cddfadbafe5e4aa381f7d28ce5f31a3f")]
        [HookAttribute.Dependencies(new System.String[] { "CanCastFishingRod" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "BaseFishingRod")]
        [MetadataAttribute.Parameter("local1", "BasePlayer")]
        [MetadataAttribute.Parameter("local2", "Item")]
        [MetadataAttribute.Category("Fishing")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Fishing_BaseFishingRod_cddfadbafe5e4aa381f7d28ce5f31a3f : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 215)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)264708708)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True BaseFishingRod 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_1  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
                    // AddYieldInstruction: Ldloc_2  True Item 
                    yield return new CodeInstruction(OpCodes.Ldloc_2);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object), typeof(object), typeof(object), }));
                    // hook call end

                    // return behaviour start
                    /* ReturnBehavior.Continue */
                    // AddYieldInstruction: Pop  True  
                    yield return new CodeInstruction(OpCodes.Pop);
                    // return behaviour end

                    yield return instruction;
                }
            }
        }
    }
}

public partial class Category_Fishing
{
    public partial class Fishing_BaseFishingRod
    {
        [HookAttribute.Patch("OnFishCaught", "OnFishCaught", "BaseFishingRod", "CatchProcessBudgeted", [])]
        [HookAttribute.Identifier("1abfc045a3cd4648bfda4f66559d2064")]
        [HookAttribute.Dependencies(new System.String[] { "OnFishCatch" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "BaseFishingRod")]
        [MetadataAttribute.Parameter("self1", "BaseFishingRod")]
        [MetadataAttribute.Parameter("local1", "BasePlayer")]
        [MetadataAttribute.Category("Fishing")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Fishing_BaseFishingRod_1abfc045a3cd4648bfda4f66559d2064 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 618)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2806373814)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True BaseFishingRod currentFishTarget
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // Set BaseFishingRod
                    // value:currentFishTarget isProperty:False runtimeType:ItemDefinition currentType:BaseFishingRod type:BaseFishingRod
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("BaseFishingRod"), "currentFishTarget"));
                    // Read BaseFishingRod : BaseFishingRod
                    // AddYieldInstruction: Ldarg_0  True BaseFishingRod 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_1  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object), typeof(object), typeof(object), }));
                    // hook call end

                    // return behaviour start
                    /* ReturnBehavior.Continue */
                    // AddYieldInstruction: Pop  True  
                    yield return new CodeInstruction(OpCodes.Pop);
                    // return behaviour end

                    yield return instruction;
                }
            }
        }
    }
}

public partial class Category_Fishing
{
    public partial class Fishing_BaseFishingRod
    {
        [HookAttribute.Patch("CanCastFishingRod", "CanCastFishingRod", "BaseFishingRod", "Server_RequestCast", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("962753c976824801a5a4c0ef46414a76")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Fishing")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Fishing_BaseFishingRod_962753c976824801a5a4c0ef46414a76 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                edit.Add(new CodeInstruction(OpCodes.Ldstr, "CanCastFishingRod").MoveLabelsFrom(original[28]));
                edit.Add(new CodeInstruction(OpCodes.Ldloc_1));
                // Opop
                edit.Add(new CodeInstruction(OpCodes.Ldarg_0));
                edit.Add(new CodeInstruction(OpCodes.Ldloc_2));
                edit.Add(new CodeInstruction(OpCodes.Ldloc_0));
                edit.Add(new CodeInstruction(OpCodes.Box, typeof(UnityEngine.Vector3)));
                edit.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("Oxide.Core.Interface"), "CallHook", new System.Type[] { typeof(System.String), typeof(System.Object), typeof(System.Object), typeof(System.Object), typeof(System.Object) })));
                edit.Add(new CodeInstruction(OpCodes.Stloc_S, (sbyte)7));
                edit.Add(new CodeInstruction(OpCodes.Ldloc_S, (sbyte)7));
                edit.Add(new CodeInstruction(OpCodes.Isinst, typeof(System.Boolean)));
                Label label_dbfbfc7c9ec447568272da4fc3fb05f7 = Generator.DefineLabel();
                original[28].labels.Add(label_dbfbfc7c9ec447568272da4fc3fb05f7);
                edit.Add(new CodeInstruction(OpCodes.Brfalse_S, label_dbfbfc7c9ec447568272da4fc3fb05f7));
                edit.Add(new CodeInstruction(OpCodes.Ldloc_S, (sbyte)7));
                edit.Add(new CodeInstruction(OpCodes.Unbox_Any, typeof(System.Boolean)));
                edit.Add(new CodeInstruction(OpCodes.Brtrue_S, label_dbfbfc7c9ec447568272da4fc3fb05f7));
                edit.Add(new CodeInstruction(OpCodes.Ret));

                original.InsertRange(28, edit);
                return original.AsEnumerable();
            }
        }
    }
}

public partial class Category_Fishing
{
    public partial class Fishing_BaseFishingRod
    {
        [HookAttribute.Patch("CanCatchFish", "CanCatchFish", "BaseFishingRod", "CatchProcessBudgeted", [])]
        [HookAttribute.Identifier("2e882150fbe14876b54be02287ea7b8e")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Fishing")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Fishing_BaseFishingRod_2e882150fbe14876b54be02287ea7b8e : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                edit.Add(new CodeInstruction(OpCodes.Ldstr, "CanCatchFish").MoveLabelsFrom(original[514]));
                edit.Add(new CodeInstruction(OpCodes.Ldloc_1));
                // Opop
                edit.Add(new CodeInstruction(OpCodes.Ldarg_0));
                edit.Add(new CodeInstruction(OpCodes.Ldloc, 14));
                edit.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("Oxide.Core.Interface"), "CallHook", new System.Type[] { typeof(System.String), typeof(System.Object), typeof(System.Object), typeof(System.Object) })));
                LocalBuilder var_d5825eec53e04bf8b0a8bba678616ac0 = Generator.DeclareLocal(typeof(System.Object));
                edit.Add(new CodeInstruction(OpCodes.Stloc, var_d5825eec53e04bf8b0a8bba678616ac0));
                edit.Add(new CodeInstruction(OpCodes.Ldloc, 15));
                edit.Add(new CodeInstruction(OpCodes.Isinst, typeof(System.Boolean)));
                Label label_c7e938626ab0412698f1d55ebdb9a344 = Generator.DefineLabel();
                original[514].labels.Add(label_c7e938626ab0412698f1d55ebdb9a344);
                edit.Add(new CodeInstruction(OpCodes.Brfalse_S, label_c7e938626ab0412698f1d55ebdb9a344));
                edit.Add(new CodeInstruction(OpCodes.Ldloc, 15));
                edit.Add(new CodeInstruction(OpCodes.Unbox_Any, typeof(System.Boolean)));
                edit.Add(new CodeInstruction(OpCodes.Brtrue_S, label_c7e938626ab0412698f1d55ebdb9a344));
                edit.Add(new CodeInstruction(OpCodes.Ret));

                original.InsertRange(514, edit);
                return original.AsEnumerable();
            }
        }
    }
}

public partial class Category_Fishing
{
    public partial class Fishing_BaseFishingRod
    {
        [HookAttribute.Patch("OnFishCatch", "OnFishCatch", "BaseFishingRod", "CatchProcessBudgeted", [])]
        [HookAttribute.Identifier("e0a6f84b74ff489681ea9377bb11a6c3")]
        [HookAttribute.Dependencies(new System.String[] { "CanCatchFish" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Fishing")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Fishing_BaseFishingRod_e0a6f84b74ff489681ea9377bb11a6c3 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                edit.Add(new CodeInstruction(OpCodes.Ldstr, "OnFishCatch").MoveLabelsFrom(original[527]));
                edit.Add(new CodeInstruction(OpCodes.Ldloc_S, (sbyte)14));
                // Opop
                edit.Add(new CodeInstruction(OpCodes.Ldarg_0));
                edit.Add(new CodeInstruction(OpCodes.Ldloc_1));
                edit.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("Oxide.Core.Interface"), "CallHook", new System.Type[] { typeof(System.String), typeof(System.Object), typeof(System.Object), typeof(System.Object) })));
                LocalBuilder var_ab5808e80df344978106b742c6c54363 = Generator.DeclareLocal(typeof(System.Object));
                edit.Add(new CodeInstruction(OpCodes.Stloc_S, var_ab5808e80df344978106b742c6c54363));
                edit.Add(new CodeInstruction(OpCodes.Ldloc_S, (sbyte)16));
                edit.Add(new CodeInstruction(OpCodes.Isinst, typeof(Item)));
                Label label_3c6d86db51534846b3579886911ee9ee = Generator.DefineLabel();
                original[527].labels.Add(label_3c6d86db51534846b3579886911ee9ee);
                edit.Add(new CodeInstruction(OpCodes.Brfalse_S, label_3c6d86db51534846b3579886911ee9ee));
                edit.Add(new CodeInstruction(OpCodes.Ldloc_S, (sbyte)16));
                edit.Add(new CodeInstruction(OpCodes.Isinst, typeof(Item)));
                edit.Add(new CodeInstruction(OpCodes.Ldloc_S, (sbyte)14));
                edit.Add(new CodeInstruction(OpCodes.Beq_S, label_3c6d86db51534846b3579886911ee9ee));
                edit.Add(new CodeInstruction(OpCodes.Ldloc_S, (sbyte)14));
                edit.Add(new CodeInstruction(OpCodes.Ldc_R4, 0f));
                edit.Add(new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("Item"), "Remove", new System.Type[] { typeof(System.Single) })));
                edit.Add(new CodeInstruction(OpCodes.Ldloc_S, (sbyte)16));
                edit.Add(new CodeInstruction(OpCodes.Castclass, typeof(Item)));
                edit.Add(new CodeInstruction(OpCodes.Stloc_S, (sbyte)14));

                original.InsertRange(527, edit);
                return original.AsEnumerable();
            }
        }
    }
}

