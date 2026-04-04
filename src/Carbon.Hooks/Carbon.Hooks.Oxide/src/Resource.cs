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
// Auto generated at 2026-03-02 11:41:09

namespace Carbon.Hooks;
#pragma warning disable IDE0051
#pragma warning disable IDE0060

public partial class Category_Resource
{
    public partial class Resource_ResourceDispenser
    {
        [HookAttribute.Patch("OnDispenserGather", "OnDispenserGather", "ResourceDispenser", "GiveResourceFromItem", ["BasePlayer", "ItemAmount", "System.Single", "System.Single", "AttackEntity"])]
        [HookAttribute.Identifier("45a32f002f33484595b411147b4953fd")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "ResourceDispenser")]
        [MetadataAttribute.Parameter("entity", "BasePlayer")]
        [MetadataAttribute.Parameter("local7", "Item")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Resource")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Resource_ResourceDispenser_45a32f002f33484595b411147b4953fd : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 119)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2949903609)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True ResourceDispenser 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Ldloc_S 7 False  
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 7);
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

public partial class Category_Resource
{
    public partial class Resource_SurveyCharge
    {
        [HookAttribute.Patch("OnSurveyGather", "OnSurveyGather", "SurveyCharge", "Explode", [])]
        [HookAttribute.Identifier("345bc5b19a674aa9a3c4582d14f709ca")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "SurveyCharge")]
        [MetadataAttribute.Category("Resource")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Resource_SurveyCharge_345bc5b19a674aa9a3c4582d14f709ca : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                Type rettype = ((MethodInfo)original[115].operand).ReturnType;
                object retvar = Generator.DeclareLocal(rettype);

                edit.Add(new CodeInstruction(OpCodes.Stloc_S, retvar));
                edit.Add(new CodeInstruction(OpCodes.Ldloc_S, retvar));

                original.InsertRange(116, edit);
                Instructions = original.AsEnumerable();

                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 118)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3947927302)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True SurveyCharge 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldloc_S, retvar);
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

public partial class Category_Resource
{
    public partial class Resource_ResourceDepositManager
    {
        [HookAttribute.Patch("OnResourceDepositCreated", "OnResourceDepositCreated", "ResourceDepositManager", "CreateFromPosition", ["UnityEngine.Vector3"])]
        [HookAttribute.Identifier("b90f76f6b42f4bd4bfb301efa9c07972")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("local1", "ResourceDepositManager+ResourceDeposit")]
        [MetadataAttribute.Category("Resource")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Resource_ResourceDepositManager_b90f76f6b42f4bd4bfb301efa9c07972 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 255)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)4140107910)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_1  True ResourceDepositManager+ResourceDeposit 
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object), }));
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

public partial class Category_Resource
{
    public partial class Resource_ResourceDispenser
    {
        [HookAttribute.Patch("OnDispenserBonus", "OnDispenserBonus", "ResourceDispenser", "AssignFinishBonus", ["BasePlayer", "System.Single", "AttackEntity"])]
        [HookAttribute.Identifier("7fbda78772af4b80a906483e8f9b3ea8")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "ResourceDispenser")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Parameter("local4", "Item")]
        [MetadataAttribute.Category("Resource")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Resource_ResourceDispenser_7fbda78772af4b80a906483e8f9b3ea8 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 52)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2399681302)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True ResourceDispenser 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Ldloc_S 4 False  
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 4);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object), typeof(object), typeof(object), }));
                    // hook call end

                    // return behaviour start
                    Label label1 = Generator.DefineLabel();
                    instruction.labels.Add(label1);
                    object retvar = Generator.DeclareLocal(typeof(object));
                    // AddYieldInstruction: Stloc retvar False  
                    yield return new CodeInstruction(OpCodes.Stloc, retvar);
                    // AddYieldInstruction: Ldloc retvar False  
                    yield return new CodeInstruction(OpCodes.Ldloc, retvar);
                    // AddYieldInstruction: Isinst typeof(Item) False  
                    yield return new CodeInstruction(OpCodes.Isinst, typeof(Item));
                    // AddYieldInstruction: Ldnull  True  
                    yield return new CodeInstruction(OpCodes.Ldnull);
                    // AddYieldInstruction: Beq_S label1 False  
                    yield return new CodeInstruction(OpCodes.Beq_S, label1);
                    // AddYieldInstruction: Ldloc retvar False  
                    yield return new CodeInstruction(OpCodes.Ldloc, retvar);
                    // AddYieldInstruction: Unbox_Any typeof(Item) False  
                    yield return new CodeInstruction(OpCodes.Unbox_Any, typeof(Item));
                    // AddYieldInstruction: Stloc_S 4 False  
                    yield return new CodeInstruction(OpCodes.Stloc_S, 4);

                    yield return instruction;
                }
            }
        }
    }
}

