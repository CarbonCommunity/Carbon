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

public partial class Category_Crafting
{
    public partial class Crafting_Recycler
    {
        [HookAttribute.Patch("CanRecycle", "CanRecycle", "Recycler", "HasRecyclable", [])]
        [HookAttribute.Identifier("17a6a1e935924188ae9bed8ef7d55baa")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "Recycler")]
        [MetadataAttribute.Parameter("local1", "Item")]
        [MetadataAttribute.Return(typeof(System.Boolean))]
        [MetadataAttribute.Category("Crafting")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Crafting_Recycler_17a6a1e935924188ae9bed8ef7d55baa : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2338075138)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True Recycler 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_1  True Item 
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
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

public partial class Category_Crafting
{
    public partial class Crafting_ItemCrafter
    {
        [HookAttribute.Patch("CanCraft", "CanCraft [ItemCrafter]", "ItemCrafter", "CanCraft", ["ItemBlueprint", "System.Int32", "System.Boolean"])]
        [HookAttribute.Identifier("5e7412ad66f54064bbf4a732994ca82d")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "ItemCrafter")]
        [MetadataAttribute.Return(typeof(System.Boolean))]
        [MetadataAttribute.Category("Crafting")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Crafting_ItemCrafter_5e7412ad66f54064bbf4a732994ca82d : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 74)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2427842956)).MoveLabelsFrom(instruction);
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
                    // AddYieldInstruction: Box typeof(System.Boolean) False  
                    yield return new CodeInstruction(OpCodes.Box, typeof(System.Boolean));
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

