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

public partial class Category_NPC
{
    public partial class NPC_BaseNpc
    {
        [HookAttribute.Patch("CanNpcEat", "CanNpcEat [BaseNpc]", "BaseNpc", "WantsToEat", ["BaseEntity"])]
        [HookAttribute.Identifier("0bc5892a832a4742971e6e7018d63818")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "BaseNpc")]
        [MetadataAttribute.Return(typeof(System.Boolean))]
        [MetadataAttribute.Category("NPC")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class NPC_BaseNpc_0bc5892a832a4742971e6e7018d63818 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3166826695)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldarg_1  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
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

public partial class Category_NPC
{
    public partial class NPC_BaseNpc
    {
        [HookAttribute.Patch("OnNpcAttack", "OnNpcAttack [BaseNpc]", "BaseNpc", "StartAttack", [])]
        [HookAttribute.Identifier("93bf87b752574d498e0946b64baf6ea1")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "BaseNpc")]
        [MetadataAttribute.Parameter("self1", "BaseNpc")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("NPC")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class NPC_BaseNpc_93bf87b752574d498e0946b64baf6ea1 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2741067381)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True BaseNpc 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldarg_0  True BaseNpc AttackTarget
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // Set BaseNpc
                    // value:AttackTarget isProperty:True runtimeType:BaseEntity currentType:BaseNpc type:BaseNpc
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("BaseNpc"), "get_AttackTarget"));
                    // Read BaseNpc : BaseNpc
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

public partial class Category_NPC
{
    public partial class NPC_BaseNpc
    {
        [HookAttribute.Patch("IOnNpcTarget", "IOnNpcTarget [BaseNpc]", "BaseNpc", "GetWantsToAttack", ["BaseEntity"])]
        [HookAttribute.Identifier("68853df1071c4c2ab09162263aa50a9f")]
        [HookAttribute.Options(HookFlags.Static | HookFlags.Hidden)]
        [MetadataAttribute.Parameter("self", "BaseNpc")]
        [MetadataAttribute.Return(typeof(System.Single))]
        [MetadataAttribute.Category("NPC")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class NPC_BaseNpc_68853df1071c4c2ab09162263aa50a9f : API.Hooks.Patch
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
                    // AddYieldInstruction: Ldarg_0  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_1  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // AddYieldInstruction: Call AccessTools.Method(typeof(Carbon.Core.CorePlugin), "IOnNpcTarget") False  
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Carbon.Core.CorePlugin), "IOnNpcTarget"));
                    // hook call end

                    // return behaviour start
                    Label label1 = Generator.DefineLabel();
                    object retvar = Generator.DeclareLocal(typeof(object));
                    instruction.labels.Add(label1);
                    // AddYieldInstruction: Stloc retvar False  
                    yield return new CodeInstruction(OpCodes.Stloc, retvar);
                    // AddYieldInstruction: Ldloc retvar False  
                    yield return new CodeInstruction(OpCodes.Ldloc, retvar);
                    // AddYieldInstruction: Isinst typeof(System.Single) False  
                    yield return new CodeInstruction(OpCodes.Isinst, typeof(System.Single));
                    // AddYieldInstruction: Ldnull  True  
                    yield return new CodeInstruction(OpCodes.Ldnull);
                    // AddYieldInstruction: Beq_S label1 False  
                    yield return new CodeInstruction(OpCodes.Beq_S, label1);
                    // AddYieldInstruction: Ldloc retvar False  
                    yield return new CodeInstruction(OpCodes.Ldloc, retvar);
                    // AddYieldInstruction: Unbox_Any typeof(System.Single) False  
                    yield return new CodeInstruction(OpCodes.Unbox_Any, typeof(System.Single));
                    // AddYieldInstruction: Ret  True  
                    yield return new CodeInstruction(OpCodes.Ret);
                    // return behaviour end

                    yield return instruction;
                }
            }
        }
    }
}

