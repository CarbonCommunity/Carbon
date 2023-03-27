using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Carbon.Base;
using Carbon.Extensions;
using ConVar;
using Mysqlx.Expr;
using Network;
using Newtonsoft.Json;
using UnityEngine;
using Net = Network.Net;
using TimeEx = Carbon.Extensions.TimeEx;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Patrette
 * All rights reserved.
 *
 */

namespace Carbon.Modules;

public partial class WhitelistModule : CarbonModule<WhitelistConfig, EmptyModuleData>
{
	internal static WhitelistModule Instance { get; set; }

	public override string Name => "Whitelist";
	public override Type Type => typeof(WhitelistModule);
	public override bool ForceModded => true;

	public override void Init()
	{
		base.Init();

		Instance = this;
	}
	public override void Load()
	{
		base.Load();

		UnregisterPermissions();
		RegisterPermission(ConfigInstance.BypassPermission);
	}
	public override Dictionary<string, Dictionary<string, string>> GetDefaultPhrases()
	{
		return new Dictionary<string, Dictionary<string, string>>
		{
			["en"] = new ()
			{
				["denied"] = "Not whitelisted",
			}
		};
	}

	#region Hooks

	private object CanUserLogin(string name, string id, string ipAddress)
	{
		if (CanBypass(id))
		{
			return null;
		}

		return GetPhrase("denied", id);
	}

	#endregion

	public bool CanBypass(string playerId)
	{
		return HasPermission(playerId.ToString(), ConfigInstance.BypassPermission)
			|| (!string.IsNullOrEmpty(ConfigInstance.BypassGroup) && HasGroup(playerId.ToString(), ConfigInstance.BypassGroup));
	}
}

public class WhitelistConfig
{
	public string BypassPermission = "whitelist.bypass";
	public string BypassGroup = "whitelisted";
}
