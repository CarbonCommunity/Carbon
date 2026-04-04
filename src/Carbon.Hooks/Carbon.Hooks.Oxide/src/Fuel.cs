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

public partial class Category_Fuel
{
    public partial class Fuel_BaseOven
    {
        [HookAttribute.Patch("OnFuelConsume", "OnFuelConsume", "BaseOven", "ConsumeFuel", ["Item", "ItemModBurnable"])]
        [HookAttribute.Identifier("0856bb05b97f45ccb4919ec11e579188")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "BaseOven")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Fuel")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Fuel_BaseOven_0856bb05b97f45ccb4919ec11e579188 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)4089079227)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldarg_1  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // AddYieldInstruction: Ldarg_2  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
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

public partial class Category_Fuel
{
    public partial class Fuel_BaseOven
    {
        [HookAttribute.Patch("OnFuelConsumed", "OnFuelConsumed", "BaseOven", "ConsumeFuel", ["Item", "ItemModBurnable"])]
        [HookAttribute.Identifier("b4b5e469733a4d1f889b74338b6c5eea")]
        [HookAttribute.Dependencies(new System.String[] { "OnFuelConsume" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "BaseOven")]
        [MetadataAttribute.Category("Fuel")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Fuel_BaseOven_b4b5e469733a4d1f889b74338b6c5eea : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 125)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)105375550)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldarg_1  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // AddYieldInstruction: Ldarg_2  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    // AddYieldInstruction: Call AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object),typeof(object),typeof(object), }) False  
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

public partial class Category_Fuel
{
    public partial class Fuel_EntityFuelSystem
    {
        [HookAttribute.Patch("OnFuelAmountCheck", "OnFuelAmountCheck", "EntityFuelSystem", "GetFuelAmount", [])]
        [HookAttribute.Identifier("a5eff55033824ee1969f50006aaa7801")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "EntityFuelSystem")]
        [MetadataAttribute.Parameter("local0", "Item")]
        [MetadataAttribute.Return(typeof(System.Int32))]
        [MetadataAttribute.Category("Fuel")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Fuel_EntityFuelSystem_a5eff55033824ee1969f50006aaa7801 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 3)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3319857446)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True EntityFuelSystem 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_0  True Item 
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object), typeof(object), }));
                    // hook call end

                    // return behaviour start
                    Label label1 = Generator.DefineLabel();
                    object retvar = Generator.DeclareLocal(typeof(object));
                    instruction.labels.Add(label1);
                    // AddYieldInstruction: Stloc retvar False  
                    yield return new CodeInstruction(OpCodes.Stloc, retvar);
                    // AddYieldInstruction: Ldloc retvar False  
                    yield return new CodeInstruction(OpCodes.Ldloc, retvar);
                    // AddYieldInstruction: Isinst typeof(System.Int32) False  
                    yield return new CodeInstruction(OpCodes.Isinst, typeof(System.Int32));
                    // AddYieldInstruction: Ldnull  True  
                    yield return new CodeInstruction(OpCodes.Ldnull);
                    // AddYieldInstruction: Beq_S label1 False  
                    yield return new CodeInstruction(OpCodes.Beq_S, label1);
                    // AddYieldInstruction: Ldloc retvar False  
                    yield return new CodeInstruction(OpCodes.Ldloc, retvar);
                    // AddYieldInstruction: Unbox_Any typeof(System.Int32) False  
                    yield return new CodeInstruction(OpCodes.Unbox_Any, typeof(System.Int32));
                    // AddYieldInstruction: Ret  True  
                    yield return new CodeInstruction(OpCodes.Ret);
                    // return behaviour end

                    yield return instruction;
                }
            }
        }
    }
}