public partial class Category_NPC
{
    public partial class NPC_NPCVendingMachine
    {
        [HookAttribute.Patch("OnNpcGiveSoldItem", "OnNpcGiveSoldItem", "NPCVendingMachine", "GiveSoldItem", ["Item", "BasePlayer"])]
        [HookAttribute.Identifier("9c99e4f414f34e319da61a915fb70d9c")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "NPCVendingMachine")]
        [MetadataAttribute.Parameter("soldItem", "Item")]
        [MetadataAttribute.Parameter("buyer", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("NPC")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class NPC_NPCVendingMachine_9c99e4f414f34e319da61a915fb70d9c : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)650484908)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True NPCVendingMachine 
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

public partial class Category_NPC
{
    public partial class NPC_ScientistNPC
    {
        [HookAttribute.Patch("OnNpcRadioChatter", "OnNpcRadioChatter [ScientistNPC]", "ScientistNPC", "PlayRadioChatter", [])]
        [HookAttribute.Identifier("88bd33f14b55432b8a2a5541bd02846a")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "ScientistNPC")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("NPC")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class NPC_ScientistNPC_88bd33f14b55432b8a2a5541bd02846a : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 19)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2231963737)).MoveLabelsFrom(instruction);
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

public partial class Category_NPC
{
    public partial class NPC_ScientistNPC
    {
        [HookAttribute.Patch("OnNpcAlert", "OnNpcAlert [ScientistNPC]", "ScientistNPC", "Alert", [])]
        [HookAttribute.Identifier("3f5e9f56840f4c51996f7a959bf20ac7")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "ScientistNPC")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("NPC")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class NPC_ScientistNPC_3f5e9f56840f4c51996f7a959bf20ac7 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2436309797)).MoveLabelsFrom(instruction);
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

public partial class Category_NPC
{
    public partial class NPC_NPCPlayer
    {
        [HookAttribute.Patch("OnNpcEquipWeapon", "OnNpcEquipWeapon [NPCPlayer]", "NPCPlayer", "EquipWeapon", ["System.Boolean"])]
        [HookAttribute.Identifier("b06c91e24a6f4580a1d3b4cfed41baf6")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "NPCPlayer")]
        [MetadataAttribute.Parameter("local0", "Item")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("NPC")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class NPC_NPCPlayer_b06c91e24a6f4580a1d3b4cfed41baf6 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)92399643)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True NPCPlayer 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_0  True Item 
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
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

public partial class Category_NPC
{
    public partial class NPC_ScientistNPC
    {
        [HookAttribute.Patch("OnNpcEquipWeapon", "OnNpcEquipWeapon [ScientistNPC]", "ScientistNPC", "EquipWeapon", ["System.Boolean"])]
        [HookAttribute.Identifier("dd2ab06e80eb439e863a1a4086d207d4")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "ScientistNPC")]
        [MetadataAttribute.Parameter("local1", "Item")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("NPC")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class NPC_ScientistNPC_dd2ab06e80eb439e863a1a4086d207d4 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)92399643)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True ScientistNPC 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_1  True Item 
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
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

public partial class Category_NPC
{
    public partial class NPC_HumanNPC
    {
        [HookAttribute.Patch("OnNpcDuck", "OnNpcDuck [HumanNPC]", "HumanNPC", "SetDucked", ["System.Boolean"])]
        [HookAttribute.Identifier("adbee7a1a5c642b4af93c27302204c6f")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "HumanNPC")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("NPC")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class NPC_HumanNPC_adbee7a1a5c642b4af93c27302204c6f : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)120484519)).MoveLabelsFrom(instruction);
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

public partial class Category_NPC
{
    public partial class NPC_NPCTalking
    {
        [HookAttribute.Patch("OnNpcConversationRespond", "OnNpcConversationRespond", "NPCTalking", "Server_ResponsePressed", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("e7884391993a431f91e3316b1565543f")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "NPCTalking")]
        [MetadataAttribute.Parameter("local0", "BasePlayer")]
        [MetadataAttribute.Parameter("local5", "ConversationData")]
        [MetadataAttribute.Parameter("local17", "ConversationData+ResponseNode")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("NPC")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class NPC_NPCTalking_e7884391993a431f91e3316b1565543f : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 131)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1580074841)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True NPCTalking 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_0  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    // AddYieldInstruction: Ldloc_S 5 False  
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 5);
                    // AddYieldInstruction: Ldloc_S 17 False  
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 17);
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

