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

public partial class Category_Patches
{
    public partial class Patches_FacepunchRConRConListener
    {
        [HookAttribute.Patch("OnRconConnection [exp, patch]", "OnRconConnection [exp, patch]", "Facepunch.RCon/RConListener", "ProcessConnections", [])]
        [HookAttribute.Identifier("5fdea88204cb4b0ebc7e1b0ca5b149bd")]
        [HookAttribute.Dependencies(new System.String[] { "OnRconConnection [exp]" })]
        [HookAttribute.Options(HookFlags.Patch)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("_Patches")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Patches_FacepunchRConRConListener_5fdea88204cb4b0ebc7e1b0ca5b149bd : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                edit.Add(new CodeInstruction(OpCodes.Ldloc_0));
                edit.Add(new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("System.Net.Sockets.Socket"), "Close")));

                original.InsertRange(22, edit);
                return original.AsEnumerable();
            }
        }
    }
}

public partial class Category_Patches
{
    public partial class Patches_PlayerLoot
    {
        [HookAttribute.Patch("OnLootEntity [patch]", "OnLootEntity [patch]", "PlayerLoot", "StartLootingEntity", ["BaseEntity", "System.Boolean"])]
        [HookAttribute.Identifier("af4c43dfdfab4de884b36d51652aa39e")]
        [HookAttribute.Dependencies(new System.String[] { "OnLootEntity" })]
        [HookAttribute.Options(HookFlags.Patch)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("_Patches")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Patches_PlayerLoot_af4c43dfdfab4de884b36d51652aa39e : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                edit.Add(new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("UnityEngine.Component"), "GetComponent", generics: new System.Type[] { typeof(BasePlayer) })));

                original.InsertRange(29, edit);
                return original.AsEnumerable();
            }
        }
    }
}

public partial class Category_Patches
{
    public partial class Patches_PlayerLoot
    {
        [HookAttribute.Patch("OnLootItem [patch]", "OnLootItem [patch]", "PlayerLoot", "StartLootingItem", ["Item"])]
        [HookAttribute.Identifier("b4a34ff0b76b4576ae5c3f5d3705d930")]
        [HookAttribute.Dependencies(new System.String[] { "OnLootItem" })]
        [HookAttribute.Options(HookFlags.Patch)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("_Patches")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Patches_PlayerLoot_b4a34ff0b76b4576ae5c3f5d3705d930 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                edit.Add(new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("UnityEngine.Component"), "GetComponent", generics: new System.Type[] { typeof(BasePlayer) })));

                original.InsertRange(32, edit);
                return original.AsEnumerable();
            }
        }
    }
}

public partial class Category_Patches
{
    public partial class Patches_BaseMelee
    {
        [HookAttribute.Patch("OnPlayerAttack [melee, patch]", "OnPlayerAttack [melee, patch]", "BaseMelee", "DoAttackShared", ["HitInfo"])]
        [HookAttribute.Identifier("a7c1cb39f12b4830972b009378cba669")]
        [HookAttribute.Dependencies(new System.String[] { "OnPlayerAttack [Melee]" })]
        [HookAttribute.Options(HookFlags.Patch)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("_Patches")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Patches_BaseMelee_a7c1cb39f12b4830972b009378cba669 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                edit.Add(new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("HeldEntity"), "GetOwnerPlayer")));

                original.InsertRange(2, edit);
                return original.AsEnumerable();
            }
        }
    }
}

public partial class Category_Patches
{
    public partial class Patches_BaseEntity
    {
        [HookAttribute.Patch("NoLimboGroupForPlayers [patch]", "NoLimboGroupForPlayers [patch]", "BaseEntity", "UpdateNetworkGroup", [])]
        [HookAttribute.Identifier("32f191e599d94c639e1024ecf93cdbf1")]
        [HookAttribute.Options(HookFlags.Patch)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("_Patches")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Patches_BaseEntity_32f191e599d94c639e1024ecf93cdbf1 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                // Opop
                edit.Add(new CodeInstruction(OpCodes.Ldarg_0));
                edit.Add(new CodeInstruction(OpCodes.Isinst, typeof(BasePlayer)));
                Label label_d5cc08ff90bf45279f4464cc3d5a9e59 = Generator.DefineLabel();
                original[127].labels.Add(label_d5cc08ff90bf45279f4464cc3d5a9e59);
                edit.Add(new CodeInstruction(OpCodes.Brtrue_S, label_d5cc08ff90bf45279f4464cc3d5a9e59));

                original.InsertRange(119, edit);
                return original.AsEnumerable();
            }
        }
    }
}

