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

public partial class Category_Item
{
    public partial class Item_ItemContainer
    {
        [HookAttribute.Patch("OnItemRemovedFromContainer", "OnItemRemovedFromContainer", "ItemContainer", "Remove", ["Item"])]
        [HookAttribute.Identifier("65cb93a9ddc54cd6ae09779efc76180e")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "ItemContainer")]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_ItemContainer_65cb93a9ddc54cd6ae09779efc76180e : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 78)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)470300595)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldarg_1  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // AddYieldInstruction: Call AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object),typeof(object), }) False  
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

public partial class Category_Item
{
    public partial class Item_ItemContainer
    {
        [HookAttribute.Patch("OnItemAddedToContainer", "OnItemAddedToContainer", "ItemContainer", "Insert", ["Item"])]
        [HookAttribute.Identifier("23696a03bbee437ba8692b039e08dbee")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "ItemContainer")]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_ItemContainer_23696a03bbee437ba8692b039e08dbee : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 68)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)199161889)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldarg_1  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // AddYieldInstruction: Call AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object),typeof(object), }) False  
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

public partial class Category_Item
{
    public partial class Item_ItemCrafter
    {
        [HookAttribute.Patch("OnItemCraft", "OnItemCraft", "ItemCrafter", "CraftItem", ["ItemBlueprint", "BasePlayer", "ProtoBuf.Item/InstanceData", "System.Int32", "System.Int32", "Item", "System.Boolean"])]
        [HookAttribute.Identifier("fd40bfeef3fc4f3bb5a749a8921d0a44")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("local0", "ItemCraftTask")]
        [MetadataAttribute.Parameter("owner", "BasePlayer")]
        [MetadataAttribute.Parameter("fromTempBlueprint", "Item")]
        [MetadataAttribute.Return(typeof(System.Boolean))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_ItemCrafter_fd40bfeef3fc4f3bb5a749a8921d0a44 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 77)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)276522030)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_0  True ItemCraftTask 
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_2  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    // WAAAAAAA 1
                    // HERE!
                    // AddYieldInstruction: Ldarg_S 6 False  
                    yield return new CodeInstruction(OpCodes.Ldarg_S, 6);
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

public partial class Category_Item
{
    public partial class Item_Deployer
    {
        [HookAttribute.Patch("OnItemDeployed", "OnItemDeployed [Regular]", "Deployer", "DoDeploy_Regular", ["Deployable", "UnityEngine.Ray"])]
        [HookAttribute.Identifier("270c536486394261acd27df42e3f2752")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "Deployer")]
        [MetadataAttribute.Parameter("local5", "ItemModDeployable")]
        [MetadataAttribute.Parameter("local6", "BaseEntity")]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_Deployer_270c536486394261acd27df42e3f2752 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 137)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)470287711)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True Deployer 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_S 5 False  
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 5);
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

public partial class Category_Item
{
    public partial class Item_Deployer
    {
        [HookAttribute.Patch("OnItemDeployed", "OnItemDeployed [Slot]", "Deployer", "DoDeploy_Slot", ["Deployable", "UnityEngine.Ray", "NetworkableId"])]
        [HookAttribute.Identifier("18bd257863c042d7a60a6240d49f172e")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "Deployer")]
        [MetadataAttribute.Parameter("local1", "BaseEntity")]
        [MetadataAttribute.Parameter("local4", "BaseEntity")]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_Deployer_18bd257863c042d7a60a6240d49f172e : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 204)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)470287711)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True Deployer 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_1  True BaseEntity 
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
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

public partial class Category_Item
{
    public partial class Item_Item
    {
        [HookAttribute.Patch("IOnLoseCondition", "IOnLoseCondition", "Item", "LoseCondition", ["System.Single"])]
        [HookAttribute.Identifier("38e5d8353925414a9f86e71f6070ec2b")]
        [HookAttribute.Options(HookFlags.Static | HookFlags.Hidden)]
        [MetadataAttribute.Parameter("self", "Item")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_Item_38e5d8353925414a9f86e71f6070ec2b : API.Hooks.Patch
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
                    // AddYieldInstruction: Ldarg_0  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_1  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // AddYieldInstruction: Call AccessTools.Method(typeof(Carbon.Core.CorePlugin), "IOnLoseCondition") False  
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Carbon.Core.CorePlugin), "IOnLoseCondition"));
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

public partial class Category_Item
{
    public partial class Item_ItemCrafter
    {
        [HookAttribute.Patch("OnItemCraftFinished", "OnItemCraftFinished", "ItemCrafter", "FinishCrafting", ["ItemCraftTask"])]
        [HookAttribute.Identifier("3190e9d9f2674385b52cadffbc739107")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("task", "ItemCraftTask")]
        [MetadataAttribute.Parameter("local1", "Item")]
        [MetadataAttribute.Parameter("self", "ItemCrafter")]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_ItemCrafter_3190e9d9f2674385b52cadffbc739107 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 200)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)659159968)).MoveLabelsFrom(instruction);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True ItemCraftTask 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Ldloc_1  True Item 
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
                    // AddYieldInstruction: Ldarg_0  True ItemCrafter 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
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

public partial class Category_Item
{
    public partial class Item_MedicalTool
    {
        [HookAttribute.Patch("OnHealingItemUse", "OnHealingItemUse", "MedicalTool", "GiveEffectsTo", ["BasePlayer", "IMedicalToolTarget"])]
        [HookAttribute.Identifier("bd33e73e3e124945bc1363ef6d4f1897")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "MedicalTool")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_MedicalTool_bd33e73e3e124945bc1363ef6d4f1897 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 31)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2732072613)).MoveLabelsFrom(instruction);
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

public partial class Category_Item
{
    public partial class Item_ResearchTable
    {
        [HookAttribute.Patch("OnItemResearch", "OnItemResearch", "ResearchTable", "DoResearch", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("c82791b5c84a4fba845c7e73af87e5c0")]
        [HookAttribute.Dependencies(new System.String[] { "CanResearchItem" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "ResearchTable")]
        [MetadataAttribute.Parameter("local1", "Item")]
        [MetadataAttribute.Parameter("local0", "BasePlayer")]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_ResearchTable_c82791b5c84a4fba845c7e73af87e5c0 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 30)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3312631145)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True ResearchTable 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_1  True Item 
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
                    // AddYieldInstruction: Ldloc_0  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
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

public partial class Category_Item
{
    public partial class Item_ResearchTable
    {
        [HookAttribute.Patch("OnItemResearched", "OnItemResearched", "ResearchTable", "ResearchAttemptFinished", [])]
        [HookAttribute.Identifier("f27b800adaa5483485a8c50be7e29ee5")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "ResearchTable")]
        [MetadataAttribute.Parameter("local2", "System.Int32")]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_ResearchTable_f27b800adaa5483485a8c50be7e29ee5 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 13)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2094586654)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True ResearchTable 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_2  True System.Int32 
                    yield return new CodeInstruction(OpCodes.Ldloc_2);
                    // WAAAAAAA 4.2
                    // AddYieldInstruction: Box Carbon.Extensions.AccessToolsEx.TypeByName("System.Int32") False  
                    yield return new CodeInstruction(OpCodes.Box, Carbon.Extensions.AccessToolsEx.TypeByName("System.Int32"));
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
                    // AddYieldInstruction: Stloc_2  True  
                    yield return new CodeInstruction(OpCodes.Stloc_2);

                    yield return instruction;
                }
            }
        }
    }
}

public partial class Category_Item
{
    public partial class Item_Item
    {
        [HookAttribute.Patch("OnItemUse", "OnItemUse", "Item", "UseItem", ["System.Int32"])]
        [HookAttribute.Identifier("2013faa0fd414cabbdc054f9ad579c07")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "Item")]
        [MetadataAttribute.Parameter("amountToConsume", "System.Int32")]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_Item_2013faa0fd414cabbdc054f9ad579c07 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2897106601)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True Item 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True System.Int32 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Box typeof(System.Int32) False  
                    yield return new CodeInstruction(OpCodes.Box, typeof(System.Int32));
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
                    // AddYieldInstruction: Brfalse_S label1 False  
                    yield return new CodeInstruction(OpCodes.Brfalse_S, label1);
                    // AddYieldInstruction: Ldloc retvar False  
                    yield return new CodeInstruction(OpCodes.Ldloc, retvar);
                    // AddYieldInstruction: Unbox_Any typeof(System.Int32) False  
                    yield return new CodeInstruction(OpCodes.Unbox_Any, typeof(System.Int32));
                    // AddYieldInstruction: Starg 1 False  
                    yield return new CodeInstruction(OpCodes.Starg, 1);
                    // return behaviour end

                    yield return instruction;
                }
            }
        }
    }
}

