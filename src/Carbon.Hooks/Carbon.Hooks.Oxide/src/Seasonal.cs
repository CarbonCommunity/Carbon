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

public partial class Category_Seasonal
{
    public partial class Seasonal_XMasRefill
    {
        [HookAttribute.Patch("OnXmasLootDistribute", "OnXmasLootDistribute", "XMasRefill", "ServerInit", [])]
        [HookAttribute.Identifier("449fe9a6366a4aec9077992b8fe75cb8")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "XMasRefill")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Seasonal")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Seasonal_XMasRefill_449fe9a6366a4aec9077992b8fe75cb8 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2685341751)).MoveLabelsFrom(instruction);
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

public partial class Category_Seasonal
{
    public partial class Seasonal_Stocking
    {
        [HookAttribute.Patch("OnXmasStockingFill", "OnXmasStockingFill", "Stocking", "SpawnLoot", [])]
        [HookAttribute.Identifier("7de5edc944b542558909357c937a7be6")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "Stocking")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Seasonal")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Seasonal_Stocking_7de5edc944b542558909357c937a7be6 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2445742731)).MoveLabelsFrom(instruction);
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

public partial class Category_Seasonal
{
    public partial class Seasonal_AdventCalendar
    {
        [HookAttribute.Patch("OnAdventGiftAward", "OnAdventGiftAward", "AdventCalendar", "AwardGift", ["BasePlayer"])]
        [HookAttribute.Identifier("b8aa8bbdc66f49998219131dab3171cc")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "AdventCalendar")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Seasonal")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Seasonal_AdventCalendar_b8aa8bbdc66f49998219131dab3171cc : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2572515364)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldarg_1  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // AddYieldInstruction: Call AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object),typeof(object), }) False  
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

public partial class Category_Seasonal
{
    public partial class Seasonal_AdventCalendar
    {
        [HookAttribute.Patch("OnAdventGiftAwarded", "OnAdventGiftAwarded", "AdventCalendar", "AwardGift", ["BasePlayer"])]
        [HookAttribute.Identifier("828ea984d3f54be9a21c208367acbf75")]
        [HookAttribute.Dependencies(new System.String[] { "OnAdventGiftAward" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "AdventCalendar")]
        [MetadataAttribute.Category("Seasonal")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Seasonal_AdventCalendar_828ea984d3f54be9a21c208367acbf75 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 179)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1624765317)).MoveLabelsFrom(instruction);
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

public partial class Category_Seasonal
{
    public partial class Seasonal_CollectableEasterEgg
    {
        [HookAttribute.Patch("OnEventCollectablePickup", "OnEventCollectablePickup", "CollectableEasterEgg", "RPC_PickUp", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("4600be19214d40e4bccca31bfa0fc6de")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Parameter("self", "CollectableEasterEgg")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Seasonal")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Seasonal_CollectableEasterEgg_4600be19214d40e4bccca31bfa0fc6de : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)4288346836)).MoveLabelsFrom(instruction);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BaseEntity+RPCMessage player
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // Set BaseEntity+RPCMessage
                    // value:player isProperty:False runtimeType:BasePlayer currentType:BaseEntity+RPCMessage type:BaseEntity+RPCMessage
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("BaseEntity+RPCMessage"), "player"));
                    // Read BaseEntity+RPCMessage : BaseEntity+RPCMessage
                    // WAAAAAAA 1
                    // AddYieldInstruction: Ldarg_0  True CollectableEasterEgg 
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

public partial class Category_Seasonal
{
    public partial class Seasonal_EggHuntEvent
    {
        [HookAttribute.Patch("OnHuntEventStart", "OnHuntEventStart", "EggHuntEvent", "StartEvent", [])]
        [HookAttribute.Identifier("84573d149db24988bb8fc208d4d0fb47")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "EggHuntEvent")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Seasonal")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Seasonal_EggHuntEvent_84573d149db24988bb8fc208d4d0fb47 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)240697297)).MoveLabelsFrom(instruction);
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

public partial class Category_Seasonal
{
    public partial class Seasonal_EggHuntEvent
    {
        [HookAttribute.Patch("OnHuntEventEnd", "OnHuntEventEnd", "EggHuntEvent", "Update", [])]
        [HookAttribute.Identifier("338cc3dac80747af906723afb826dbd4")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "EggHuntEvent")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Seasonal")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Seasonal_EggHuntEvent_338cc3dac80747af906723afb826dbd4 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 41)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)261230988)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True EggHuntEvent 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
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

public partial class Category_Seasonal
{
    public partial class Seasonal_XMasRefill
    {
        [HookAttribute.Patch("OnXmasGiftsDistribute", "OnXmasGiftsDistribute", "XMasRefill", "DistributeGiftsForPlayer", ["BasePlayer"])]
        [HookAttribute.Identifier("02a792765be44999815e72c3a0ca9b8a")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Seasonal")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Seasonal_XMasRefill_02a792765be44999815e72c3a0ca9b8a : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                edit.Add(new CodeInstruction(OpCodes.Ldstr, "OnXmasGiftsDistribute").MoveLabelsFrom(original[0]));
                // Opop
                edit.Add(new CodeInstruction(OpCodes.Ldarg_0));
                edit.Add(new CodeInstruction(OpCodes.Ldarg_1));
                edit.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("Oxide.Core.Interface"), "CallHook", new System.Type[] { typeof(System.String), typeof(System.Object), typeof(System.Object) })));
                edit.Add(new CodeInstruction(OpCodes.Ldnull));
                Label label_cd5d8f75dba94f3982a8aae7edb9c7a5 = Generator.DefineLabel();
                original[0].labels.Add(label_cd5d8f75dba94f3982a8aae7edb9c7a5);
                edit.Add(new CodeInstruction(OpCodes.Beq_S, label_cd5d8f75dba94f3982a8aae7edb9c7a5));
                edit.Add(new CodeInstruction(OpCodes.Ldc_I4_0));
                edit.Add(new CodeInstruction(OpCodes.Ret));

