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

public partial class Category_Electronic
{
    public partial class Electronic_IOEntity
    {
        [HookAttribute.Patch("OnOutputUpdate", "OnOutputUpdate", "IOEntity", "UpdateOutputs", [])]
        [HookAttribute.Identifier("3e5cf459024341f0b3f46e32e927ad76")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "IOEntity")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Electronic")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Electronic_IOEntity_3e5cf459024341f0b3f46e32e927ad76 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3549948601)).MoveLabelsFrom(instruction);
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

public partial class Category_Electronic
{
    public partial class Electronic_IOEntity
    {
        [HookAttribute.Patch("OnInputUpdate", "OnInputUpdate", "IOEntity", "UpdateFromInput", ["System.Int32", "System.Int32"])]
        [HookAttribute.Identifier("9b1e4a35254643dd9d58531b22b9fa75")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "IOEntity")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Electronic")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Electronic_IOEntity_9b1e4a35254643dd9d58531b22b9fa75 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1169047808)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldarg_1  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 3
                    // AddYieldInstruction: Box typeof(System.Int32) False  
                    yield return new CodeInstruction(OpCodes.Box, typeof(System.Int32));
                    // AddYieldInstruction: Ldarg_2  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    // WAAAAAAA 3
                    // AddYieldInstruction: Box typeof(System.Int32) False  
                    yield return new CodeInstruction(OpCodes.Box, typeof(System.Int32));
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

public partial class Category_Electronic
{
    public partial class Electronic_CardReader
    {
        [HookAttribute.Patch("OnCardSwipe", "OnCardSwipe", "CardReader", "ServerCardSwiped", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("fcea11ebc4fd44d5bff5b8d3b59f469d")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "CardReader")]
        [MetadataAttribute.Parameter("local1", "Keycard")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Electronic")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Electronic_CardReader_fcea11ebc4fd44d5bff5b8d3b59f469d : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3533854112)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True CardReader 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_1  True Keycard 
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
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

public partial class Category_Electronic
{
    public partial class Electronic_DigitalClock
    {
        [HookAttribute.Patch("OnDigitalClockRing", "OnDigitalClockRing", "DigitalClock", "Ring", [])]
        [HookAttribute.Identifier("3cdca06e281244e29e90c4b7f054dd63")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "DigitalClock")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Electronic")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Electronic_DigitalClock_3cdca06e281244e29e90c4b7f054dd63 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)762478235)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True DigitalClock 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
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

public partial class Category_Electronic
{
    public partial class Electronic_DigitalClock
    {
        [HookAttribute.Patch("OnDigitalClockRingStop", "OnDigitalClockRingStop", "DigitalClock", "StopRinging", [])]
        [HookAttribute.Identifier("b44c2863f8c34823af3608102d2c75b6")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "DigitalClock")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Electronic")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Electronic_DigitalClock_b44c2863f8c34823af3608102d2c75b6 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1462957189)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True DigitalClock 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
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

public partial class Category_Electronic
{
    public partial class Electronic_DigitalClock
    {
        [HookAttribute.Patch("OnDigitalClockAlarmsSet", "OnDigitalClockAlarmsSet", "DigitalClock", "RPC_SetAlarms", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("39371aa4997c448e92ff970f404161ea")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "DigitalClock")]
        [MetadataAttribute.Parameter("local0", "ProtoBuf.DigitalClockMessage")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Electronic")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Electronic_DigitalClock_39371aa4997c448e92ff970f404161ea : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1813865950)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True DigitalClock 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_0  True ProtoBuf.DigitalClockMessage 
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

public partial class Category_Electronic
{
    public partial class Electronic_PressButton
    {
        [HookAttribute.Patch("OnButtonPress", "OnButtonPress", "PressButton", "RPC_Press", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("20b743a778364166aad8544496e24fda")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "PressButton")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Electronic")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Electronic_PressButton_20b743a778364166aad8544496e24fda : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2616103367)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True PressButton 
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

public partial class Category_Electronic
{
    public partial class Electronic_PhoneController
    {
        [HookAttribute.Patch("OnPhoneNameUpdate", "OnPhoneNameUpdate", "PhoneController", "UpdatePhoneName", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("f31864420e4e476184cd67d27c868a11")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "PhoneController")]
        [MetadataAttribute.Parameter("local0", "System.String")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Electronic")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Electronic_PhoneController_f31864420e4e476184cd67d27c868a11 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 22)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3039361701)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True PhoneController 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_0  True System.String 
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
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

public partial class Category_Electronic
{
    public partial class Electronic_SolarPanel
    {
        [HookAttribute.Patch("OnSolarPanelSunUpdate", "OnSolarPanelSunUpdate", "SolarPanel", "SunUpdate", [])]
        [HookAttribute.Identifier("682ea8a48dfb4fd3a13a85fac6037bf7")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "SolarPanel")]
        [MetadataAttribute.Parameter("local0", "System.Int32")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Electronic")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Electronic_SolarPanel_682ea8a48dfb4fd3a13a85fac6037bf7 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 58)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1805755454)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True SolarPanel 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldloc_0  True System.Int32 
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    // WAAAAAAA 4.2
                    // AddYieldInstruction: Box Carbon.Extensions.AccessToolsEx.TypeByName("System.Int32") False  
                    yield return new CodeInstruction(OpCodes.Box, Carbon.Extensions.AccessToolsEx.TypeByName("System.Int32"));
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

public partial class Category_Electronic
{
    public partial class Electronic_AutoTurret
    {
        [HookAttribute.Patch("OnEntityControl", "OnEntityControl [AutoTurret]", "AutoTurret", "CanControl", ["System.UInt64"])]
        [HookAttribute.Identifier("91a6878cd4344cce9d188c26e3a00446")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "AutoTurret")]
        [MetadataAttribute.Return(typeof(System.Boolean))]
        [MetadataAttribute.Category("Electronic")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Electronic_AutoTurret_91a6878cd4344cce9d188c26e3a00446 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1276273354)).MoveLabelsFrom(instruction);
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

public partial class Category_Electronic
{
    public partial class Electronic_PoweredRemoteControlEntity
    {
        [HookAttribute.Patch("OnEntityControl", "OnEntityControl [PoweredRemoteControl]", "PoweredRemoteControlEntity", "CanControl", ["System.UInt64"])]
        [HookAttribute.Identifier("be6d5c9dabc24b2c9437ed63dbf082b8")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "PoweredRemoteControlEntity")]
        [MetadataAttribute.Return(typeof(System.Boolean))]
        [MetadataAttribute.Category("Electronic")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Electronic_PoweredRemoteControlEntity_be6d5c9dabc24b2c9437ed63dbf082b8 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1276273354)).MoveLabelsFrom(instruction);
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

public partial class Category_Electronic
{
    public partial class Electronic_RemoteControlEntity
    {
        [HookAttribute.Patch("OnEntityControl", "OnEntityControl [RemoteControlEntity]", "RemoteControlEntity", "CanControl", ["System.UInt64"])]
        [HookAttribute.Identifier("21af5321b44a43339f32d7d7372ea358")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "RemoteControlEntity")]
        [MetadataAttribute.Return(typeof(System.Boolean))]
        [MetadataAttribute.Category("Electronic")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Electronic_RemoteControlEntity_21af5321b44a43339f32d7d7372ea358 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1276273354)).MoveLabelsFrom(instruction);
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

public partial class Category_Electronic
{
    public partial class Electronic_PhoneController
    {
        [HookAttribute.Patch("OnPhoneNameUpdated", "OnPhoneNameUpdated", "PhoneController", "UpdatePhoneName", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("df15f1c50e964ae3b797a0978ccf931c")]
        [HookAttribute.Dependencies(new System.String[] { "OnPhoneNameUpdate" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "PhoneController")]
        [MetadataAttribute.Parameter("self1", "PhoneController")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Category("Electronic")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Electronic_PhoneController_df15f1c50e964ae3b797a0978ccf931c : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 38)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)587926495)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True PhoneController 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldarg_0  True PhoneController PhoneName
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // Set PhoneController
                    // value:PhoneName isProperty:False runtimeType:System.String currentType:PhoneController type:PhoneController
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("PhoneController"), "PhoneName"));
                    // Read PhoneController : PhoneController
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

public partial class Category_Electronic
{
    public partial class Electronic_IOEntityIORef
    {
        [HookAttribute.Patch("OnIORefCleared", "OnIORefCleared", "IOEntity/IORef", "Clear", [])]
        [HookAttribute.Identifier("5779e90417474b519623a283d16926c8")]
        [HookAttribute.Dependencies(new System.String[] { "OnIORefCleared [patch]" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "IOEntity+IORef")]
        [MetadataAttribute.Category("Electronic")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Electronic_IOEntityIORef_5779e90417474b519623a283d16926c8 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)92897215)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True IOEntity+IORef 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    /* out of bounds 0/0 */
                    // DEBUG CENTRAL: 0
                    // AddYieldInstruction: Ldloc_0  True  
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

public partial class Category_Electronic
{
    public partial class Electronic_CCTVRC
    {
        [HookAttribute.Patch("OnCCTVDirectionChange", "OnCCTVDirectionChange", "CCTV_RC", "Server_SetDir", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("e2ce022220c44b708da40b915d9a484c")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "CCTV_RC")]
        [MetadataAttribute.Parameter("local0", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Electronic")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Electronic_CCTVRC_e2ce022220c44b708da40b915d9a484c : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1209570562)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True CCTV_RC 
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

public partial class Category_Electronic
{
    public partial class Electronic_ExcavatorSignalComputer
    {
        [HookAttribute.Patch("OnExcavatorSuppliesRequest", "OnExcavatorSuppliesRequest", "ExcavatorSignalComputer", "RequestSupplies", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("0bfa6db6bc504f92b557e82ac95079e8")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "ExcavatorSignalComputer")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Electronic")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Electronic_ExcavatorSignalComputer_0bfa6db6bc504f92b557e82ac95079e8 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 15)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)134449885)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True ExcavatorSignalComputer 
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

public partial class Category_Electronic
{
    public partial class Electronic_ExcavatorSignalComputer
    {
        [HookAttribute.Patch("OnExcavatorSuppliesRequested", "OnExcavatorSuppliesRequested", "ExcavatorSignalComputer", "RequestSupplies", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("794f0b7d3a7b470ab952460ed1a49270")]
        [HookAttribute.Dependencies(new System.String[] { "OnExcavatorSuppliesRequest" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("self", "ExcavatorSignalComputer")]
        [MetadataAttribute.Parameter("player", "BasePlayer")]
        [MetadataAttribute.Parameter("local0", "BaseEntity")]
        [MetadataAttribute.Category("Electronic")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Electronic_ExcavatorSignalComputer_794f0b7d3a7b470ab952460ed1a49270 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 69)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)49012084)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True ExcavatorSignalComputer 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True BaseEntity+RPCMessage player
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // Set BaseEntity+RPCMessage
                    // value:player isProperty:False runtimeType:BasePlayer currentType:BaseEntity+RPCMessage type:BaseEntity+RPCMessage
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("BaseEntity+RPCMessage"), "player"));
                    // Read BaseEntity+RPCMessage : BaseEntity+RPCMessage
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

public partial class Category_Electronic
{
    public partial class Electronic_PowerCounter
    {
        [HookAttribute.Patch("OnCounterTargetChange", "OnCounterTargetChange", "PowerCounter", "SERVER_SetTarget", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("e0c62a20795c45be9ccf0fa71fdbaeb5")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Electronic")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Electronic_PowerCounter_e0c62a20795c45be9ccf0fa71fdbaeb5 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                edit.Add(new CodeInstruction(OpCodes.Ldarg_1));
                edit.Add(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("BaseEntity/RPCMessage"), "read")));
                edit.Add(new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("Network.NetRead"), "Int32")));
                LocalBuilder var_cf599fed60de4bd593e172a024236041 = Generator.DeclareLocal(typeof(System.Int32));
                edit.Add(new CodeInstruction(OpCodes.Stloc_0));
                edit.Add(new CodeInstruction(OpCodes.Ldstr, "OnCounterTargetChange"));
                // Opop
                edit.Add(new CodeInstruction(OpCodes.Ldarg_0));
                edit.Add(new CodeInstruction(OpCodes.Ldarg_1));
                edit.Add(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("BaseEntity/RPCMessage"), "player")));
                edit.Add(new CodeInstruction(OpCodes.Ldloc_0));
                edit.Add(new CodeInstruction(OpCodes.Box, typeof(System.Int32)));
                edit.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("Oxide.Core.Interface"), "CallHook", new System.Type[] { typeof(System.String), typeof(System.Object), typeof(System.Object), typeof(System.Object) })));
                Label label_1360b1e520a04a2c8c76f8e1e0d95879 = Generator.DefineLabel();
                original[0].labels.Add(label_1360b1e520a04a2c8c76f8e1e0d95879);
                edit.Add(new CodeInstruction(OpCodes.Brfalse_S, label_1360b1e520a04a2c8c76f8e1e0d95879));
                edit.Add(new CodeInstruction(OpCodes.Ret));

                original.InsertRange(0, edit);
                return original.AsEnumerable();
            }
        }
    }
}

public partial class Category_Electronic
{
    public partial class Electronic_PowerCounter
    {
        [HookAttribute.Patch("OnCounterTargetChange [patch]", "OnCounterTargetChange [patch]", "PowerCounter", "SERVER_SetTarget", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("3f09ca4811654c6c926f57c2751a452b")]
        [HookAttribute.Dependencies(new System.String[] { "OnCounterTargetChange" })]
        [HookAttribute.Options(HookFlags.Patch)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Electronic")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Electronic_PowerCounter_3f09ca4811654c6c926f57c2751a452b : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                edit.Add(new CodeInstruction(OpCodes.Ldloc_0));

                original[20 + 3].MoveLabelsFrom(original[20]);
                original.RemoveRange(20, 3);
                original.InsertRange(20, edit);
                return original.AsEnumerable();
            }
        }
    }
}

public partial class Category_Electronic
{
    public partial class Electronic_PowerCounter
    {
        [HookAttribute.Patch("OnCounterModeToggle", "OnCounterModeToggle", "PowerCounter", "ToggleDisplayMode", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("5e89989ffb724ff4bf5c59dc667d4662")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Electronic")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Electronic_PowerCounter_5e89989ffb724ff4bf5c59dc667d4662 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                edit.Add(new CodeInstruction(OpCodes.Ldarg_1));
                edit.Add(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("BaseEntity/RPCMessage"), "read")));
                edit.Add(new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("Network.NetRead"), "Bit")));
                LocalBuilder var_397802e3bde5414480d2ea2b1f00d72b = Generator.DeclareLocal(typeof(System.Boolean));
                edit.Add(new CodeInstruction(OpCodes.Stloc_0));
                edit.Add(new CodeInstruction(OpCodes.Ldstr, "OnCounterModeToggle"));
                // Opop
                edit.Add(new CodeInstruction(OpCodes.Ldarg_0));
                edit.Add(new CodeInstruction(OpCodes.Ldarg_1));
                edit.Add(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("BaseEntity/RPCMessage"), "player")));
                edit.Add(new CodeInstruction(OpCodes.Ldloc_0));
                edit.Add(new CodeInstruction(OpCodes.Box, typeof(System.Boolean)));
                edit.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("Oxide.Core.Interface"), "CallHook", new System.Type[] { typeof(System.String), typeof(System.Object), typeof(System.Object), typeof(System.Object) })));
                Label label_d7811ac1fdb84863b2686a8a002f8628 = Generator.DefineLabel();
                original[0].labels.Add(label_d7811ac1fdb84863b2686a8a002f8628);
                edit.Add(new CodeInstruction(OpCodes.Brfalse_S, label_d7811ac1fdb84863b2686a8a002f8628));
                edit.Add(new CodeInstruction(OpCodes.Ret));

                original.InsertRange(0, edit);
                return original.AsEnumerable();
            }
        }
    }
}

public partial class Category_Electronic
{
    public partial class Electronic_PowerCounter
    {
        [HookAttribute.Patch("OnCounterModeToggle [patch]", "OnCounterModeToggle [patch]", "PowerCounter", "ToggleDisplayMode", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("71d1d57380b6431db7a89dcf7b8cf8b7")]
        [HookAttribute.Dependencies(new System.String[] { "OnCounterModeToggle" })]
        [HookAttribute.Options(HookFlags.Patch)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Electronic")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Electronic_PowerCounter_71d1d57380b6431db7a89dcf7b8cf8b7 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                edit.Add(new CodeInstruction(OpCodes.Ldloc_0));

                original[20 + 3].MoveLabelsFrom(original[20]);
                original.RemoveRange(20, 3);
                original.InsertRange(20, edit);
                return original.AsEnumerable();
            }
        }
    }
}

public partial class Category_Electronic
{
    public partial class Electronic_IOEntityIORef
    {
        [HookAttribute.Patch("OnIORefCleared [patch]", "OnIORefCleared [patch]", "IOEntity/IORef", "Clear", [])]
        [HookAttribute.Identifier("1eb2bb1fb6f0485e83ef791a564487a9")]
        [HookAttribute.Options(HookFlags.Patch)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Electronic")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Electronic_IOEntityIORef_1eb2bb1fb6f0485e83ef791a564487a9 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                // Opop
                edit.Add(new CodeInstruction(OpCodes.Ldarg_0));
                edit.Add(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("IOEntity/IORef"), "ioEnt")));
                LocalBuilder var_4b868fb818ca407d98bfee08207c8b72 = Generator.DeclareLocal(typeof(System.Object));
                edit.Add(new CodeInstruction(OpCodes.Stloc_0));

                original.InsertRange(0, edit);
                return original.AsEnumerable();
            }
        }
    }
}

public partial class Category_Electronic
{
    public partial class Electronic_HBHFSensor
    {
        [HookAttribute.Patch("OnSensorDetect", "OnSensorDetect", "HBHFSensor", "CountDetectedPlayers", [])]
        [HookAttribute.Identifier("02cbe4dacb9d4603a1da946ccdc936ca")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Electronic")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Electronic_HBHFSensor_02cbe4dacb9d4603a1da946ccdc936ca : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                edit.Add(new CodeInstruction(OpCodes.Ldstr, "OnSensorDetect").MoveLabelsFrom(original[27]));
                // Opop
                edit.Add(new CodeInstruction(OpCodes.Ldarg_0));
                edit.Add(new CodeInstruction(OpCodes.Ldloc, 5));
                edit.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("Oxide.Core.Interface"), "CallHook", new System.Type[] { typeof(System.String), typeof(System.Object), typeof(System.Object) })));
                Label label_d9ba711967b34d298cd68db85088652b = Generator.DefineLabel();
                original[88].labels.Add(label_d9ba711967b34d298cd68db85088652b);
                edit.Add(new CodeInstruction(OpCodes.Brtrue_S, label_d9ba711967b34d298cd68db85088652b));

                original.InsertRange(27, edit);
                return original.AsEnumerable();
            }
        }
    }
}

