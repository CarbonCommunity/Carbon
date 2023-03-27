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

	internal List<Connection> _passwordAuthed { get; } = new();
	internal List<Connection> _waitingList = new();

	public override void Init()
	{
		base.Init();

		Instance = this;
	}
	public override void Load()
	{
		base.Load();

		RegisterPermission(ConfigInstance.BypassPermission);
	}
	public override Dictionary<string, Dictionary<string, string>> GetDefaultPhrases()
	{
		return new Dictionary<string, Dictionary<string, string>>
		{
			["en"] = new ()
			{
				["title"] = "Pending password",
				["subtitle"] = "Type in F1 | Timeout: {0}",
				["enter_password"] = "Enter password:",
				["access_denied"] = "Access denied.",
				["access_granted"] = "Access granted.",
				["timed_out"] = "Password timeout.",
				["loading_in"] = "Loading in..."
			}
		};
	}

	#region Custom Hooks

	private void IOnNetworkMessage() { }
	private void IAuthorisationRoutine() { }

	#endregion

	[CommandVar("whitelist.password", Protected = true)]
	private string Password { get { return ConfigInstance.Password; } set { if (value.Length > 32) { PutsWarn($"The password must not be more than 32 characters."); return; } ConfigInstance.Password = value; Save(); } }

	public IEnumerator Run(Connection connection)
	{
		if (Instance._passwordAuthed.Contains(connection))
		{
			connection.authStatus = "ok";
			yield break;
		}

		connection.authStatus = "";

		ConsoleNetwork.SendClientCommand(connection, $"echo {GetPhrase("enter_password", connection.userid)}");

		_waitingList.Add(connection);

		var timeout = Stopwatch.StartNew();
		var s = ConfigInstance.Timeout;

		while (timeout.Elapsed.TotalSeconds < ConfigInstance.Timeout && connection.active && connection.authStatus == "")
		{
			if (s != timeout.Elapsed.Seconds)
			{
				DisplayLoadingMessage(connection, GetPhrase("title", connection.userid), string.Format(GetPhrase("subtitle", connection.userid), TimeEx.Format(s).ToLower()));
			}

			var net = new Steamworks.ServerList.Internet();
			net.RunQueryAsync();

			s = ConfigInstance.Timeout - timeout.Elapsed.Seconds;
			yield return null;
		}

		_waitingList.Remove(connection);

		if (connection.active)
		{
			var publicMessage = string.Empty;
			var secretMessage = string.Empty;

			switch (connection.authStatus)
			{
				case string str when str.StartsWith("wrong_password="):
					publicMessage = GetPhrase("access_denied", connection.userid);
					secretMessage = $"wrong password: {connection.authStatus.Substring("wrong_password=".Length)}";
					break;

				case "":
					publicMessage = GetPhrase("password_timeout", connection.userid);
					secretMessage = "password timeout";
					break;

				case not "ok":
					publicMessage = GetPhrase("access_denied", connection.userid);
					secretMessage = "Server password failure {connection.authStatus}";
					break;

				default:
					ConsoleNetwork.SendClientCommand(connection, $"echo {GetPhrase("access_granted", connection.userid)}");
					DisplayLoadingMessage(connection, GetPhrase("access_granted", connection.userid), GetPhrase("loading_in", connection.userid));
					Instance.PutsWarn($"{connection}: {GetPhrase("access_granted")}");
					Instance._passwordAuthed.Add(connection);
					yield break;
			}

			ConsoleNetwork.SendClientCommand(connection, $"echo {publicMessage}");
			Community.Runtime.CorePlugin.NextFrame(() => ConnectionAuth.Reject(connection, publicMessage, secretMessage));
		}
	}
	public void OnPacket(Message packet)
	{
		if (IsPasswordEnabled())
		{
			var password = packet.read.StringRaw(32);

			if (string.IsNullOrEmpty(password)) return;

			if (password.EndsWith(" True"))
			{
				password = password.Substring(0, password.Length - " True".Length);
			}

			if (password != Password)
			{
				packet.connection.authStatus = "wrong_password=" + password;
				return;
			}

			packet.connection.authStatus = "ok";
		}
	}

	public bool CanBypass(string playerId)
	{
		return HasPermission(playerId.ToString(), ConfigInstance.BypassPermission);
	}
	public bool IsPasswordEnabled()
	{
		return !string.IsNullOrEmpty(Password);
	}

	#region Helpers

	internal static void DisplayLoadingMessage(Connection c, string top = null, string bottom = null)
	{
		try
		{
			if (top == null && bottom == null)
			{
				return;
			}

			var nw = Net.sv.StartWrite();
			nw.PacketID(Message.Type.Message);

			if (top != null)
			{
				nw.String(top);
			}

			if (bottom != null)
			{
				nw.String(bottom);
			}

			nw.Send(new SendInfo(c));
		}
		catch { }
	}

	#endregion
}

public class WhitelistConfig
{
	public string Password = "";
	public int Timeout = 30;
	public string BypassPermission = "whitelist.bypass";
}