public partial class Category_Patches
{
    public partial class Patches_ItemCrafter
    {
        [HookAttribute.Patch("FixItemKeyId [patch]", "FixItemKeyId [patch]", "ItemCrafter", "CraftItem", ["ItemBlueprint", "BasePlayer", "ProtoBuf.Item/InstanceData", "System.Int32", "System.Int32", "Item", "System.Boolean"])]
        [HookAttribute.Identifier("bd3ee4147414432392233cc0e319cb14")]
        [HookAttribute.Dependencies(new System.String[] { "OnItemCraft" })]
        [HookAttribute.Options(HookFlags.Patch)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("_Patches")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Patches_ItemCrafter_bd3ee4147414432392233cc0e319cb14 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                Label label_a1afd24813584e9eacc9698cb005b21f = Generator.DefineLabel();
                original[90].labels.Add(label_a1afd24813584e9eacc9698cb005b21f);
                edit.Add(new CodeInstruction(OpCodes.Brfalse_S, label_a1afd24813584e9eacc9698cb005b21f));
                edit.Add(new CodeInstruction(OpCodes.Ldarg_S, (sbyte)6));
                Label label_142c2f9b81814c3d8399856950af4996 = Generator.DefineLabel();
                original[87].labels.Add(label_142c2f9b81814c3d8399856950af4996);
                edit.Add(new CodeInstruction(OpCodes.Brfalse_S, label_142c2f9b81814c3d8399856950af4996));
                edit.Add(new CodeInstruction(OpCodes.Ldloc_0));
                edit.Add(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("ItemCraftTask"), "instanceData")));
                edit.Add(new CodeInstruction(OpCodes.Brfalse_S, label_142c2f9b81814c3d8399856950af4996));
                edit.Add(new CodeInstruction(OpCodes.Ldarg_S, (sbyte)6));
                edit.Add(new CodeInstruction(OpCodes.Ldloc_0));
                edit.Add(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("ItemCraftTask"), "instanceData")));
                edit.Add(new CodeInstruction(OpCodes.Stfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("Item"), "instanceData")));

                original[85 + 2].MoveLabelsFrom(original[85]);
                original.RemoveRange(85, 2);
                original.InsertRange(85, edit);
                return original.AsEnumerable();
            }
        }
    }
}

public partial class Category_Patches
{
    public partial class Patches_SupplySignal
    {
        [HookAttribute.Patch("OnCargoPlaneSignaled [Patch]", "OnCargoPlaneSignaled [Patch]", "SupplySignal", "Explode", [])]
        [HookAttribute.Identifier("30acae28396e46ae85a519694e793cb9")]
        [HookAttribute.Dependencies(new System.String[] { "OnCargoPlaneSignaled" })]
        [HookAttribute.Options(HookFlags.Patch)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("_Patches")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Patches_SupplySignal_30acae28396e46ae85a519694e793cb9 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                Label label_8fd172de1581460abc56a2d01dee3b1f = Generator.DefineLabel();
                original[42].labels.Add(label_8fd172de1581460abc56a2d01dee3b1f);
                edit.Add(new CodeInstruction(OpCodes.Brfalse_S, label_8fd172de1581460abc56a2d01dee3b1f));

                original[15 + 1].MoveLabelsFrom(original[15]);
                original.RemoveRange(15, 1);
                original.InsertRange(15, edit);
                return original.AsEnumerable();
            }
        }
    }
}

