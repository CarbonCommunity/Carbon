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

public partial class Category_Clan
{
    public partial class Clan_LocalClanDisbandd72
    {
        [HookAttribute.Patch("OnClanDisbanded", "OnClanDisbanded", "LocalClan/<Disband>d__72", "MoveNext", [])]
        [HookAttribute.Identifier("0dc007a4f010438ca94b23510d1c73f5")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("local1", "LocalClan")]
        [MetadataAttribute.Parameter("self", "LocalClan+<Disband>d__72")]
        [MetadataAttribute.Category("Clan")]
        [MetadataAttribute.Assembly("Rust.Clans.Local.dll")]
        public class Clan_LocalClanDisbandd72_0dc007a4f010438ca94b23510d1c73f5 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 74)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3208843923)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_1  True LocalClan 
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
                    // AddYieldInstruction: Ldarg_0  True LocalClan+<Disband>d__72 bySteamId
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // Set LocalClan+<Disband>d__72
                    // value:bySteamId isProperty:False runtimeType:System.UInt64 currentType:LocalClan+<Disband>d__72 type:LocalClan+<Disband>d__72
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("LocalClan+<Disband>d__72"), "bySteamId"));
                    yield return new CodeInstruction(OpCodes.Box, Carbon.Extensions.AccessToolsEx.TypeByName("System.UInt64"));
                    // Read LocalClan+<Disband>d__72 : LocalClan+<Disband>d__72
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

public partial class Category_Clan
{
    public partial class Clan_LocalClanBackendCreated11
    {
        [HookAttribute.Patch("OnClanCreated", "OnClanCreated", "LocalClanBackend/<Create>d__11", "MoveNext", [])]
        [HookAttribute.Identifier("de754b13f82247708a31baaaece02d76")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("local3", "LocalClan")]
        [MetadataAttribute.Parameter("self", "LocalClanBackend+<Create>d__11")]
        [MetadataAttribute.Category("Clan")]
        [MetadataAttribute.Assembly("Rust.Clans.Local.dll")]
        public class Clan_LocalClanBackendCreated11_de754b13f82247708a31baaaece02d76 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 199)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3053309100)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_3  True LocalClan 
                    yield return new CodeInstruction(OpCodes.Ldloc_3);
                    // AddYieldInstruction: Ldarg_0  True LocalClanBackend+<Create>d__11 leaderSteamId
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // Set LocalClanBackend+<Create>d__11
                    // value:leaderSteamId isProperty:False runtimeType:System.UInt64 currentType:LocalClanBackend+<Create>d__11 type:LocalClanBackend+<Create>d__11
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("LocalClanBackend+<Create>d__11"), "leaderSteamId"));
                    yield return new CodeInstruction(OpCodes.Box, Carbon.Extensions.AccessToolsEx.TypeByName("System.UInt64"));
                    // Read LocalClanBackend+<Create>d__11 : LocalClanBackend+<Create>d__11
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

public partial class Category_Clan
{
    public partial class Clan_LocalClanDatabase
    {
        [HookAttribute.Patch("OnClanMemberAdded", "OnClanMemberAdded", "LocalClanDatabase", "AcceptInvite", ["System.Int64", "System.UInt64"])]
        [HookAttribute.Identifier("5984808a5d12474a83071806a13f30ac")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Category("Clan")]
        [MetadataAttribute.Assembly("Rust.Clans.Local.dll")]
        public class Clan_LocalClanDatabase_5984808a5d12474a83071806a13f30ac : API.Hooks.Patch
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
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)895202397)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldarg_1  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    // WAAAAAAA 3
                    // AddYieldInstruction: Box typeof(System.Int64) False  
                    yield return new CodeInstruction(OpCodes.Box, typeof(System.Int64));
                    // AddYieldInstruction: Ldarg_2  True  
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    // WAAAAAAA 3
                    // AddYieldInstruction: Box typeof(System.UInt64) False  
                    yield return new CodeInstruction(OpCodes.Box, typeof(System.UInt64));
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