public partial class Category_NPC
{
    public partial class NPC_NPCTalking
    {
        [HookAttribute.Patch("OnNpcConversationResponded", "OnNpcConversationResponded", "NPCTalking", "Server_ResponsePressed", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("5ee5196ce2f54fe4b84aaab539e3c34c")]
        [HookAttribute.Dependencies(new System.String[] { "OnNpcConversationRespond" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "NPCTalking")]
        [MetadataAttribute.Parameter("local0", "BasePlayer")]
        [MetadataAttribute.Parameter("local5", "ConversationData")]
        [MetadataAttribute.Parameter("local17", "ConversationData+ResponseNode")]
        [MetadataAttribute.Category("NPC")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class NPC_NPCTalking_5ee5196ce2f54fe4b84aaab539e3c34c : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 190)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2723808568)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True NPCTalking 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_0  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    // AddYieldInstruction: Ldloc_S 5 False  
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 5);
                    // AddYieldInstruction: Ldloc_S 17 False  
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 17);
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

public partial class Category_NPC
{
    public partial class NPC_RustAiSimpleAIMemory
    {
        [HookAttribute.Patch("OnNpcTargetSense", "OnNpcTargetSense", "Rust.Ai.SimpleAIMemory", "SetKnown", ["BaseEntity", "BaseEntity", "AIBrainSenses"])]
        [HookAttribute.Identifier("3166fbfb74204437abcb4366f6795acc")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("owner", "BaseEntity")]
        [MetadataAttribute.Parameter("ent", "BaseEntity")]
        [MetadataAttribute.Parameter("brainSenses", "AIBrainSenses")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("NPC")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class NPC_RustAiSimpleAIMemory_3166fbfb74204437abcb4366f6795acc : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)978969450)).MoveLabelsFrom(instruction);
                    // HERE!
                    // AddYieldInstruction: Ldarg_2  True BaseEntity 
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    // WAAAAAAA 1
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BaseEntity 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // HERE!
                    // AddYieldInstruction: Ldarg_3  True AIBrainSenses 
                    yield return new CodeInstruction(OpCodes.Ldarg_3);
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

public partial class Category_NPC
{
    public partial class NPC_NPCTalking
    {
        [HookAttribute.Patch("OnNpcConversationEnded", "OnNpcConversationEnded", "NPCTalking", "Server_OnConversationEnded", ["BasePlayer"])]
        [HookAttribute.Identifier("366fa279f4f948be928d7e9b121df8ce")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "NPCTalking")]
        [MetadataAttribute.Category("NPC")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class NPC_NPCTalking_366fa279f4f948be928d7e9b121df8ce : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1853740711)).MoveLabelsFrom(instruction);
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

public partial class Category_NPC
{
    public partial class NPC_NPCTalking
    {
        [HookAttribute.Patch("OnNpcConversationStart", "OnNpcConversationStart", "NPCTalking", "Server_BeginTalking", ["BasePlayer"])]
        [HookAttribute.Identifier("09682043d5374f6d99c37ab3892f345e")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "NPCTalking")]
        [MetadataAttribute.Parameter("ply", "BasePlayer")]
        [MetadataAttribute.Parameter("local2", "ConversationData")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("NPC")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class NPC_NPCTalking_09682043d5374f6d99c37ab3892f345e : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 51)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3194214181)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True NPCTalking 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Ldloc_2  True ConversationData 
                    yield return new CodeInstruction(OpCodes.Ldloc_2);
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

public partial class Category_NPC
{
    public partial class NPC_BaseAIBrain
    {
        [HookAttribute.Patch("OnAIBrainStateSwitch", "OnAIBrainStateSwitch", "BaseAIBrain", "SwitchToState", ["BaseAIBrain/BasicAIState", "System.Int32"])]
        [HookAttribute.Identifier("977f33af6b5947f2a99b0820a41bc8d9")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "BaseAIBrain")]
        [MetadataAttribute.Parameter("self1", "BaseAIBrain")]
        [MetadataAttribute.Parameter("newState", "BaseAIBrain+BasicAIState")]
        [MetadataAttribute.Return(typeof(System.Boolean))]
        [MetadataAttribute.Category("NPC")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class NPC_BaseAIBrain_977f33af6b5947f2a99b0820a41bc8d9 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3661887738)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True BaseAIBrain 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldarg_0  True BaseAIBrain CurrentState
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // Set BaseAIBrain
                    // value:CurrentState isProperty:True runtimeType:BaseAIBrain+BasicAIState currentType:BaseAIBrain type:BaseAIBrain
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("BaseAIBrain"), "get_CurrentState"));
                    // Read BaseAIBrain : BaseAIBrain
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BaseAIBrain+BasicAIState 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
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