public partial class Category_Item
{
    public partial class Item_RepairBench
    {
        [HookAttribute.Patch("OnItemRepair", "OnItemRepair", "RepairBench", "RepairAnItem", ["Item", "BasePlayer", "BaseEntity", "System.Single", "System.Boolean"])]
        [HookAttribute.Identifier("d62eb5fd359e44b0a01233a79d73734a")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Parameter("itemToRepair", "Item")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_RepairBench_d62eb5fd359e44b0a01233a79d73734a : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 78)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)768721788)).MoveLabelsFrom(instruction);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // HERE!
                    // AddYieldInstruction: Ldarg_0  True Item 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
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

public partial class Category_Item
{
    public partial class Item_MapEntity
    {
        [HookAttribute.Patch("OnMapImageUpdated", "OnMapImageUpdated", "MapEntity", "ImageUpdate", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("0d1d255e64554660841763139fa5f110")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_MapEntity_0d1d255e64554660841763139fa5f110 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 84)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)322168974)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Call AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint) }) False  
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint) }));
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

public partial class Category_Item
{
    public partial class Item_ItemCrafter
    {
        [HookAttribute.Patch("OnItemCraftCancelled", "OnItemCraftCancelled", "ItemCrafter", "CancelTask", ["System.Int32"])]
        [HookAttribute.Identifier("06402b3f5957444cb7eaa8138943f884")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("local1", "ItemCraftTask")]
        [MetadataAttribute.Parameter("self", "ItemCrafter")]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_ItemCrafter_06402b3f5957444cb7eaa8138943f884 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 43)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)195516419)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_1  True ItemCraftTask 
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
                    // AddYieldInstruction: Ldarg_0  True ItemCrafter 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
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

public partial class Category_Item
{
    public partial class Item_ItemModUpgrade
    {
        [HookAttribute.Patch("OnItemUpgrade", "OnItemUpgrade", "ItemModUpgrade", "ServerCommand", ["Item", "System.String", "BasePlayer"])]
        [HookAttribute.Identifier("c3bd707b53574534923f6fe97b6774c2")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("item", "Item")]
        [MetadataAttribute.Parameter("local0", "Item")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_ItemModUpgrade_c3bd707b53574534923f6fe97b6774c2 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 29)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3643324244)).MoveLabelsFrom(instruction);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True Item 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Ldloc_0  True Item 
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_3  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldarg_3);
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

public partial class Category_Item
{
    public partial class Item_PlayerInventory
    {
        [HookAttribute.Patch("CanEquipItem", "CanEquipItem", "PlayerInventory", "CanEquipItem", ["Item", "System.Int32"])]
        [HookAttribute.Identifier("2dba9846eccb445081ee7dc8d4a6c85d")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "PlayerInventory")]
        [MetadataAttribute.Return(typeof(System.Boolean))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_PlayerInventory_2dba9846eccb445081ee7dc8d4a6c85d : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3471265898)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldarg_1  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // AddYieldInstruction: Ldarg_2  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    // WAAAAAAA 3
                    // AddYieldInstruction: Box typeof(System.Int32) False  
                    yield return new CodeInstruction(OpCodes.Box, typeof(System.Int32));
                    // AddYieldInstruction: Call AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object),typeof(object),typeof(object), }) False  
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

public partial class Category_Item
{
    public partial class Item_PlayerInventory
    {
        [HookAttribute.Patch("CanWearItem", "CanWearItem", "PlayerInventory", "CanWearItem", ["Item", "System.Int32"])]
        [HookAttribute.Identifier("8b1da3b592b54feb95a96b758351cab6")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "PlayerInventory")]
        [MetadataAttribute.Return(typeof(System.Boolean))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_PlayerInventory_8b1da3b592b54feb95a96b758351cab6 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)4048531747)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldarg_1  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // AddYieldInstruction: Ldarg_2  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    // WAAAAAAA 3
                    // AddYieldInstruction: Box typeof(System.Int32) False  
                    yield return new CodeInstruction(OpCodes.Box, typeof(System.Int32));
                    // AddYieldInstruction: Call AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object),typeof(object),typeof(object), }) False  
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

public partial class Category_Item
{
    public partial class Item_ItemContainer
    {
        [HookAttribute.Patch("CanAcceptItem", "CanAcceptItem", "ItemContainer", "CanAcceptItem", ["Item", "System.Int32"])]
        [HookAttribute.Identifier("b34c9d6c48a44cd189d4887f90ee42a7")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "ItemContainer")]
        [MetadataAttribute.Return(typeof(ItemContainer.CanAcceptResult))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_ItemContainer_b34c9d6c48a44cd189d4887f90ee42a7 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1360889797)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldarg_1  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // AddYieldInstruction: Ldarg_2  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    // WAAAAAAA 3
                    // AddYieldInstruction: Box typeof(System.Int32) False  
                    yield return new CodeInstruction(OpCodes.Box, typeof(System.Int32));
                    // AddYieldInstruction: Call AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object),typeof(object),typeof(object), }) False  
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
                    // AddYieldInstruction: Isinst typeof(ItemContainer.CanAcceptResult) False  
                    yield return new CodeInstruction(OpCodes.Isinst, typeof(ItemContainer.CanAcceptResult));
                    // AddYieldInstruction: Ldnull  True  
                    yield return new CodeInstruction(OpCodes.Ldnull);
                    // AddYieldInstruction: Beq_S label1 False  
                    yield return new CodeInstruction(OpCodes.Beq_S, label1);
                    // AddYieldInstruction: Ldloc retvar False  
                    yield return new CodeInstruction(OpCodes.Ldloc, retvar);
                    // AddYieldInstruction: Unbox_Any typeof(ItemContainer.CanAcceptResult) False  
                    yield return new CodeInstruction(OpCodes.Unbox_Any, typeof(ItemContainer.CanAcceptResult));
                    // AddYieldInstruction: Ret  True  
                    yield return new CodeInstruction(OpCodes.Ret);
                    // return behaviour end

                    yield return instruction;
                }
            }
        }
    }
}

public partial class Category_Item
{
    public partial class Item_Item
    {
        [HookAttribute.Patch("OnItemSplit", "OnItemSplit", "Item", "SplitItem", ["System.Int32"])]
        [HookAttribute.Identifier("4a8c5ae8ed3f49d28e306bacb0924bc1")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "Item")]
        [MetadataAttribute.Return(typeof(Item))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_Item_4a8c5ae8ed3f49d28e306bacb0924bc1 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 16)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)983035860)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldarg_1  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 3
                    // AddYieldInstruction: Box typeof(System.Int32) False  
                    yield return new CodeInstruction(OpCodes.Box, typeof(System.Int32));
                    // AddYieldInstruction: Call AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object),typeof(object), }) False  
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

public partial class Category_Item
{
    public partial class Item_WorldItem
    {
        [HookAttribute.Patch("OnItemPickup", "OnItemPickup", "WorldItem", "Pickup", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("3be0093ecebe405c82440d9630c3b623")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "WorldItem")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Parameter("self1", "WorldItem")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_WorldItem_3be0093ecebe405c82440d9630c3b623 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 13)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1833117670)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True WorldItem item
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // Set WorldItem
                    // value:item isProperty:False runtimeType:Item currentType:WorldItem type:WorldItem
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("WorldItem"), "item"));
                    // Read WorldItem : WorldItem
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BaseEntity+RPCMessage player
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // Set BaseEntity+RPCMessage
                    // value:player isProperty:False runtimeType:BasePlayer currentType:BaseEntity+RPCMessage type:BaseEntity+RPCMessage
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("BaseEntity+RPCMessage"), "player"));
                    // Read BaseEntity+RPCMessage : BaseEntity+RPCMessage
                    // WAAAAAAA 1
                    // AddYieldInstruction: Ldarg_0  True WorldItem 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
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

public partial class Category_Item
{
    public partial class Item_BaseOven
    {
        [HookAttribute.Patch("OnFindBurnable", "OnFindBurnable", "BaseOven", "FindBurnable", [])]
        [HookAttribute.Identifier("15947ac931f34b32b4e154755d56313c")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "BaseOven")]
        [MetadataAttribute.Return(typeof(Item))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_BaseOven_15947ac931f34b32b4e154755d56313c : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2432278859)).MoveLabelsFrom(instruction);
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