public partial class Category_Clan
{
    public partial class Clan_LocalClanKickd65
    {
        [HookAttribute.Patch("OnClanMemberLeft", "OnClanMemberLeft", "LocalClan/<Kick>d__65", "MoveNext", [])]
        [HookAttribute.Identifier("23da889b5950408b95a6b1fe8f3d6087")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("local1", "LocalClan")]
        [MetadataAttribute.Parameter("self", "LocalClan+<Kick>d__65")]
        [MetadataAttribute.Category("Clan")]
        [MetadataAttribute.Assembly("Rust.Clans.Local.dll")]
        public class Clan_LocalClanKickd65_23da889b5950408b95a6b1fe8f3d6087 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 127)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1270737373)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_1  True LocalClan 
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
                    // AddYieldInstruction: Ldarg_0  True LocalClan+<Kick>d__65 steamId
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // Set LocalClan+<Kick>d__65
                    // value:steamId isProperty:False runtimeType:System.UInt64 currentType:LocalClan+<Kick>d__65 type:LocalClan+<Kick>d__65
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("LocalClan+<Kick>d__65"), "steamId"));
                    yield return new CodeInstruction(OpCodes.Box, Carbon.Extensions.AccessToolsEx.TypeByName("System.UInt64"));
                    // Read LocalClan+<Kick>d__65 : LocalClan+<Kick>d__65
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

public partial class Category_Clan
{
    public partial class Clan_LocalClanKickd65
    {
        [HookAttribute.Patch("OnClanMemberKicked", "OnClanMemberKicked", "LocalClan/<Kick>d__65", "MoveNext", [])]
        [HookAttribute.Identifier("44e93ae43713435daef6f22cc21c633b")]
        [HookAttribute.Dependencies(new System.String[] { "OnClanMemberLeft" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("local1", "LocalClan")]
        [MetadataAttribute.Parameter("self", "LocalClan+<Kick>d__65")]
        [MetadataAttribute.Parameter("self1", "LocalClan+<Kick>d__65")]
        [MetadataAttribute.Category("Clan")]
        [MetadataAttribute.Assembly("Rust.Clans.Local.dll")]
        public class Clan_LocalClanKickd65_44e93ae43713435daef6f22cc21c633b : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 144)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)151056655)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_1  True LocalClan 
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
                    // AddYieldInstruction: Ldarg_0  True LocalClan+<Kick>d__65 steamId
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // Set LocalClan+<Kick>d__65
                    // value:steamId isProperty:False runtimeType:System.UInt64 currentType:LocalClan+<Kick>d__65 type:LocalClan+<Kick>d__65
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("LocalClan+<Kick>d__65"), "steamId"));
                    yield return new CodeInstruction(OpCodes.Box, Carbon.Extensions.AccessToolsEx.TypeByName("System.UInt64"));
                    // Read LocalClan+<Kick>d__65 : LocalClan+<Kick>d__65
                    // AddYieldInstruction: Ldarg_0  True LocalClan+<Kick>d__65 bySteamId
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // Set LocalClan+<Kick>d__65
                    // value:bySteamId isProperty:False runtimeType:System.UInt64 currentType:LocalClan+<Kick>d__65 type:LocalClan+<Kick>d__65
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("LocalClan+<Kick>d__65"), "bySteamId"));
                    yield return new CodeInstruction(OpCodes.Box, Carbon.Extensions.AccessToolsEx.TypeByName("System.UInt64"));
                    // Read LocalClan+<Kick>d__65 : LocalClan+<Kick>d__65
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

public partial class Category_Clan
{
    public partial class Clan_LocalClanSetColord61
    {
        [HookAttribute.Patch("OnClanColorChanged", "OnClanColorChanged", "LocalClan/<SetColor>d__61", "MoveNext", [])]
        [HookAttribute.Identifier("f7f2b2efc903467eb0ad37dc4194c56a")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("local0", "LocalClan")]
        [MetadataAttribute.Parameter("self", "LocalClan+<SetColor>d__61")]
        [MetadataAttribute.Parameter("self1", "LocalClan+<SetColor>d__61")]
        [MetadataAttribute.Category("Clan")]
        [MetadataAttribute.Assembly("Rust.Clans.Local.dll")]
        public class Clan_LocalClanSetColord61_f7f2b2efc903467eb0ad37dc4194c56a : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 54)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)1049643152)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_0  True LocalClan 
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    // AddYieldInstruction: Ldarg_0  True LocalClan+<SetColor>d__61 newColor
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // Set LocalClan+<SetColor>d__61
                    // value:newColor isProperty:False runtimeType:UnityEngine.Color32 currentType:LocalClan+<SetColor>d__61 type:LocalClan+<SetColor>d__61
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("LocalClan+<SetColor>d__61"), "newColor"));
                    yield return new CodeInstruction(OpCodes.Box, Carbon.Extensions.AccessToolsEx.TypeByName("UnityEngine.Color32"));
                    // Read LocalClan+<SetColor>d__61 : LocalClan+<SetColor>d__61
                    // AddYieldInstruction: Ldarg_0  True LocalClan+<SetColor>d__61 bySteamId
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // Set LocalClan+<SetColor>d__61
                    // value:bySteamId isProperty:False runtimeType:System.UInt64 currentType:LocalClan+<SetColor>d__61 type:LocalClan+<SetColor>d__61
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("LocalClan+<SetColor>d__61"), "bySteamId"));
                    yield return new CodeInstruction(OpCodes.Box, Carbon.Extensions.AccessToolsEx.TypeByName("System.UInt64"));
                    // Read LocalClan+<SetColor>d__61 : LocalClan+<SetColor>d__61
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