public partial class Category_Fuel
{
    public partial class Fuel_EntityFuelSystem
    {
        [HookAttribute.Patch("OnFuelItemCheck", "OnFuelItemCheck", "EntityFuelSystem", "GetFuelItem", [])]
        [HookAttribute.Identifier("11dff87a9fd8489db7afc435443c23d2")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "EntityFuelSystem")]
        [MetadataAttribute.Parameter("local0", "StorageContainer")]
        [MetadataAttribute.Return(typeof(Item))]
        [MetadataAttribute.Category("Fuel")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Fuel_EntityFuelSystem_11dff87a9fd8489db7afc435443c23d2 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 3)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2913063700)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True EntityFuelSystem 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_0  True StorageContainer 
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object), typeof(object), }));
                    // hook call end

                    // return behaviour start
                    Label label1 = Generator.DefineLabel();
                    object retvar = Generator.DeclareLocal(typeof(object));
                    instruction.labels.Add(label1);
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
                    // AddYieldInstruction: Ret  True  
                    yield return new CodeInstruction(OpCodes.Ret);
                    // return behaviour end

                    yield return instruction;
                }
            }
        }
    }
}

public partial class Category_Fuel
{
    public partial class Fuel_EntityFuelSystem
    {
        [HookAttribute.Patch("OnFuelCheck", "OnFuelCheck", "EntityFuelSystem", "HasFuel", ["System.Boolean"])]
        [HookAttribute.Identifier("72d0ece7b87d4a11b88ff8941779d214")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "EntityFuelSystem")]
        [MetadataAttribute.Return(typeof(System.Boolean))]
        [MetadataAttribute.Category("Fuel")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Fuel_EntityFuelSystem_72d0ece7b87d4a11b88ff8941779d214 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)436282086)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Call AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object), }) False  
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object), }));
                    // hook call end

                    // return behaviour start
                    Label label1 = Generator.DefineLabel();
                    object retvar = Generator.DeclareLocal(typeof(object));
                    instruction.labels.Add(label1);
                    // AddYieldInstruction: Stloc retvar False  
                    yield return new CodeInstruction(OpCodes.Stloc, retvar);
                    // AddYieldInstruction: Ldloc retvar False  
                    yield return new CodeInstruction(OpCodes.Ldloc, retvar);
                    // AddYieldInstruction: Isinst typeof(System.Boolean) False  
                    yield return new CodeInstruction(OpCodes.Isinst, typeof(System.Boolean));
                    // AddYieldInstruction: Ldnull  True  
                    yield return new CodeInstruction(OpCodes.Ldnull);
                    // AddYieldInstruction: Beq_S label1 False  
                    yield return new CodeInstruction(OpCodes.Beq_S, label1);
                    // AddYieldInstruction: Ldloc retvar False  
                    yield return new CodeInstruction(OpCodes.Ldloc, retvar);
                    // AddYieldInstruction: Unbox_Any typeof(System.Boolean) False  
                    yield return new CodeInstruction(OpCodes.Unbox_Any, typeof(System.Boolean));
                    // AddYieldInstruction: Ret  True  
                    yield return new CodeInstruction(OpCodes.Ret);
                    // return behaviour end

                    yield return instruction;
                }
            }
        }
    }
}

public partial class Category_Fuel
{
    public partial class Fuel_EntityFuelSystem
    {
        [HookAttribute.Patch("CanCheckFuel", "CanCheckFuel", "EntityFuelSystem", "IsInFuelInteractionRange", ["BasePlayer"])]
        [HookAttribute.Identifier("e63143c8a14a4949850db3b05ab78c2f")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "EntityFuelSystem")]
        [MetadataAttribute.Parameter("local0", "StorageContainer")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Boolean))]
        [MetadataAttribute.Category("Fuel")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Fuel_EntityFuelSystem_e63143c8a14a4949850db3b05ab78c2f : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 3)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1390296394)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True EntityFuelSystem 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_0  True StorageContainer 
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object), typeof(object), typeof(object), }));
                    // hook call end

                    // return behaviour start
                    Label label1 = Generator.DefineLabel();
                    object retvar = Generator.DeclareLocal(typeof(object));
                    instruction.labels.Add(label1);
                    // AddYieldInstruction: Stloc retvar False  
                    yield return new CodeInstruction(OpCodes.Stloc, retvar);
                    // AddYieldInstruction: Ldloc retvar False  
                    yield return new CodeInstruction(OpCodes.Ldloc, retvar);
                    // AddYieldInstruction: Isinst typeof(System.Boolean) False  
                    yield return new CodeInstruction(OpCodes.Isinst, typeof(System.Boolean));
                    // AddYieldInstruction: Ldnull  True  
                    yield return new CodeInstruction(OpCodes.Ldnull);
                    // AddYieldInstruction: Beq_S label1 False  
                    yield return new CodeInstruction(OpCodes.Beq_S, label1);
                    // AddYieldInstruction: Ldloc retvar False  
                    yield return new CodeInstruction(OpCodes.Ldloc, retvar);
                    // AddYieldInstruction: Unbox_Any typeof(System.Boolean) False  
                    yield return new CodeInstruction(OpCodes.Unbox_Any, typeof(System.Boolean));
                    // AddYieldInstruction: Ret  True  
                    yield return new CodeInstruction(OpCodes.Ret);
                    // return behaviour end

                    yield return instruction;
                }
            }
        }
    }
}