public partial class Category_NPC
{
    public partial class NPC_BaseAIBrain
    {
        [HookAttribute.Patch("OnAIBrainStateSwitched", "OnAIBrainStateSwitched", "BaseAIBrain", "SwitchToState", ["BaseAIBrain/BasicAIState", "System.Int32"])]
        [HookAttribute.Identifier("0025556076e44099996397bf97aeb769")]
        [HookAttribute.Dependencies(new System.String[] { "OnAIBrainStateSwitch" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "BaseAIBrain")]
        [MetadataAttribute.Parameter("self1", "BaseAIBrain")]
        [MetadataAttribute.Category("NPC")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class NPC_BaseAIBrain_0025556076e44099996397bf97aeb769 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3422229130)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True BaseAIBrain 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldarg_0  True BaseAIBrain CurrentState
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // Set BaseAIBrain
                    // value:CurrentState isProperty:True runtimeType:BaseAIBrain+BasicAIState currentType:BaseAIBrain type:BaseAIBrain
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("BaseAIBrain"), "get_CurrentState"));
                    // Read BaseAIBrain : BaseAIBrain
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

public partial class Category_NPC
{
    public partial class NPC_BradleyAPC
    {
        [HookAttribute.Patch("CanDeployScientists", "CanDeployScientists [BradleyAPC]", "BradleyAPC", "CanDeployScientists", ["BaseEntity", "System.Collections.Generic.List`1<GameObjectRef>", "System.Collections.Generic.List`1<UnityEngine.Vector3>"])]
        [HookAttribute.Identifier("e2b673fc6e38443380fd249a6c059b95")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "BradleyAPC")]
        [MetadataAttribute.Return(typeof(System.Boolean))]
        [MetadataAttribute.Category("NPC")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class NPC_BradleyAPC_e2b673fc6e38443380fd249a6c059b95 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1075290686)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldarg_1  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // AddYieldInstruction: Ldarg_2  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    // AddYieldInstruction: Ldarg_3  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_3);
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

public partial class Category_NPC
{
    public partial class NPC_BradleyAPC
    {
        [HookAttribute.Patch("OnScientistInitialized", "OnScientistInitialized [BradleyAPC]", "BradleyAPC", "InitScientist", ["ScientistNPC", "UnityEngine.Vector3", "BasePlayer", "System.Boolean", "System.Boolean"])]
        [HookAttribute.Identifier("88817091be1540dfa4a9d187dd8073ba")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "BradleyAPC")]
        [MetadataAttribute.Parameter("scientist", "ScientistNPC")]
        [MetadataAttribute.Parameter("spawnPos", "UnityEngine.Vector3")]
        [MetadataAttribute.Category("NPC")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class NPC_BradleyAPC_88817091be1540dfa4a9d187dd8073ba : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 123)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)321045627)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True BradleyAPC 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True ScientistNPC 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // HERE!
                    // AddYieldInstruction: Ldarg_2  True UnityEngine.Vector3 
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Box typeof(UnityEngine.Vector3) False  
                    yield return new CodeInstruction(OpCodes.Box, typeof(UnityEngine.Vector3));
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

public partial class Category_NPC
{
    public partial class NPC_BradleyAPC
    {
        [HookAttribute.Patch("OnScientistRecalled", "OnScientistRecalled [BradleyAPC]", "BradleyAPC", "OnScientistMounted", ["ScientistNPC"])]
        [HookAttribute.Identifier("51a0ccefa0494d879cdf1acf29b6a4b7")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "BradleyAPC")]
        [MetadataAttribute.Category("NPC")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class NPC_BradleyAPC_51a0ccefa0494d879cdf1acf29b6a4b7 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3620384264)).MoveLabelsFrom(instruction);
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

public partial class Category_NPC
{
    public partial class NPC_NPCPlayer
    {
        [HookAttribute.Patch("OnCorpsePopulate", "OnCorpsePopulate", "NPCPlayer", "CreateCorpse", ["BasePlayer/PlayerFlags", "UnityEngine.Vector3", "UnityEngine.Quaternion", "System.Collections.Generic.List`1<TriggerBase>", "System.Boolean"])]
        [HookAttribute.Identifier("ddecc2dba3aa4d7a9bb6dbb460a28035")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "NPCPlayer")]
        [MetadataAttribute.Parameter("local1", "NPCPlayerCorpse")]
        [MetadataAttribute.Return(typeof(BaseCorpse))]
        [MetadataAttribute.Category("NPC")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class NPC_NPCPlayer_ddecc2dba3aa4d7a9bb6dbb460a28035 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 114)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)338615359)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True NPCPlayer 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_1  True NPCPlayerCorpse 
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
                    // AddYieldInstruction: Isinst typeof(BaseCorpse) False  
                    yield return new CodeInstruction(OpCodes.Isinst, typeof(BaseCorpse));
                    // AddYieldInstruction: Ldnull  True  
                    yield return new CodeInstruction(OpCodes.Ldnull);
                    // AddYieldInstruction: Beq_S label1 False  
                    yield return new CodeInstruction(OpCodes.Beq_S, label1);
                    // AddYieldInstruction: Ldloc retvar False  
                    yield return new CodeInstruction(OpCodes.Ldloc, retvar);
                    // AddYieldInstruction: Unbox_Any typeof(BaseCorpse) False  
                    yield return new CodeInstruction(OpCodes.Unbox_Any, typeof(BaseCorpse));
                    // AddYieldInstruction: Ret  True  
                    yield return new CodeInstruction(OpCodes.Ret);
                    // return behaviour end

                    yield return instruction;
                }
            }
        }
    }
}

