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

public partial class Category_Mission
{
    public partial class Mission_BaseMission
    {
        [HookAttribute.Patch("OnMissionFailed", "OnMissionFailed", "BaseMission", "MissionFailed", ["BaseMission/MissionInstance", "BasePlayer", "BaseMission/MissionFailReason", "System.Boolean"])]
        [HookAttribute.Identifier("f0a8cd6415ad426d9d84156b994a71c1")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "BaseMission")]
        [MetadataAttribute.Parameter("instance", "BaseMission+MissionInstance")]
        [MetadataAttribute.Parameter("assignee", "BasePlayer")]
        [MetadataAttribute.Parameter("failReason", "BaseMission+MissionFailReason")]
        [MetadataAttribute.Category("Mission")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Mission_BaseMission_f0a8cd6415ad426d9d84156b994a71c1 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 29)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)63503158)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True BaseMission 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BaseMission+MissionInstance 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // HERE!
                    // AddYieldInstruction: Ldarg_2  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    // WAAAAAAA 1
                    // HERE!
                    // AddYieldInstruction: Ldarg_3  True BaseMission+MissionFailReason 
                    yield return new CodeInstruction(OpCodes.Ldarg_3);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Box typeof(BaseMission.MissionFailReason) False  
                    yield return new CodeInstruction(OpCodes.Box, typeof(BaseMission.MissionFailReason));
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

public partial class Category_Mission
{
    public partial class Mission_BaseMission
    {
        [HookAttribute.Patch("OnMissionSucceeded", "OnMissionSucceeded", "BaseMission", "MissionSuccess", ["BaseMission/MissionInstance", "BasePlayer"])]
        [HookAttribute.Identifier("46276929c97e4eddba8c85ce18e23963")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "BaseMission")]
        [MetadataAttribute.Category("Mission")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Mission_BaseMission_46276929c97e4eddba8c85ce18e23963 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2044371482)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldarg_1  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // AddYieldInstruction: Ldarg_2  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    // AddYieldInstruction: Call AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object),typeof(object),typeof(object), }) False  
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

public partial class Category_Mission
{
    public partial class Mission_BaseMission
    {
        [HookAttribute.Patch("OnMissionStart", "OnMissionStart", "BaseMission", "MissionStart", ["BaseMission/MissionInstance", "BasePlayer"])]
        [HookAttribute.Identifier("e05027f217b64aeab0841336e9e5d7d7")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "BaseMission")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Mission")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Mission_BaseMission_e05027f217b64aeab0841336e9e5d7d7 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)711033722)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldarg_1  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // AddYieldInstruction: Ldarg_2  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    // AddYieldInstruction: Call AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object),typeof(object),typeof(object), }) False  
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

public partial class Category_Mission
{
    public partial class Mission_BaseMission
    {
        [HookAttribute.Patch("CanAssignMission", "CanAssignMission", "BaseMission", "AssignMission", ["BasePlayer", "IMissionProvider", "BaseMission"])]
        [HookAttribute.Identifier("4da9c4ebe4d746c186d06a319274eb51")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("assignee", "BasePlayer")]
        [MetadataAttribute.Parameter("mission", "BaseMission")]
        [MetadataAttribute.Parameter("provider", "IMissionProvider")]
        [MetadataAttribute.Return(typeof(System.Boolean))]
        [MetadataAttribute.Category("Mission")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Mission_BaseMission_4da9c4ebe4d746c186d06a319274eb51 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1070103224)).MoveLabelsFrom(instruction);
                    // HERE!
                    // AddYieldInstruction: Ldarg_0  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // WAAAAAAA 1
                    // HERE!
                    // AddYieldInstruction: Ldarg_2  True BaseMission 
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    // WAAAAAAA 1
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True IMissionProvider 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
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

public partial class Category_Mission
{
    public partial class Mission_BaseMission
    {
        [HookAttribute.Patch("OnMissionAssigned", "OnMissionAssigned", "BaseMission", "AssignMission", ["BasePlayer", "IMissionProvider", "BaseMission"])]
        [HookAttribute.Identifier("a8a1807dc070408b86d73f16b44c7435")]
        [HookAttribute.Dependencies(new System.String[] { "CanAssignMission" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("mission", "BaseMission")]
        [MetadataAttribute.Parameter("provider", "IMissionProvider")]
        [MetadataAttribute.Parameter("assignee", "BasePlayer")]
        [MetadataAttribute.Category("Mission")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Mission_BaseMission_a8a1807dc070408b86d73f16b44c7435 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 200)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2072569864)).MoveLabelsFrom(instruction);
                    // HERE!
                    // AddYieldInstruction: Ldarg_2  True BaseMission 
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    // WAAAAAAA 1
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True IMissionProvider 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // HERE!
                    // AddYieldInstruction: Ldarg_0  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
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

public partial class Category_Mission
{
    public partial class Mission_BaseMission
    {
        [HookAttribute.Patch("OnMissionStarted", "OnMissionStarted", "BaseMission", "MissionStart", ["BaseMission/MissionInstance", "BasePlayer"])]
        [HookAttribute.Identifier("603ea8c2a4044d7cbdedeeaa72a283ee")]
        [HookAttribute.Dependencies(new System.String[] { "OnMissionStart" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "BaseMission")]
        [MetadataAttribute.Category("Mission")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Mission_BaseMission_603ea8c2a4044d7cbdedeeaa72a283ee : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 67)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)616743836)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldarg_1  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // AddYieldInstruction: Ldarg_2  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    // AddYieldInstruction: Call AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object),typeof(object),typeof(object), }) False  
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