public partial class Category_Fuel
{
    public partial class Fuel_EntityFuelSystem
    {
        [HookAttribute.Patch("CanUseFuel", "CanUseFuel", "EntityFuelSystem", "TryUseFuel", ["System.Single", "System.Single"])]
        [HookAttribute.Identifier("132e7cbe233e4cbabcf2a49098f83d9c")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "EntityFuelSystem")]
        [MetadataAttribute.Parameter("local0", "StorageContainer")]
        [MetadataAttribute.Parameter("seconds", "System.Single")]
        [MetadataAttribute.Parameter("fuelUsedPerSecond", "System.Single")]
        [MetadataAttribute.Return(typeof(System.Boolean))]
        [MetadataAttribute.Category("Fuel")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Fuel_EntityFuelSystem_132e7cbe233e4cbabcf2a49098f83d9c : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 3)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)4053503248)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True EntityFuelSystem 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_0  True StorageContainer 
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True System.Single 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Box typeof(System.Single) False  
                    yield return new CodeInstruction(OpCodes.Box, typeof(System.Single));
                    // HERE!
                    // AddYieldInstruction: Ldarg_2  True System.Single 
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Box typeof(System.Single) False  
                    yield return new CodeInstruction(OpCodes.Box, typeof(System.Single));
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object), typeof(object), typeof(object), typeof(object), }));
                    // hook call end

                    // return behaviour start
                    Label label1 = Generator.DefineLabel();
                    Label label2 = Generator.DefineLabel();
                    object retvar = Generator.DeclareLocal(typeof(object));
                    instruction.labels.Add(label1);
                    // AddYieldInstruction: Stloc retvar False  
                    yield return new CodeInstruction(OpCodes.Stloc, retvar);
                    // AddYieldInstruction: Ldloc retvar False  
                    yield return new CodeInstruction(OpCodes.Ldloc, retvar);
                    // AddYieldInstruction: Brfalse_S label1 False  
                    yield return new CodeInstruction(OpCodes.Brfalse_S, label1);
                    // AddYieldInstruction: Ldloc retvar False  
                    yield return new CodeInstruction(OpCodes.Ldloc, retvar);
                    // AddYieldInstruction: Isinst typeof(System.Boolean) False  
                    yield return new CodeInstruction(OpCodes.Isinst, typeof(System.Boolean));
                    // AddYieldInstruction: Brtrue_S label2 False  
                    yield return new CodeInstruction(OpCodes.Brtrue_S, label2);
                    // AddYieldInstruction: Ldc_I4_0  True  
                    yield return new CodeInstruction(OpCodes.Ldc_I4_0);
                    // AddYieldInstruction: Ret  True  
                    yield return new CodeInstruction(OpCodes.Ret);
                    // AddYieldInstruction: Ldloc retvar).WithLabels(label2 False  
                    yield return new CodeInstruction(OpCodes.Ldloc, retvar).WithLabels(label2);
                    // AddYieldInstruction: Unbox_Any typeof(System.Int32) False  
                    yield return new CodeInstruction(OpCodes.Unbox_Any, typeof(System.Int32));
                    // AddYieldInstruction: Ret  True  
                    yield return new CodeInstruction(OpCodes.Ret);
                    // return behaviour end

                    yield return instruction;
                }
            }
        }
    }
}