public partial class Category_Item
{
    public partial class Item_Item
    {
        [HookAttribute.Patch("CanStackItem", "CanStackItem", "Item", "CanStack", ["Item"])]
        [HookAttribute.Identifier("01f6ba7ebf91432e9a8d1892f3baf68d")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "Item")]
        [MetadataAttribute.Parameter("item", "Item")]
        [MetadataAttribute.Return(typeof(System.Boolean))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_Item_01f6ba7ebf91432e9a8d1892f3baf68d : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2594779146)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True Item 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True Item 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
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

public partial class Category_Item
{
    public partial class Item_PlayerInventory
    {
        [HookAttribute.Patch("OnItemAction", "OnItemAction", "PlayerInventory", "ItemCmd", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("c1e7d0650a0545d4bb13e83e4601ce01")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("local2", "Item")]
        [MetadataAttribute.Parameter("local1", "System.String")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_PlayerInventory_c1e7d0650a0545d4bb13e83e4601ce01 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 32)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3403522973)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_2  True Item 
                    yield return new CodeInstruction(OpCodes.Ldloc_2);
                    // AddYieldInstruction: Ldloc_1  True System.String 
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
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

public partial class Category_Item
{
    public partial class Item_Recycler
    {
        [HookAttribute.Patch("OnItemRecycle", "OnItemRecycle", "Recycler", "RecycleThink", [])]
        [HookAttribute.Identifier("cfa37da4858c45dea50d011605d8e5fd")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("local3", "Item")]
        [MetadataAttribute.Parameter("self", "Recycler")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_Recycler_cfa37da4858c45dea50d011605d8e5fd : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 23)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2718382832)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_3  True Item 
                    yield return new CodeInstruction(OpCodes.Ldloc_3);
                    // AddYieldInstruction: Ldarg_0  True Recycler 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
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

public partial class Category_Item
{
    public partial class Item_Item
    {
        [HookAttribute.Patch("OnItemDropped", "OnItemDropped", "Item", "Drop", ["UnityEngine.Vector3", "UnityEngine.Vector3", "UnityEngine.Quaternion"])]
        [HookAttribute.Identifier("ee3619f09f344740a6bb47e83367bcd5")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "Item")]
        [MetadataAttribute.Parameter("local1", "BaseEntity")]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_Item_ee3619f09f344740a6bb47e83367bcd5 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 102)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)226172740)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True Item 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_1  True BaseEntity 
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
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

public partial class Category_Item
{
    public partial class Item_PlayerInventory
    {
        [HookAttribute.Patch("CanMoveItem", "CanMoveItem", "PlayerInventory", "MoveItem", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("67d0371467294590b7c012d60e0d5441")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("local5", "Item")]
        [MetadataAttribute.Parameter("self", "PlayerInventory")]
        [MetadataAttribute.Parameter("local1", "ItemContainerId")]
        [MetadataAttribute.Parameter("local2", "System.Int32")]
        [MetadataAttribute.Parameter("local3", "System.Int32")]
        [MetadataAttribute.Parameter("local4", "ItemMoveModifier")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_PlayerInventory_67d0371467294590b7c012d60e0d5441 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 45)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3939984194)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_S 5 False  
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 5);
                    // AddYieldInstruction: Ldarg_0  True PlayerInventory 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_1  True ItemContainerId 
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
                    // WAAAAAAA 4.2
                    // AddYieldInstruction: Box Carbon.Extensions.AccessToolsEx.TypeByName("ItemContainerId") False  
                    yield return new CodeInstruction(OpCodes.Box, Carbon.Extensions.AccessToolsEx.TypeByName("ItemContainerId"));
                    // AddYieldInstruction: Ldloc_2  True System.Int32 
                    yield return new CodeInstruction(OpCodes.Ldloc_2);
                    // WAAAAAAA 4.2
                    // AddYieldInstruction: Box Carbon.Extensions.AccessToolsEx.TypeByName("System.Int32") False  
                    yield return new CodeInstruction(OpCodes.Box, Carbon.Extensions.AccessToolsEx.TypeByName("System.Int32"));
                    // AddYieldInstruction: Ldloc_3  True System.Int32 
                    yield return new CodeInstruction(OpCodes.Ldloc_3);
                    // WAAAAAAA 4.2
                    // AddYieldInstruction: Box Carbon.Extensions.AccessToolsEx.TypeByName("System.Int32") False  
                    yield return new CodeInstruction(OpCodes.Box, Carbon.Extensions.AccessToolsEx.TypeByName("System.Int32"));
                    // AddYieldInstruction: Ldloc_S 4 False  
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 4);
                    // WAAAAAAA 4.2
                    // AddYieldInstruction: Box Carbon.Extensions.AccessToolsEx.TypeByName("ItemMoveModifier") False  
                    yield return new CodeInstruction(OpCodes.Box, Carbon.Extensions.AccessToolsEx.TypeByName("ItemMoveModifier"));
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), }));
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

public partial class Category_Item
{
    public partial class Item_DroppedItem
    {
        [HookAttribute.Patch("CanCombineDroppedItem", "CanCombineDroppedItem", "DroppedItem", "OnDroppedOn", ["DroppedItem"])]
        [HookAttribute.Identifier("f2ffd245a0fc42df88c222ef3ec54e7b")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "DroppedItem")]
        [MetadataAttribute.Parameter("di", "DroppedItem")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_DroppedItem_f2ffd245a0fc42df88c222ef3ec54e7b : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 8)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3506991312)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True DroppedItem 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True DroppedItem 
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

public partial class Category_Item
{
    public partial class Item_Item
    {
        [HookAttribute.Patch("OnMaxStackable", "OnMaxStackable", "Item", "MaxStackable", [])]
        [HookAttribute.Identifier("f8fe6d3696394382ab2b3d6a0846358c")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "Item")]
        [MetadataAttribute.Return(typeof(System.Int32))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_Item_f8fe6d3696394382ab2b3d6a0846358c : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)418610024)).MoveLabelsFrom(instruction);
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

public partial class Category_Item
{
    public partial class Item_ResearchTable
    {
        [HookAttribute.Patch("OnResearchCostDetermine", "OnResearchCostDetermine [Item]", "ResearchTable", "ScrapForResearch", ["Item"])]
        [HookAttribute.Identifier("1e30a44372b9440090b2a9fb7d57544d")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("item", "Item")]
        [MetadataAttribute.Return(typeof(System.Int32))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_ResearchTable_1e30a44372b9440090b2a9fb7d57544d : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3044147384)).MoveLabelsFrom(instruction);
                    // HERE!
                    // AddYieldInstruction: Ldarg_0  True Item 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // WAAAAAAA 1
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

public partial class Category_Item
{
    public partial class Item_Item
    {
        [HookAttribute.Patch("OnItemRemove", "OnItemRemove", "Item", "Remove", ["System.Single"])]
        [HookAttribute.Identifier("a4f451fd2e4c46d691f9ecc46fcd7594")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "Item")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_Item_a4f451fd2e4c46d691f9ecc46fcd7594 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)4018892167)).MoveLabelsFrom(instruction);
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

public partial class Category_Item
{
    public partial class Item_Deployer
    {
        [HookAttribute.Patch("CanDeployItem", "CanDeployItem", "Deployer", "DoDeploy", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("926a60b26bd04acc9a47f3e634504e76")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Parameter("self", "Deployer")]
        [MetadataAttribute.Parameter("local2", "NetworkableId")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_Deployer_926a60b26bd04acc9a47f3e634504e76 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 21)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)208822718)).MoveLabelsFrom(instruction);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BaseEntity+RPCMessage player
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // Set BaseEntity+RPCMessage
                    // value:player isProperty:False runtimeType:BasePlayer currentType:BaseEntity+RPCMessage type:BaseEntity+RPCMessage
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("BaseEntity+RPCMessage"), "player"));
                    // Read BaseEntity+RPCMessage : BaseEntity+RPCMessage
                    // WAAAAAAA 1
                    // AddYieldInstruction: Ldarg_0  True Deployer 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_2  True NetworkableId 
                    yield return new CodeInstruction(OpCodes.Ldloc_2);
                    // WAAAAAAA 4.2
                    // AddYieldInstruction: Box Carbon.Extensions.AccessToolsEx.TypeByName("NetworkableId") False  
                    yield return new CodeInstruction(OpCodes.Box, Carbon.Extensions.AccessToolsEx.TypeByName("NetworkableId"));
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

