using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Carbon.Base;
using HarmonyLib;
using JetBrains.Annotations;
using Network;
using Oxide.Core;
using Oxide.Core.Plugins;

/*
 *
 * Copyright (c) 2023-2024 Patrette, under the GNU v3 license rights
 * Copyright (c) 2023-2024 Carbon Community, under the GNU v3 license rights
 *
 */

namespace Carbon.Modules;

public partial class SelectiveEACModule : CarbonModule<SelectiveEACConfig, EmptyModuleData>
{
	internal static SelectiveEACModule Singleton { get; set; }

	public override string Name => "SelectiveEAC";
	public override VersionNumber Version => new(1, 0, 0);
	public override Type Type => typeof(SelectiveEACModule);
	public override bool ForceModded => false;

	public static readonly int DefaultEncryption = 1;

	public SelectiveEACModule()
	{
		Singleton = this;
	}

	public override void OnServerInit(bool initial)
	{
		base.OnServerInit(initial);

		if (!initial)
		{
			return;
		}

		OnEnabled(true);
	}
	public override void OnEnabled(bool initialized)
	{
		base.OnEnabled(initialized);

		if (!initialized)
		{
			return;
		}

		Permissions.UnregisterPermissions(this);
		Permissions.RegisterPermission(ConfigInstance.UsePermission, this);

		if (!Permissions.GroupExists(ConfigInstance.UseGroup))
		{
			Permissions.CreateGroup(ConfigInstance.UseGroup, "Selective EAC", 0);
		}
	}

	private static bool CanBypass(Connection connection)
	{
		var id = connection.userid.ToString();
		var permissions = Community.Runtime.Core.permission;

		if (!permissions.UserExists(id))
		{
			permissions.GetUserData(id, true);

			if (Community.Runtime.Config.Permissions.AutoGrantPlayerGroup && !string.IsNullOrEmpty(Community.Runtime.Config.Permissions.PlayerDefaultGroup))
			{
				permissions.AddUserGroup(id, Community.Runtime.Config.Permissions.PlayerDefaultGroup);
			}
		}

		return Community.Runtime.Core.permission.UserHasPermission(id, Singleton.ConfigInstance.UsePermission) || Community.Runtime.Core.permission.UserHasGroup(id, Singleton.ConfigInstance.UseGroup);
	}

	private static int UserEncryptionOverride(Network.Server sv, Connection connection)
	{
		try
		{
			if (CanBypass(connection))
			{
				return DefaultEncryption;
			}
		}
		catch (Exception e)
		{
			Logger.Error($"Failed getting UserEncryptionOverride", e);
		}

		return ConVar.Server.encryption;
	}

	#region Patches

	[AutoPatch, UsedImplicitly, HarmonyPatch(typeof(EACServer), nameof(EACServer.OnJoinGame))]
	private class EACServer_OnJoinGame
	{
		[UsedImplicitly]
		private static bool Prefix(Connection connection)
		{
			try
			{
				if (CanBypass(connection))
				{
					EACServer.OnAuthenticatedLocal(connection);
					EACServer.OnAuthenticatedRemote(connection);
					return false;
				}
			}
			catch (Exception e)
			{
				Logger.Error($"EACServer.OnJoinGame CanBypass failure", e);
			}

			return true;
		}
	}

	[AutoPatch, UsedImplicitly, HarmonyPatch(typeof(ServerMgr), nameof(ServerMgr.JoinGame))]
	private class ServerMgr_JoinGame
	{
		[UsedImplicitly]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> op)
		{
			List<CodeInstruction> il = new(op);

			for (int i = 0; i < il.Count; i++)
			{
				var cil = il[i];

				if (cil.opcode == OpCodes.Ldfld && cil.operand is FieldInfo
					{
						Name: "encryption",
						DeclaringType.Name: "Server"
					})
				{
					cil.opcode = OpCodes.Ldarg_1;
					cil.operand = null;
					il.Insert(i + 1, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SelectiveEACModule), nameof(UserEncryptionOverride))));
					i++;
				}
			}

			return il;
		}
	}

	#endregion
}

public class SelectiveEACConfig
{
	public string UsePermission = "selectiveeac.use";
	public string UseGroup = "selectiveeac";
}
