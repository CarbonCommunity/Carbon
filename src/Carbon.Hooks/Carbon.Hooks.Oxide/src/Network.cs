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

public partial class Category_Network
{
    public partial class Network_BaseNetworkable
    {
        [HookAttribute.Patch("CanNetworkTo", "CanNetworkTo", "BaseNetworkable", "ShouldNetworkTo", ["BasePlayer"])]
        [HookAttribute.Identifier("4655dfcfbb3e414bae7bea7fa62c692e")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "BaseNetworkable")]
        [MetadataAttribute.Return(typeof(System.Boolean))]
        [MetadataAttribute.Category("Network")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Network_BaseNetworkable_4655dfcfbb3e414bae7bea7fa62c692e : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1622751857)).MoveLabelsFrom(instruction);
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

public partial class Category_Network
{
    public partial class Network_BaseNetworkable
    {
        [HookAttribute.Patch("OnNetworkGroupEntered", "OnNetworkGroupEntered", "BaseNetworkable", "OnNetworkGroupEnter", ["Network.Visibility.Group"])]
        [HookAttribute.Identifier("9328aa2ba74b4adbb9067f6291f11db6")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "BaseNetworkable")]
        [MetadataAttribute.Category("Network")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Network_BaseNetworkable_9328aa2ba74b4adbb9067f6291f11db6 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2439782111)).MoveLabelsFrom(instruction);
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

public partial class Category_Network
{
    public partial class Network_BaseNetworkable
    {
        [HookAttribute.Patch("OnNetworkGroupLeft", "OnNetworkGroupLeft", "BaseNetworkable", "OnNetworkGroupLeave", ["Network.Visibility.Group"])]
        [HookAttribute.Identifier("32cee69c5cb24454a3ede8cbc0f03055")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "BaseNetworkable")]
        [MetadataAttribute.Category("Network")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Network_BaseNetworkable_32cee69c5cb24454a3ede8cbc0f03055 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)226983605)).MoveLabelsFrom(instruction);
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

public partial class Category_Network
{
    public partial class Network_NetworkVisibilityGrid
    {
        [HookAttribute.Patch("OnNetworkSubscriptionsGather", "OnNetworkSubscriptionsGather", "NetworkVisibilityGrid", "GetVisibleFrom", ["Network.Visibility.Group", "ListHashSet`1<Network.Visibility.Group>", "System.Int32"])]
        [HookAttribute.Identifier("bac10237be1c49689c51352dc7c7d79f")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "NetworkVisibilityGrid")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Network")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Network_NetworkVisibilityGrid_bac10237be1c49689c51352dc7c7d79f : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)26233677)).MoveLabelsFrom(instruction);
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
                    // AddYieldInstruction: Call AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object),typeof(object),typeof(object),typeof(object), }) False  
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

public partial class Category_Network
{
    public partial class Network_NetworkNetworkable
    {
        [HookAttribute.Patch("OnNetworkSubscriptionsUpdate", "OnNetworkSubscriptionsUpdate", "Network.Networkable", "UpdateSubscriptions", ["System.Int32", "System.Int32"])]
        [HookAttribute.Identifier("0813c560c1b243e4b5a46a7bdf9acf99")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Category("Network")]
        [MetadataAttribute.Assembly("Facepunch.Network.dll")]
        public class Network_NetworkNetworkable_0813c560c1b243e4b5a46a7bdf9acf99 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                edit.Add(new CodeInstruction(OpCodes.Ldstr, "OnNetworkSubscriptionsUpdate").MoveLabelsFrom(original[36]));
                // Opop
                edit.Add(new CodeInstruction(OpCodes.Ldarg_0));
                edit.Add(new CodeInstruction(OpCodes.Ldloc_1));
                edit.Add(new CodeInstruction(OpCodes.Ldloc_2));
                edit.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("Oxide.Core.Interface"), "CallHook", new System.Type[] { typeof(System.String), typeof(System.Object), typeof(System.Object), typeof(System.Object) })));
                Label label_acb0dee1421e4c188c524a6f187ac566 = Generator.DefineLabel();
                original[122].labels.Add(label_acb0dee1421e4c188c524a6f187ac566);
                edit.Add(new CodeInstruction(OpCodes.Brtrue_S, label_acb0dee1421e4c188c524a6f187ac566));

                original.InsertRange(36, edit);
                return original.AsEnumerable();
            }
        }
    }
}

public partial class Category_Network
{
    public partial class Network_NetworkNetworkable
    {
        [HookAttribute.Patch("OnNetworkSubscriptionsUpdate", "OnNetworkSubscriptionsUpdate [2]", "Network.Networkable", "UpdateHighPrioritySubscriptions", [])]
        [HookAttribute.Identifier("67a28f3a9aef4092a23934f89c29ec83")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Category("Network")]
        [MetadataAttribute.Assembly("Facepunch.Network.dll")]
        public class Network_NetworkNetworkable_67a28f3a9aef4092a23934f89c29ec83 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                edit.Add(new CodeInstruction(OpCodes.Ldstr, "OnNetworkSubscriptionsUpdate").MoveLabelsFrom(original[26]));
                // Opop
                edit.Add(new CodeInstruction(OpCodes.Ldarg_0));
                edit.Add(new CodeInstruction(OpCodes.Ldloc_1));
                edit.Add(new CodeInstruction(OpCodes.Ldnull));
                edit.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("Oxide.Core.Interface"), "CallHook", new System.Type[] { typeof(System.String), typeof(System.Object), typeof(System.Object), typeof(System.Object) })));
                Label label_7a6ac676a6c84a13ac0bb8b06e77ffd0 = Generator.DefineLabel();
                original[60].labels.Add(label_7a6ac676a6c84a13ac0bb8b06e77ffd0);
                edit.Add(new CodeInstruction(OpCodes.Brtrue_S, label_7a6ac676a6c84a13ac0bb8b06e77ffd0));

                original.InsertRange(26, edit);
                return original.AsEnumerable();
            }
        }
    }
}