public partial class Category_Resource
{
    public partial class Resource_LootContainer
    {
        [HookAttribute.Patch("OnLootSpawn", "OnLootSpawn [LootContainer]", "LootContainer", "SpawnLoot", [])]
        [HookAttribute.Identifier("dd45cc6eec7b4d54a198fbcaf154cf08")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "LootContainer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Resource")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Resource_LootContainer_dd45cc6eec7b4d54a198fbcaf154cf08 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 15)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)767976070)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Call AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object), }) False  
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object), }));
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

public partial class Category_Resource
{
    public partial class Resource_CollectibleEntity
    {
        [HookAttribute.Patch("OnCollectiblePickup", "OnCollectiblePickup", "CollectibleEntity", "DoPickup", ["BasePlayer", "System.Boolean"])]
        [HookAttribute.Identifier("cbe3116dac354c169cce08a9b0605102")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "CollectibleEntity")]
        [MetadataAttribute.Parameter("reciever", "BasePlayer")]
        [MetadataAttribute.Parameter("eat", "System.Boolean")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Resource")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Resource_CollectibleEntity_cbe3116dac354c169cce08a9b0605102 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 4)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3290943891)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True CollectibleEntity 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // HERE!
                    // AddYieldInstruction: Ldarg_2  True System.Boolean 
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Box typeof(System.Boolean) False  
                    yield return new CodeInstruction(OpCodes.Box, typeof(System.Boolean));
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

public partial class Category_Resource
{
    public partial class Resource_ExcavatorArm
    {
        [HookAttribute.Patch("OnExcavatorGather", "OnExcavatorGather", "ExcavatorArm", "ProduceResources", [])]
        [HookAttribute.Identifier("0ae3edaa8fe147ad999e6d579bfca969")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "ExcavatorArm")]
        [MetadataAttribute.Parameter("local8", "Item")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Resource")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Resource_ExcavatorArm_0ae3edaa8fe147ad999e6d579bfca969 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 80)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2447060701)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True ExcavatorArm 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_S 8 False  
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 8);
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

public partial class Category_Resource
{
    public partial class Resource_ExcavatorArm
    {
        [HookAttribute.Patch("OnExcavatorMiningToggled", "OnExcavatorMiningToggled [start]", "ExcavatorArm", "BeginMining", [])]
        [HookAttribute.Identifier("9bb09dc153544f7e99c4e9a5b63c9ab0")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "ExcavatorArm")]
        [MetadataAttribute.Category("Resource")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Resource_ExcavatorArm_9bb09dc153544f7e99c4e9a5b63c9ab0 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 46)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3475217791)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Call AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object), }) False  
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object), }));
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

public partial class Category_Resource
{
    public partial class Resource_ExcavatorArm
    {
        [HookAttribute.Patch("OnExcavatorMiningToggled", "OnExcavatorMiningToggled [stop]", "ExcavatorArm", "StopMining", [])]
        [HookAttribute.Identifier("84e6b7504a3d4616921a4056338b698b")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "ExcavatorArm")]
        [MetadataAttribute.Category("Resource")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Resource_ExcavatorArm_84e6b7504a3d4616921a4056338b698b : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 14)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3475217791)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Call AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object), }) False  
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object), }));
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