public partial class Category_Crafting
{
    public partial class Crafting_PlayerBlueprints
    {
        [HookAttribute.Patch("CanCraft", "CanCraft [PlayerBlueprints]", "PlayerBlueprints", "CanCraft", ["System.Int32", "System.Int32", "BasePlayer"])]
        [HookAttribute.Identifier("b547ec7e4ee04d01b63117b1d0653e99")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "PlayerBlueprints")]
        [MetadataAttribute.Parameter("local0", "ItemDefinition")]
        [MetadataAttribute.Parameter("skinItemId", "System.Int32")]
        [MetadataAttribute.Return(typeof(System.Boolean))]
        [MetadataAttribute.Category("Crafting")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Crafting_PlayerBlueprints_b547ec7e4ee04d01b63117b1d0653e99 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2427842956)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True PlayerBlueprints 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_0  True ItemDefinition 
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
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

public partial class Category_Crafting
{
    public partial class Crafting_ItemCrafter
    {
        [HookAttribute.Patch("OnIngredientsCollect", "OnIngredientsCollect", "ItemCrafter", "CollectIngredients", ["ItemBlueprint", "ItemCraftTask", "System.Int32", "BasePlayer", "System.Boolean"])]
        [HookAttribute.Identifier("f7d7b05695534e45a2bd5b082401fc3c")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "ItemCrafter")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Crafting")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Crafting_ItemCrafter_f7d7b05695534e45a2bd5b082401fc3c : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)696922433)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldarg_1  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // AddYieldInstruction: Ldarg_2  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    // AddYieldInstruction: Ldarg_3  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_3);
                    // WAAAAAAA 3
                    // AddYieldInstruction: Box typeof(System.Int32) False  
                    yield return new CodeInstruction(OpCodes.Box, typeof(System.Int32));
                    // AddYieldInstruction: Ldarg_S 4 False  
                    yield return new CodeInstruction(OpCodes.Ldarg_S, 4);
                    // AddYieldInstruction: Ldarg_S 5 False  
                    yield return new CodeInstruction(OpCodes.Ldarg_S, 5);
                    // WAAAAAAA 3
                    // AddYieldInstruction: Box typeof(System.Boolean) False  
                    yield return new CodeInstruction(OpCodes.Box, typeof(System.Boolean));
                    // AddYieldInstruction: Call AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object),typeof(object),typeof(object),typeof(object),typeof(object),typeof(object), }) False  
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

public partial class Category_Crafting
{
    public partial class Crafting_Recycler
    {
        [HookAttribute.Patch("CanBeRecycled", "CanBeRecycled", "Recycler", "CanBeRecycled", ["Item"])]
        [HookAttribute.Identifier("263226148eae4a3591149584d20414ad")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("item", "Item")]
        [MetadataAttribute.Parameter("self", "Recycler")]
        [MetadataAttribute.Return(typeof(System.Boolean))]
        [MetadataAttribute.Category("Crafting")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Crafting_Recycler_263226148eae4a3591149584d20414ad : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2745299625)).MoveLabelsFrom(instruction);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True Item 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Ldarg_0  True Recycler 
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

public partial class Category_Crafting
{
    public partial class Crafting_SprayCan
    {
        [HookAttribute.Patch("OnEntityReskin", "OnEntityReskin", "SprayCan", "ChangeItemSkin", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("249088a3e98c4036987682eee807c307")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("local3", "BaseEntity")]
        [MetadataAttribute.Parameter("local7", "ItemSkinDirectory+Skin")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Crafting")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Crafting_SprayCan_249088a3e98c4036987682eee807c307 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)990784691)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_3  True BaseEntity 
                    yield return new CodeInstruction(OpCodes.Ldloc_3);
                    // AddYieldInstruction: Ldloc_S 7 False  
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 7);
                    // WAAAAAAA 4.2
                    // AddYieldInstruction: Box Carbon.Extensions.AccessToolsEx.TypeByName("ItemSkinDirectory+Skin") False  
                    yield return new CodeInstruction(OpCodes.Box, Carbon.Extensions.AccessToolsEx.TypeByName("ItemSkinDirectory+Skin"));
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

public partial class Category_Crafting
{
    public partial class Crafting_SprayCan
    {
        [HookAttribute.Patch("OnEntityReskinned", "OnEntityReskinned", "SprayCan", "ChangeItemSkin", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("8c3be5dfcf9848409c02bc62d2f254b2")]
        [HookAttribute.Dependencies(new System.String[] { "OnEntityReskin" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("local3", "BaseEntity")]
        [MetadataAttribute.Parameter("local7", "ItemSkinDirectory+Skin")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Category("Crafting")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Crafting_SprayCan_8c3be5dfcf9848409c02bc62d2f254b2 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 1226)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3335109763)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_3  True BaseEntity 
                    yield return new CodeInstruction(OpCodes.Ldloc_3);
                    // AddYieldInstruction: Ldloc_S 7 False  
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 7);
                    // WAAAAAAA 4.2
                    // AddYieldInstruction: Box Carbon.Extensions.AccessToolsEx.TypeByName("ItemSkinDirectory+Skin") False  
                    yield return new CodeInstruction(OpCodes.Box, Carbon.Extensions.AccessToolsEx.TypeByName("ItemSkinDirectory+Skin"));
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

public partial class Category_Crafting
{
    public partial class Crafting_SprayCan
    {
        [HookAttribute.Patch("OnSprayCreate", "OnSprayCreate", "SprayCan", "CreateSpray", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("5555a8f6be2d427a95e3071218ce3c85")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "SprayCan")]
        [MetadataAttribute.Parameter("local0", "UnityEngine.Vector3")]
        [MetadataAttribute.Parameter("local5", "UnityEngine.Quaternion")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Crafting")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Crafting_SprayCan_5555a8f6be2d427a95e3071218ce3c85 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)777087196)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True SprayCan 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_0  True UnityEngine.Vector3 
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    // WAAAAAAA 4.2
                    // AddYieldInstruction: Box Carbon.Extensions.AccessToolsEx.TypeByName("UnityEngine.Vector3") False  
                    yield return new CodeInstruction(OpCodes.Box, Carbon.Extensions.AccessToolsEx.TypeByName("UnityEngine.Vector3"));
                    // AddYieldInstruction: Ldloc_S 5 False  
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 5);
                    // WAAAAAAA 4.2
                    // AddYieldInstruction: Box Carbon.Extensions.AccessToolsEx.TypeByName("UnityEngine.Quaternion") False  
                    yield return new CodeInstruction(OpCodes.Box, Carbon.Extensions.AccessToolsEx.TypeByName("UnityEngine.Quaternion"));
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

