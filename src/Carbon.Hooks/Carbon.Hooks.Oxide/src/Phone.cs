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

public partial class Category_Phone
{
    public partial class Phone_PhoneController
    {
        [HookAttribute.Patch("OnPhoneAnswer", "OnPhoneAnswer", "PhoneController", "AnswerPhone", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("c27e241b3f2b4f21a3d1152d338f1186")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "PhoneController")]
        [MetadataAttribute.Parameter("self1", "PhoneController")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Phone")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Phone_PhoneController_c27e241b3f2b4f21a3d1152d338f1186 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 20)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1818430455)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True PhoneController 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldarg_0  True PhoneController activeCallTo
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // Set PhoneController
                    // value:activeCallTo isProperty:False runtimeType:PhoneController currentType:PhoneController type:PhoneController
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("PhoneController"), "activeCallTo"));
                    // Read PhoneController : PhoneController
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

public partial class Category_Phone
{
    public partial class Phone_PhoneController
    {
        [HookAttribute.Patch("OnPhoneCallStart", "OnPhoneCallStart", "PhoneController", "BeginCall", [])]
        [HookAttribute.Identifier("6eea8bca571f4102984d6adc9319fa80")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "PhoneController")]
        [MetadataAttribute.Parameter("self1", "PhoneController")]
        [MetadataAttribute.Parameter("self2", "PhoneController")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Phone")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Phone_PhoneController_6eea8bca571f4102984d6adc9319fa80 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3536074469)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True PhoneController 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldarg_0  True PhoneController activeCallTo
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // Set PhoneController
                    // value:activeCallTo isProperty:False runtimeType:PhoneController currentType:PhoneController type:PhoneController
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("PhoneController"), "activeCallTo"));
                    // Read PhoneController : PhoneController
                    // AddYieldInstruction: Ldarg_0  True PhoneController currentPlayer
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // Set PhoneController
                    // value:currentPlayer isProperty:True runtimeType:BasePlayer currentType:PhoneController type:PhoneController
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("PhoneController"), "get_currentPlayer"));
                    // Read PhoneController : PhoneController
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

public partial class Category_Phone
{
    public partial class Phone_PhoneController
    {
        [HookAttribute.Patch("OnPhoneCallStarted", "OnPhoneCallStarted", "PhoneController", "BeginCall", [])]
        [HookAttribute.Identifier("af14d9e6584e43a8a258f313f7d805ae")]
        [HookAttribute.Dependencies(new System.String[] { "OnPhoneCallStart" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "PhoneController")]
        [MetadataAttribute.Parameter("self1", "PhoneController")]
        [MetadataAttribute.Parameter("self2", "PhoneController")]
        [MetadataAttribute.Category("Phone")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Phone_PhoneController_af14d9e6584e43a8a258f313f7d805ae : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 37)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2840999274)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True PhoneController 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldarg_0  True PhoneController activeCallTo
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // Set PhoneController
                    // value:activeCallTo isProperty:False runtimeType:PhoneController currentType:PhoneController type:PhoneController
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("PhoneController"), "activeCallTo"));
                    // Read PhoneController : PhoneController
                    // AddYieldInstruction: Ldarg_0  True PhoneController currentPlayer
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // Set PhoneController
                    // value:currentPlayer isProperty:True runtimeType:BasePlayer currentType:PhoneController type:PhoneController
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("PhoneController"), "get_currentPlayer"));
                    // Read PhoneController : PhoneController
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

public partial class Category_Phone
{
    public partial class Phone_PhoneController
    {
        [HookAttribute.Patch("CanReceiveCall", "CanReceiveCall", "PhoneController", "CanReceiveCall", [])]
        [HookAttribute.Identifier("6ee30630fcf04f338e74752ca064489a")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "PhoneController")]
        [MetadataAttribute.Return(typeof(System.Boolean))]
        [MetadataAttribute.Category("Phone")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Phone_PhoneController_6ee30630fcf04f338e74752ca064489a : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1680123496)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True PhoneController 
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

public partial class Category_Phone
{
    public partial class Phone_PhoneController
    {
        [HookAttribute.Patch("OnPhoneDial", "OnPhoneDial", "PhoneController", "CallPhone", ["System.Int32"])]
        [HookAttribute.Identifier("1a40aec72b234edfa32ebefa8bdb0a95")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "PhoneController")]
        [MetadataAttribute.Parameter("local0", "PhoneController")]
        [MetadataAttribute.Parameter("self1", "PhoneController")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Phone")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Phone_PhoneController_1a40aec72b234edfa32ebefa8bdb0a95 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1241230080)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True PhoneController 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_0  True PhoneController 
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    // AddYieldInstruction: Ldarg_0  True PhoneController currentPlayer
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // Set PhoneController
                    // value:currentPlayer isProperty:True runtimeType:BasePlayer currentType:PhoneController type:PhoneController
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("PhoneController"), "get_currentPlayer"));
                    // Read PhoneController : PhoneController
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

public partial class Category_Phone
{
    public partial class Phone_PhoneController
    {
        [HookAttribute.Patch("OnPhoneDialFail", "OnPhoneDialFail", "PhoneController", "OnDialFailed", ["Telephone/DialFailReason"])]
        [HookAttribute.Identifier("68f38776a821447798bcb8108ed27921")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "PhoneController")]
        [MetadataAttribute.Parameter("reason", "Telephone+DialFailReason")]
        [MetadataAttribute.Parameter("self1", "PhoneController")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Phone")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Phone_PhoneController_68f38776a821447798bcb8108ed27921 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2555568850)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True PhoneController 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True Telephone+DialFailReason 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Box typeof(Telephone.DialFailReason) False  
                    yield return new CodeInstruction(OpCodes.Box, typeof(Telephone.DialFailReason));
                    // AddYieldInstruction: Ldarg_0  True PhoneController currentPlayer
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // Set PhoneController
                    // value:currentPlayer isProperty:True runtimeType:BasePlayer currentType:PhoneController type:PhoneController
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("PhoneController"), "get_currentPlayer"));
                    // Read PhoneController : PhoneController
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

public partial class Category_Phone
{
    public partial class Phone_PhoneController
    {
        [HookAttribute.Patch("OnPhoneDialTimeout", "OnPhoneDialTimeout", "PhoneController", "TimeOutDialing", [])]
        [HookAttribute.Identifier("ae2d4cd445964cb795d4e0a102c45827")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "PhoneController")]
        [MetadataAttribute.Parameter("self1", "PhoneController")]
        [MetadataAttribute.Parameter("self2", "PhoneController")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Phone")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Phone_PhoneController_ae2d4cd445964cb795d4e0a102c45827 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)772238272)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True PhoneController activeCallTo
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // Set PhoneController
                    // value:activeCallTo isProperty:False runtimeType:PhoneController currentType:PhoneController type:PhoneController
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("PhoneController"), "activeCallTo"));
                    // Read PhoneController : PhoneController
                    // AddYieldInstruction: Ldarg_0  True PhoneController 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldarg_0  True PhoneController activeCallTo, currentPlayer
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // Set PhoneController
                    // value:activeCallTo isProperty:False runtimeType:PhoneController currentType:PhoneController type:PhoneController
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("PhoneController"), "activeCallTo"));
                    // Set PhoneController
                    // value:currentPlayer isProperty:True runtimeType:BasePlayer currentType:PhoneController type:PhoneController
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("PhoneController"), "get_currentPlayer"));
                    // Read PhoneController : PhoneController
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