public partial class Category_Patches
{
    public partial class Patches_CargoPlane
    {
        [HookAttribute.Patch("OnSupplyDropDropped [patch 1]", "OnSupplyDropDropped [patch 1]", "CargoPlane", "Update", [])]
        [HookAttribute.Identifier("9c671f3992ab4908840ba2bf82bbc001")]
        [HookAttribute.Dependencies(new System.String[] { "OnSupplyDropDropped" })]
        [HookAttribute.Options(HookFlags.Patch)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("_Patches")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Patches_CargoPlane_9c671f3992ab4908840ba2bf82bbc001 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                Label label_330e12170e564f79b302d1d292a1a6da = Generator.DefineLabel();
                original[52].labels.Add(label_330e12170e564f79b302d1d292a1a6da);
                edit.Add(new CodeInstruction(OpCodes.Brtrue_S, label_330e12170e564f79b302d1d292a1a6da));

                original[19 + 1].MoveLabelsFrom(original[19]);
                original.RemoveRange(19, 1);
                original.InsertRange(19, edit);
                return original.AsEnumerable();
            }
        }
    }
}

public partial class Category_Patches
{
    public partial class Patches_CargoPlane
    {
        [HookAttribute.Patch("OnSupplyDropDropped [patch 2]", "OnSupplyDropDropped [patch 2]", "CargoPlane", "Update", [])]
        [HookAttribute.Identifier("f5bea2b052c248bc8661850575061951")]
        [HookAttribute.Dependencies(new System.String[] { "OnSupplyDropDropped [patch 1]" })]
        [HookAttribute.Options(HookFlags.Patch)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("_Patches")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Patches_CargoPlane_f5bea2b052c248bc8661850575061951 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                Label label_72658aa9ef8d409a92b63ce98b79dfc3 = Generator.DefineLabel();
                original[52].labels.Add(label_72658aa9ef8d409a92b63ce98b79dfc3);
                edit.Add(new CodeInstruction(OpCodes.Blt_Un_S, label_72658aa9ef8d409a92b63ce98b79dfc3));

                original[22 + 1].MoveLabelsFrom(original[22]);
                original.RemoveRange(22, 1);
                original.InsertRange(22, edit);
                return original.AsEnumerable();
            }
        }
    }
}

public partial class Category_Patches
{
    public partial class Patches_CargoPlane
    {
        [HookAttribute.Patch("OnSupplyDropDropped [patch 3]", "OnSupplyDropDropped [patch 3]", "CargoPlane", "Update", [])]
        [HookAttribute.Identifier("90b17191dc69445ea109bf884eb650a1")]
        [HookAttribute.Dependencies(new System.String[] { "OnSupplyDropDropped [patch 2]" })]
        [HookAttribute.Options(HookFlags.Patch)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("_Patches")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Patches_CargoPlane_90b17191dc69445ea109bf884eb650a1 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                Label label_7002c99dd6c94a12b405cb1f5d3db827 = Generator.DefineLabel();
                original[52].labels.Add(label_7002c99dd6c94a12b405cb1f5d3db827);
                edit.Add(new CodeInstruction(OpCodes.Brfalse_S, label_7002c99dd6c94a12b405cb1f5d3db827));

                original[41 + 1].MoveLabelsFrom(original[41]);
                original.RemoveRange(41, 1);
                original.InsertRange(41, edit);
                return original.AsEnumerable();
            }
        }
    }
}

