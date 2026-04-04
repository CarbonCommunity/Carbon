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

public partial class Category_Vending
{
    public partial class Vending_VendingMachine
    {
        [HookAttribute.Patch("OnBuyVendingItem", "OnBuyVendingItem", "VendingMachine", "BuyItem", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("7233059f0ee144b688f472f91eec5930")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "VendingMachine")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Parameter("local0", "System.Int32")]
        [MetadataAttribute.Parameter("local1", "System.Int32")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Vending")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Vending_VendingMachine_7233059f0ee144b688f472f91eec5930 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2954685032)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True VendingMachine 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BaseEntity+RPCMessage player
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // Set BaseEntity+RPCMessage
                    // value:player isProperty:False runtimeType:BasePlayer currentType:BaseEntity+RPCMessage type:BaseEntity+RPCMessage
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("BaseEntity+RPCMessage"), "player"));
                    // Read BaseEntity+RPCMessage : BaseEntity+RPCMessage
                    // WAAAAAAA 1
                    // AddYieldInstruction: Ldloc_0  True System.Int32 
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    // WAAAAAAA 4.2
                    // AddYieldInstruction: Box Carbon.Extensions.AccessToolsEx.TypeByName("System.Int32") False  
                    yield return new CodeInstruction(OpCodes.Box, Carbon.Extensions.AccessToolsEx.TypeByName("System.Int32"));
                    // AddYieldInstruction: Ldloc_1  True System.Int32 
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
                    // WAAAAAAA 4.2
                    // AddYieldInstruction: Box Carbon.Extensions.AccessToolsEx.TypeByName("System.Int32") False  
                    yield return new CodeInstruction(OpCodes.Box, Carbon.Extensions.AccessToolsEx.TypeByName("System.Int32"));
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

public partial class Category_Vending
{
    public partial class Vending_VendingMachine
    {
        [HookAttribute.Patch("CanUseVending", "CanUseVending", "VendingMachine", "CanOpenLootPanel", ["BasePlayer", "System.String"])]
        [HookAttribute.Identifier("498e338074b149f796cc03c2be4be828")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Parameter("self", "VendingMachine")]
        [MetadataAttribute.Return(typeof(System.Boolean))]
        [MetadataAttribute.Category("Vending")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Vending_VendingMachine_498e338074b149f796cc03c2be4be828 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1307174715)).MoveLabelsFrom(instruction);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Ldarg_0  True VendingMachine 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
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

public partial class Category_Vending
{
    public partial class Vending_VendingMachine
    {
        [HookAttribute.Patch("CanAdministerVending", "CanAdministerVending", "VendingMachine", "CanPlayerAdmin", ["BasePlayer"])]
        [HookAttribute.Identifier("c00d4aafc7a54900874db2fc447b356c")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Parameter("self", "VendingMachine")]
        [MetadataAttribute.Return(typeof(System.Boolean))]
        [MetadataAttribute.Category("Vending")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Vending_VendingMachine_c00d4aafc7a54900874db2fc447b356c : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3072084583)).MoveLabelsFrom(instruction);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Ldarg_0  True VendingMachine 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
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

public partial class Category_Vending
{
    public partial class Vending_VendingMachine
    {
        [HookAttribute.Patch("OnRefreshVendingStock", "OnRefreshVendingStock", "VendingMachine", "RefreshSellOrderStockLevel", ["ItemDefinition"])]
        [HookAttribute.Identifier("083049f5d8b34200adf0a3d65ff694ce")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "VendingMachine")]
        [MetadataAttribute.Parameter("itemDef", "ItemDefinition")]
        [MetadataAttribute.Category("Vending")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Vending_VendingMachine_083049f5d8b34200adf0a3d65ff694ce : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 61)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1089385978)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True VendingMachine 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True ItemDefinition 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
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

public partial class Category_Vending
{
    public partial class Vending_VendingMachine
    {
        [HookAttribute.Patch("OnToggleVendingBroadcast", "OnToggleVendingBroadcast", "VendingMachine", "RPC_Broadcast", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("b13122723934496aa73d0ee7d9d2a56c")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "VendingMachine")]
        [MetadataAttribute.Parameter("local0", "BasePlayer")]
        [MetadataAttribute.Category("Vending")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Vending_VendingMachine_b13122723934496aa73d0ee7d9d2a56c : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 17)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2821238809)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True VendingMachine 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_0  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
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

public partial class Category_Vending
{
    public partial class Vending_VendingMachine
    {
        [HookAttribute.Patch("OnDeleteVendingOffer", "OnDeleteVendingOffer", "VendingMachine", "RPC_DeleteSellOrder", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("3ceccfa28d3041aaa9adfe4a121c811b")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "VendingMachine")]
        [MetadataAttribute.Parameter("local1", "System.Int32")]
        [MetadataAttribute.Category("Vending")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Vending_VendingMachine_3ceccfa28d3041aaa9adfe4a121c811b : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1045055657)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True VendingMachine 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_1  True System.Int32 
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
                    // WAAAAAAA 4.2
                    // AddYieldInstruction: Box Carbon.Extensions.AccessToolsEx.TypeByName("System.Int32") False  
                    yield return new CodeInstruction(OpCodes.Box, Carbon.Extensions.AccessToolsEx.TypeByName("System.Int32"));
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

public partial class Category_Vending
{
    public partial class Vending_VendingMachine
    {
        [HookAttribute.Patch("OnOpenVendingAdmin", "OnOpenVendingAdmin", "VendingMachine", "RPC_OpenAdmin", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("0988efa78a7941b6ad4f36ad144292c8")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "VendingMachine")]
        [MetadataAttribute.Parameter("local0", "BasePlayer")]
        [MetadataAttribute.Category("Vending")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Vending_VendingMachine_0988efa78a7941b6ad4f36ad144292c8 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)130032208)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True VendingMachine 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_0  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
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

public partial class Category_Vending
{
    public partial class Vending_VendingMachine
    {
        [HookAttribute.Patch("OnVendingShopOpen", "OnVendingShopOpen [VendingMachine]", "VendingMachine", "RPC_OpenShop", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("8ece15b50de94f2da780b3d54a44dfe4")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "VendingMachine")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Vending")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Vending_VendingMachine_8ece15b50de94f2da780b3d54a44dfe4 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1029092944)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True VendingMachine 
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

public partial class Category_Vending
{
    public partial class Vending_VendingMachine
    {
        [HookAttribute.Patch("OnRotateVendingMachine", "OnRotateVendingMachine", "VendingMachine", "RPC_RotateVM", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("b73f1c75f1bd4f739fa1c2863fb9e11d")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "VendingMachine")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Vending")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Vending_VendingMachine_b73f1c75f1bd4f739fa1c2863fb9e11d : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3251770984)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True VendingMachine 
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

public partial class Category_Vending
{
    public partial class Vending_VendingMachine
    {
        [HookAttribute.Patch("CanVendingAcceptItem", "CanVendingAcceptItem", "VendingMachine", "CanAcceptItem", ["Item", "System.Int32"])]
        [HookAttribute.Identifier("9c9ef03f2e95456e8878f8f848d367a4")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "VendingMachine")]
        [MetadataAttribute.Return(typeof(System.Boolean))]
        [MetadataAttribute.Category("Vending")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Vending_VendingMachine_9c9ef03f2e95456e8878f8f848d367a4 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)4058960600)).MoveLabelsFrom(instruction);
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

public partial class Category_Vending
{
    public partial class Vending_VendingMachine
    {
        [HookAttribute.Patch("OnAddVendingOffer", "OnAddVendingOffer", "VendingMachine", "AddSellOrder", ["System.Int32", "System.Int32", "System.Int32", "System.Int32", "System.Byte"])]
        [HookAttribute.Identifier("2d41662c401d4ab8aa49e0312f634b41")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "VendingMachine")]
        [MetadataAttribute.Parameter("local2", "ProtoBuf.VendingMachine+SellOrder")]
        [MetadataAttribute.Category("Vending")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Vending_VendingMachine_2d41662c401d4ab8aa49e0312f634b41 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 63)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)4219534513)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True VendingMachine 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_2  True ProtoBuf.VendingMachine+SellOrder 
                    yield return new CodeInstruction(OpCodes.Ldloc_2);
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

public partial class Category_Vending
{
    public partial class Vending_NPCVendingMachine
    {
        [HookAttribute.Patch("CanAdministerVending", "CanAdministerVending [NPC]", "NPCVendingMachine", "CanPlayerAdmin", ["BasePlayer"])]
        [HookAttribute.Identifier("be3450d20c6c47d0b6fb1a32d8537d6c")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Parameter("self", "NPCVendingMachine")]
        [MetadataAttribute.Return(typeof(System.Boolean))]
        [MetadataAttribute.Category("Vending")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Vending_NPCVendingMachine_be3450d20c6c47d0b6fb1a32d8537d6c : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3072084583)).MoveLabelsFrom(instruction);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Ldarg_0  True NPCVendingMachine 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
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

public partial class Category_Vending
{
    public partial class Vending_VendingMachine
    {
        [HookAttribute.Patch("OnGiveSoldItem", "OnGiveSoldItem", "VendingMachine", "GiveSoldItem", ["Item", "BasePlayer"])]
        [HookAttribute.Identifier("6377369bb5fa41f2afe31cfed18d3e13")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "VendingMachine")]
        [MetadataAttribute.Parameter("soldItem", "Item")]
        [MetadataAttribute.Parameter("buyer", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Vending")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Vending_VendingMachine_6377369bb5fa41f2afe31cfed18d3e13 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)42703263)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True VendingMachine 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True Item 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // HERE!
                    // AddYieldInstruction: Ldarg_2  True BasePlayer 
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

public partial class Category_Vending
{
    public partial class Vending_VendingMachine
    {
        [HookAttribute.Patch("OnVendingShopRename", "OnVendingShopRename", "VendingMachine", "RPC_UpdateShopName", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("b2de6bf2e965434db7e6523011da1924")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "VendingMachine")]
        [MetadataAttribute.Parameter("local1", "System.String")]
        [MetadataAttribute.Parameter("local0", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Vending")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Vending_VendingMachine_b2de6bf2e965434db7e6523011da1924 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3675496111)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True VendingMachine 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_1  True System.String 
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
                    // AddYieldInstruction: Ldloc_0  True BasePlayer 
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

public partial class Category_Vending
{
    public partial class Vending_VendingMachine
    {
        [HookAttribute.Patch("OnTakeCurrencyItem", "OnTakeCurrencyItem", "VendingMachine", "TakeCurrencyItem", ["Item"])]
        [HookAttribute.Identifier("58c28e9692ec4f588b432721c4c0d069")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "VendingMachine")]
        [MetadataAttribute.Parameter("takenCurrencyItem", "Item")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Vending")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Vending_VendingMachine_58c28e9692ec4f588b432721c4c0d069 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)819770087)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True VendingMachine 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True Item 
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

public partial class Category_Vending
{
    public partial class Vending_NPCVendingMachine
    {
        [HookAttribute.Patch("OnTakeCurrencyItem", "OnTakeCurrencyItem [NPC]", "NPCVendingMachine", "TakeCurrencyItem", ["Item"])]
        [HookAttribute.Identifier("ad914d7eb0f44b13b50991c5edeaf1f5")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "NPCVendingMachine")]
        [MetadataAttribute.Parameter("takenCurrencyItem", "Item")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Vending")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Vending_NPCVendingMachine_ad914d7eb0f44b13b50991c5edeaf1f5 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)819770087)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True NPCVendingMachine 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True Item 
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

public partial class Category_Vending
{
    public partial class Vending_VendingMachine
    {
        [HookAttribute.Patch("OnVendingTransaction", "OnVendingTransaction", "VendingMachine", "DoTransaction", ["BasePlayer", "System.Int32", "System.Int32", "ItemContainer", "System.Action`2<BasePlayer,Item>", "System.Action`2<BasePlayer,Item>", "MarketTerminal"])]
        [HookAttribute.Identifier("a7d01351fa2541ed9346aca09c282ec7")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "VendingMachine")]
        [MetadataAttribute.Parameter("buyer", "BasePlayer")]
        [MetadataAttribute.Parameter("sellOrderId", "System.Int32")]
        [MetadataAttribute.Parameter("numberOfTransactions", "System.Int32")]
        [MetadataAttribute.Parameter("targetContainer", "ItemContainer")]
        [MetadataAttribute.Return(typeof(System.Boolean))]
        [MetadataAttribute.Category("Vending")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Vending_VendingMachine_a7d01351fa2541ed9346aca09c282ec7 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 24)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)101737999)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True VendingMachine 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // HERE!
                    // AddYieldInstruction: Ldarg_2  True System.Int32 
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Box typeof(System.Int32) False  
                    yield return new CodeInstruction(OpCodes.Box, typeof(System.Int32));
                    // HERE!
                    // AddYieldInstruction: Ldarg_3  True System.Int32 
                    yield return new CodeInstruction(OpCodes.Ldarg_3);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Box typeof(System.Int32) False  
                    yield return new CodeInstruction(OpCodes.Box, typeof(System.Int32));
                    // HERE!
                    // AddYieldInstruction: Ldarg_S 4 False  
                    yield return new CodeInstruction(OpCodes.Ldarg_S, 4);
                    // WAAAAAAA 1
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), }));
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

public partial class Category_Vending
{
    public partial class Vending_MarketTerminal
    {
        [HookAttribute.Patch("CanAccessVendingMachine", "CanAccessVendingMachine", "MarketTerminal", "<GetDeliveryEligibleVendingMachines>g__IsEligible|43_0", ["VendingMachine", "UnityEngine.Vector3", "System.Int32"])]
        [HookAttribute.Identifier("e124ca6e4ba34222b49c958f05b3adc5")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "MarketTerminal")]
        [MetadataAttribute.Parameter("vendingMachine", "VendingMachine")]
        [MetadataAttribute.Return(typeof(System.Boolean))]
        [MetadataAttribute.Category("Vending")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Vending_MarketTerminal_e124ca6e4ba34222b49c958f05b3adc5 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1949289818)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True MarketTerminal config
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // Set MarketTerminal
                    // value:config isProperty:False runtimeType:DeliveryDroneConfig currentType:MarketTerminal type:MarketTerminal
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("MarketTerminal"), "config"));
                    // Read MarketTerminal : MarketTerminal
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True VendingMachine 
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

public partial class Category_Vending
{
    public partial class Vending_VendingMachine
    {
        [HookAttribute.Patch("CanPurchaseItem", "CanPurchaseItem", "VendingMachine", "DoTransaction", ["BasePlayer", "System.Int32", "System.Int32", "ItemContainer", "System.Action`2<BasePlayer,Item>", "System.Action`2<BasePlayer,Item>", "MarketTerminal"])]
        [HookAttribute.Identifier("68b438e80e1c4c6491f70e84eacef264")]
        [HookAttribute.Dependencies(new System.String[] { "OnVendingTransaction" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("buyer", "BasePlayer")]
        [MetadataAttribute.Parameter("local21", "Item")]
        [MetadataAttribute.Parameter("onItemPurchased", "System.Action`2[BasePlayer,Item]")]
        [MetadataAttribute.Parameter("self", "VendingMachine")]
        [MetadataAttribute.Parameter("targetContainer", "ItemContainer")]
        [MetadataAttribute.Return(typeof(System.Boolean))]
        [MetadataAttribute.Category("Vending")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Vending_VendingMachine_68b438e80e1c4c6491f70e84eacef264 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 315)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3418689761)).MoveLabelsFrom(instruction);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Ldloc_S 21 False  
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 21);
                    // HERE!
                    // AddYieldInstruction: Ldarg_S 6 False  
                    yield return new CodeInstruction(OpCodes.Ldarg_S, 6);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Ldarg_0  True VendingMachine 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_S 4 False  
                    yield return new CodeInstruction(OpCodes.Ldarg_S, 4);
                    // WAAAAAAA 1
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), }));
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

public partial class Category_Vending
{
    public partial class Vending_NPCTalking
    {
        [HookAttribute.Patch("OnVendingShopOpen", "OnVendingShopOpen [NPCTalking]", "NPCTalking", "OnConversationAction", ["BasePlayer", "System.String"])]
        [HookAttribute.Identifier("11c1b07ee1a446ab98c7318e687b1ed8")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("local1", "InvisibleVendingMachine")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Vending")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Vending_NPCTalking_11c1b07ee1a446ab98c7318e687b1ed8 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1029092944)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_1  True InvisibleVendingMachine 
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
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

public partial class Category_Vending
{
    public partial class Vending_VendingMachine
    {
        [HookAttribute.Patch("OnVendingShopOpened", "OnVendingShopOpened [VendingMachine]", "VendingMachine", "RPC_OpenShop", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("7b2bdcd3968d44228cbd8ccb121b3bdc")]
        [HookAttribute.Dependencies(new System.String[] { "OnVendingShopOpen [VendingMachine]" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "VendingMachine")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Category("Vending")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Vending_VendingMachine_7b2bdcd3968d44228cbd8ccb121b3bdc : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)397104876)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True VendingMachine 
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

public partial class Category_Vending
{
    public partial class Vending_NPCTalking
    {
        [HookAttribute.Patch("OnVendingShopOpened", "OnVendingShopOpened [NPCTalking]", "NPCTalking", "OnConversationAction", ["BasePlayer", "System.String"])]
        [HookAttribute.Identifier("9db892c9e77e415faa95d7edfdd26ddc")]
        [HookAttribute.Dependencies(new System.String[] { "OnVendingShopOpen [NPCTalking]" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("local1", "InvisibleVendingMachine")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Category("Vending")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Vending_NPCTalking_9db892c9e77e415faa95d7edfdd26ddc : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 36)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)397104876)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_1  True InvisibleVendingMachine 
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
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

public partial class Category_Vending
{
    public partial class Vending_TravellingVendor
    {
        [HookAttribute.Patch("OnVendingShopOpen", "OnVendingShopOpen [TravellingVendor]", "TravellingVendor", "SV_OpenMenu", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("a83f5022a5744f14bce412deeda23632")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "TravellingVendor")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Vending")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Vending_TravellingVendor_a83f5022a5744f14bce412deeda23632 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1029092944)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True TravellingVendor vendingMachine
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // Set TravellingVendor
                    // value:vendingMachine isProperty:False runtimeType:NPCVendingMachine currentType:TravellingVendor type:TravellingVendor
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("TravellingVendor"), "vendingMachine"));
                    // Read TravellingVendor : TravellingVendor
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

public partial class Category_Vending
{
    public partial class Vending_TravellingVendor
    {
        [HookAttribute.Patch("OnVendingShopOpened", "OnVendingShopOpened [TravellingVendor]", "TravellingVendor", "SV_OpenMenu", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("dbc0da2f1a594a9f9b8f6c63bbf44a66")]
        [HookAttribute.Dependencies(new System.String[] { "OnVendingShopOpen [TravellingVendor]" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "TravellingVendor")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Category("Vending")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Vending_TravellingVendor_dbc0da2f1a594a9f9b8f6c63bbf44a66 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)397104876)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True TravellingVendor vendingMachine
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // Set TravellingVendor
                    // value:vendingMachine isProperty:False runtimeType:NPCVendingMachine currentType:TravellingVendor type:TravellingVendor
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("TravellingVendor"), "vendingMachine"));
                    // Read TravellingVendor : TravellingVendor
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

