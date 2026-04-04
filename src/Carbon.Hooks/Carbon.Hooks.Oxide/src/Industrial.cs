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

public partial class Category_Industrial
{
    public partial class Industrial_IndustrialConveyor
    {
        [HookAttribute.Patch("OnConveyorFiltersChange", "OnConveyorFiltersChange", "IndustrialConveyor", "RPC_ChangeFilters", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("847e99e28af042bb888eb10c20dc913f")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "IndustrialConveyor")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Parameter("local0", "ProtoBuf.IndustrialConveyor+ItemFilterList")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Industrial")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Industrial_IndustrialConveyor_847e99e28af042bb888eb10c20dc913f : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 27)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)874753198)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True IndustrialConveyor 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BaseEntity+RPCMessage player
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // Set BaseEntity+RPCMessage
                    // value:player isProperty:False runtimeType:BasePlayer currentType:BaseEntity+RPCMessage type:BaseEntity+RPCMessage
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("BaseEntity+RPCMessage"), "player"));
                    // Read BaseEntity+RPCMessage : BaseEntity+RPCMessage
                    // WAAAAAAA 1
                    // AddYieldInstruction: Ldloc_0  True ProtoBuf.IndustrialConveyor+ItemFilterList 
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
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

public partial class Category_Industrial
{
    public partial class Industrial_IndustrialCrafter
    {
        [HookAttribute.Patch("OnItemCraft", "OnItemCraft [IndustrialCrafter]", "IndustrialCrafter", "RunJob", [])]
        [HookAttribute.Identifier("74e27a8a03cd4da2878611ccc3234d5a")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "IndustrialCrafter")]
        [MetadataAttribute.Parameter("local2", "ItemBlueprint")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Industrial")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Industrial_IndustrialCrafter_74e27a8a03cd4da2878611ccc3234d5a : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 42)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)276522030)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True IndustrialCrafter 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_2  True ItemBlueprint 
                    yield return new CodeInstruction(OpCodes.Ldloc_2);
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