public partial class Category_Item
{
    public partial class Item_RepairBench
    {
        [HookAttribute.Patch("OnItemSkinChange", "OnItemSkinChange", "RepairBench", "ChangeSkin", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("1ca87aacef0f48d6aeb5fe823f8b2eda")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("inventoryId", "System.Int32")]
        [MetadataAttribute.Parameter("local5", "Item")]
        [MetadataAttribute.Parameter("self", "RepairBench")]
        [MetadataAttribute.Parameter("local1", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_RepairBench_1ca87aacef0f48d6aeb5fe823f8b2eda : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 44)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)4093960919)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_0  True RepairBench+<>c__DisplayClass12_0 inventoryId
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    // Set RepairBench+<>c__DisplayClass12_0
                    // value:inventoryId isProperty:False runtimeType:System.Int32 currentType:RepairBench+<>c__DisplayClass12_0 type:RepairBench+<>c__DisplayClass12_0
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("RepairBench+<>c__DisplayClass12_0"), "inventoryId"));
                    yield return new CodeInstruction(OpCodes.Box, Carbon.Extensions.AccessToolsEx.TypeByName("System.Int32"));
                    // AddYieldInstruction: Ldloc_S 5 False  
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 5);
                    // AddYieldInstruction: Ldarg_0  True RepairBench 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_1  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object), typeof(object), typeof(object), typeof(object), }));
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

public partial class Category_Item
{
    public partial class Item_LootContainer
    {
        [HookAttribute.Patch("OnBonusItemDrop", "OnBonusItemDrop", "LootContainer", "DropBonusItems", ["BaseEntity", "ItemContainer"])]
        [HookAttribute.Identifier("77a2053d17014a79a4176079debec9c2")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("local5", "Item")]
        [MetadataAttribute.Parameter("local0", "BasePlayer")]
        [MetadataAttribute.Parameter("container", "ItemContainer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_LootContainer_77a2053d17014a79a4176079debec9c2 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 97)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1235377971)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_S 5 False  
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 5);
                    // AddYieldInstruction: Ldloc_0  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_2  True ItemContainer 
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
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

public partial class Category_Item
{
    public partial class Item_ItemModRepair
    {
        [HookAttribute.Patch("OnItemRefill", "OnItemRefill", "ItemModRepair", "ServerCommand", ["Item", "System.String", "BasePlayer"])]
        [HookAttribute.Identifier("c24b9221b85546da9279c73d915fbb63")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("item", "Item")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_ItemModRepair_c24b9221b85546da9279c73d915fbb63 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2274779210)).MoveLabelsFrom(instruction);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True Item 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // HERE!
                    // AddYieldInstruction: Ldarg_3  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldarg_3);
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

public partial class Category_Item
{
    public partial class Item_LootContainer
    {
        [HookAttribute.Patch("OnBonusItemDropped", "OnBonusItemDropped", "LootContainer", "DropBonusItems", ["BaseEntity", "ItemContainer"])]
        [HookAttribute.Identifier("3764430e5ae24cc2a58ffcde8a490416")]
        [HookAttribute.Dependencies(new System.String[] { "OnBonusItemDrop" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("local5", "Item")]
        [MetadataAttribute.Parameter("local0", "BasePlayer")]
        [MetadataAttribute.Parameter("container", "ItemContainer")]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_LootContainer_3764430e5ae24cc2a58ffcde8a490416 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 122)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)4037264508)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_S 5 False  
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 5);
                    // AddYieldInstruction: Ldloc_0  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_2  True ItemContainer 
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
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

public partial class Category_Item
{
    public partial class Item_ResearchTable
    {
        [HookAttribute.Patch("OnResearchCostDetermine", "OnResearchCostDetermine [ItemDef]", "ResearchTable", "ScrapForResearch", ["ItemDefinition"])]
        [HookAttribute.Identifier("7c6401d60e9e4fc5a4da1dc51f880b3b")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("info", "ItemDefinition")]
        [MetadataAttribute.Return(typeof(System.Int32))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_ResearchTable_7c6401d60e9e4fc5a4da1dc51f880b3b : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3044147384)).MoveLabelsFrom(instruction);
                    // HERE!
                    // AddYieldInstruction: Ldarg_0  True ItemDefinition 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // WAAAAAAA 1
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

public partial class Category_Item
{
    public partial class Item_Mailbox
    {
        [HookAttribute.Patch("OnItemSubmit", "OnItemSubmit", "Mailbox", "SubmitInputItems", ["BasePlayer"])]
        [HookAttribute.Identifier("4d0eba73023d423ea1ce3cb70cbc3701")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("local1", "Item")]
        [MetadataAttribute.Parameter("self", "Mailbox")]
        [MetadataAttribute.Parameter("fromPlayer", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_Mailbox_4d0eba73023d423ea1ce3cb70cbc3701 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2108135861)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_1  True Item 
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
                    // AddYieldInstruction: Ldarg_0  True Mailbox 
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

public partial class Category_Item
{
    public partial class Item_Item
    {
        [HookAttribute.Patch("OnItemStacked", "OnItemStacked [1]", "Item", "MoveToContainer", ["ItemContainer", "System.Int32", "System.Boolean", "System.Boolean", "BasePlayer", "System.Boolean"])]
        [HookAttribute.Identifier("5065f34804af47feb078c17811f891eb")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("local13", "Item")]
        [MetadataAttribute.Parameter("self", "Item")]
        [MetadataAttribute.Parameter("newcontainer", "ItemContainer")]
        [MetadataAttribute.Parameter("local15", "System.Int32")]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_Item_5065f34804af47feb078c17811f891eb : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 291)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)746311991)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_S 13 False  
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 13);
                    // AddYieldInstruction: Ldarg_0  True Item 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True ItemContainer 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Ldloc_S 15 False  
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 15);
                    // WAAAAAAA 4.2
                    // AddYieldInstruction: Box Carbon.Extensions.AccessToolsEx.TypeByName("System.Int32") False  
                    yield return new CodeInstruction(OpCodes.Box, Carbon.Extensions.AccessToolsEx.TypeByName("System.Int32"));
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object), typeof(object), typeof(object), typeof(object), }));
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

public partial class Category_Item
{
    public partial class Item_ItemModUnwrap
    {
        [HookAttribute.Patch("OnItemUnwrap", "OnItemUnwrap", "ItemModUnwrap", "ServerCommand", ["Item", "System.String", "BasePlayer"])]
        [HookAttribute.Identifier("5e8883af0d624918a34ee845a639b597")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("item", "Item")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Parameter("self", "ItemModUnwrap")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_ItemModUnwrap_5e8883af0d624918a34ee845a639b597 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 9)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)283863316)).MoveLabelsFrom(instruction);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True Item 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // HERE!
                    // AddYieldInstruction: Ldarg_3  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldarg_3);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Ldarg_0  True ItemModUnwrap 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
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

public partial class Category_Item
{
    public partial class Item_Item
    {
        [HookAttribute.Patch("OnItemStacked", "OnItemStacked [2]", "Item", "MoveToContainer", ["ItemContainer", "System.Int32", "System.Boolean", "System.Boolean", "BasePlayer", "System.Boolean"])]
        [HookAttribute.Identifier("edc2f512b22640f6b8e0726043761771")]
        [HookAttribute.Dependencies(new System.String[] { "OnItemStacked [1]" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("local23", "Item")]
        [MetadataAttribute.Parameter("self", "Item")]
        [MetadataAttribute.Parameter("newcontainer", "ItemContainer")]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_Item_edc2f512b22640f6b8e0726043761771 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 573)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)746311991)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_S 23 False  
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 23);
                    // AddYieldInstruction: Ldarg_0  True Item 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True ItemContainer 
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

