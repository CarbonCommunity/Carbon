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

public partial class Category_Turret
{
    public partial class Turret_AutoTurret
    {
        [HookAttribute.Patch("OnTurretTarget", "OnTurretTarget", "AutoTurret", "SetTarget", ["BaseCombatEntity"])]
        [HookAttribute.Identifier("c86f90473cfb44f68a47135568535d8b")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "AutoTurret")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Turret")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Turret_AutoTurret_c86f90473cfb44f68a47135568535d8b : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3691091596)).MoveLabelsFrom(instruction);
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

public partial class Category_Turret
{
    public partial class Turret_AutoTurret
    {
        [HookAttribute.Patch("OnTurretStartup", "OnTurretStartup", "AutoTurret", "InitiateStartup", [])]
        [HookAttribute.Identifier("449cb20139ae4958bbac9fef94c296e2")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "AutoTurret")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Turret")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Turret_AutoTurret_449cb20139ae4958bbac9fef94c296e2 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3717999484)).MoveLabelsFrom(instruction);
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

public partial class Category_Turret
{
    public partial class Turret_AutoTurret
    {
        [HookAttribute.Patch("OnTurretShutdown", "OnTurretShutdown", "AutoTurret", "InitiateShutdown", [])]
        [HookAttribute.Identifier("c82c8aea3eb948ca9bf387c1bae07838")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "AutoTurret")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Turret")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Turret_AutoTurret_c82c8aea3eb948ca9bf387c1bae07838 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1020909229)).MoveLabelsFrom(instruction);
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

public partial class Category_Turret
{
    public partial class Turret_AutoTurret
    {
        [HookAttribute.Patch("OnTurretToggle", "OnTurretToggle", "AutoTurret", "SetIsOnline", ["System.Boolean"])]
        [HookAttribute.Identifier("281805fc43b1458e86bf2a8a7eea74ae")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "AutoTurret")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Turret")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Turret_AutoTurret_281805fc43b1458e86bf2a8a7eea74ae : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)168477335)).MoveLabelsFrom(instruction);
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

public partial class Category_Turret
{
    public partial class Turret_AutoTurret
    {
        [HookAttribute.Patch("OnTurretDeauthorize", "OnTurretDeauthorize", "AutoTurret", "RemoveSelfAuthorize", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("abdd5bda39d14f93806bc7e66f48cde3")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "AutoTurret")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Turret")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Turret_AutoTurret_abdd5bda39d14f93806bc7e66f48cde3 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3383609399)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True AutoTurret 
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

public partial class Category_Turret
{
    public partial class Turret_AutoTurret
    {
        [HookAttribute.Patch("OnTurretClearList", "OnTurretClearList", "AutoTurret", "ClearList", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("e10c64aa2d6a4093a3396674a16d7e52")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "AutoTurret")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Turret")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Turret_AutoTurret_e10c64aa2d6a4093a3396674a16d7e52 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3764210145)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True AutoTurret 
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

public partial class Category_Turret
{
    public partial class Turret_AutoTurret
    {
        [HookAttribute.Patch("OnTurretModeToggle", "OnTurretModeToggle [Peacekeeper]", "AutoTurret", "SERVER_Peacekeeper", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("9711abf5363547d8abc24826be4bcba5")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "AutoTurret")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Turret")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Turret_AutoTurret_9711abf5363547d8abc24826be4bcba5 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 5)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)273264000)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True AutoTurret 
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

public partial class Category_Turret
{
    public partial class Turret_AutoTurret
    {
        [HookAttribute.Patch("OnTurretRotate", "OnTurretRotate", "AutoTurret", "FlipAim", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("68319632dd57436698a0ead25f793082")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "AutoTurret")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Turret")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Turret_AutoTurret_68319632dd57436698a0ead25f793082 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1562791322)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True AutoTurret 
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

public partial class Category_Turret
{
    public partial class Turret_AutoTurret
    {
        [HookAttribute.Patch("OnTurretAuthorize", "OnTurretAuthorize", "AutoTurret", "AddSelfAuthorize", ["BasePlayer"])]
        [HookAttribute.Identifier("4176f1b4d9de4ca2b501998c4b3294fc")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "AutoTurret")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Turret")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Turret_AutoTurret_4176f1b4d9de4ca2b501998c4b3294fc : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1791975019)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True AutoTurret 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
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

public partial class Category_Turret
{
    public partial class Turret_AutoTurret
    {
        [HookAttribute.Patch("OnTurretAssign", "OnTurretAssign", "AutoTurret", "AssignToFriend", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("904a067df5e944fba7186e8803488638")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "AutoTurret")]
        [MetadataAttribute.Parameter("local0", "System.UInt64")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Turret")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Turret_AutoTurret_904a067df5e944fba7186e8803488638 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 32)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)554719856)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True AutoTurret 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_0  True System.UInt64 
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    // WAAAAAAA 4.2
                    // AddYieldInstruction: Box Carbon.Extensions.AccessToolsEx.TypeByName("System.UInt64") False  
                    yield return new CodeInstruction(OpCodes.Box, Carbon.Extensions.AccessToolsEx.TypeByName("System.UInt64"));
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

public partial class Category_Turret
{
    public partial class Turret_AutoTurret
    {
        [HookAttribute.Patch("OnTurretAssigned", "OnTurretAssigned", "AutoTurret", "AssignToFriend", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("53507bf5f1104d708f3dff1588d999e6")]
        [HookAttribute.Dependencies(new System.String[] { "OnTurretAssign" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "AutoTurret")]
        [MetadataAttribute.Parameter("local0", "System.UInt64")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Category("Turret")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Turret_AutoTurret_53507bf5f1104d708f3dff1588d999e6 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 60)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)498675742)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True AutoTurret 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_0  True System.UInt64 
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    // WAAAAAAA 4.2
                    // AddYieldInstruction: Box Carbon.Extensions.AccessToolsEx.TypeByName("System.UInt64") False  
                    yield return new CodeInstruction(OpCodes.Box, Carbon.Extensions.AccessToolsEx.TypeByName("System.UInt64"));
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

public partial class Category_Turret
{
    public partial class Turret_AutoTurret
    {
        [HookAttribute.Patch("OnInterferenceUpdate", "OnInterferenceUpdate", "AutoTurret", "RecalculateInterference", [])]
        [HookAttribute.Identifier("bf024fe1fb8c4cff9e69caeff97b6afe")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "AutoTurret")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Turret")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Turret_AutoTurret_bf024fe1fb8c4cff9e69caeff97b6afe : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2075584415)).MoveLabelsFrom(instruction);
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

public partial class Category_Turret
{
    public partial class Turret_AutoTurret
    {
        [HookAttribute.Patch("OnTurretModeToggle", "OnTurretModeToggle [AttackAll]", "AutoTurret", "SERVER_AttackAll", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("e588f95ea92241bfb01cf0940707fa44")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "AutoTurret")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Turret")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Turret_AutoTurret_e588f95ea92241bfb01cf0940707fa44 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 5)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)273264000)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True AutoTurret 
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

public partial class Category_Turret
{
    public partial class Turret_AutoTurret
    {
        [HookAttribute.Patch("OnTurretIdentifierSet", "OnTurretIdentifierSet", "AutoTurret", "SetID", ["BasePlayer", "System.String", "System.String", "System.Boolean"])]
        [HookAttribute.Identifier("31e38246d0404358b7b38f9f026a4049")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "AutoTurret")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Parameter("newID", "System.String")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Turret")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Turret_AutoTurret_31e38246d0404358b7b38f9f026a4049 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)931035950)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True AutoTurret 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // HERE!
                    // AddYieldInstruction: Ldarg_3  True System.String 
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

public partial class Category_Turret
{
    public partial class Turret_AutoTurret
    {
        [HookAttribute.Patch("OnNearbyTurretsScan", "OnNearbyTurretsScan", "AutoTurret", "AddNearbyTurrets", [])]
        [HookAttribute.Identifier("d3de474dbef7450580443517d313a773")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Turret")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Turret_AutoTurret_d3de474dbef7450580443517d313a773 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                edit.Add(new CodeInstruction(OpCodes.Ldstr, "OnNearbyTurretsScan").MoveLabelsFrom(original[5]));
                // Opop
                edit.Add(new CodeInstruction(OpCodes.Ldarg_0));
                edit.Add(new CodeInstruction(OpCodes.Ldloc_0));
                edit.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("Oxide.Core.Interface"), "CallHook", new System.Type[] { typeof(System.String), typeof(System.Object), typeof(System.Object) })));
                Label label_ed131dfd62ea4728a216af30c7c7cf84 = Generator.DefineLabel();
                original[13].labels.Add(label_ed131dfd62ea4728a216af30c7c7cf84);
                edit.Add(new CodeInstruction(OpCodes.Brtrue_S, label_ed131dfd62ea4728a216af30c7c7cf84));

                original.InsertRange(5, edit);
                return original.AsEnumerable();
            }
        }
    }
}