public partial class Category_Patches
{
    public partial class Patches_Effectserver
    {
        [HookAttribute.Patch("LimitNetworkingNoEffect [patch 1]", "LimitNetworkingNoEffect [patch 1]", "Effect/server", "ImpactEffect", ["HitInfo", "System.String"])]
        [HookAttribute.Identifier("1e62c22796234b3cade2bae37161457e")]
        [HookAttribute.Dependencies(new System.String[] { "OnImpactEffectCreate" })]
        [HookAttribute.Options(HookFlags.Patch)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("_Patches")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Patches_Effectserver_1e62c22796234b3cade2bae37161457e : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                // Opop
                edit.Add(new CodeInstruction(OpCodes.Ldarg_0));
                edit.Add(new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("HitInfo"), "get_InitiatorPlayer")));
                edit.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("UnityEngine.Object"), "op_Implicit")));
                Label label_3981260037b34e83bf7691302f3ec894 = Generator.DefineLabel();
                original[7].labels.Add(label_3981260037b34e83bf7691302f3ec894);
                edit.Add(new CodeInstruction(OpCodes.Brfalse_S, label_3981260037b34e83bf7691302f3ec894));
                // Opop
                edit.Add(new CodeInstruction(OpCodes.Ldarg_0));
                edit.Add(new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("HitInfo"), "get_InitiatorPlayer")));
                edit.Add(new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("BaseNetworkable"), "get_limitNetworking")));
                edit.Add(new CodeInstruction(OpCodes.Brfalse_S, label_3981260037b34e83bf7691302f3ec894));
                edit.Add(new CodeInstruction(OpCodes.Ret));

                original.InsertRange(7, edit);
                return original.AsEnumerable();
            }
        }
    }
}

public partial class Category_Patches
{
    public partial class Patches_BaseProjectile
    {
        [HookAttribute.Patch("LimitNetworkingNoEffect [patch 2]", "LimitNetworkingNoEffect [patch 2]", "BaseProjectile", "CLProject", ["BaseEntity/RPCMessage"])]
        [HookAttribute.Identifier("6188f04eb0e244c5888523a859556a7e")]
        [HookAttribute.Dependencies(new System.String[] { "OnWeaponFired" })]
        [HookAttribute.Options(HookFlags.Patch)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("_Patches")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Patches_BaseProjectile_6188f04eb0e244c5888523a859556a7e : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                edit.Add(new CodeInstruction(OpCodes.Ldloc_0));
                edit.Add(new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("BaseNetworkable"), "get_limitNetworking")));
                Label label_b74c904249d84d7499a4ca72f5286cc9 = Generator.DefineLabel();
                original[273].labels.Add(label_b74c904249d84d7499a4ca72f5286cc9);
                edit.Add(new CodeInstruction(OpCodes.Brtrue_S, label_b74c904249d84d7499a4ca72f5286cc9));

                original.InsertRange(254, edit);
                return original.AsEnumerable();
            }
        }
    }
}

public partial class Category_Patches
{
    public partial class Patches_BasePlayer
    {
        [HookAttribute.Patch("LimitNetworkingNoEffect [patch 3]", "LimitNetworkingNoEffect [patch 3]", "BasePlayer", "OnAttacked", ["HitInfo"])]
        [HookAttribute.Identifier("39624a8bffec4f32b1fc9a4bc6975c4e")]
        [HookAttribute.Dependencies(new System.String[] { "IOnBasePlayerAttacked" })]
        [HookAttribute.Options(HookFlags.Patch)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("_Patches")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Patches_BasePlayer_39624a8bffec4f32b1fc9a4bc6975c4e : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                edit.Add(new CodeInstruction(OpCodes.Ldloc_S, (sbyte)8));
                edit.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("UnityEngine.Object"), "op_Implicit")));
                Label label_65c1a1830a28431d99d9e205b857d296 = Generator.DefineLabel();
                original[221].labels.Add(label_65c1a1830a28431d99d9e205b857d296);
                edit.Add(new CodeInstruction(OpCodes.Brfalse_S, label_65c1a1830a28431d99d9e205b857d296));
                edit.Add(new CodeInstruction(OpCodes.Ldloc_S, (sbyte)8));
                edit.Add(new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("BaseNetworkable"), "get_limitNetworking")));
                Label label_0392299bda004d528147af07e89e5911 = Generator.DefineLabel();
                original[243].labels.Add(label_0392299bda004d528147af07e89e5911);
                edit.Add(new CodeInstruction(OpCodes.Brtrue_S, label_0392299bda004d528147af07e89e5911));

                original.InsertRange(221, edit);
                return original.AsEnumerable();
            }
        }
    }
}