public partial class Category_Clan
{
    public partial class Clan_LocalClanSetLogod60
    {
        [HookAttribute.Patch("OnClanLogoChanged", "OnClanLogoChanged", "LocalClan/<SetLogo>d__60", "MoveNext", [])]
        [HookAttribute.Identifier("70890d20f3c5475d87ff4b9c45da0004")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Parameter("local0", "LocalClan")]
        [MetadataAttribute.Parameter("self", "LocalClan+<SetLogo>d__60")]
        [MetadataAttribute.Parameter("self1", "LocalClan+<SetLogo>d__60")]
        [MetadataAttribute.Category("Clan")]
        [MetadataAttribute.Assembly("Rust.Clans.Local.dll")]
        public class Clan_LocalClanSetLogod60_70890d20f3c5475d87ff4b9c45da0004 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                int x = 0;
                foreach (CodeInstruction instruction in Instructions)
                {
                    if (x++ != 62)
                    {
                        yield return instruction;
                        continue;
                    }

                    // hook call start
                    yield return new CodeInstruction(OpCodes.Ldc_I4, unchecked((int)3755965056)).MoveLabelsFrom(instruction);
                    // AddYieldInstruction: Ldloc_0  True LocalClan 
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    // AddYieldInstruction: Ldarg_0  True LocalClan+<SetLogo>d__60 newLogo
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // Set LocalClan+<SetLogo>d__60
                    // value:newLogo isProperty:False runtimeType:System.Byte[] currentType:LocalClan+<SetLogo>d__60 type:LocalClan+<SetLogo>d__60
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("LocalClan+<SetLogo>d__60"), "newLogo"));
                    // Read LocalClan+<SetLogo>d__60 : LocalClan+<SetLogo>d__60
                    // AddYieldInstruction: Ldarg_0  True LocalClan+<SetLogo>d__60 bySteamId
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // Set LocalClan+<SetLogo>d__60
                    // value:bySteamId isProperty:False runtimeType:System.UInt64 currentType:LocalClan+<SetLogo>d__60 type:LocalClan+<SetLogo>d__60
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("LocalClan+<SetLogo>d__60"), "bySteamId"));
                    yield return new CodeInstruction(OpCodes.Box, Carbon.Extensions.AccessToolsEx.TypeByName("System.UInt64"));
                    // Read LocalClan+<SetLogo>d__60 : LocalClan+<SetLogo>d__60
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

public partial class Category_Clan
{
    public partial class Clan_LocalClanSetLogod60
    {
        [HookAttribute.Patch("OnClanLogoChanged [patch]", "OnClanLogoChanged [patch]", "LocalClan/<SetLogo>d__60", "MoveNext", [])]
        [HookAttribute.Identifier("fbdffeeea1f640aba963f0d8094b2d8b")]
        [HookAttribute.Dependencies(new System.String[] { "OnClanLogoChanged" })]
        [HookAttribute.Options(HookFlags.Patch)]
        [MetadataAttribute.Category("Clan")]
        [MetadataAttribute.Assembly("Rust.Clans.Local.dll")]
        public class Clan_LocalClanSetLogod60_fbdffeeea1f640aba963f0d8094b2d8b : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);


                original[66 + 1].MoveLabelsFrom(original[66]);
                original.RemoveRange(66, 1);
                original.InsertRange(66, edit);
                return original.AsEnumerable();
            }
        }
    }
}