public partial class Category_Item
{
    public partial class Item_PaintedItemStorageEntity
    {
        [HookAttribute.Patch("OnItemPainted", "OnItemPainted", "PaintedItemStorageEntity", "Server_UpdateImage", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("44a749e28ec14aaab2c901759c28d494")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "PaintedItemStorageEntity")]
        [MetadataAttribute.Parameter("local0", "Item")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Parameter("local1", "System.Byte[]")]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_PaintedItemStorageEntity_44a749e28ec14aaab2c901759c28d494 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 126)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2127678047)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True PaintedItemStorageEntity 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_0  True Item 
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BaseEntity+RPCMessage player
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // Set BaseEntity+RPCMessage
                    // value:player isProperty:False runtimeType:BasePlayer currentType:BaseEntity+RPCMessage type:BaseEntity+RPCMessage
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("BaseEntity+RPCMessage"), "player"));
                    // Read BaseEntity+RPCMessage : BaseEntity+RPCMessage
                    // WAAAAAAA 1
                    // AddYieldInstruction: Ldloc_1  True System.Byte[] 
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object), typeof(object), typeof(object), typeof(object), }));
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

public partial class Category_Item
{
    public partial class Item_Recycler
    {
        [HookAttribute.Patch("OnItemRecycleAmount", "OnItemRecycleAmount", "Recycler", "RecycleThink", [])]
        [HookAttribute.Identifier("4688e3d7f0ff4a62a203bdf8361d0a28")]
        [HookAttribute.Dependencies(new System.String[] { "OnItemRecycle [2]" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("local3", "Item")]
        [MetadataAttribute.Parameter("local4", "System.Int32")]
        [MetadataAttribute.Parameter("self", "Recycler")]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_Recycler_4688e3d7f0ff4a62a203bdf8361d0a28 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 67)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3746186934)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_3  True Item 
                    yield return new CodeInstruction(OpCodes.Ldloc_3);
                    // AddYieldInstruction: Ldloc_S 4 False  
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 4);
                    // WAAAAAAA 4.2
                    // AddYieldInstruction: Box Carbon.Extensions.AccessToolsEx.TypeByName("System.Int32") False  
                    yield return new CodeInstruction(OpCodes.Box, Carbon.Extensions.AccessToolsEx.TypeByName("System.Int32"));
                    // AddYieldInstruction: Ldarg_0  True Recycler 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
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
                    // AddYieldInstruction: Stloc_S 4 False  
                    yield return new CodeInstruction(OpCodes.Stloc_S, 4);

                    yield return instruction;
                }
            }
        }
    }
}

public partial class Category_Item
{
    public partial class Item_PlayerInventory
    {
        [HookAttribute.Patch("OnInventoryItemsCount", "OnInventoryItemsCount", "PlayerInventory", "GetAmount", ["System.Int32", "System.Boolean"])]
        [HookAttribute.Identifier("fe7a4522b51042cbae5b848801417694")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "PlayerInventory")]
        [MetadataAttribute.Return(typeof(System.Int32))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_PlayerInventory_fe7a4522b51042cbae5b848801417694 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1800965447)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldarg_1  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 3
                    // AddYieldInstruction: Box typeof(System.Int32) False  
                    yield return new CodeInstruction(OpCodes.Box, typeof(System.Int32));
                    // AddYieldInstruction: Ldarg_2  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    // WAAAAAAA 3
                    // AddYieldInstruction: Box typeof(System.Boolean) False  
                    yield return new CodeInstruction(OpCodes.Box, typeof(System.Boolean));
                    // AddYieldInstruction: Call AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object),typeof(object),typeof(object), }) False  
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

public partial class Category_Item
{
    public partial class Item_PlayerInventory
    {
        [HookAttribute.Patch("OnInventoryItemsTake", "OnInventoryItemsTake", "PlayerInventory", "Take", ["System.Collections.Generic.List`1<Item>", "System.Int32", "System.Int32"])]
        [HookAttribute.Identifier("2c1d772df06349d5b95633963deef004")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "PlayerInventory")]
        [MetadataAttribute.Return(typeof(System.Int32))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_PlayerInventory_2c1d772df06349d5b95633963deef004 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)565506075)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldarg_1  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // AddYieldInstruction: Ldarg_2  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    // WAAAAAAA 3
                    // AddYieldInstruction: Box typeof(System.Int32) False  
                    yield return new CodeInstruction(OpCodes.Box, typeof(System.Int32));
                    // AddYieldInstruction: Ldarg_3  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_3);
                    // WAAAAAAA 3
                    // AddYieldInstruction: Box typeof(System.Int32) False  
                    yield return new CodeInstruction(OpCodes.Box, typeof(System.Int32));
                    // AddYieldInstruction: Call AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object),typeof(object),typeof(object),typeof(object), }) False  
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object), typeof(object), typeof(object), typeof(object), }));
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

public partial class Category_Item
{
    public partial class Item_PlayerInventory
    {
        [HookAttribute.Patch("OnInventoryItemsFind", "OnInventoryItemsFind", "PlayerInventory", "FindItemsByItemID", ["System.Collections.Generic.List`1<Item>", "System.Int32"])]
        [HookAttribute.Identifier("1fa5ad21935f4abb83a9e035f02ff068")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "PlayerInventory")]
        [MetadataAttribute.Parameter("id", "System.Int32")]
        [MetadataAttribute.Parameter("list", "System.Collections.Generic.List`1[Item]")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_PlayerInventory_1fa5ad21935f4abb83a9e035f02ff068 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3014194616)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True PlayerInventory 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_2  True System.Int32 
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Box typeof(System.Int32) False  
                    yield return new CodeInstruction(OpCodes.Box, typeof(System.Int32));
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True System.Collections.Generic.List`1[Item] 
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

public partial class Category_Item
{
    public partial class Item_PlayerInventory
    {
        [HookAttribute.Patch("OnInventoryAmmoFind", "OnInventoryAmmoFind", "PlayerInventory", "FindAmmo", ["System.Collections.Generic.List`1<Item>", "Rust.AmmoTypes"])]
        [HookAttribute.Identifier("dee759a17f74434092b971358fe56a65")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "PlayerInventory")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_PlayerInventory_dee759a17f74434092b971358fe56a65 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2374724162)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldarg_1  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // AddYieldInstruction: Ldarg_2  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    // WAAAAAAA 3
                    // AddYieldInstruction: Box typeof(Rust.AmmoTypes) False  
                    yield return new CodeInstruction(OpCodes.Box, typeof(Rust.AmmoTypes));
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

public partial class Category_Item
{
    public partial class Item_PlayerInventory
    {
        [HookAttribute.Patch("OnBackpackDrop", "OnBackpackDrop", "PlayerInventory", "TryDropBackpack", [])]
        [HookAttribute.Identifier("83ee1950ceff4e6a9755385cd666e07e")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("local0", "Item")]
        [MetadataAttribute.Parameter("self", "PlayerInventory")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_PlayerInventory_83ee1950ceff4e6a9755385cd666e07e : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 9)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1554715023)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_0  True Item 
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    // AddYieldInstruction: Ldarg_0  True PlayerInventory 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
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

public partial class Category_Item
{
    public partial class Item_DroppedItem
    {
        [HookAttribute.Patch("OnDroppedItemCombined", "OnDroppedItemCombined", "DroppedItem", "OnDroppedOn", ["DroppedItem"])]
        [HookAttribute.Identifier("9168f81e2c324427ab8275a0184764d0")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "DroppedItem")]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_DroppedItem_9168f81e2c324427ab8275a0184764d0 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 146)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)744056691)).MoveLabelsFrom(instruction);
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

public partial class Category_Item
{
    public partial class Item_DroppedItem
    {
        [HookAttribute.Patch("OnItemDespawn", "OnItemDespawn", "DroppedItem", "IdleDestroy", [])]
        [HookAttribute.Identifier("e4b468c130494fc084513749d18bc833")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "DroppedItem")]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_DroppedItem_e4b468c130494fc084513749d18bc833 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)214500625)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True DroppedItem item
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // Set DroppedItem
                    // value:item isProperty:False runtimeType:Item currentType:DroppedItem type:DroppedItem
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("DroppedItem"), "item"));
                    // Read DroppedItem : DroppedItem
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

public partial class Category_Item
{
    public partial class Item_Chainsaw
    {
        [HookAttribute.Patch("OnInventoryAmmoItemFind", "OnInventoryAmmoItemFind [Chainsaw]", "Chainsaw", "GetAmmo", [])]
        [HookAttribute.Identifier("cf09102a84194fefb9c777815a5a5e86")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("inventory", "PlayerInventory")]
        [MetadataAttribute.Parameter("self", "Chainsaw")]
        [MetadataAttribute.Return(typeof(Item))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_Chainsaw_cf09102a84194fefb9c777815a5a5e86 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 8)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3730208425)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_0  True BasePlayer inventory
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    // Set BasePlayer
                    // value:inventory isProperty:True runtimeType:PlayerInventory currentType:BasePlayer type:BasePlayer
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("BasePlayer"), "get_inventory"));
                    // AddYieldInstruction: Ldarg_0  True Chainsaw fuelType
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // Set Chainsaw
                    // value:fuelType isProperty:False runtimeType:ItemDefinition currentType:Chainsaw type:Chainsaw
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("Chainsaw"), "fuelType"));
                    // Read Chainsaw : Chainsaw
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