public partial class Category_Patches
{
    public partial class Patches_AutoTurret
    {
        [HookAttribute.Patch("ContinueTargetScan [patch]", "ContinueTargetScan [patch]", "AutoTurret", "TargetScan", [])]
        [HookAttribute.Identifier("021a039178354faeaf7bea3046cd9b99")]
        [HookAttribute.Options(HookFlags.Patch)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("_Patches")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Patches_AutoTurret_021a039178354faeaf7bea3046cd9b99 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                // Opop
                edit.Add(new CodeInstruction(OpCodes.Ldarg_0));
                edit.Add(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName("AutoTurret"), "target")));
                Label label_ade611830fee48509a74738b38de236a = Generator.DefineLabel();
                original[184].labels.Add(label_ade611830fee48509a74738b38de236a);
                edit.Add(new CodeInstruction(OpCodes.Brfalse_S, label_ade611830fee48509a74738b38de236a));

                original.InsertRange(183, edit);
                return original.AsEnumerable();
            }
        }
    }
}

public partial class Category_Patches
{
    public partial class Patches_RelationshipManager
    {
        [HookAttribute.Patch("LimitNetworkingAcquaintances [patch]", "LimitNetworkingAcquaintances [patch]", "RelationshipManager", "UpdateAcquaintancesFor", ["BasePlayer", "System.Single"])]
        [HookAttribute.Identifier("61474ffd1d4e48eca13a6c1f9cdb1ffc")]
        [HookAttribute.Options(HookFlags.Patch)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("_Patches")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Patches_RelationshipManager_61474ffd1d4e48eca13a6c1f9cdb1ffc : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                edit.Add(new CodeInstruction(OpCodes.Ldloc_3));
                edit.Add(new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("BaseNetworkable"), "get_limitNetworking")));
                Label label_bd841dfe2869492b84c8962b8196af01 = Generator.DefineLabel();
                original[107].labels.Add(label_bd841dfe2869492b84c8962b8196af01);
                edit.Add(new CodeInstruction(OpCodes.Brtrue_S, label_bd841dfe2869492b84c8962b8196af01));

                original.InsertRange(35, edit);
                return original.AsEnumerable();
            }
        }
    }
}

public partial class Category_Patches
{
    public partial class Patches_CH47HelicopterAIController
    {
        [HookAttribute.Patch("AllowNpcNonAdminHeliUse [patch]", "AllowNpcNonAdminHeliUse [patch]", "CH47HelicopterAIController", "AttemptMount", ["BasePlayer", "System.Boolean"])]
        [HookAttribute.Identifier("6117e28727c347d8ae76c0e2f41c53dc")]
        [HookAttribute.Dependencies(new System.String[] { "CanUseHelicopter" })]
        [HookAttribute.Options(HookFlags.Patch)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("_Patches")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Patches_CH47HelicopterAIController_6117e28727c347d8ae76c0e2f41c53dc : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);


                original[7 + 7].MoveLabelsFrom(original[7]);
                original.RemoveRange(7, 7);
                original.InsertRange(7, edit);
                return original.AsEnumerable();
            }
        }
    }
}

public partial class Category_Patches
{
    public partial class Patches_BasePlayerOnFeedbackReportd674
    {
        [HookAttribute.Patch("OnFeedbackReported", "OnFeedbackReported [patch]", "BasePlayer/<OnFeedbackReport>d__674", "MoveNext", [])]
        [HookAttribute.Identifier("53283ffed94c448d9a615bdf0432263e")]
        [HookAttribute.Dependencies(new System.String[] { "OnFeedbackReported" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("_Patches")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Patches_BasePlayerOnFeedbackReportd674_53283ffed94c448d9a615bdf0432263e : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);


                original[11 + 6].MoveLabelsFrom(original[11]);
                original.RemoveRange(11, 6);
                original.InsertRange(11, edit);
                return original.AsEnumerable();
            }
        }
    }
}