public partial class Category_Resource
{
    public partial class Resource_ExcavatorArm
    {
        [HookAttribute.Patch("OnExcavatorResourceSet", "OnExcavatorResourceSet", "ExcavatorArm", "RPC_SetResourceTarget", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("0350751e9ab04fe4816948de1afaeeab")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "ExcavatorArm")]
        [MetadataAttribute.Parameter("local0", "System.String")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Resource")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Resource_ExcavatorArm_0350751e9ab04fe4816948de1afaeeab : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 6)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)972234103)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True ExcavatorArm 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_0  True System.String 
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BaseEntity+RPCMessage player
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // Set BaseEntity+RPCMessage
                    // value:player isProperty:False runtimeType:BasePlayer currentType:BaseEntity+RPCMessage type:BaseEntity+RPCMessage
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("BaseEntity+RPCMessage"), "player"));
                    // Read BaseEntity+RPCMessage : BaseEntity+RPCMessage
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

public partial class Category_Resource
{
    public partial class Resource_GrowableEntity
    {
        [HookAttribute.Patch("OnGrowableGathered", "OnGrowableGathered", "GrowableEntity", "GiveFruit", ["BasePlayer", "System.Int32", "System.Boolean", "System.Boolean"])]
        [HookAttribute.Identifier("370c2a1f920d410b948de1faa7104dda")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "GrowableEntity")]
        [MetadataAttribute.Parameter("local0", "Item")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Category("Resource")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Resource_GrowableEntity_370c2a1f920d410b948de1faa7104dda : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 50)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2863302180)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True GrowableEntity 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_0  True Item 
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
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

public partial class Category_Resource
{
    public partial class Resource_MiningQuarry
    {
        [HookAttribute.Patch("OnQuarryConsumeFuel", "OnQuarryConsumeFuel", "MiningQuarry", "FuelCheck", [])]
        [HookAttribute.Identifier("a50c537f798d4a639c12360f2760877f")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "MiningQuarry")]
        [MetadataAttribute.Parameter("local0", "Item")]
        [MetadataAttribute.Category("Resource")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Resource_MiningQuarry_a50c537f798d4a639c12360f2760877f : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 14)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1723311060)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True MiningQuarry 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_0  True Item 
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object), typeof(object), }));
                    // hook call end

                    // return behaviour start
                    Label label1 = Generator.DefineLabel();
                    instruction.labels.Add(label1);
                    object retvar = Generator.DeclareLocal(typeof(object));
                    // AddYieldInstruction: Stloc retvar False  
                    yield return new CodeInstruction(OpCodes.Stloc, retvar);
                    // AddYieldInstruction: Ldloc retvar False  
                    yield return new CodeInstruction(OpCodes.Ldloc, retvar);
                    // AddYieldInstruction: Isinst typeof(Item) False  
                    yield return new CodeInstruction(OpCodes.Isinst, typeof(Item));
                    // AddYieldInstruction: Ldnull  True  
                    yield return new CodeInstruction(OpCodes.Ldnull);
                    // AddYieldInstruction: Beq_S label1 False  
                    yield return new CodeInstruction(OpCodes.Beq_S, label1);
                    // AddYieldInstruction: Ldloc retvar False  
                    yield return new CodeInstruction(OpCodes.Ldloc, retvar);
                    // AddYieldInstruction: Unbox_Any typeof(Item) False  
                    yield return new CodeInstruction(OpCodes.Unbox_Any, typeof(Item));
                    // AddYieldInstruction: Stloc_0  True  
                    yield return new CodeInstruction(OpCodes.Stloc_0);

                    yield return instruction;
                }
            }
        }
    }
}

