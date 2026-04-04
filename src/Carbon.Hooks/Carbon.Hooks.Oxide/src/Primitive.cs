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

public partial class Category_Primitive
{
    public partial class Primitive_BallistaGun
    {
        [HookAttribute.Patch("OnBallistaGunReload", "OnBallistaGunReload", "BallistaGun", "SERVER_ReloadStart", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("d58fb908654a4678a5bfdf5491af3a33")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "BallistaGun")]
        [MetadataAttribute.Parameter("local0", "BasePlayer")]
        [MetadataAttribute.Return(typeof(void))]
        [MetadataAttribute.Category("Primitive")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Primitive_BallistaGun_d58fb908654a4678a5bfdf5491af3a33 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2994302818)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True BallistaGun 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_0  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
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
                    // AddYieldInstruction: Isinst typeof(object) False  
                    yield return new CodeInstruction(OpCodes.Isinst, typeof(object));
                    // AddYieldInstruction: Ldnull  True  
                    yield return new CodeInstruction(OpCodes.Ldnull);
                    // AddYieldInstruction: Beq_S label1 False  
                    yield return new CodeInstruction(OpCodes.Beq_S, label1);
                    // AddYieldInstruction: Ldloc retvar False  
                    yield return new CodeInstruction(OpCodes.Ldloc, retvar);
                    // AddYieldInstruction: Unbox_Any typeof(object) False  
                    yield return new CodeInstruction(OpCodes.Unbox_Any, typeof(object));
                    // AddYieldInstruction: Ret  True  
                    yield return new CodeInstruction(OpCodes.Ret);
                    // return behaviour end

                    yield return instruction;
                }
            }
        }
    }
}

public partial class Category_Primitive
{
    public partial class Primitive_Catapult
    {
        [HookAttribute.Patch("OnCatapultFireForce", "OnCatapultFireForce", "Catapult", "Fire", ["BasePlayer", "System.Single"])]
        [HookAttribute.Identifier("897c10fa5053478185969f49edf722da")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "Catapult")]
        [MetadataAttribute.Parameter("shooter", "BasePlayer")]
        [MetadataAttribute.Parameter("local1", "System.Single")]
        [MetadataAttribute.Category("Primitive")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Primitive_Catapult_897c10fa5053478185969f49edf722da : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2897059420)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True Catapult 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Ldloc_1  True System.Single 
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
                    // WAAAAAAA 4.2
                    // AddYieldInstruction: Box Carbon.Extensions.AccessToolsEx.TypeByName("System.Single") False  
                    yield return new CodeInstruction(OpCodes.Box, Carbon.Extensions.AccessToolsEx.TypeByName("System.Single"));
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint), typeof(object), typeof(object), typeof(object), }));
                    // hook call end

                    // return behaviour start
                    Label label1 = Generator.DefineLabel();
                    instruction.labels.Add(label1);
                    object retvar = Generator.DeclareLocal(typeof(object));
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
                    // AddYieldInstruction: Stloc_1  True  
                    yield return new CodeInstruction(OpCodes.Stloc_1);

                    yield return instruction;
                }
            }
        }
    }
}

public partial class Category_Primitive
{
    public partial class Primitive_Catapult
    {
        [HookAttribute.Patch("OnSiegeWeaponFire", "OnSiegeWeaponFire [Catapult]", "Catapult", "SERVER_WantsFire", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("e3994fd6dc5c407c953d2120c1f6eadc")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "Catapult")]
        [MetadataAttribute.Parameter("local0", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Primitive")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Primitive_Catapult_e3994fd6dc5c407c953d2120c1f6eadc : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3849505322)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True Catapult 
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

public partial class Category_Primitive
{
    public partial class Primitive_BatteringRam
    {
        [HookAttribute.Patch("OnSiegeWeaponFire", "OnSiegeWeaponFire [BatteringRam]", "BatteringRam", "SERVER_WantsAttack", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("c628650510904eec92190f75a8032fdc")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "BatteringRam")]
        [MetadataAttribute.Parameter("local1", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Primitive")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Primitive_BatteringRam_c628650510904eec92190f75a8032fdc : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3849505322)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True BatteringRam 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_1  True BasePlayer 
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

public partial class Category_Primitive
{
    public partial class Primitive_BatteringRam
    {
        [HookAttribute.Patch("OnSiegeWeaponDoorOpen", "OnSiegeWeaponDoorOpen [BatteringRam]", "BatteringRam", "RPC_OpenDoor", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("82b7b5f880354c309e32a65a10a87436")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "BatteringRam")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Primitive")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Primitive_BatteringRam_82b7b5f880354c309e32a65a10a87436 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 10)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1497098116)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True BatteringRam 
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

public partial class Category_Primitive
{
    public partial class Primitive_BatteringRam
    {
        [HookAttribute.Patch("OnSiegeWeaponDoorClose", "OnSiegeWeaponDoorClose [BatteringRam]", "BatteringRam", "RPC_CloseDoor", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("45fff12777f84d96bd54774cc5f32450")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "BatteringRam")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Primitive")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Primitive_BatteringRam_45fff12777f84d96bd54774cc5f32450 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 10)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2664272771)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True BatteringRam 
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

public partial class Category_Primitive
{
    public partial class Primitive_BaseSiegeWeapon
    {
        [HookAttribute.Patch("OnSiegeWeaponPull", "OnSiegeWeaponPull", "BaseSiegeWeapon", "SERVER_StartPulling", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("33652556fdfb442e8293f0022ab440b6")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "BaseSiegeWeapon")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Primitive")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Primitive_BaseSiegeWeapon_33652556fdfb442e8293f0022ab440b6 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2446236079)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True BaseSiegeWeapon 
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