public partial class Category_Item
{
    public partial class Item_FlameThrower
    {
        [HookAttribute.Patch("OnInventoryAmmoItemFind", "OnInventoryAmmoItemFind [FlameThrower]", "FlameThrower", "GetAmmo", [])]
        [HookAttribute.Identifier("2e42442073af42b385c673b914515b9f")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("inventory", "PlayerInventory")]
        [MetadataAttribute.Parameter("self", "FlameThrower")]
        [MetadataAttribute.Return(typeof(Item))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_FlameThrower_2e42442073af42b385c673b914515b9f : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 8)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3730208425)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_0  True BasePlayer inventory
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    // Set BasePlayer
                    // value:inventory isProperty:True runtimeType:PlayerInventory currentType:BasePlayer type:BasePlayer
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("BasePlayer"), "get_inventory"));
                    // AddYieldInstruction: Ldarg_0  True FlameThrower fuelType
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // Set FlameThrower
                    // value:fuelType isProperty:False runtimeType:ItemDefinition currentType:FlameThrower type:FlameThrower
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("FlameThrower"), "fuelType"));
                    // Read FlameThrower : FlameThrower
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

public partial class Category_Item
{
    public partial class Item_WeaponRack
    {
        [HookAttribute.Patch("OnRackedWeaponMount", "OnRackedWeaponMount", "WeaponRack", "MountWeapon", ["Item", "BasePlayer", "System.Int32", "System.Int32", "System.Boolean"])]
        [HookAttribute.Identifier("254e980b27be4e57b578dfabb854b63f")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("item", "Item")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Parameter("self", "WeaponRack")]
        [MetadataAttribute.Return(typeof(System.Boolean))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_WeaponRack_254e980b27be4e57b578dfabb854b63f : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1157802010)).MoveLabelsFrom(instruction);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True Item 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // HERE!
                    // AddYieldInstruction: Ldarg_2  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Ldarg_0  True WeaponRack 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object), typeof(object), typeof(object), }));
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

public partial class Category_Item
{
    public partial class Item_WeaponRack
    {
        [HookAttribute.Patch("OnRackedWeaponMounted", "OnRackedWeaponMounted", "WeaponRack", "MountWeapon", ["Item", "BasePlayer", "System.Int32", "System.Int32", "System.Boolean"])]
        [HookAttribute.Identifier("75befb1537d8432094be4b8b1c468912")]
        [HookAttribute.Dependencies(new System.String[] { "OnRackedWeaponMount" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("item", "Item")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Parameter("self", "WeaponRack")]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_WeaponRack_75befb1537d8432094be4b8b1c468912 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 109)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1049908860)).MoveLabelsFrom(instruction);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True Item 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // HERE!
                    // AddYieldInstruction: Ldarg_2  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Ldarg_0  True WeaponRack 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
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

public partial class Category_Item
{
    public partial class Item_WeaponRack
    {
        [HookAttribute.Patch("OnRackedWeaponSwap", "OnRackedWeaponSwap", "WeaponRack", "SwapPlayerWeapon", ["BasePlayer", "System.Int32", "System.Int32", "System.Int32"])]
        [HookAttribute.Identifier("2e00592eda5143a4bea6a908a403ffd5")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("local0", "Item")]
        [MetadataAttribute.Parameter("local2", "WeaponRackSlot")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Parameter("self", "WeaponRack")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_WeaponRack_2e00592eda5143a4bea6a908a403ffd5 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 48)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3562654725)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_0  True Item 
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    // AddYieldInstruction: Ldloc_2  True WeaponRackSlot 
                    yield return new CodeInstruction(OpCodes.Ldloc_2);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Ldarg_0  True WeaponRack 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object), typeof(object), typeof(object), typeof(object), }));
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

public partial class Category_Item
{
    public partial class Item_WeaponRack
    {
        [HookAttribute.Patch("OnRackedWeaponSwapped", "OnRackedWeaponSwapped", "WeaponRack", "SwapPlayerWeapon", ["BasePlayer", "System.Int32", "System.Int32", "System.Int32"])]
        [HookAttribute.Identifier("20b0954e6871475eb007bb48e57a4a8d")]
        [HookAttribute.Dependencies(new System.String[] { "OnRackedWeaponSwap" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("local0", "Item")]
        [MetadataAttribute.Parameter("local2", "WeaponRackSlot")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Parameter("self", "WeaponRack")]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_WeaponRack_20b0954e6871475eb007bb48e57a4a8d : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 78)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2954263053)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_0  True Item 
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    // AddYieldInstruction: Ldloc_2  True WeaponRackSlot 
                    yield return new CodeInstruction(OpCodes.Ldloc_2);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Ldarg_0  True WeaponRack 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object), typeof(object), typeof(object), typeof(object), }));
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

public partial class Category_Item
{
    public partial class Item_WeaponRack
    {
        [HookAttribute.Patch("OnRackedWeaponTake", "OnRackedWeaponTake", "WeaponRack", "GivePlayerWeapon", ["BasePlayer", "System.Int32", "System.Int32", "System.Boolean", "System.Boolean"])]
        [HookAttribute.Identifier("d0c19ad4de1f459a8e57613833dfdfa1")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("local1", "Item")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Parameter("self", "WeaponRack")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_WeaponRack_d0c19ad4de1f459a8e57613833dfdfa1 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 21)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)411927536)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_1  True Item 
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Ldarg_0  True WeaponRack 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
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