public partial class Category_Phone
{
    public partial class Phone_PhoneController
    {
        [HookAttribute.Patch("OnPhoneDialFailed", "OnPhoneDialFailed", "PhoneController", "OnDialFailed", ["Telephone/DialFailReason"])]
        [HookAttribute.Identifier("f46157e5300f4ad599c6d88d4f6b2c18")]
        [HookAttribute.Dependencies(new System.String[] { "OnPhoneDialFail" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "PhoneController")]
        [MetadataAttribute.Parameter("reason", "Telephone+DialFailReason")]
        [MetadataAttribute.Parameter("self1", "PhoneController")]
        [MetadataAttribute.Category("Phone")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Phone_PhoneController_f46157e5300f4ad599c6d88d4f6b2c18 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 55)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2908630240)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True PhoneController 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True Telephone+DialFailReason 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Box typeof(Telephone.DialFailReason) False  
                    yield return new CodeInstruction(OpCodes.Box, typeof(Telephone.DialFailReason));
                    // AddYieldInstruction: Ldarg_0  True PhoneController currentPlayer
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // Set PhoneController
                    // value:currentPlayer isProperty:True runtimeType:BasePlayer currentType:PhoneController type:PhoneController
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("PhoneController"), "get_currentPlayer"));
                    // Read PhoneController : PhoneController
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

public partial class Category_Phone
{
    public partial class Phone_PhoneController
    {
        [HookAttribute.Patch("OnPhoneDialTimedOut", "OnPhoneDialTimedOut", "PhoneController", "TimeOutDialing", [])]
        [HookAttribute.Identifier("f1203aead2894305ad29b1ec94f2519b")]
        [HookAttribute.Dependencies(new System.String[] { "OnPhoneDialTimeout" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "PhoneController")]
        [MetadataAttribute.Parameter("self1", "PhoneController")]
        [MetadataAttribute.Parameter("self2", "PhoneController")]
        [MetadataAttribute.Category("Phone")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Phone_PhoneController_f1203aead2894305ad29b1ec94f2519b : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)31481961)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True PhoneController activeCallTo
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // Set PhoneController
                    // value:activeCallTo isProperty:False runtimeType:PhoneController currentType:PhoneController type:PhoneController
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("PhoneController"), "activeCallTo"));
                    // Read PhoneController : PhoneController
                    // AddYieldInstruction: Ldarg_0  True PhoneController 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldarg_0  True PhoneController activeCallTo, currentPlayer
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // Set PhoneController
                    // value:activeCallTo isProperty:False runtimeType:PhoneController currentType:PhoneController type:PhoneController
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("PhoneController"), "activeCallTo"));
                    // Set PhoneController
                    // value:currentPlayer isProperty:True runtimeType:BasePlayer currentType:PhoneController type:PhoneController
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("PhoneController"), "get_currentPlayer"));
                    // Read PhoneController : PhoneController
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

public partial class Category_Phone
{
    public partial class Phone_PhoneController
    {
        [HookAttribute.Patch("OnPhoneAnswered", "OnPhoneAnswered", "PhoneController", "AnswerPhone", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("a189f0ca8422452084048c5d77e8d9a1")]
        [HookAttribute.Dependencies(new System.String[] { "OnPhoneAnswer" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "PhoneController")]
        [MetadataAttribute.Parameter("self1", "PhoneController")]
        [MetadataAttribute.Category("Phone")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Phone_PhoneController_a189f0ca8422452084048c5d77e8d9a1 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)459265237)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True PhoneController 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldarg_0  True PhoneController activeCallTo
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // Set PhoneController
                    // value:activeCallTo isProperty:False runtimeType:PhoneController currentType:PhoneController type:PhoneController
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("PhoneController"), "activeCallTo"));
                    // Read PhoneController : PhoneController
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

