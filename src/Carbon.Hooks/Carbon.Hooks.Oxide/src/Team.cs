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

public partial class Category_Team
{
    public partial class Team_RelationshipManager
    {
        [HookAttribute.Patch("OnTeamCreate", "OnTeamCreate", "RelationshipManager", "TryCreateTeam", ["BasePlayer"])]
        [HookAttribute.Identifier("c03489971ab24f08afaf491fbe101e82")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Team")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Team_RelationshipManager_c03489971ab24f08afaf491fbe101e82 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 4)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2721988387)).MoveLabelsFrom(instruction);
                    // HERE!
                    // AddYieldInstruction: Ldarg_0  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // WAAAAAAA 1
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

public partial class Category_Team
{
    public partial class Team_RelationshipManager
    {
        [HookAttribute.Patch("IOnTeamInvite", "IOnTeamInvite", "RelationshipManager", "sendinvite", ["ConsoleSystem/Arg"])]
        [HookAttribute.Identifier("b1c81c7fb80b45179bb55279d6856778")]
        [HookAttribute.Options(HookFlags.Static | HookFlags.Hidden)]
        [MetadataAttribute.Parameter("local0", "BasePlayer")]
        [MetadataAttribute.Parameter("local3", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Team")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Team_RelationshipManager_b1c81c7fb80b45179bb55279d6856778 : API.Hooks.Patch
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
                    // AddYieldInstruction: Ldloc_0  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldloc_0).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_3  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldloc_3);
                    // AddYieldInstruction: Call AccessTools.Method(typeof(Carbon.Core.CorePlugin), "IOnTeamInvite") False  
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Carbon.Core.CorePlugin), "IOnTeamInvite"));
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

public partial class Category_Team
{
    public partial class Team_RelationshipManager
    {
        [HookAttribute.Patch("OnTeamRejectInvite", "OnTeamRejectInvite", "RelationshipManager", "rejectinvite", ["ConsoleSystem/Arg"])]
        [HookAttribute.Identifier("fd0fc3f18f244850a30931cfc318045a")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("local0", "BasePlayer")]
        [MetadataAttribute.Parameter("local2", "RelationshipManager+PlayerTeam")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Team")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Team_RelationshipManager_fd0fc3f18f244850a30931cfc318045a : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1844985686)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_0  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    // AddYieldInstruction: Ldloc_2  True RelationshipManager+PlayerTeam 
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

public partial class Category_Team
{
    public partial class Team_RelationshipManager
    {
        [HookAttribute.Patch("OnTeamLeave", "OnTeamLeave", "RelationshipManager", "leaveteam", ["ConsoleSystem/Arg"])]
        [HookAttribute.Identifier("915d1cc88d00407c9a0bb91cae21a92b")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("local1", "RelationshipManager+PlayerTeam")]
        [MetadataAttribute.Parameter("local0", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Team")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Team_RelationshipManager_915d1cc88d00407c9a0bb91cae21a92b : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2913633678)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_1  True RelationshipManager+PlayerTeam 
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
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

public partial class Category_Team
{
    public partial class Team_RelationshipManager
    {
        [HookAttribute.Patch("OnTeamKick", "OnTeamKick", "RelationshipManager", "kickmember", ["ConsoleSystem/Arg"])]
        [HookAttribute.Identifier("27090ce84daa45b396863e99de0f337b")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("local1", "RelationshipManager+PlayerTeam")]
        [MetadataAttribute.Parameter("local0", "BasePlayer")]
        [MetadataAttribute.Parameter("local2", "System.UInt64")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Team")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Team_RelationshipManager_27090ce84daa45b396863e99de0f337b : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3265417256)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_1  True RelationshipManager+PlayerTeam 
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
                    // AddYieldInstruction: Ldloc_0  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    // AddYieldInstruction: Ldloc_2  True System.UInt64 
                    yield return new CodeInstruction(OpCodes.Ldloc_2);
                    // WAAAAAAA 4.2
                    // AddYieldInstruction: Box Carbon.Extensions.AccessToolsEx.TypeByName("System.UInt64") False  
                    yield return new CodeInstruction(OpCodes.Box, Carbon.Extensions.AccessToolsEx.TypeByName("System.UInt64"));
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

public partial class Category_Team
{
    public partial class Team_RelationshipManager
    {
        [HookAttribute.Patch("OnTeamAcceptInvite", "OnTeamAcceptInvite", "RelationshipManager", "acceptinvite", ["ConsoleSystem/Arg"])]
        [HookAttribute.Identifier("88c64c0a99354197abf696df1b5052bf")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("local2", "RelationshipManager+PlayerTeam")]
        [MetadataAttribute.Parameter("local0", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Team")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Team_RelationshipManager_88c64c0a99354197abf696df1b5052bf : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2723952333)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_2  True RelationshipManager+PlayerTeam 
                    yield return new CodeInstruction(OpCodes.Ldloc_2);
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

public partial class Category_Team
{
    public partial class Team_RelationshipManager
    {
        [HookAttribute.Patch("OnTeamDisband", "OnTeamDisband", "RelationshipManager", "DisbandTeam", ["RelationshipManager/PlayerTeam"])]
        [HookAttribute.Identifier("920325a546004d56ac8754f482283715")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Team")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Team_RelationshipManager_920325a546004d56ac8754f482283715 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2528539314)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_1  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
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

public partial class Category_Team
{
    public partial class Team_RelationshipManager
    {
        [HookAttribute.Patch("OnTeamCreated", "OnTeamCreated", "RelationshipManager", "TryCreateTeam", ["BasePlayer"])]
        [HookAttribute.Identifier("d816eff01d4e4c77a772e894fe11a7c2")]
        [HookAttribute.Dependencies(new System.String[] { "OnTeamCreate" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Parameter("local0", "RelationshipManager+PlayerTeam")]
        [MetadataAttribute.Category("Team")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Team_RelationshipManager_d816eff01d4e4c77a772e894fe11a7c2 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 35)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)4221161380)).MoveLabelsFrom(instruction);
                    // HERE!
                    // AddYieldInstruction: Ldarg_0  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Ldloc_0  True RelationshipManager+PlayerTeam 
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

public partial class Category_Team
{
    public partial class Team_RelationshipManager
    {
        [HookAttribute.Patch("OnTeamDisbanded", "OnTeamDisbanded", "RelationshipManager", "DisbandTeam", ["RelationshipManager/PlayerTeam"])]
        [HookAttribute.Identifier("cb565902b42544b89db6619b03da64c7")]
        [HookAttribute.Dependencies(new System.String[] { "OnTeamDisband" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Category("Team")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Team_RelationshipManager_cb565902b42544b89db6619b03da64c7 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)197406368)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_1  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
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

public partial class Category_Team
{
    public partial class Team_BasePlayer
    {
        [HookAttribute.Patch("OnTeamUpdate", "OnTeamUpdate", "BasePlayer", "UpdateTeam", ["System.UInt64"])]
        [HookAttribute.Identifier("c3d1450efb0848979adac59dbac60642")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "BasePlayer")]
        [MetadataAttribute.Parameter("newTeam", "System.UInt64")]
        [MetadataAttribute.Parameter("self1", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Team")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Team_BasePlayer_c3d1450efb0848979adac59dbac60642 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1613227947)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True BasePlayer currentTeam
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // Set BasePlayer
                    // value:currentTeam isProperty:False runtimeType:System.UInt64 currentType:BasePlayer type:BasePlayer
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("BasePlayer"), "currentTeam"));
                    yield return new CodeInstruction(OpCodes.Box, Carbon.Extensions.AccessToolsEx.TypeByName("System.UInt64"));
                    // Read BasePlayer : BasePlayer
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True System.UInt64 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Box typeof(System.UInt64) False  
                    yield return new CodeInstruction(OpCodes.Box, typeof(System.UInt64));
                    // AddYieldInstruction: Ldarg_0  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
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

public partial class Category_Team
{
    public partial class Team_BasePlayer
    {
        [HookAttribute.Patch("OnTeamUpdated", "OnTeamUpdated", "BasePlayer", "TeamUpdate", ["System.Boolean"])]
        [HookAttribute.Identifier("7349c7ecdb36438986e85905dc2e305d")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "BasePlayer")]
        [MetadataAttribute.Parameter("local3", "ProtoBuf.PlayerTeam")]
        [MetadataAttribute.Parameter("self1", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Team")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Team_BasePlayer_7349c7ecdb36438986e85905dc2e305d : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 275)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1173491625)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True BasePlayer currentTeam
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // Set BasePlayer
                    // value:currentTeam isProperty:False runtimeType:System.UInt64 currentType:BasePlayer type:BasePlayer
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("BasePlayer"), "currentTeam"));
                    yield return new CodeInstruction(OpCodes.Box, Carbon.Extensions.AccessToolsEx.TypeByName("System.UInt64"));
                    // Read BasePlayer : BasePlayer
                    // AddYieldInstruction: Ldloc_3  True ProtoBuf.PlayerTeam 
                    yield return new CodeInstruction(OpCodes.Ldloc_3);
                    // AddYieldInstruction: Ldarg_0  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
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

public partial class Category_Team
{
    public partial class Team_RelationshipManagerPlayerTeam
    {
        [HookAttribute.Patch("OnTeamMemberPromote", "OnTeamMemberPromote", "RelationshipManager/PlayerTeam", "SetTeamLeader", ["System.UInt64"])]
        [HookAttribute.Identifier("258a67f6dc3347b4a72831123a510474")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "RelationshipManager+PlayerTeam")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Team")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Team_RelationshipManagerPlayerTeam_258a67f6dc3347b4a72831123a510474 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1658239813)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldarg_1  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 3
                    // AddYieldInstruction: Box typeof(System.UInt64) False  
                    yield return new CodeInstruction(OpCodes.Box, typeof(System.UInt64));
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

public partial class Category_Team
{
    public partial class Team_RelationshipManager
    {
        [HookAttribute.Patch("OnTeamMemberInvite", "OnTeamMemberInvite [sendofflineinvite]", "RelationshipManager", "sendofflineinvite", ["ConsoleSystem/Arg"])]
        [HookAttribute.Identifier("59550ef95fe144b5a09cd51bc7f03593")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("local1", "RelationshipManager+PlayerTeam")]
        [MetadataAttribute.Parameter("local0", "BasePlayer")]
        [MetadataAttribute.Parameter("local2", "System.UInt64")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Team")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Team_RelationshipManager_59550ef95fe144b5a09cd51bc7f03593 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)844539354)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_1  True RelationshipManager+PlayerTeam 
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
                    // AddYieldInstruction: Ldloc_0  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    // AddYieldInstruction: Ldloc_2  True System.UInt64 
                    yield return new CodeInstruction(OpCodes.Ldloc_2);
                    // WAAAAAAA 4.2
                    // AddYieldInstruction: Box Carbon.Extensions.AccessToolsEx.TypeByName("System.UInt64") False  
                    yield return new CodeInstruction(OpCodes.Box, Carbon.Extensions.AccessToolsEx.TypeByName("System.UInt64"));
                    // AddYieldInstruction: Ldnull  True  
                    yield return new CodeInstruction(OpCodes.Ldnull);
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

public partial class Category_Team
{
    public partial class Team_RelationshipManager
    {
        [HookAttribute.Patch("OnTeamMemberInvite", "OnTeamMemberInvite [sendinvite]", "RelationshipManager", "sendinvite", ["ConsoleSystem/Arg"])]
        [HookAttribute.Identifier("a1b0d2ccaeb5474b971985a14de30ef3")]
        [HookAttribute.Dependencies(new System.String[] { "IOnTeamInvite" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Team")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Team_RelationshipManager_a1b0d2ccaeb5474b971985a14de30ef3 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                edit.Add(new CodeInstruction(OpCodes.Ldstr, "OnTeamMemberInvite").MoveLabelsFrom(original[70]));
                edit.Add(new CodeInstruction(OpCodes.Ldloc_1));
                edit.Add(new CodeInstruction(OpCodes.Ldloc_0));
                edit.Add(new CodeInstruction(OpCodes.Ldloc_3));
                edit.Add(new CodeInstruction(OpCodes.Ldflda, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("BasePlayer"), "userID")));
                edit.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("EncryptedValue`1[System.UInt64]"), "Get")));
                edit.Add(new CodeInstruction(OpCodes.Box, typeof(System.UInt64)));
                edit.Add(new CodeInstruction(OpCodes.Ldc_I4_0));
                edit.Add(new CodeInstruction(OpCodes.Box, typeof(System.Boolean)));
                edit.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("Oxide.Core.Interface"), "CallHook", new System.Type[] { typeof(System.String), typeof(System.Object), typeof(System.Object), typeof(System.Object), typeof(System.Object) })));
                Label label_4c27be16015e439fb3da4ba823c2a047 = Generator.DefineLabel();
                original[70].labels.Add(label_4c27be16015e439fb3da4ba823c2a047);
                edit.Add(new CodeInstruction(OpCodes.Brfalse_S, label_4c27be16015e439fb3da4ba823c2a047));
                edit.Add(new CodeInstruction(OpCodes.Ret));

                original.InsertRange(70, edit);
                return original.AsEnumerable();
            }
        }
    }
}