public partial class Category_NPC
{
    public partial class NPC_RustAiGen2SenseComponent
    {
        [HookAttribute.Patch("IOnNpcTarget", "IOnNpcTarget [SenseComponent]", "Rust.Ai.Gen2.SenseComponent", "CanTarget", ["BaseEntity"])]
        [HookAttribute.Identifier("c767fa9d27644b548c25bb6fc5d4af69")]
        [HookAttribute.Options(HookFlags.Static | HookFlags.Hidden)]
        [MetadataAttribute.Parameter("self", "Rust.Ai.Gen2.SenseComponent")]
        [MetadataAttribute.Return(typeof(System.Boolean))]
        [MetadataAttribute.Category("NPC")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class NPC_RustAiGen2SenseComponent_c767fa9d27644b548c25bb6fc5d4af69 : API.Hooks.Patch
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
                    // AddYieldInstruction: Ldarg_0  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_1  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // AddYieldInstruction: Call AccessTools.Method(typeof(Carbon.Core.CorePlugin), "IOnNpcTarget") False  
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Carbon.Core.CorePlugin), "IOnNpcTarget"));
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

public partial class Category_NPC
{
    public partial class NPC_RustAiGen2StateDead
    {
        [HookAttribute.Patch("OnCorpsePopulate", "OnCorpsePopulate [Rust.Ai.Gen2.State_Dead]", "Rust.Ai.Gen2.State_Dead", "StartRagdoll", [])]
        [HookAttribute.Identifier("fb537b2d21a442498796a581d1a3e669")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "Rust.Ai.Gen2.State_Dead")]
        [MetadataAttribute.Parameter("local1", "LootableCorpse")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("NPC")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class NPC_RustAiGen2StateDead_fb537b2d21a442498796a581d1a3e669 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 51)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)338615359)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True Rust.Ai.Gen2.State_Dead Owner
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // Set Rust.Ai.Gen2.State_Dead
                    // value:Owner isProperty:False runtimeType:BaseEntity currentType:Rust.Ai.Gen2.State_Dead type:Rust.Ai.Gen2.State_Dead
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("Rust.Ai.Gen2.State_Dead"), "Owner"));
                    // Read Rust.Ai.Gen2.State_Dead : Rust.Ai.Gen2.State_Dead
                    // AddYieldInstruction: Ldloc_1  True LootableCorpse 
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
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

