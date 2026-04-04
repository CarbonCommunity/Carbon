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

public partial class Category_Pet
{
    public partial class Pet_FrankensteinTable
    {
        [HookAttribute.Patch("OnFrankensteinPetWake", "OnFrankensteinPetWake [FrankensteinTable]", "FrankensteinTable", "WakeFrankenstein", ["BasePlayer"])]
        [HookAttribute.Identifier("6b52363ecd784ca2afdfe59a9945ead9")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "FrankensteinTable")]
        [MetadataAttribute.Parameter("owner", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Pet")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Pet_FrankensteinTable_6b52363ecd784ca2afdfe59a9945ead9 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 10)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)584448208)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True FrankensteinTable 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object), typeof(object), }));
                    // hook call end

                    // return behaviour start
                    Label label = Generator.DefineLabel();
                    instruction.labels.Add(label);
                    // AddYieldInstruction: Ldnull  True  
                    yield return new CodeInstruction(OpCodes.Ldnull);
                    // AddYieldInstruction: Beq_S label False  
                    yield return new CodeInstruction(OpCodes.Beq_S, label);
                    // AddYieldInstruction: Ret  True  
                    yield return new CodeInstruction(OpCodes.Ret);
                    // return behaviour end

                    yield return instruction;
                }
            }
        }
    }
}

public partial class Category_Pet
{
    public partial class Pet_FrankensteinTable
    {
        [HookAttribute.Patch("OnFrankensteinPetSleep", "OnFrankensteinPetSleep [FrankensteinTable]", "FrankensteinTable", "SleepFrankenstein", ["BasePlayer"])]
        [HookAttribute.Identifier("f01d06863c534d2486aea568e4e674b9")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("local0", "FrankensteinPet")]
        [MetadataAttribute.Parameter("self", "FrankensteinTable")]
        [MetadataAttribute.Parameter("owner", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Pet")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Pet_FrankensteinTable_f01d06863c534d2486aea568e4e674b9 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 34)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1722860509)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_0  True FrankensteinPet 
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    // AddYieldInstruction: Ldarg_0  True FrankensteinTable 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object), typeof(object), typeof(object), }));
                    // hook call end

                    // return behaviour start
                    Label label = Generator.DefineLabel();
                    instruction.labels.Add(label);
                    // AddYieldInstruction: Ldnull  True  
                    yield return new CodeInstruction(OpCodes.Ldnull);
                    // AddYieldInstruction: Beq_S label False  
                    yield return new CodeInstruction(OpCodes.Beq_S, label);
                    // AddYieldInstruction: Ret  True  
                    yield return new CodeInstruction(OpCodes.Ret);
                    // return behaviour end

                    yield return instruction;
                }
            }
        }
    }
}