public partial class Category_Resource
{
    public partial class Resource_GrowableEntity
    {
        [HookAttribute.Patch("OnGrowableGather", "OnGrowableGather", "GrowableEntity", "PickFruit", ["BasePlayer", "System.Boolean"])]
        [HookAttribute.Identifier("4292861fa0c149609c8f19c4066301fa")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "GrowableEntity")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Resource")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Resource_GrowableEntity_4292861fa0c149609c8f19c4066301fa : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 5)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1267491132)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldarg_1  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // AddYieldInstruction: Ldarg_2  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    // WAAAAAAA 3
                    // AddYieldInstruction: Box typeof(System.Boolean) False  
                    yield return new CodeInstruction(OpCodes.Box, typeof(System.Boolean));
                    // AddYieldInstruction: Call AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object),typeof(object),typeof(object), }) False  
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

public partial class Category_Resource
{
    public partial class Resource_GrowableEntity
    {
        [HookAttribute.Patch("OnRemoveDying", "OnRemoveDying", "GrowableEntity", "RemoveDying", ["BasePlayer"])]
        [HookAttribute.Identifier("b74c4433be4e4eba8ca122b682597b5d")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "GrowableEntity")]
        [MetadataAttribute.Parameter("receiver", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Resource")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Resource_GrowableEntity_b74c4433be4e4eba8ca122b682597b5d : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 12)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1776723751)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True GrowableEntity 
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

public partial class Category_Resource
{
    public partial class Resource_GrowableEntity
    {
        [HookAttribute.Patch("OnGrowableStateChange", "OnGrowableStateChange", "GrowableEntity", "ChangeState", ["PlantProperties/State", "System.Boolean", "System.Boolean"])]
        [HookAttribute.Identifier("bd084471d90d474c940e24676eeef665")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "GrowableEntity")]
        [MetadataAttribute.Parameter("state", "PlantProperties+State")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Resource")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Resource_GrowableEntity_bd084471d90d474c940e24676eeef665 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 0)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3507234587)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True GrowableEntity 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True PlantProperties+State 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Box typeof(PlantProperties.State) False  
                    yield return new CodeInstruction(OpCodes.Box, typeof(PlantProperties.State));
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

public partial class Category_Resource
{
    public partial class Resource_CoalingTower
    {
        [HookAttribute.Patch("OnCoalingTowerStart", "OnCoalingTowerStart", "CoalingTower", "RPC_Unload", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("64d808dd417240b9bf2015a91ac23ab0")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "CoalingTower")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Resource")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Resource_CoalingTower_64d808dd417240b9bf2015a91ac23ab0 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 0)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1616988590)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True CoalingTower 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BaseEntity+RPCMessage player
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // Set BaseEntity+RPCMessage
                    // value:player isProperty:False runtimeType:BasePlayer currentType:BaseEntity+RPCMessage type:BaseEntity+RPCMessage
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("BaseEntity+RPCMessage"), "player"));
                    // Read BaseEntity+RPCMessage : BaseEntity+RPCMessage
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

public partial class Category_Resource
{
    public partial class Resource_EngineSwitch
    {
        [HookAttribute.Patch("OnQuarryToggle", "OnQuarryToggle [on]", "EngineSwitch", "StartEngine", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("2e0571c189e249d6b2ba3987350473c9")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("local0", "MiningQuarry")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Resource")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Resource_EngineSwitch_2e0571c189e249d6b2ba3987350473c9 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 7)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2397758053)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_0  True MiningQuarry 
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BaseEntity+RPCMessage player
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // Set BaseEntity+RPCMessage
                    // value:player isProperty:False runtimeType:BasePlayer currentType:BaseEntity+RPCMessage type:BaseEntity+RPCMessage
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("BaseEntity+RPCMessage"), "player"));
                    // Read BaseEntity+RPCMessage : BaseEntity+RPCMessage
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

public partial class Category_Resource
{
    public partial class Resource_EngineSwitch
    {
        [HookAttribute.Patch("OnQuarryToggle", "OnQuarryToggle [off]", "EngineSwitch", "StopEngine", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("012f741f002749d6bcdda669c47f044a")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("local0", "MiningQuarry")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Resource")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Resource_EngineSwitch_012f741f002749d6bcdda669c47f044a : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 7)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2397758053)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_0  True MiningQuarry 
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BaseEntity+RPCMessage player
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // Set BaseEntity+RPCMessage
                    // value:player isProperty:False runtimeType:BasePlayer currentType:BaseEntity+RPCMessage type:BaseEntity+RPCMessage
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("BaseEntity+RPCMessage"), "player"));
                    // Read BaseEntity+RPCMessage : BaseEntity+RPCMessage
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

public partial class Category_Resource
{
    public partial class Resource_EngineSwitch
    {
        [HookAttribute.Patch("OnQuarryToggled", "OnQuarryToggled [off]", "EngineSwitch", "StopEngine", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("b03d1f493c0c44b0a09a3cde9caaf220")]
        [HookAttribute.Dependencies(new System.String[] { "OnQuarryToggle [off]" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("local0", "MiningQuarry")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Category("Resource")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Resource_EngineSwitch_b03d1f493c0c44b0a09a3cde9caaf220 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 18)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1754663591)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_0  True MiningQuarry 
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BaseEntity+RPCMessage player
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // Set BaseEntity+RPCMessage
                    // value:player isProperty:False runtimeType:BasePlayer currentType:BaseEntity+RPCMessage type:BaseEntity+RPCMessage
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("BaseEntity+RPCMessage"), "player"));
                    // Read BaseEntity+RPCMessage : BaseEntity+RPCMessage
                    // WAAAAAAA 1
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

public partial class Category_Resource
{
    public partial class Resource_EngineSwitch
    {
        [HookAttribute.Patch("OnQuarryToggled", "OnQuarryToggled [on]", "EngineSwitch", "StartEngine", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("4306a0b81df84799bc3783dea519b8d2")]
        [HookAttribute.Dependencies(new System.String[] { "OnQuarryToggle [on]" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("local0", "MiningQuarry")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Category("Resource")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Resource_EngineSwitch_4306a0b81df84799bc3783dea519b8d2 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 18)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1754663591)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_0  True MiningQuarry 
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BaseEntity+RPCMessage player
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // Set BaseEntity+RPCMessage
                    // value:player isProperty:False runtimeType:BasePlayer currentType:BaseEntity+RPCMessage type:BaseEntity+RPCMessage
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("BaseEntity+RPCMessage"), "player"));
                    // Read BaseEntity+RPCMessage : BaseEntity+RPCMessage
                    // WAAAAAAA 1
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

public partial class Category_Resource
{
    public partial class Resource_ResourceDispenser
    {
        [HookAttribute.Patch("OnDispenserGathered", "OnDispenserGathered", "ResourceDispenser", "GiveResourceFromItem", ["BasePlayer", "ItemAmount", "System.Single", "System.Single", "AttackEntity"])]
        [HookAttribute.Identifier("f30a9b6dd0c14df2a56a69909aa79a23")]
        [HookAttribute.Dependencies(new System.String[] { "OnDispenserGather" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "ResourceDispenser")]
        [MetadataAttribute.Parameter("entity", "BasePlayer")]
        [MetadataAttribute.Parameter("local7", "Item")]
        [MetadataAttribute.Category("Resource")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Resource_ResourceDispenser_f30a9b6dd0c14df2a56a69909aa79a23 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 149)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2805087710)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True ResourceDispenser 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Ldloc_S 7 False  
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 7);
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

public partial class Category_Resource
{
    public partial class Resource_CollectibleEntity
    {
        [HookAttribute.Patch("OnCollectiblePickedup", "OnCollectiblePickedup", "CollectibleEntity", "DoPickup", ["BasePlayer", "System.Boolean"])]
        [HookAttribute.Identifier("4ed80ddb0783423e91a471a3bc583a10")]
        [HookAttribute.Dependencies(new System.String[] { "OnCollectiblePickup" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "CollectibleEntity")]
        [MetadataAttribute.Parameter("reciever", "BasePlayer")]
        [MetadataAttribute.Parameter("local6", "Item")]
        [MetadataAttribute.Category("Resource")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Resource_CollectibleEntity_4ed80ddb0783423e91a471a3bc583a10 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 116)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)93268119)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True CollectibleEntity 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Ldloc_S 6 False  
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 6);
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

public partial class Category_Resource
{
    public partial class Resource_ResourceDispenser
    {
        [HookAttribute.Patch("OnDispenserBonusReceived", "OnDispenserBonusReceived", "ResourceDispenser", "AssignFinishBonus", ["BasePlayer", "System.Single", "AttackEntity"])]
        [HookAttribute.Identifier("f066046ec7a64b72a0b815e09585eb41")]
        [HookAttribute.Dependencies(new System.String[] { "OnDispenserBonus" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "ResourceDispenser")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Parameter("local4", "Item")]
        [MetadataAttribute.Category("Resource")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Resource_ResourceDispenser_f066046ec7a64b72a0b815e09585eb41 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 79)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2862399138)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True ResourceDispenser 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Ldloc_S 4 False  
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 4);
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

public partial class Category_Resource
{
    public partial class Resource_LootFill
    {
        [HookAttribute.Patch("OnLootSpawn", "OnLootSpawn [LootFill]", "LootFill", "DelayFill", [])]
        [HookAttribute.Identifier("cc1c23c6300c4d1bbf9c58a39bee43f4")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "LootFill")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Resource")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Resource_LootFill_cc1c23c6300c4d1bbf9c58a39bee43f4 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 6)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)767976070)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Call AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object), }) False  
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object), }));
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

public partial class Category_Resource
{
    public partial class Resource_MiningQuarry
    {
        [HookAttribute.Patch("OnQuarryGather", "OnQuarryGather", "MiningQuarry", "ProcessResources", [])]
        [HookAttribute.Identifier("b4e67d693e6e406b8d95e8cc6083e437")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Resource")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Resource_MiningQuarry_b4e67d693e6e406b8d95e8cc6083e437 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                edit.Add(new CodeInstruction(OpCodes.Ldstr, "OnQuarryGather").MoveLabelsFrom(original[104]));
                // Opop
                edit.Add(new CodeInstruction(OpCodes.Ldarg_0));
                edit.Add(new CodeInstruction(OpCodes.Ldloc, 7));
                edit.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("Oxide.Core.Interface"), "CallHook", new System.Type[] { typeof(System.String), typeof(System.Object), typeof(System.Object) })));
                Label label_032dd49dbe304c57a6cd3b771496784b = Generator.DefineLabel();
                original[104].labels.Add(label_032dd49dbe304c57a6cd3b771496784b);
                edit.Add(new CodeInstruction(OpCodes.Brfalse_S, label_032dd49dbe304c57a6cd3b771496784b));
                edit.Add(new CodeInstruction(OpCodes.Ldloc, 7));
                edit.Add(new CodeInstruction(OpCodes.Ldc_R4, 0.0f));
                edit.Add(new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("Item"), "Remove", new System.Type[] { typeof(System.Single) })));
                Label label_ca0ecca8e05f4c6ea1072dd5af845b93 = Generator.DefineLabel();
                original[123].labels.Add(label_ca0ecca8e05f4c6ea1072dd5af845b93);
                edit.Add(new CodeInstruction(OpCodes.Br_S, label_ca0ecca8e05f4c6ea1072dd5af845b93));

                original.InsertRange(104, edit);
                return original.AsEnumerable();
            }
        }
    }
}

public partial class Category_Resource
{
    public partial class Resource_RandomItemDispenser
    {
        [HookAttribute.Patch("OnRandomItemAward", "OnRandomItemAward", "RandomItemDispenser", "TryAward", ["RandomItemDispenser/RandomItemChance", "BasePlayer", "UnityEngine.Vector3"])]
        [HookAttribute.Identifier("d6e0cd3457194e68a2c616b50d81c6f7")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Resource")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Resource_RandomItemDispenser_d6e0cd3457194e68a2c616b50d81c6f7 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                edit.Add(new CodeInstruction(OpCodes.Ldstr, "OnRandomItemAward").MoveLabelsFrom(original[0]));
                // Opop
                edit.Add(new CodeInstruction(OpCodes.Ldarg_0));
                edit.Add(new CodeInstruction(OpCodes.Ldarg_1));
                edit.Add(new CodeInstruction(OpCodes.Box, typeof(RandomItemDispenser.RandomItemChance)));
                edit.Add(new CodeInstruction(OpCodes.Ldarg_2));
                edit.Add(new CodeInstruction(OpCodes.Ldarg_3));
                edit.Add(new CodeInstruction(OpCodes.Box, typeof(UnityEngine.Vector3)));
                edit.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("Oxide.Core.Interface"), "CallHook", new System.Type[] { typeof(System.String), typeof(System.Object), typeof(System.Object), typeof(System.Object), typeof(System.Object) })));
                Label label_d8d2a761b6054006b9cb1c9be3122ec8 = Generator.DefineLabel();
                original[0].labels.Add(label_d8d2a761b6054006b9cb1c9be3122ec8);
                edit.Add(new CodeInstruction(OpCodes.Brfalse_S, label_d8d2a761b6054006b9cb1c9be3122ec8));
                edit.Add(new CodeInstruction(OpCodes.Ldc_I4_0));
                edit.Add(new CodeInstruction(OpCodes.Ret));

                original.InsertRange(0, edit);
                return original.AsEnumerable();
            }
        }
    }
}

public partial class Category_Resource
{
    public partial class Resource_CoalingTower
    {
        [HookAttribute.Patch("OnCoalingTowerGather", "OnCoalingTowerGather", "CoalingTower", "EmptyTenPercent", [])]
        [HookAttribute.Identifier("d7e59406e4f44f858e6dc7df5827391b")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Resource")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Resource_CoalingTower_d7e59406e4f44f858e6dc7df5827391b : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                edit.Add(new CodeInstruction(OpCodes.Ldstr, "OnCoalingTowerGather").MoveLabelsFrom(original[122]));
                // Opop
                edit.Add(new CodeInstruction(OpCodes.Ldarg_0));
                edit.Add(new CodeInstruction(OpCodes.Ldloc_S, (sbyte)13));
                edit.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("Oxide.Core.Interface"), "CallHook", new System.Type[] { typeof(System.String), typeof(System.Object), typeof(System.Object) })));
                Label label_77f2ff05825f4820a9b8d649734bdaaa = Generator.DefineLabel();
                original[122].labels.Add(label_77f2ff05825f4820a9b8d649734bdaaa);
                edit.Add(new CodeInstruction(OpCodes.Brfalse_S, label_77f2ff05825f4820a9b8d649734bdaaa));
                edit.Add(new CodeInstruction(OpCodes.Ldloc_S, (sbyte)13));
                edit.Add(new CodeInstruction(OpCodes.Ldc_R4, 0f));
                edit.Add(new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("Item"), "Remove", new System.Type[] { typeof(System.Single) })));
                Label label_619ac60504144abaacb303a28950010b = Generator.DefineLabel();
                original[147].labels.Add(label_619ac60504144abaacb303a28950010b);
                edit.Add(new CodeInstruction(OpCodes.Br_S, label_619ac60504144abaacb303a28950010b));

                original.InsertRange(122, edit);
                return original.AsEnumerable();
            }
        }
    }
}

