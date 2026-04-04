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
// Auto generated at 2026-03-02 11:41:08

namespace Carbon.Hooks;
#pragma warning disable IDE0051
#pragma warning disable IDE0060

public partial class Category_Server
{
    public partial class Server_Bootstrap
    {
        [HookAttribute.Patch("InitLogging", "InitLogging", "Bootstrap", "StartupShared", [])]
        [HookAttribute.Identifier("22e4f27f247947caa2232b3aafb21835")]
        [HookAttribute.Options(HookFlags.Static | HookFlags.Hidden)]
        [MetadataAttribute.Category("Server")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Server_Bootstrap_22e4f27f247947caa2232b3aafb21835 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2759983490)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Call AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint) }) False  
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint) }));
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

public partial class Category_Server
{
    public partial class Server_ServerMgr
    {
        [HookAttribute.Patch("OnTick", "OnTick", "ServerMgr", "DoTick", [])]
        [HookAttribute.Identifier("44acc7fde5264c72a7f6434be68bc1da")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Category("Server")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Server_ServerMgr_44acc7fde5264c72a7f6434be68bc1da : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1216670645)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Call AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint) }) False  
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint) }));
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

public partial class Category_Server
{
    public partial class Server_BasePlayer
    {
        [HookAttribute.Patch("OnMessagePlayer", "OnMessagePlayer", "BasePlayer", "ChatMessage", ["System.String"])]
        [HookAttribute.Identifier("922c85e42a304970b52d10e01195f857")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("msg", "System.String")]
        [MetadataAttribute.Parameter("self", "BasePlayer")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Server")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Server_BasePlayer_922c85e42a304970b52d10e01195f857 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 3)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1279972524)).MoveLabelsFrom(instruction);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True System.String 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Ldarg_0  True BasePlayer 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
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

public partial class Category_Server
{
    public partial class Server_ConVarChat
    {
        [HookAttribute.Patch("OnServerMessage", "OnServerMessage", "ConVar.Chat", "Broadcast", ["System.String", "System.String", "System.String", "System.UInt64"])]
        [HookAttribute.Identifier("848b390057ef4621917a1a0d148e9df7")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Server")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Server_ConVarChat_848b390057ef4621917a1a0d148e9df7 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3155060134)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // AddYieldInstruction: Ldarg_1  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // AddYieldInstruction: Ldarg_2  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    // AddYieldInstruction: Ldarg_3  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_3);
                    // WAAAAAAA 3
                    // AddYieldInstruction: Box typeof(System.UInt64) False  
                    yield return new CodeInstruction(OpCodes.Box, typeof(System.UInt64));
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

public partial class Category_Server
{
    public partial class Server_FacepunchRConRConListener
    {
        [HookAttribute.Patch("OnRconConnection", "OnRconConnection [exp]", "Facepunch.RCon/RConListener", "ProcessConnections", [])]
        [HookAttribute.Identifier("6966a7e6922d4cbeb24a9b18e9484ea9")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("address", "System.Net.IPAddress")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Server")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Server_FacepunchRConRConListener_6966a7e6922d4cbeb24a9b18e9484ea9 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3983888645)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_1  True System.Net.IPEndPoint Address
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
                    // Set System.Net.IPEndPoint
                    // value:Address isProperty:True runtimeType:System.Net.IPAddress currentType:System.Net.IPEndPoint type:System.Net.IPEndPoint
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("System.Net.IPEndPoint"), "get_Address"));
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

public partial class Category_Server
{
    public partial class Server_SaveRestore
    {
        [HookAttribute.Patch("OnNewSave", "OnNewSave", "SaveRestore", "Load", ["System.String", "System.Boolean"])]
        [HookAttribute.Identifier("ad4941b2258e42918f5d26ba080704d2")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("strFilename", "System.String")]
        [MetadataAttribute.Category("Server")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Server_SaveRestore_ad4941b2258e42918f5d26ba080704d2 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2815160424)).MoveLabelsFrom(instruction);
                    // HERE!
                    // AddYieldInstruction: Ldarg_0  True System.String 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // WAAAAAAA 1
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

public partial class Category_Server
{
    public partial class Server_ServerMgr
    {
        [HookAttribute.Patch("IOnServerShutdown", "IOnServerShutdown", "ServerMgr", "Shutdown", [])]
        [HookAttribute.Identifier("10e8d9d15bfd45e183cde8cd6420f234")]
        [HookAttribute.Options(HookFlags.Static | HookFlags.Hidden)]
        [MetadataAttribute.Category("Server")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Server_ServerMgr_10e8d9d15bfd45e183cde8cd6420f234 : API.Hooks.Patch
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
                    // AddYieldInstruction: Call AccessTools.Method(typeof(Carbon.Core.CorePlugin), "IOnServerShutdown") False  
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Carbon.Core.CorePlugin), "IOnServerShutdown"));
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

public partial class Category_Server
{
    public partial class Server_SaveRestore
    {
        [HookAttribute.Patch("OnSaveLoad", "OnSaveLoad", "SaveRestore", "Load", ["System.String", "System.Boolean"])]
        [HookAttribute.Identifier("1994b73a1e31452eb6b6c810e7cc6825")]
        [HookAttribute.Dependencies(new System.String[] { "OnNewSave" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("local1", "System.Collections.Generic.Dictionary`2[BaseEntity,ProtoBuf.Entity]")]
        [MetadataAttribute.Return(typeof(System.Boolean))]
        [MetadataAttribute.Category("Server")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Server_SaveRestore_1994b73a1e31452eb6b6c810e7cc6825 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 367)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)106238856)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_1  True System.Collections.Generic.Dictionary`2[BaseEntity,ProtoBuf.Entity] 
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
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

public partial class Category_Server
{
    public partial class Server_ServerUsers
    {
        [HookAttribute.Patch("OnServerUserSet", "OnServerUserSet", "ServerUsers", "Set", ["System.UInt64", "ServerUsers/UserGroup", "System.String", "System.String", "System.Int64"])]
        [HookAttribute.Identifier("88c569b04f9e4738aa139fb8da4287e6")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Category("Server")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Server_ServerUsers_88c569b04f9e4738aa139fb8da4287e6 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)931424179)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // WAAAAAAA 3
                    // AddYieldInstruction: Box typeof(System.UInt64) False  
                    yield return new CodeInstruction(OpCodes.Box, typeof(System.UInt64));
                    // AddYieldInstruction: Ldarg_1  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 3
                    // AddYieldInstruction: Box typeof(ServerUsers.UserGroup) False  
                    yield return new CodeInstruction(OpCodes.Box, typeof(ServerUsers.UserGroup));
                    // AddYieldInstruction: Ldarg_2  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    // AddYieldInstruction: Ldarg_3  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_3);
                    // AddYieldInstruction: Ldarg_S 4 False  
                    yield return new CodeInstruction(OpCodes.Ldarg_S, 4);
                    // WAAAAAAA 3
                    // AddYieldInstruction: Box typeof(System.Int64) False  
                    yield return new CodeInstruction(OpCodes.Box, typeof(System.Int64));
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

public partial class Category_Server
{
    public partial class Server_SaveRestore
    {
        [HookAttribute.Patch("OnServerSave", "OnServerSave", "SaveRestore", "DoAutomatedSave", ["System.Boolean"])]
        [HookAttribute.Identifier("0324483d737b4bf39c97f56d2828630d")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Category("Server")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Server_SaveRestore_0324483d737b4bf39c97f56d2828630d : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2396958305)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Call AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint) }) False  
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint) }));
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

public partial class Category_Server
{
    public partial class Server_FacepunchRCon
    {
        [HookAttribute.Patch("IOnRconInitialize", "IOnRconInitialize", "Facepunch.RCon", "Initialize", [])]
        [HookAttribute.Identifier("23a8bc5b09064a3d8dc9cb703a488bc3")]
        [HookAttribute.Options(HookFlags.Static | HookFlags.Hidden)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Server")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Server_FacepunchRCon_23a8bc5b09064a3d8dc9cb703a488bc3 : API.Hooks.Patch
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
                    // AddYieldInstruction: Call AccessTools.Method(typeof(Carbon.Core.CorePlugin), "IOnRconInitialize") False  
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Carbon.Core.CorePlugin), "IOnRconInitialize"));
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

public partial class Category_Server
{
    public partial class Server_ServerUsers
    {
        [HookAttribute.Patch("OnServerUserRemove", "OnServerUserRemove", "ServerUsers", "Remove", ["System.UInt64"])]
        [HookAttribute.Identifier("b0a6dee3dbd347a4907f8360bdb58a43")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Category("Server")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Server_ServerUsers_b0a6dee3dbd347a4907f8360bdb58a43 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)2043356880)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_0  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // WAAAAAAA 3
                    // AddYieldInstruction: Box typeof(System.UInt64) False  
                    yield return new CodeInstruction(OpCodes.Box, typeof(System.UInt64));
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

public partial class Category_Server
{
    public partial class Server_ConsoleNetwork
    {
        [HookAttribute.Patch("OnSendCommand", "OnSendCommand", "ConsoleNetwork", "SendClientCommand", ["Network.Connection", "System.String", "System.Object[]"])]
        [HookAttribute.Identifier("7e2673fa385841f9b185536cffc66c88")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("cn", "Network.Connection")]
        [MetadataAttribute.Parameter("strCommand", "System.String")]
        [MetadataAttribute.Parameter("args", "System.Object[]")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Server")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Server_ConsoleNetwork_7e2673fa385841f9b185536cffc66c88 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)156775275)).MoveLabelsFrom(instruction);
                    // HERE!
                    // AddYieldInstruction: Ldarg_0  True Network.Connection 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // WAAAAAAA 1
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True System.String 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // HERE!
                    // AddYieldInstruction: Ldarg_2  True System.Object[] 
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
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

public partial class Category_Server
{
    public partial class Server_ConsoleNetwork
    {
        [HookAttribute.Patch("OnSendCommand", "OnSendCommand [list]", "ConsoleNetwork", "SendClientCommand", ["System.Collections.Generic.List`1<Network.Connection>", "System.String", "System.Object[]"])]
        [HookAttribute.Identifier("923b859ce900495aa23797840df82996")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("cn", "Network.Connection")]
        [MetadataAttribute.Parameter("strCommand", "System.String")]
        [MetadataAttribute.Parameter("args", "System.Object[]")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Server")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Server_ConsoleNetwork_923b859ce900495aa23797840df82996 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)156775275)).MoveLabelsFrom(instruction);
                    // HERE!
                    // AddYieldInstruction: Ldarg_0  True Network.Connection 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // WAAAAAAA 1
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True System.String 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // HERE!
                    // AddYieldInstruction: Ldarg_2  True System.Object[] 
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
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

public partial class Category_Server
{
    public partial class Server_ConsoleNetwork
    {
        [HookAttribute.Patch("OnBroadcastCommand", "OnBroadcastCommand", "ConsoleNetwork", "BroadcastToAllClients", ["System.String", "System.Object[]"])]
        [HookAttribute.Identifier("dbc7beaaadb5461bbfd9a1a4fb165929")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("strCommand", "System.String")]
        [MetadataAttribute.Parameter("args", "System.Object[]")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Server")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Server_ConsoleNetwork_dbc7beaaadb5461bbfd9a1a4fb165929 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)4110903621)).MoveLabelsFrom(instruction);
                    // HERE!
                    // AddYieldInstruction: Ldarg_0  True System.String 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // WAAAAAAA 1
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True System.Object[] 
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

public partial class Category_Server
{
    public partial class Server_ServerMgr
    {
        [HookAttribute.Patch("OnServerInitialize", "OnServerInitialize", "ServerMgr", "Initialize", ["System.Boolean", "System.String", "System.Boolean", "System.Boolean"])]
        [HookAttribute.Identifier("5b97029a1c3f4ca3b559a612d4f4a4c4")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Category("Server")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Server_ServerMgr_5b97029a1c3f4ca3b559a612d4f4a4c4 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3248723167)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Call AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint) }) False  
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint) }));
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

public partial class Category_Server
{
    public partial class Server_ServerMgr
    {
        [HookAttribute.Patch("IOnServerInitialized", "IOnServerInitialized", "ServerMgr", "OpenConnection", ["System.Boolean"])]
        [HookAttribute.Identifier("b047513670e34940a83ad69339c4226b")]
        [HookAttribute.Options(HookFlags.Static | HookFlags.Hidden)]
        [MetadataAttribute.Category("Server")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Server_ServerMgr_b047513670e34940a83ad69339c4226b : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 119)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    // AddYieldInstruction: Call AccessTools.Method(typeof(Carbon.Core.CorePlugin), "IOnServerInitialized") False  
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Carbon.Core.CorePlugin), "IOnServerInitialized"));
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

public partial class Category_Server
{
    public partial class Server_ServerMgr
    {
        [HookAttribute.Patch("OnServerRestartInterrupt", "OnServerRestartInterrupt", "ServerMgr", "RestartServer", ["System.String", "System.Int32"])]
        [HookAttribute.Identifier("ca6369516ff54eb69a82862181608ec8")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Server")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Server_ServerMgr_ca6369516ff54eb69a82862181608ec8 : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1183644224)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Call AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint) }) False  
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint) }));
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

public partial class Category_Server
{
    public partial class Server_ServerMgr
    {
        [HookAttribute.Patch("OnServerRestart", "OnServerRestart", "ServerMgr", "RestartServer", ["System.String", "System.Int32"])]
        [HookAttribute.Identifier("2b40067ee842477190cecc6108912478")]
        [HookAttribute.Dependencies(new System.String[] { "OnServerRestartInterrupt" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("strNotice", "System.String")]
        [MetadataAttribute.Parameter("iSeconds", "System.Int32")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Server")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Server_ServerMgr_2b40067ee842477190cecc6108912478 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 25)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3716566533)).MoveLabelsFrom(instruction);
                    // HERE!
                    // AddYieldInstruction: Ldarg_0  True System.String 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // WAAAAAAA 1
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True System.Int32 
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Box typeof(System.Int32) False  
                    yield return new CodeInstruction(OpCodes.Box, typeof(System.Int32));
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

public partial class Category_Server
{
    public partial class Server_ServerMgr
    {
        [HookAttribute.Patch("OnServerInformationUpdated", "OnServerInformationUpdated", "ServerMgr", "UpdateServerInformation", [])]
        [HookAttribute.Identifier("ff6185b62cdb4c2695a9acc834174073")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Category("Server")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Server_ServerMgr_ff6185b62cdb4c2695a9acc834174073 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 372)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)4109979236)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Call AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint) }) False  
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HookCaller), nameof(HookCaller.CallStaticHook), new System.Type[] { typeof(uint) }));
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

public partial class Category_Server
{
    public partial class Server_ConsoleSystem
    {
        [HookAttribute.Patch("IOnServerCommand", "IOnServerCommand", "ConsoleSystem", "Internal", ["ConsoleSystem/Arg"])]
        [HookAttribute.Identifier("2afd7c7dc17648a6bde8b010c9956232")]
        [HookAttribute.Options(HookFlags.Static | HookFlags.Hidden)]
        [MetadataAttribute.Parameter("arg", "ConsoleSystem+Arg")]
        [MetadataAttribute.Return(typeof(System.Boolean))]
        [MetadataAttribute.Category("Server")]
        [MetadataAttribute.Assembly("Facepunch.Console.dll")]
        public class Server_ConsoleSystem_2afd7c7dc17648a6bde8b010c9956232 : API.Hooks.Patch
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
                    // HERE!
                    // AddYieldInstruction: Ldarg_0  True ConsoleSystem+Arg 
                    yield return new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(instruction);
                    // WAAAAAAA 1
                    // AddYieldInstruction: Call AccessTools.Method(typeof(Carbon.Core.CorePlugin), "IOnServerCommand") False  
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Carbon.Core.CorePlugin), "IOnServerCommand"));
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

public partial class Category_Server
{
    public partial class Server_ConsoleSystem
    {
        [HookAttribute.Patch("IOnRunCommandLine", "IOnRunCommandLine", "ConsoleSystem", "UpdateValuesFromCommandLine", [])]
        [HookAttribute.Identifier("f7e038af31504ed6b7cc1da55828b1cb")]
        [HookAttribute.Options(HookFlags.Static | HookFlags.Hidden)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Server")]
        [MetadataAttribute.Assembly("Facepunch.Console.dll")]
        public class Server_ConsoleSystem_f7e038af31504ed6b7cc1da55828b1cb : API.Hooks.Patch
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
                    // AddYieldInstruction: Call AccessTools.Method(typeof(Carbon.Core.CorePlugin), "IOnRunCommandLine") False  
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Carbon.Core.CorePlugin), "IOnRunCommandLine"));
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

public partial class Category_Server
{
    public partial class Server_FacepunchRconListenercDisplayClass300
    {
        [HookAttribute.Patch("OnRconConnection", "OnRconConnection [web]", "Facepunch.Rcon.Listener/<>c__DisplayClass30_0", "<Start>b__0", ["Fleck.IWebSocketConnection"])]
        [HookAttribute.Identifier("692168627f4d4b05a70d13eb334309dc")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("clientIpAddress", "System.Net.IPAddress")]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("Server")]
        [MetadataAttribute.Assembly("Facepunch.Rcon.dll")]
        public class Server_FacepunchRconListenercDisplayClass300_692168627f4d4b05a70d13eb334309dc : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 46)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3983888645)).MoveLabelsFrom(instruction);
                    // HERE!
                    // AddYieldInstruction: Ldarg_1  True Fleck.IWebSocketConnection ConnectionInfo, ClientIpAddress
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // Set Fleck.IWebSocketConnection
                    // value:ConnectionInfo isProperty:True runtimeType:Fleck.IWebSocketConnectionInfo currentType:Fleck.IWebSocketConnection type:Fleck.IWebSocketConnection
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("Fleck.IWebSocketConnection"), "get_ConnectionInfo"));
                    // Set Fleck.IWebSocketConnectionInfo
                    // value:ClientIpAddress isProperty:True runtimeType:System.Net.IPAddress currentType:Fleck.IWebSocketConnectionInfo type:Fleck.IWebSocketConnection
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("Fleck.IWebSocketConnectionInfo"), "get_ClientIpAddress"));
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

