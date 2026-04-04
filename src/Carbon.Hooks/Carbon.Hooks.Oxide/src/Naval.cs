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

public partial class Category_Naval
{
    public partial class Naval_SmallEngine
    {
        [HookAttribute.Patch("OnEngineReverse", "OnEngineReverse", "SmallEngine", "SV_ToggleReverse", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("7cdf661e05f145c3a7b181408482ad1c")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "SmallEngine")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Naval")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Naval_SmallEngine_7cdf661e05f145c3a7b181408482ad1c : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2798657606)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True SmallEngine 
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

public partial class Category_Naval
{
    public partial class Naval_Sail
    {
        [HookAttribute.Patch("CanRotateSail", "CanRotateSail", "Sail", "CanRotate", ["BasePlayer"])]
        [HookAttribute.Identifier("7088e2dbed9647bd99b017f6497d9f43")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "Sail")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Boolean))]
        [MetadataAttribute.Category("Naval")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Naval_Sail_7088e2dbed9647bd99b017f6497d9f43 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3819973365)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True Sail 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BasePlayer 
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

public partial class Category_Naval
{
    public partial class Naval_Sail
    {
        [HookAttribute.Patch("CanRaiseSail", "CanRaiseSail", "Sail", "CanBeRaised", ["BasePlayer"])]
        [HookAttribute.Identifier("7d7d663c67c0434ba8b313cc7de0a982")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "Sail")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Boolean))]
        [MetadataAttribute.Category("Naval")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Naval_Sail_7d7d663c67c0434ba8b313cc7de0a982 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)439109945)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True Sail 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BasePlayer 
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

public partial class Category_Naval
{
    public partial class Naval_Sail
    {
        [HookAttribute.Patch("CanLowerSail", "CanLowerSail", "Sail", "CanBeLowered", ["BasePlayer"])]
        [HookAttribute.Identifier("7760972e18c1495f8b8ab55f6761cbdb")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "Sail")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Boolean))]
        [MetadataAttribute.Category("Naval")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Naval_Sail_7760972e18c1495f8b8ab55f6761cbdb : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1493169743)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True Sail 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BasePlayer 
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