public partial class Category_NPC
{
    public partial class NPC_AIBrainSenses
    {
        [HookAttribute.Patch("OnNpcTarget", "OnNpcTarget [AIBrainSenses]", "AIBrainSenses", "GetNearest", ["System.Collections.Generic.List`1<BaseEntity>", "System.Single"])]
        [HookAttribute.Identifier("6455f083d0484d608346bcf2e19b1808")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("NPC")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class NPC_AIBrainSenses_6455f083d0484d608346bcf2e19b1808 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                edit.Add(new CodeInstruction(OpCodes.Ldstr, "OnNpcTarget").MoveLabelsFrom(original[26]));
                // Opop
                edit.Add(new CodeInstruction(OpCodes.Ldarg_0));
                edit.Add(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("AIBrainSenses"), "owner")));
                edit.Add(new CodeInstruction(OpCodes.Ldloc_3));
                edit.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("Oxide.Core.Interface"), "CallHook", new System.Type[] { typeof(System.String), typeof(System.Object), typeof(System.Object) })));
                Label label_2de8012ee5234ac7a63f1af161f95e9b = Generator.DefineLabel();
                original[46].labels.Add(label_2de8012ee5234ac7a63f1af161f95e9b);
                edit.Add(new CodeInstruction(OpCodes.Brtrue_S, label_2de8012ee5234ac7a63f1af161f95e9b));

                original.InsertRange(26, edit);
                return original.AsEnumerable();
            }
        }
    }
}

public partial class Category_NPC
{
    public partial class NPC_HumanNPC
    {
        [HookAttribute.Patch("OnNpcTarget", "OnNpcTarget [HumanNPC]", "HumanNPC", "GetBestTarget", [])]
        [HookAttribute.Identifier("457e8c154b8b4f97963e4567478abbba")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("NPC")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class NPC_HumanNPC_457e8c154b8b4f97963e4567478abbba : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                edit.Add(new CodeInstruction(OpCodes.Ldstr, "OnNpcTarget").MoveLabelsFrom(original[22]));
                // Opop
                edit.Add(new CodeInstruction(OpCodes.Ldarg_0));
                edit.Add(new CodeInstruction(OpCodes.Ldloc_3));
                edit.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("Oxide.Core.Interface"), "CallHook", new System.Type[] { typeof(System.String), typeof(System.Object), typeof(System.Object) })));
                Label label_c52057d391d841818c1c8b87994b2b9d = Generator.DefineLabel();
                original[85].labels.Add(label_c52057d391d841818c1c8b87994b2b9d);
                edit.Add(new CodeInstruction(OpCodes.Brtrue_S, label_c52057d391d841818c1c8b87994b2b9d));

                original.InsertRange(22, edit);
                return original.AsEnumerable();
            }
        }
    }
}

public partial class Category_NPC
{
    public partial class NPC_RustAiGen2StateDead
    {
        [HookAttribute.Patch("OnCorpsePopulate", "OnCorpsePopulate [Rust.Ai.Gen2.State_Dead] [Patch]", "Rust.Ai.Gen2.State_Dead", "StartRagdoll", [])]
        [HookAttribute.Identifier("721be2c5e92444b68fed8f610ab5afb7")]
        [HookAttribute.Dependencies(new System.String[] { "OnCorpsePopulate [Rust.Ai.Gen2.State_Dead]" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("NPC")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class NPC_RustAiGen2StateDead_721be2c5e92444b68fed8f610ab5afb7 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                Label label_1b63d6c40a6e4a56872bdc2605f17539 = Generator.DefineLabel();
                original[121].labels.Add(label_1b63d6c40a6e4a56872bdc2605f17539);
                edit.Add(new CodeInstruction(OpCodes.Bne_Un_S, label_1b63d6c40a6e4a56872bdc2605f17539));

                original[57 + 2].MoveLabelsFrom(original[57]);
                original.RemoveRange(57, 2);
                original.InsertRange(57, edit);
                return original.AsEnumerable();
            }
        }
    }
}