public partial class Category_Resource
{
    public partial class Resource_EngineSwitch
    {
        [HookAttribute.Patch("OnQuarryToggled", "OnQuarryToggled [off] [patch]", "EngineSwitch", "StopEngine", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("ef52f916e1bb443492b8e9916313ebff")]
        [HookAttribute.Dependencies(new System.String[] { "OnQuarryToggled [off]" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Resource")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Resource_EngineSwitch_ef52f916e1bb443492b8e9916313ebff : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                Label label_12bd2e40f5a443d688606f2a5743d75c = Generator.DefineLabel();
                original[14].labels.Add(label_12bd2e40f5a443d688606f2a5743d75c);
                edit.Add(new CodeInstruction(OpCodes.Brfalse_S, label_12bd2e40f5a443d688606f2a5743d75c));

                original[6 + 1].MoveLabelsFrom(original[6]);
                original.RemoveRange(6, 1);
                original.InsertRange(6, edit);
                return original.AsEnumerable();
            }
        }
    }
}

public partial class Category_Resource
{
    public partial class Resource_EngineSwitch
    {
        [HookAttribute.Patch("OnQuarryToggled", "OnQuarryToggled [on] [patch]", "EngineSwitch", "StartEngine", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("15ae4dd5a75f469fa5284a246db5f61a")]
        [HookAttribute.Dependencies(new System.String[] { "OnQuarryToggled [on]" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Resource")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Resource_EngineSwitch_15ae4dd5a75f469fa5284a246db5f61a : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                Label label_164d4bcfabe14a52a8493cf310e85e3c = Generator.DefineLabel();
                original[14].labels.Add(label_164d4bcfabe14a52a8493cf310e85e3c);
                edit.Add(new CodeInstruction(OpCodes.Brfalse_S, label_164d4bcfabe14a52a8493cf310e85e3c));

                original[6 + 1].MoveLabelsFrom(original[6]);
                original.RemoveRange(6, 1);
                original.InsertRange(6, edit);
                return original.AsEnumerable();
            }
        }
    }
}