public partial class Category_Patches
{
    public partial class Patches_BaseEntity
    {
        [HookAttribute.Patch("LimitNetworkingSignalBroadcast [Patch]", "LimitNetworkingSignalBroadcast [Patch]", "BaseEntity", "SignalBroadcast", ["BaseEntity/Signal", "System.String", "Network.Connection"])]
        [HookAttribute.Identifier("f990352ed8f348f1a12d73bece2414b5")]
        [HookAttribute.Dependencies(new System.String[] { "OnSignalBroadcast" })]
        [HookAttribute.Options(HookFlags.Patch)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("_Patches")]
        [MetadataAttribute.Assembly("Assembly-CSharp.dll")]
        public class Patches_BaseEntity_f990352ed8f348f1a12d73bece2414b5 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                // Opop
                edit.Add(new CodeInstruction(OpCodes.Ldarg_0));
                edit.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("BaseNetworkable"), "get_limitNetworking")));
                Label label_c86b0361b6164ad5a7d3756fd0e61fb3 = Generator.DefineLabel();
                original[8].labels.Add(label_c86b0361b6164ad5a7d3756fd0e61fb3);
                edit.Add(new CodeInstruction(OpCodes.Brtrue_S, label_c86b0361b6164ad5a7d3756fd0e61fb3));

                original.InsertRange(9, edit);
                return original.AsEnumerable();
            }
        }
    }
}

public partial class Category_Patches
{
    public partial class Patches_FacepunchRconListenercDisplayClass300
    {
        [HookAttribute.Patch("OnRconConnection", "OnRconConnection [web, patch]", "Facepunch.Rcon.Listener/<>c__DisplayClass30_0", "<Start>b__0", ["Fleck.IWebSocketConnection"])]
        [HookAttribute.Identifier("407a472ceccc4253b117b3c856d1852f")]
        [HookAttribute.Dependencies(new System.String[] { "OnRconConnection [web]" })]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Return(typeof(System.Object))]
        [MetadataAttribute.Category("_Patches")]
        [MetadataAttribute.Assembly("Facepunch.Rcon.dll")]
        public class Patches_FacepunchRconListenercDisplayClass300_407a472ceccc4253b117b3c856d1852f : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                Label label_2c173debeff24082866e7da8de11b7d7 = Generator.DefineLabel();
                original[69].labels.Add(label_2c173debeff24082866e7da8de11b7d7);
                edit.Add(new CodeInstruction(OpCodes.Bne_Un_S, label_2c173debeff24082866e7da8de11b7d7));

                original[52 + 2].MoveLabelsFrom(original[52]);
                original.RemoveRange(52, 2);
                original.InsertRange(52, edit);
                return original.AsEnumerable();
            }
        }
    }
}

public partial class Category_Patches
{
    public partial class Patches_FacepunchSqliteDatabase
    {
        [HookAttribute.Patch("NoPragmaColumnExists", "NoPragmaColumnExists [patch]", "Facepunch.Sqlite.Database", "ColumnExists", ["System.String", "System.String"])]
        [HookAttribute.Identifier("9015b205ff784647a4ab63afe89dc4d1")]
        [HookAttribute.Options(HookFlags.None)]
        [MetadataAttribute.Category("_Patches")]
        [MetadataAttribute.Assembly("Facepunch.Sqlite.dll")]
        public class Patches_FacepunchSqliteDatabase_9015b205ff784647a4ab63afe89dc4d1 : API.Hooks.Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method)
            {
                List<CodeInstruction> edit = new List<CodeInstruction>();
                List<CodeInstruction> original = new List<CodeInstruction>(Instructions);

                edit.Add(new CodeInstruction(OpCodes.Ldstr, "select count(*) from sqlite_master where tbl_name=? and sql like ?;").MoveLabelsFrom(original[1]));
                edit.Add(new CodeInstruction(OpCodes.Ldarg_1));
                edit.Add(new CodeInstruction(OpCodes.Ldstr, "% "));
                edit.Add(new CodeInstruction(OpCodes.Ldarg_2));
                edit.Add(new CodeInstruction(OpCodes.Ldstr, " %"));
                edit.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName("System.String"), "Concat", new System.Type[] { typeof(System.String), typeof(System.String), typeof(System.String) })));

                original[1 + 3].MoveLabelsFrom(original[1]);
                original.RemoveRange(1, 3);
                original.InsertRange(1, edit);
                return original.AsEnumerable();
            }
        }
    }
}