public partial class Category_Item
{
    public partial class Item_WeaponRack
    {
        [HookAttribute.Patch("OnRackedWeaponTaken", "OnRackedWeaponTaken", "WeaponRack", "GivePlayerWeapon", ["BasePlayer", "System.Int32", "System.Int32", "System.Boolean", "System.Boolean"])]
        [HookAttribute.Identifier("98ca64af176a463c96b1619fc4416bba")]
        [HookAttribute.Dependencies(new System.String[] { "OnRackedWeaponTake" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("local1", "Item")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Parameter("self", "WeaponRack")]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_WeaponRack_98ca64af176a463c96b1619fc4416bba : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 136)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3033876624)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_1  True Item 
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Ldarg_0  True WeaponRack 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
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

public partial class Category_Item
{
    public partial class Item_WeaponRack
    {
        [HookAttribute.Patch("OnRackedWeaponUnload", "OnRackedWeaponUnload", "WeaponRack", "UnloadWeapon", ["BasePlayer", "System.Int32"])]
        [HookAttribute.Identifier("4e8786a361ea49a4896766bda95bc866")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("local1", "Item")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Parameter("self", "WeaponRack")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_WeaponRack_4e8786a361ea49a4896766bda95bc866 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 25)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2853729425)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_1  True Item 
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Ldarg_0  True WeaponRack 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
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

public partial class Category_Item
{
    public partial class Item_WeaponRack
    {
        [HookAttribute.Patch("OnRackedWeaponUnloaded", "OnRackedWeaponUnloaded", "WeaponRack", "UnloadWeapon", ["BasePlayer", "System.Int32"])]
        [HookAttribute.Identifier("7a42f2d1a7084a61915240a31c43ec33")]
        [HookAttribute.Dependencies(new System.String[] { "OnRackedWeaponUnload" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("local1", "Item")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Parameter("self", "WeaponRack")]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_WeaponRack_7a42f2d1a7084a61915240a31c43ec33 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)18069347)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_1  True Item 
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Ldarg_0  True WeaponRack 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
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

public partial class Category_Item
{
    public partial class Item_WeaponRack
    {
        [HookAttribute.Patch("OnRackedWeaponLoad", "OnRackedWeaponLoad", "WeaponRack", "LoadWeaponAmmo", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("2fde6f3b39d345df9f4d3b6f4e5a9313")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("local4", "Item")]
        [MetadataAttribute.Parameter("local7", "ItemDefinition")]
        [MetadataAttribute.Parameter("local0", "BasePlayer")]
        [MetadataAttribute.Parameter("self", "WeaponRack")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_WeaponRack_2fde6f3b39d345df9f4d3b6f4e5a9313 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 59)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2919494006)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_S 4 False  
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 4);
                    // AddYieldInstruction: Ldloc_S 7 False  
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 7);
                    // AddYieldInstruction: Ldloc_0  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    // AddYieldInstruction: Ldarg_0  True WeaponRack 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object), typeof(object), typeof(object), typeof(object), }));
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

public partial class Category_Item
{
    public partial class Item_WeaponRack
    {
        [HookAttribute.Patch("OnRackedWeaponLoaded", "OnRackedWeaponLoaded", "WeaponRack", "LoadWeaponAmmo", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("35120dd6fa2445b581afac056d0e10b4")]
        [HookAttribute.Dependencies(new System.String[] { "OnRackedWeaponLoad" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("local4", "Item")]
        [MetadataAttribute.Parameter("local7", "ItemDefinition")]
        [MetadataAttribute.Parameter("local0", "BasePlayer")]
        [MetadataAttribute.Parameter("self", "WeaponRack")]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_WeaponRack_35120dd6fa2445b581afac056d0e10b4 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 155)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2867370870)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_S 4 False  
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 4);
                    // AddYieldInstruction: Ldloc_S 7 False  
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 7);
                    // AddYieldInstruction: Ldloc_0  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    // AddYieldInstruction: Ldarg_0  True WeaponRack 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object), typeof(object), typeof(object), typeof(object), }));
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

public partial class Category_Item
{
    public partial class Item_ItemCrafter
    {
        [HookAttribute.Patch("CanFastTrackCraftTask", "CanFastTrackCraftTask", "ItemCrafter", "FastTrackTask", ["System.Int32"])]
        [HookAttribute.Identifier("325a402443dc44f8b96cacfb407b19ef")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "ItemCrafter")]
        [MetadataAttribute.Parameter("local2", "ItemCraftTask")]
        [MetadataAttribute.Parameter("taskID", "System.Int32")]
        [MetadataAttribute.Return(typeof(System.Boolean))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_ItemCrafter_325a402443dc44f8b96cacfb407b19ef : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 47)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2901690581)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True ItemCrafter 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_2  True ItemCraftTask 
                    yield return new CodeInstruction(OpCodes.Ldloc_2);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True System.Int32 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Box typeof(System.Int32) False  
                    yield return new CodeInstruction(OpCodes.Box, typeof(System.Int32));
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

public partial class Category_Item
{
    public partial class Item_Locker
    {
        [HookAttribute.Patch("CanLockerAcceptItem", "CanLockerAcceptItem", "Locker", "ItemFilter", ["Item", "System.Int32"])]
        [HookAttribute.Identifier("114ae07278034c6f8cf4c23fe1e3d7c0")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "Locker")]
        [MetadataAttribute.Return(typeof(System.Boolean))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_Locker_114ae07278034c6f8cf4c23fe1e3d7c0 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1026445911)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldarg_1  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // AddYieldInstruction: Ldarg_2  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    // WAAAAAAA 3
                    // AddYieldInstruction: Box typeof(System.Int32) False  
                    yield return new CodeInstruction(OpCodes.Box, typeof(System.Int32));
                    // AddYieldInstruction: Call AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object),typeof(object),typeof(object), }) False  
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

public partial class Category_Item
{
    public partial class Item_StorageContainer
    {
        [HookAttribute.Patch("OnItemFilter", "OnItemFilter", "StorageContainer", "ItemFilter", ["Item", "System.Int32"])]
        [HookAttribute.Identifier("4e1b85f221e6410eac5b14598ffd6460")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("item", "Item")]
        [MetadataAttribute.Parameter("self", "StorageContainer")]
        [MetadataAttribute.Parameter("targetSlot", "System.Int32")]
        [MetadataAttribute.Return(typeof(System.Boolean))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_StorageContainer_4e1b85f221e6410eac5b14598ffd6460 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)44080502)).MoveLabelsFrom(instruction);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True Item 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Ldarg_0  True StorageContainer 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_2  True System.Int32 
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Box typeof(System.Int32) False  
                    yield return new CodeInstruction(OpCodes.Box, typeof(System.Int32));
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

public partial class Category_Item
{
    public partial class Item_PlayerInventory
    {
        [HookAttribute.Patch("OnInventoryItemFind", "OnInventoryItemFind", "PlayerInventory", "FindItemByItemID", ["System.Int32"])]
        [HookAttribute.Identifier("86963112b56745c89e9efcfb6efa733a")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "PlayerInventory")]
        [MetadataAttribute.Return(typeof(Item))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_PlayerInventory_86963112b56745c89e9efcfb6efa733a : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3087521756)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldarg_1  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 3
                    // AddYieldInstruction: Box typeof(System.Int32) False  
                    yield return new CodeInstruction(OpCodes.Box, typeof(System.Int32));
                    // AddYieldInstruction: Call AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object),typeof(object), }) False  
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

public partial class Category_Item
{
    public partial class Item_PlayerInventory
    {
        [HookAttribute.Patch("OnInventoryAmmoItemFind", "OnInventoryAmmoItemFind [PlayerInventory]", "PlayerInventory", "FindAmmo", ["Rust.AmmoTypes"])]
        [HookAttribute.Identifier("8fe4f48138cd4c23bc80ba236ac7652f")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "PlayerInventory")]
        [MetadataAttribute.Return(typeof(Item))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_PlayerInventory_8fe4f48138cd4c23bc80ba236ac7652f : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3730208425)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldarg_1  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 3
                    // AddYieldInstruction: Box typeof(Rust.AmmoTypes) False  
                    yield return new CodeInstruction(OpCodes.Box, typeof(Rust.AmmoTypes));
                    // AddYieldInstruction: Call AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object),typeof(object), }) False  
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

public partial class Category_Item
{
    public partial class Item_Recycler
    {
        [HookAttribute.Patch("OnItemRecycle", "OnItemRecycle [2]", "Recycler", "RecycleThink", [])]
        [HookAttribute.Identifier("1458dd463dfb4493babe40d3ec3824d2")]
        [HookAttribute.Dependencies(new System.String[] { "OnItemRecycle" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_Recycler_1458dd463dfb4493babe40d3ec3824d2 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                // Opop
                edit.Add(new CodeInstruction(OpCodes.Ldarg_0));
                edit.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("Recycler"), "HasRecyclable")));
                Label label_d51ffc6f195e48e7862180daf9a590e8 = Generator.DefineLabel();
                original[378].labels.Add(label_d51ffc6f195e48e7862180daf9a590e8);
                edit.Add(new CodeInstruction(OpCodes.Brtrue_S, label_d51ffc6f195e48e7862180daf9a590e8));
                // Opop
                edit.Add(new CodeInstruction(OpCodes.Ldarg_0));
                edit.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("Recycler"), "StopRecycling")));

                original.InsertRange(29, edit);
                return original.AsEnumerable();
            }
        }
    }
}

public partial class Category_Item
{
    public partial class Item_Item
    {
        [HookAttribute.Patch("OnItemLock", "OnItemLock", "Item", "LockUnlock", ["System.Boolean"])]
        [HookAttribute.Identifier("065c900e1f3b4c72a695b0c9c3153585")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_Item_065c900e1f3b4c72a695b0c9c3153585 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                edit.Add(new CodeInstruction(OpCodes.Ldarg_1));
                Label label_ee078646d5534705aeba143a9097bfad = Generator.DefineLabel();
                original[6].labels.Add(label_ee078646d5534705aeba143a9097bfad);
                edit.Add(new CodeInstruction(OpCodes.Brfalse_S, label_ee078646d5534705aeba143a9097bfad));
                edit.Add(new CodeInstruction(OpCodes.Ldstr, "OnItemLock"));
                // Opop
                edit.Add(new CodeInstruction(OpCodes.Ldarg_0));
                edit.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("Oxide.Core.Interface"), "CallHook", new System.Type[] { typeof(System.String), typeof(System.Object) })));
                edit.Add(new CodeInstruction(OpCodes.Ldnull));
                edit.Add(new CodeInstruction(OpCodes.Beq_S, label_ee078646d5534705aeba143a9097bfad));
                edit.Add(new CodeInstruction(OpCodes.Ret));

                original.InsertRange(6, edit);
                return original.AsEnumerable();
            }
        }
    }
}