                original.InsertRange(0, edit);
                return original.AsEnumerable();
            }
        }
    }
}

public partial class Category_Seasonal
{
    public partial class Seasonal_AdventCalendar
    {
        [HookAttribute.Patch("CanBeAwardedAdventGift", "CanBeAwardedAdventGift", "AdventCalendar", "WasAwardedTodaysGift", ["BasePlayer"])]
        [HookAttribute.Identifier("a33dc5efc97f49ea9fcea4aaa199fe32")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Seasonal")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Seasonal_AdventCalendar_a33dc5efc97f49ea9fcea4aaa199fe32 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                edit.Add(new CodeInstruction(OpCodes.Ldstr, "CanBeAwardedAdventGift").MoveLabelsFrom(original[0]));
                // Opop
                edit.Add(new CodeInstruction(OpCodes.Ldarg_0));
                edit.Add(new CodeInstruction(OpCodes.Ldarg_1));
                edit.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("Oxide.Core.Interface"), "CallHook", new System.Type[] { typeof(System.String), typeof(System.Object), typeof(System.Object) })));
                LocalBuilder var_8dda1e0c03454bc182bac4b893333d6b = Generator.DeclareLocal(typeof(System.Object));
                edit.Add(new CodeInstruction(OpCodes.Stloc_S, var_8dda1e0c03454bc182bac4b893333d6b));
                edit.Add(new CodeInstruction(OpCodes.Ldloc_S, (sbyte)2));
                edit.Add(new CodeInstruction(OpCodes.Isinst, typeof(System.Boolean)));
                Label label_e895c34935a54a0a996003815fb6853d = Generator.DefineLabel();
                original[0].labels.Add(label_e895c34935a54a0a996003815fb6853d);
                edit.Add(new CodeInstruction(OpCodes.Brfalse_S, label_e895c34935a54a0a996003815fb6853d));
                edit.Add(new CodeInstruction(OpCodes.Ldloc_S, (sbyte)2));
                edit.Add(new CodeInstruction(OpCodes.Unbox_Any, typeof(System.Boolean)));
                edit.Add(new CodeInstruction(OpCodes.Ldc_I4_0));
                edit.Add(new CodeInstruction(OpCodes.Ceq));
                edit.Add(new CodeInstruction(OpCodes.Ret));

                original.InsertRange(0, edit);
                return original.AsEnumerable();
            }
        }
    }
}

