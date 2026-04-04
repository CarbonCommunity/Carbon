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

public partial class Category_Bookmark
{
    public partial class Bookmark_ComputerStation
    {
        [HookAttribute.Patch("OnBookmarkControl", "OnBookmarkControl", "ComputerStation", "BeginControllingBookmark", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("f1263b6ba2bb43a0845b01282d78d8e2")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "ComputerStation")]
        [MetadataAttribute.Parameter("local0", "BasePlayer")]
        [MetadataAttribute.Parameter("local1", "System.String")]
        [MetadataAttribute.Parameter("local2", "IRemoteControllable")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Bookmark")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Bookmark_ComputerStation_f1263b6ba2bb43a0845b01282d78d8e2 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2564888972)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True ComputerStation 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_0  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    // AddYieldInstruction: Ldloc_1  True System.String 
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
                    // AddYieldInstruction: Ldloc_2  True IRemoteControllable 
                    yield return new CodeInstruction(OpCodes.Ldloc_2);
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

public partial class Category_Bookmark
{
    public partial class Bookmark_ComputerStation
    {
        [HookAttribute.Patch("OnBookmarkAdd", "OnBookmarkAdd", "ComputerStation", "AddBookmark", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("3ad025a08e224f589bfd2da3bcee85f8")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "ComputerStation")]
        [MetadataAttribute.Parameter("local0", "BasePlayer")]
        [MetadataAttribute.Parameter("local1", "System.String")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Bookmark")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Bookmark_ComputerStation_3ad025a08e224f589bfd2da3bcee85f8 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1206891691)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True ComputerStation 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_0  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    // AddYieldInstruction: Ldloc_1  True System.String 
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
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

public partial class Category_Bookmark
{
    public partial class Bookmark_ComputerStation
    {
        [HookAttribute.Patch("OnBookmarksSendControl", "OnBookmarksSendControl", "ComputerStation", "SendControlBookmarks", ["BasePlayer"])]
        [HookAttribute.Identifier("71e949f041274c23be0f4e36e4f27ca9")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "ComputerStation")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Parameter("local0", "System.String")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Bookmark")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Bookmark_ComputerStation_71e949f041274c23be0f4e36e4f27ca9 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2021417980)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True ComputerStation 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Ldloc_0  True System.String 
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

public partial class Category_Bookmark
{
    public partial class Bookmark_ComputerStation
    {
        [HookAttribute.Patch("OnBookmarkControlEnd", "OnBookmarkControlEnd", "ComputerStation", "StopControl", ["BasePlayer"])]
        [HookAttribute.Identifier("bb74875508714411af431092d8c18aec")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "ComputerStation")]
        [MetadataAttribute.Parameter("ply", "BasePlayer")]
        [MetadataAttribute.Parameter("local0", "BaseEntity")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Bookmark")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Bookmark_ComputerStation_bb74875508714411af431092d8c18aec : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3610365652)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True ComputerStation 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Ldloc_0  True BaseEntity 
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

public partial class Category_Bookmark
{
    public partial class Bookmark_ComputerStation
    {
        [HookAttribute.Patch("OnBookmarkInput", "OnBookmarkInput", "ComputerStation", "PlayerServerInput", ["InputState", "BasePlayer"])]
        [HookAttribute.Identifier("ef369c571dea46f79b1cc7b2741993bd")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "ComputerStation")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Parameter("inputState", "InputState")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Bookmark")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Bookmark_ComputerStation_ef369c571dea46f79b1cc7b2741993bd : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3231378518)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True ComputerStation 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_2  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    // WAAAAAAA 1
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True InputState 
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

public partial class Category_Bookmark
{
    public partial class Bookmark_ComputerStation
    {
        [HookAttribute.Patch("OnBookmarkControlStarted", "OnBookmarkControlStarted", "ComputerStation", "BeginControllingBookmark", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("41a0a13165f2456a80d16994ba91456c")]
        [HookAttribute.Dependencies(new System.String[] { "OnBookmarkControl" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "ComputerStation")]
        [MetadataAttribute.Parameter("local0", "BasePlayer")]
        [MetadataAttribute.Parameter("local1", "System.String")]
        [MetadataAttribute.Parameter("local2", "IRemoteControllable")]
        [MetadataAttribute.Category("Bookmark")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Bookmark_ComputerStation_41a0a13165f2456a80d16994ba91456c : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2318406615)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True ComputerStation 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_0  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    // AddYieldInstruction: Ldloc_1  True System.String 
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
                    // AddYieldInstruction: Ldloc_2  True IRemoteControllable 
                    yield return new CodeInstruction(OpCodes.Ldloc_2);
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

public partial class Category_Bookmark
{
    public partial class Bookmark_ComputerStation
    {
        [HookAttribute.Patch("OnBookmarkControlEnded", "OnBookmarkControlEnded", "ComputerStation", "StopControl", ["BasePlayer"])]
        [HookAttribute.Identifier("704e497917894f75be10edf10645b0b1")]
        [HookAttribute.Dependencies(new System.String[] { "OnBookmarkControlEnd" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "ComputerStation")]
        [MetadataAttribute.Parameter("ply", "BasePlayer")]
        [MetadataAttribute.Parameter("local0", "BaseEntity")]
        [MetadataAttribute.Category("Bookmark")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Bookmark_ComputerStation_704e497917894f75be10edf10645b0b1 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 64)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)764919103)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True ComputerStation 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Ldloc_0  True BaseEntity 
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

public partial class Category_Bookmark
{
    public partial class Bookmark_ComputerStation
    {
        [HookAttribute.Patch("OnBookmarkControlEnded", "OnBookmarkControlEnded [2]", "ComputerStation", "BeginControllingBookmark", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("c71dbe6666024935b1e84c8278b2efa3")]
        [HookAttribute.Dependencies(new System.String[] { "OnBookmarkControlStarted" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "ComputerStation")]
        [MetadataAttribute.Parameter("local0", "BasePlayer")]
        [MetadataAttribute.Parameter("local6", "IRemoteControllable")]
        [MetadataAttribute.Category("Bookmark")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Bookmark_ComputerStation_c71dbe6666024935b1e84c8278b2efa3 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 90)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)764919103)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True ComputerStation 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_0  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
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

public partial class Category_Bookmark
{
    public partial class Bookmark_ComputerStation
    {
        [HookAttribute.Patch("OnBookmarkDelete", "OnBookmarkDelete", "ComputerStation", "RemoveBookmark", ["System.String", "BasePlayer"])]
        [HookAttribute.Identifier("24ce9156a68f4c98932afea1d6661d60")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "ComputerStation")]
        [MetadataAttribute.Parameter("mountedPlayer", "BasePlayer")]
        [MetadataAttribute.Parameter("identifier", "System.String")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Bookmark")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Bookmark_ComputerStation_24ce9156a68f4c98932afea1d6661d60 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)373279383)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True ComputerStation 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_2  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    // WAAAAAAA 1
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True System.String 
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

public partial class Category_Bookmark
{
    public partial class Bookmark_ComputerStation
    {
        [HookAttribute.Patch("OnBookmarkControlEnded [2] [patch]", "OnBookmarkControlEnded [2] [patch]", "ComputerStation", "BeginControllingBookmark", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("1b075c4115b742bb8b1f6f226686de0c")]
        [HookAttribute.Dependencies(new System.String[] { "OnBookmarkControlEnded [2]" })]
        [HookAttribute.Options(HookFlags.Patch)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Bookmark")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Bookmark_ComputerStation_1b075c4115b742bb8b1f6f226686de0c : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                Label label_c3f38ef0192a4cb191c4eec93e852518 = Generator.DefineLabel();
                original[96].labels.Add(label_c3f38ef0192a4cb191c4eec93e852518);
                edit.Add(new CodeInstruction(OpCodes.Brfalse_S, label_c3f38ef0192a4cb191c4eec93e852518));

                original[77 + 1].MoveLabelsFrom(original[77]);
                original.RemoveRange(77, 1);
                original.InsertRange(77, edit);
                return original.AsEnumerable();
            }
        }
    }
}