public partial class Category_Item
{
    public partial class Item_Item
    {
        [HookAttribute.Patch("OnItemUnlock", "OnItemUnlock", "Item", "LockUnlock", ["System.Boolean"])]
        [HookAttribute.Identifier("d90f4a1fb73f41fcad3bff6271311d5c")]
        [HookAttribute.Dependencies(new System.String[] { "OnItemLock" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_Item_d90f4a1fb73f41fcad3bff6271311d5c : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                edit.Add(new CodeInstruction(OpCodes.Ldarg_1));
                Label label_b81b36e1ae77455d917bbbb41184e653 = Generator.DefineLabel();
                original[14].labels.Add(label_b81b36e1ae77455d917bbbb41184e653);
                edit.Add(new CodeInstruction(OpCodes.Brtrue_S, label_b81b36e1ae77455d917bbbb41184e653));
                edit.Add(new CodeInstruction(OpCodes.Ldstr, "OnItemUnlock"));
                // Opop
                edit.Add(new CodeInstruction(OpCodes.Ldarg_0));
                edit.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("Oxide.Core.Interface"), "CallHook", new System.Type[] { typeof(System.String), typeof(System.Object) })));
                edit.Add(new CodeInstruction(OpCodes.Ldnull));
                edit.Add(new CodeInstruction(OpCodes.Beq_S, label_b81b36e1ae77455d917bbbb41184e653));
                edit.Add(new CodeInstruction(OpCodes.Ret));

                original.InsertRange(14, edit);
                return original.AsEnumerable();
            }
        }
    }
}

public partial class Category_Item
{
    public partial class Item_LootContainer
    {
        [HookAttribute.Patch("OnBonusItemDropped [patch 1]", "OnBonusItemDropped [patch 1]", "LootContainer", "DropBonusItems", ["BaseEntity", "ItemContainer"])]
        [HookAttribute.Identifier("7b93d14e11fc4cfea59901e808002573")]
        [HookAttribute.Dependencies(new System.String[] { "OnBonusItemDropped" })]
        [HookAttribute.Options(HookFlags.Patch)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_LootContainer_7b93d14e11fc4cfea59901e808002573 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                Label label_615ea367dab74a9da1089069037ab873 = Generator.DefineLabel();
                original[128].labels.Add(label_615ea367dab74a9da1089069037ab873);
                edit.Add(new CodeInstruction(OpCodes.Ble, label_615ea367dab74a9da1089069037ab873));

                original[22 + 1].MoveLabelsFrom(original[22]);
                original.RemoveRange(22, 1);
                original.InsertRange(22, edit);
                return original.AsEnumerable();
            }
        }
    }
}

public partial class Category_Item
{
    public partial class Item_LootContainer
    {
        [HookAttribute.Patch("OnBonusItemDropped [patch 2]", "OnBonusItemDropped [patch 2]", "LootContainer", "DropBonusItems", ["BaseEntity", "ItemContainer"])]
        [HookAttribute.Identifier("bf24903823554fef8613c00d2178b586")]
        [HookAttribute.Dependencies(new System.String[] { "OnBonusItemDropped [patch 1]" })]
        [HookAttribute.Options(HookFlags.Patch)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_LootContainer_bf24903823554fef8613c00d2178b586 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                Label label_fb3d5afb3df84c7eb46adbb53d9db4c6 = Generator.DefineLabel();
                original[128].labels.Add(label_fb3d5afb3df84c7eb46adbb53d9db4c6);
                edit.Add(new CodeInstruction(OpCodes.Brfalse, label_fb3d5afb3df84c7eb46adbb53d9db4c6));

                original[26 + 1].MoveLabelsFrom(original[26]);
                original.RemoveRange(26, 1);
                original.InsertRange(26, edit);
                return original.AsEnumerable();
            }
        }
    }
}

public partial class Category_Item
{
    public partial class Item_LootContainer
    {
        [HookAttribute.Patch("OnBonusItemDropped [patch 3]", "OnBonusItemDropped [patch 3]", "LootContainer", "DropBonusItems", ["BaseEntity", "ItemContainer"])]
        [HookAttribute.Identifier("d0a737bae1ca4c0bac9dc3a62108822c")]
        [HookAttribute.Dependencies(new System.String[] { "OnBonusItemDropped [patch 2]" })]
        [HookAttribute.Options(HookFlags.Patch)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_LootContainer_d0a737bae1ca4c0bac9dc3a62108822c : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                Label label_09d9d6d4397f4c3a965da55fc1c13d7c = Generator.DefineLabel();
                original[128].labels.Add(label_09d9d6d4397f4c3a965da55fc1c13d7c);
                edit.Add(new CodeInstruction(OpCodes.Ble_Un, label_09d9d6d4397f4c3a965da55fc1c13d7c));

                original[44 + 1].MoveLabelsFrom(original[44]);
                original.RemoveRange(44, 1);
                original.InsertRange(44, edit);
                return original.AsEnumerable();
            }
        }
    }
}

public partial class Category_Item
{
    public partial class Item_LootContainer
    {
        [HookAttribute.Patch("OnBonusItemDropped [patch 4]", "OnBonusItemDropped [patch 4]", "LootContainer", "DropBonusItems", ["BaseEntity", "ItemContainer"])]
        [HookAttribute.Identifier("a465f64ebd4c4781b675f63fb9f898f9")]
        [HookAttribute.Dependencies(new System.String[] { "OnBonusItemDropped [patch 3]" })]
        [HookAttribute.Options(HookFlags.Patch)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_LootContainer_a465f64ebd4c4781b675f63fb9f898f9 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                Label label_b8184a669d3349ad956b8e6997c4ea35 = Generator.DefineLabel();
                original[128].labels.Add(label_b8184a669d3349ad956b8e6997c4ea35);
                edit.Add(new CodeInstruction(OpCodes.Ble_S, label_b8184a669d3349ad956b8e6997c4ea35));

                original[87 + 1].MoveLabelsFrom(original[87]);
                original.RemoveRange(87, 1);
                original.InsertRange(87, edit);
                return original.AsEnumerable();
            }
        }
    }
}

public partial class Category_Item
{
    public partial class Item_LootContainer
    {
        [HookAttribute.Patch("OnBonusItemDropped [patch 5]", "OnBonusItemDropped [patch 5]", "LootContainer", "DropBonusItems", ["BaseEntity", "ItemContainer"])]
        [HookAttribute.Identifier("bbea5354ea90446e908e8f21d09dae18")]
        [HookAttribute.Dependencies(new System.String[] { "OnBonusItemDropped [patch 4]" })]
        [HookAttribute.Options(HookFlags.Patch)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_LootContainer_bbea5354ea90446e908e8f21d09dae18 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                Label label_082e5880880f49e58e5501cc8f36cc5b = Generator.DefineLabel();
                original[128].labels.Add(label_082e5880880f49e58e5501cc8f36cc5b);
                edit.Add(new CodeInstruction(OpCodes.Brfalse_S, label_082e5880880f49e58e5501cc8f36cc5b));

                original[96 + 1].MoveLabelsFrom(original[96]);
                original.RemoveRange(96, 1);
                original.InsertRange(96, edit);
                return original.AsEnumerable();
            }
        }
    }
}

public partial class Category_Item
{
    public partial class Item_Mailbox
    {
        [HookAttribute.Patch("OnItemSubmit", "OnItemSubmit [patch]", "Mailbox", "SubmitInputItems", ["BasePlayer"])]
        [HookAttribute.Identifier("a4c9d456a6a64266927dedfd34694988")]
        [HookAttribute.Dependencies(new System.String[] { "OnItemSubmit" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Item")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Item_Mailbox_a4c9d456a6a64266927dedfd34694988 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                Label label_5d7f0c7c4dff4e1ba9b2605614a01600 = Generator.DefineLabel();
                original[18].labels.Add(label_5d7f0c7c4dff4e1ba9b2605614a01600);
                edit.Add(new CodeInstruction(OpCodes.Beq_S, label_5d7f0c7c4dff4e1ba9b2605614a01600));
                Label label_46a57e8f1a3d4d25828677ed08accbc1 = Generator.DefineLabel();
                original[56].labels.Add(label_46a57e8f1a3d4d25828677ed08accbc1);
                edit.Add(new CodeInstruction(OpCodes.Br_S, label_46a57e8f1a3d4d25828677ed08accbc1));

                original[16 + 2].MoveLabelsFrom(original[16]);
                original.RemoveRange(16, 2);
                original.InsertRange(16, edit);
                return original.AsEnumerable();
            }
        }
    }
}

