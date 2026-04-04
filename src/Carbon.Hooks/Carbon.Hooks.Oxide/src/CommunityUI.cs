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

public partial class Category_CommunityUI
{
    public partial class CommunityUI_CommunityEntity
    {
        [HookAttribute.Patch("OnCuiDraggableDrag", "OnCuiDraggableDrag", "CommunityEntity", "Hook_DragRPC", ["BasePlayer", "System.String", "UnityEngine.Vector3", "CommunityEntity/DraggablePositionSendType"])]
        [HookAttribute.Identifier("5c748baaf55c489daf51121998232fe8")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Category("CommunityUI")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class CommunityUI_CommunityEntity_5c748baaf55c489daf51121998232fe8 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1614693435)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_1  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // AddYieldInstruction: Ldarg_2  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    // AddYieldInstruction: Ldarg_3  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_3);
                    // WAAAAAAA 3
                    // AddYieldInstruction: Box typeof(UnityEngine.Vector3) False  
                    yield return new CodeInstruction(OpCodes.Box, typeof(UnityEngine.Vector3));
                    // AddYieldInstruction: Ldarg_S 4 False  
                    yield return new CodeInstruction(OpCodes.Ldarg_S, 4);
                    // WAAAAAAA 3
                    // AddYieldInstruction: Box typeof(CommunityEntity.DraggablePositionSendType) False  
                    yield return new CodeInstruction(OpCodes.Box, typeof(CommunityEntity.DraggablePositionSendType));
                    // AddYieldInstruction: Call AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object),typeof(object),typeof(object),typeof(object), }) False  
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

public partial class Category_CommunityUI
{
    public partial class CommunityUI_CommunityEntity
    {
        [HookAttribute.Patch("OnCuiDraggableDrop", "OnCuiDraggableDrop", "CommunityEntity", "Hook_DropRPC", ["BasePlayer", "System.String", "System.String", "System.String", "System.String"])]
        [HookAttribute.Identifier("0ce646e3b9844187999f275a54a7d4e1")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Category("CommunityUI")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class CommunityUI_CommunityEntity_0ce646e3b9844187999f275a54a7d4e1 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2642769942)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_1  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // AddYieldInstruction: Ldarg_2  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    // AddYieldInstruction: Ldarg_3  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_3);
                    // AddYieldInstruction: Ldarg_S 4 False  
                    yield return new CodeInstruction(OpCodes.Ldarg_S, 4);
                    // AddYieldInstruction: Ldarg_S 5 False  
                    yield return new CodeInstruction(OpCodes.Ldarg_S, 5);
                    // AddYieldInstruction: Call AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object),typeof(object),typeof(object),typeof(object),typeof(object), }) False  
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), }));
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

