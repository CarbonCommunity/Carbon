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

public partial class Category_Firework
{
    public partial class Firework_PatternFirework
    {
        [HookAttribute.Patch("CanDesignFirework", "CanDesignFirework", "PatternFirework", "PlayerCanModify", ["BasePlayer"])]
        [HookAttribute.Identifier("557b85588b9048a386eb4b46b57d04c9")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Parameter("self", "PatternFirework")]
        [MetadataAttribute.Return(typeof(System.Boolean))]
        [MetadataAttribute.Category("Firework")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Firework_PatternFirework_557b85588b9048a386eb4b46b57d04c9 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)31771492)).MoveLabelsFrom(instruction);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Ldarg_0  True PatternFirework 
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

public partial class Category_Firework
{
    public partial class Firework_BaseFirework
    {
        [HookAttribute.Patch("OnFireworkStarted", "OnFireworkStarted", "BaseFirework", "Begin", [])]
        [HookAttribute.Identifier("387f5cea2a64493bbf1e437a4ffacda9")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "BaseFirework")]
        [MetadataAttribute.Category("Firework")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Firework_BaseFirework_387f5cea2a64493bbf1e437a4ffacda9 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1475245939)).MoveLabelsFrom(instruction);
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

public partial class Category_Firework
{
    public partial class Firework_BaseFirework
    {
        [HookAttribute.Patch("OnFireworkExhausted", "OnFireworkExhausted", "BaseFirework", "OnExhausted", [])]
        [HookAttribute.Identifier("87d668c7941e4039a33f2c9cb7c38c02")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "BaseFirework")]
        [MetadataAttribute.Category("Firework")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Firework_BaseFirework_87d668c7941e4039a33f2c9cb7c38c02 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3435805585)).MoveLabelsFrom(instruction);
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

public partial class Category_Firework
{
    public partial class Firework_BaseFirework
    {
        [HookAttribute.Patch("OnFireworkDamage", "OnFireworkDamage", "BaseFirework", "OnAttacked", ["HitInfo"])]
        [HookAttribute.Identifier("f63ea93d0ea1432fbc39049295a3f6d0")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "BaseFirework")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Firework")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Firework_BaseFirework_f63ea93d0ea1432fbc39049295a3f6d0 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2487799829)).MoveLabelsFrom(instruction);
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

public partial class Category_Firework
{
    public partial class Firework_PatternFirework
    {
        [HookAttribute.Patch("OnFireworkDesignChange", "OnFireworkDesignChange", "PatternFirework", "ServerSetFireworkDesign", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("4fc3d6bc39554707bf3237aa83032b28")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "PatternFirework")]
        [MetadataAttribute.Parameter("local0", "ProtoBuf.PatternFirework+Design")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Firework")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Firework_PatternFirework_4fc3d6bc39554707bf3237aa83032b28 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 11)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)814018261)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True PatternFirework 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_0  True ProtoBuf.PatternFirework+Design 
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

public partial class Category_Firework
{
    public partial class Firework_PatternFirework
    {
        [HookAttribute.Patch("OnFireworkDesignChanged", "OnFireworkDesignChanged", "PatternFirework", "ServerSetFireworkDesign", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("d2a1d4af182f4893beba34aa69f5ddf3")]
        [HookAttribute.Dependencies(new System.String[] { "OnFireworkDesignChange" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "PatternFirework")]
        [MetadataAttribute.Parameter("local0", "ProtoBuf.PatternFirework+Design")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Category("Firework")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Firework_PatternFirework_d2a1d4af182f4893beba34aa69f5ddf3 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 112)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2779499551)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True PatternFirework 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_0  True ProtoBuf.PatternFirework+Design 
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

