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

public partial class Category_Animal
{
    public partial class Animal_RidableHorse
    {
        [HookAttribute.Patch("OnHorseLead", "OnHorseLead [RidableHorse]", "RidableHorse", "SERVER_Lead", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("6915edc1f5314b349b8ea31429def43c")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "RidableHorse")]
        [MetadataAttribute.Parameter("local0", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Animal")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Animal_RidableHorse_6915edc1f5314b349b8ea31429def43c : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2434657963)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True RidableHorse 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_0  True BasePlayer 
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

public partial class Category_Animal
{
    public partial class Animal_HitchTrough
    {
        [HookAttribute.Patch("OnHorseHitch", "OnHorseHitch", "HitchTrough", "AttemptToHitch", ["HitchTrough/IHitchable", "HitchTrough/HitchSpot"])]
        [HookAttribute.Identifier("5f6c0f05c223402586b879f70cdc7b34")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Return(typeof(System.Boolean))]
        [MetadataAttribute.Category("Animal")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Animal_HitchTrough_5f6c0f05c223402586b879f70cdc7b34 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3062212715)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_1  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // AddYieldInstruction: Ldarg_2  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
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

public partial class Category_Animal
{
    public partial class Animal_HitchTrough
    {
        [HookAttribute.Patch("OnHorseUnhitch", "OnHorseUnhitch", "HitchTrough", "UnHitch", ["HitchTrough/IHitchable"])]
        [HookAttribute.Identifier("ae24db6df5374a83951bdb70b5a9bf79")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("hitchable", "HitchTrough+IHitchable")]
        [MetadataAttribute.Parameter("local2", "HitchTrough+HitchSpot")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Animal")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Animal_HitchTrough_ae24db6df5374a83951bdb70b5a9bf79 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1764412199)).MoveLabelsFrom(instruction);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True HitchTrough+IHitchable 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Ldloc_2  True HitchTrough+HitchSpot 
                    yield return new CodeInstruction(OpCodes.Ldloc_2);
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

public partial class Category_Animal
{
    public partial class Animal_RidableHorse
    {
        [HookAttribute.Patch("OnRidableAnimalClaim", "OnRidableAnimalClaim [RidableHorse]", "RidableHorse", "SERVER_Claim", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("0901ddc88089453495773fe0250c5d99")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "RidableHorse")]
        [MetadataAttribute.Parameter("local0", "BasePlayer")]
        [MetadataAttribute.Parameter("local2", "Item")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Animal")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Animal_RidableHorse_0901ddc88089453495773fe0250c5d99 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2611596445)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True RidableHorse 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_0  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    // AddYieldInstruction: Ldloc_2  True Item 
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

public partial class Category_Animal
{
    public partial class Animal_RidableHorse
    {
        [HookAttribute.Patch("OnRidableAnimalClaimed", "OnRidableAnimalClaimed [RidableHorse]", "RidableHorse", "SERVER_Claim", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("7f7494ec5d1d4ce1b9f25214696cb1ef")]
        [HookAttribute.Dependencies(new System.String[] { "OnRidableAnimalClaim [RidableHorse]" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "RidableHorse")]
        [MetadataAttribute.Parameter("local0", "BasePlayer")]
        [MetadataAttribute.Category("Animal")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Animal_RidableHorse_7f7494ec5d1d4ce1b9f25214696cb1ef : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 56)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)617527508)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True RidableHorse 
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

public partial class Category_Animal
{
    public partial class Animal_RidableHorse
    {
        [HookAttribute.Patch("OnAnimalDungProduce", "OnAnimalDungProduce [RidableHorse]", "RidableHorse", "DoDung", [])]
        [HookAttribute.Identifier("364ef332826b4857a257ce8f88ff95f4")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "RidableHorse")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Animal")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Animal_RidableHorse_364ef332826b4857a257ce8f88ff95f4 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)73579294)).MoveLabelsFrom(instruction);
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

public partial class Category_Animal
{
    public partial class Animal_RidableHorse
    {
        [HookAttribute.Patch("OnAnimalDungProduced", "OnAnimalDungProduced [RidableHorse]", "RidableHorse", "DoDung", [])]
        [HookAttribute.Identifier("4227770ed20f4a7a8e8dc517e079d67b")]
        [HookAttribute.Dependencies(new System.String[] { "OnAnimalDungProduced [RidableHorse] [Variable]" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "RidableHorse")]
        [MetadataAttribute.Category("Animal")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Animal_RidableHorse_4227770ed20f4a7a8e8dc517e079d67b : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)878484420)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True RidableHorse 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    /* out of bounds 2/2 */
                    // DEBUG CENTRAL: 2
                    // AddYieldInstruction: Ldloc_2  True  
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

public partial class Category_Animal
{
    public partial class Animal_RidableHorse
    {
        [HookAttribute.Patch("OnAnimalDungProduced", "OnAnimalDungProduced [RidableHorse] [Variable]", "RidableHorse", "DoDung", [])]
        [HookAttribute.Identifier("c098425bc6f145808ccd193e517c47db")]
        [HookAttribute.Dependencies(new System.String[] { "OnAnimalDungProduce [RidableHorse]" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Animal")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Animal_RidableHorse_c098425bc6f145808ccd193e517c47db : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                edit.Add(new CodeInstruction(OpCodes.Stloc_2));
                edit.Add(new CodeInstruction(OpCodes.Ldloc_2));

                original.InsertRange(41, edit);
                return original.AsEnumerable();
            }
        }
    }
}