public partial class Category_Naval
{
    public partial class Naval_BoatGroupSpawner
    {
        [HookAttribute.Patch("OnBoatGroupSpawn", "OnBoatGroupSpawn", "BoatGroupSpawner", "SpawnBoatGroup", ["System.Collections.Generic.HashSet`1<RHIB>", "BoatAI/AILoadMode", "System.Boolean", "ScientistBoatOilrigManager"])]
        [HookAttribute.Identifier("7b13956a965240e78b659ed530475b94")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "BoatGroupSpawner")]
        [MetadataAttribute.Parameter("local1", "UnityEngine.Vector2")]
        [MetadataAttribute.Parameter("local4", "UnityEngine.Quaternion")]
        [MetadataAttribute.Parameter("list", "System.Collections.Generic.HashSet`1[RHIB]")]
        [MetadataAttribute.Parameter("local3", "System.Boolean")]
        [MetadataAttribute.Parameter("spawnsPT", "System.Boolean")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Naval")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Naval_BoatGroupSpawner_7b13956a965240e78b659ed530475b94 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 40)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)909607648)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True BoatGroupSpawner 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_1  True UnityEngine.Vector2 
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
                    // WAAAAAAA 4.2
                    // AddYieldInstruction: Box Carbon.Extensions.AccessToolsEx.TypeByName("UnityEngine.Vector2") False  
                    yield return new CodeInstruction(OpCodes.Box, Carbon.Extensions.AccessToolsEx.TypeByName("UnityEngine.Vector2"));
                    // AddYieldInstruction: Ldloc_S 4 False  
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 4);
                    // WAAAAAAA 4.2
                    // AddYieldInstruction: Box Carbon.Extensions.AccessToolsEx.TypeByName("UnityEngine.Quaternion") False  
                    yield return new CodeInstruction(OpCodes.Box, Carbon.Extensions.AccessToolsEx.TypeByName("UnityEngine.Quaternion"));
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True System.Collections.Generic.HashSet`1[RHIB] 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Ldloc_3  True System.Boolean 
                    yield return new CodeInstruction(OpCodes.Ldloc_3);
                    // WAAAAAAA 4.2
                    // AddYieldInstruction: Box Carbon.Extensions.AccessToolsEx.TypeByName("System.Boolean") False  
                    yield return new CodeInstruction(OpCodes.Box, Carbon.Extensions.AccessToolsEx.TypeByName("System.Boolean"));
                    // HERE!
                    // AddYieldInstruction: Ldarg_3  True System.Boolean 
                    yield return new CodeInstruction(OpCodes.Ldarg_3);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Box typeof(System.Boolean) False  
                    yield return new CodeInstruction(OpCodes.Box, typeof(System.Boolean));
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

public partial class Category_Naval
{
    public partial class Naval_TriggerDeepSeaPortal
    {
        [HookAttribute.Patch("OnDeepSeaTeleport", "OnDeepSeaTeleport", "TriggerDeepSeaPortal", "OnEntityEnter", ["BaseEntity"])]
        [HookAttribute.Identifier("7fb067ca616e4c56a10691c01dd151c2")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "TriggerDeepSeaPortal")]
        [MetadataAttribute.Parameter("ent", "BaseEntity")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Naval")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Naval_TriggerDeepSeaPortal_7fb067ca616e4c56a10691c01dd151c2 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 121)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1355776107)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True TriggerDeepSeaPortal 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_0  True TriggerDeepSeaPortal+<>c__DisplayClass3_0 ent
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    // Set TriggerDeepSeaPortal+<>c__DisplayClass3_0
                    // value:ent isProperty:False runtimeType:BaseEntity currentType:TriggerDeepSeaPortal+<>c__DisplayClass3_0 type:TriggerDeepSeaPortal+<>c__DisplayClass3_0
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("TriggerDeepSeaPortal+<>c__DisplayClass3_0"), "ent"));
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

public partial class Category_Naval
{
    public partial class Naval_Cannon
    {
        [HookAttribute.Patch("CanLightCannonFuse", "CanLightCannonFuse", "Cannon", "CanLightFuse", [])]
        [HookAttribute.Identifier("b4c29f8cedc54753b006a0b273cdaa81")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "Cannon")]
        [MetadataAttribute.Return(typeof(System.Boolean))]
        [MetadataAttribute.Category("Naval")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Naval_Cannon_b4c29f8cedc54753b006a0b273cdaa81 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1935391534)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True Cannon 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
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

public partial class Category_Naval
{
    public partial class Naval_SmallEngine
    {
        [HookAttribute.Patch("OnEngineStart", "OnEngineStart [SmallEngine]", "SmallEngine", "TurnOn", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("9c3d517ee8864680942b0ba2645ece11")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "SmallEngine")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Naval")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Naval_SmallEngine_9c3d517ee8864680942b0ba2645ece11 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1113127637)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True SmallEngine 
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

public partial class Category_Naval
{
    public partial class Naval_SmallEngine
    {
        [HookAttribute.Patch("OnEngineStop", "OnEngineStop [SmallEngine]", "SmallEngine", "TurnOff", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("dd9297c7017840938aca4754954bc4c2")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "SmallEngine")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Naval")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Naval_SmallEngine_dd9297c7017840938aca4754954bc4c2 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3054039405)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True SmallEngine 
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

public partial class Category_Naval
{
    public partial class Naval_PlayerBoat
    {
        [HookAttribute.Patch("OnPlayerBoatCollide", "OnPlayerBoatCollide", "PlayerBoat", "ProcessCollision", ["UnityEngine.Collision"])]
        [HookAttribute.Identifier("7b49bc7552e54c2aa081c0e3165c5c4a")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "PlayerBoat")]
        [MetadataAttribute.Parameter("local0", "BaseEntity")]
        [MetadataAttribute.Parameter("collision", "UnityEngine.Collision")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Naval")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Naval_PlayerBoat_7b49bc7552e54c2aa081c0e3165c5c4a : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2449028929)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True PlayerBoat 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_0  True BaseEntity 
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True UnityEngine.Collision 
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

public partial class Category_Naval
{
    public partial class Naval_DeepSeaManagerCloseDeepSeaAsyncd119
    {
        [HookAttribute.Patch("OnDeepSeaClosed", "OnDeepSeaClosed", "DeepSeaManager/<CloseDeepSeaAsync>d__119", "MoveNext", [])]
        [HookAttribute.Identifier("bc6a473ad0ed449d8e4d59bb16690d26")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("local1", "DeepSeaManager")]
        [MetadataAttribute.Category("Naval")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Naval_DeepSeaManagerCloseDeepSeaAsyncd119_bc6a473ad0ed449d8e4d59bb16690d26 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 148)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)565264246)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_1  True DeepSeaManager 
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

public partial class Category_Naval
{
    public partial class Naval_DeepSeaManagerOpenDeepSeaAsyncd117
    {
        [HookAttribute.Patch("OnDeepSeaOpened", "OnDeepSeaOpened", "DeepSeaManager/<OpenDeepSeaAsync>d__117", "MoveNext", [])]
        [HookAttribute.Identifier("eb2de8a6a074469a8534fe98ecc0f8e8")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("local1", "DeepSeaManager")]
        [MetadataAttribute.Category("Naval")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Naval_DeepSeaManagerOpenDeepSeaAsyncd117_eb2de8a6a074469a8534fe98ecc0f8e8 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)631225454)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_1  True DeepSeaManager 
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

public partial class Category_Naval
{
    public partial class Naval_TriggerDeepSeaPortal
    {
        [HookAttribute.Patch("CanTeleportDeepSea", "CanTeleportDeepSea", "TriggerDeepSeaPortal", "CanEntityTeleport", ["BaseEntity"])]
        [HookAttribute.Identifier("73db497e1ad04129ad2501a8537b9491")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("entity", "BaseEntity")]
        [MetadataAttribute.Parameter("self", "TriggerDeepSeaPortal")]
        [MetadataAttribute.Return(typeof(System.ValueTuple<System.Boolean, Translate.Phrase>))]
        [MetadataAttribute.Category("Naval")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Naval_TriggerDeepSeaPortal_73db497e1ad04129ad2501a8537b9491 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)4288677601)).MoveLabelsFrom(instruction);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BaseEntity 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Ldarg_0  True TriggerDeepSeaPortal Portal
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // Set TriggerDeepSeaPortal
                    // value:Portal isProperty:False runtimeType:DeepSeaPortal currentType:TriggerDeepSeaPortal type:TriggerDeepSeaPortal
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("TriggerDeepSeaPortal"), "Portal"));
                    // Read TriggerDeepSeaPortal : TriggerDeepSeaPortal
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
                    // AddYieldInstruction: Isinst typeof(System.ValueTuple<System.Boolean,Translate.Phrase>) False  
                    yield return new CodeInstruction(OpCodes.Isinst, typeof(System.ValueTuple<System.Boolean, Translate.Phrase>));
                    // AddYieldInstruction: Ldnull  True  
                    yield return new CodeInstruction(OpCodes.Ldnull);
                    // AddYieldInstruction: Beq_S label1 False  
                    yield return new CodeInstruction(OpCodes.Beq_S, label1);
                    // AddYieldInstruction: Ldloc retvar False  
                    yield return new CodeInstruction(OpCodes.Ldloc, retvar);
                    // AddYieldInstruction: Unbox_Any typeof(System.ValueTuple<System.Boolean,Translate.Phrase>) False  
                    yield return new CodeInstruction(OpCodes.Unbox_Any, typeof(System.ValueTuple<System.Boolean, Translate.Phrase>));
                    // AddYieldInstruction: Ret  True  
                    yield return new CodeInstruction(OpCodes.Ret);
                    // return behaviour end

                    yield return instruction;
                }
            }
        }
    }
}

public partial class Category_Naval
{
    public partial class Naval_BoatGroupSpawner
    {
        [HookAttribute.Patch("OnBoatGroupSpawned", "OnBoatGroupSpawned", "BoatGroupSpawner", "SpawnBoatGroup", ["System.Collections.Generic.HashSet`1<RHIB>", "BoatAI/AILoadMode", "System.Boolean", "ScientistBoatOilrigManager"])]
        [HookAttribute.Identifier("63a6c54accc44d299c8de3fdc6ec30db")]
        [HookAttribute.Dependencies(new System.String[] { "OnBoatGroupSpawn" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "BoatGroupSpawner")]
        [MetadataAttribute.Parameter("local1", "UnityEngine.Vector2")]
        [MetadataAttribute.Parameter("local4", "UnityEngine.Quaternion")]
        [MetadataAttribute.Parameter("list", "System.Collections.Generic.HashSet`1[RHIB]")]
        [MetadataAttribute.Parameter("local3", "System.Boolean")]
        [MetadataAttribute.Parameter("spawnsPT", "System.Boolean")]
        [MetadataAttribute.Category("Naval")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Naval_BoatGroupSpawner_63a6c54accc44d299c8de3fdc6ec30db : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 95)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2424062390)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True BoatGroupSpawner 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_1  True UnityEngine.Vector2 
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
                    // WAAAAAAA 4.2
                    // AddYieldInstruction: Box Carbon.Extensions.AccessToolsEx.TypeByName("UnityEngine.Vector2") False  
                    yield return new CodeInstruction(OpCodes.Box, Carbon.Extensions.AccessToolsEx.TypeByName("UnityEngine.Vector2"));
                    // AddYieldInstruction: Ldloc_S 4 False  
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 4);
                    // WAAAAAAA 4.2
                    // AddYieldInstruction: Box Carbon.Extensions.AccessToolsEx.TypeByName("UnityEngine.Quaternion") False  
                    yield return new CodeInstruction(OpCodes.Box, Carbon.Extensions.AccessToolsEx.TypeByName("UnityEngine.Quaternion"));
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True System.Collections.Generic.HashSet`1[RHIB] 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Ldloc_3  True System.Boolean 
                    yield return new CodeInstruction(OpCodes.Ldloc_3);
                    // WAAAAAAA 4.2
                    // AddYieldInstruction: Box Carbon.Extensions.AccessToolsEx.TypeByName("System.Boolean") False  
                    yield return new CodeInstruction(OpCodes.Box, Carbon.Extensions.AccessToolsEx.TypeByName("System.Boolean"));
                    // HERE!
                    // AddYieldInstruction: Ldarg_3  True System.Boolean 
                    yield return new CodeInstruction(OpCodes.Ldarg_3);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Box typeof(System.Boolean) False  
                    yield return new CodeInstruction(OpCodes.Box, typeof(System.Boolean));
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), }));
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

public partial class Category_Naval
{
    public partial class Naval_BoatBuildingStation
    {
        [HookAttribute.Patch("OnPlayerBoatEditStarted", "OnPlayerBoatEditStarted", "BoatBuildingStation", "ConvertPlayerBoatToConstruction", [])]
        [HookAttribute.Identifier("496a8e7cc95e4f40b9c3934b3e8ca197")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("local1", "PlayerBoat")]
        [MetadataAttribute.Parameter("self", "BoatBuildingStation")]
        [MetadataAttribute.Category("Naval")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Naval_BoatBuildingStation_496a8e7cc95e4f40b9c3934b3e8ca197 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 70)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2230452787)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_1  True PlayerBoat 
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
                    // AddYieldInstruction: Ldarg_0  True BoatBuildingStation 
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

public partial class Category_Naval
{
    public partial class Naval_PlayerBoat
    {
        [HookAttribute.Patch("CanEditPlayerBoat", "CanEditPlayerBoat", "PlayerBoat", "CanStartEditing", ["BasePlayer", "System.Boolean"])]
        [HookAttribute.Identifier("b68ca70068314edea3efdad5ef974117")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "PlayerBoat")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Boolean))]
        [MetadataAttribute.Category("Naval")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Naval_PlayerBoat_b68ca70068314edea3efdad5ef974117 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1618730577)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True PlayerBoat 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BasePlayer 
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

public partial class Category_Naval
{
    public partial class Naval_DeepSeaManagerOpenDeepSeaAsyncd117
    {
        [HookAttribute.Patch("OnDeepSeaOpen", "OnDeepSeaOpen", "DeepSeaManager/<OpenDeepSeaAsync>d__117", "MoveNext", [])]
        [HookAttribute.Identifier("c6a2a9199dee40628f1c61db2b906e80")]
        [HookAttribute.Dependencies(new System.String[] { "OnDeepSeaOpened" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("local1", "DeepSeaManager")]
        [MetadataAttribute.Category("Naval")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Naval_DeepSeaManagerOpenDeepSeaAsyncd117_c6a2a9199dee40628f1c61db2b906e80 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)4076046452)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_1  True DeepSeaManager 
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

public partial class Category_Naval
{
    public partial class Naval_DeepSeaManagerCloseDeepSeaAsyncd119
    {
        [HookAttribute.Patch("OnDeepSeaClose", "OnDeepSeaClose", "DeepSeaManager/<CloseDeepSeaAsync>d__119", "MoveNext", [])]
        [HookAttribute.Identifier("679ce4df768447eb8e6b0360de5053d6")]
        [HookAttribute.Dependencies(new System.String[] { "OnDeepSeaClosed" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("local1", "DeepSeaManager")]
        [MetadataAttribute.Category("Naval")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Naval_DeepSeaManagerCloseDeepSeaAsyncd119_679ce4df768447eb8e6b0360de5053d6 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1501492042)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_1  True DeepSeaManager 
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

public partial class Category_Naval
{
    public partial class Naval_BoatGroupSpawner
    {
        [HookAttribute.Patch("OnBoatGroupSpawned", "OnBoatGroupSpawned [Patch]", "BoatGroupSpawner", "SpawnBoatGroup", ["System.Collections.Generic.HashSet`1<RHIB>", "BoatAI/AILoadMode", "System.Boolean", "ScientistBoatOilrigManager"])]
        [HookAttribute.Identifier("0325888b91d240f9bfcf3aedcfc190e1")]
        [HookAttribute.Dependencies(new System.String[] { "OnBoatGroupSpawned" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Naval")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Naval_BoatGroupSpawner_0325888b91d240f9bfcf3aedcfc190e1 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                Label label_defd9897f885418392ec186b22f8070d = Generator.DefineLabel();
                original[108].labels.Add(label_defd9897f885418392ec186b22f8070d);
                edit.Add(new CodeInstruction(OpCodes.Brfalse, label_defd9897f885418392ec186b22f8070d));

                original[15 + 1].MoveLabelsFrom(original[15]);
                original.RemoveRange(15, 1);
                original.InsertRange(15, edit);
                return original.AsEnumerable();
            }
        }
    }
}

