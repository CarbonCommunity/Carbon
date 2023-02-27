using System;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using API.Contracts;
using API.Structs;
using HarmonyLib;
using Newtonsoft.Json;
using UnityEngine;
using Utility;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Components;


internal sealed class IdentityManager : MonoBehaviour, IIdentityManager
{
	private static string _location;

	private static readonly Lazy<Identity> _serverInfo = new(() =>
	{
		Identity info;

		try
		{
			string identity = (string)AccessTools.TypeByName("ConVar.Server")
				?.GetField("identity", BindingFlags.Public | BindingFlags.Static)?.GetValue(null);

			_location = Path.Combine(Context.Game, "server", identity, "carbon.id");

			if (File.Exists(_location))
			{
				string raw = File.ReadAllText(_location);
				info = JsonConvert.DeserializeObject<Identity>(raw);
				if (!_serverInfo.Equals(default(Identity))) return info;
			}

			info = new Identity { UID = Generate() };
			File.WriteAllText(_location, JsonConvert.SerializeObject(info, Formatting.Indented));
		}
		catch (Exception e)
		{
			Utility.Logger.Error("Unable to process server identity", e);
		}

		return info;
	});

	private static string Generate()
	{
		//SteamServer.PublicIp.ToString();
		//CommunityEntity.ServerInstance.net.ID.ToString();

		// string serverid = (string)AccessTools.TypeByName("ConVar.App")
		// 	?.GetField("serverid", BindingFlags.Public | BindingFlags.Static)?.GetValue(null);

		Utility.Logger.Warn($"A new server identity will be generated.");

		string macAddress = NetworkInterface
			.GetAllNetworkInterfaces()
			.Where(nic => nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
			.Select(nic => nic.GetPhysicalAddress().ToString()).FirstOrDefault();

		string address = NetworkInterface
			.GetAllNetworkInterfaces()
			.Where(nic => nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
			.Select(nic => nic.GetIPProperties().UnicastAddresses.ToList())
			.SelectMany(uni => uni).Where(uni => uni.Address.AddressFamily == AddressFamily.InterNetwork)
			.Select(ip => ip.Address.ToString()).FirstOrDefault();

		int port = (int)(AccessTools.TypeByName("ConVar.Server")
			?.GetField("port", BindingFlags.Public | BindingFlags.Static)?.GetValue(null) ?? 0);

		if (macAddress == string.Empty || address == string.Empty || port == 0)
			Utility.Logger.Warn($"Found an empty server property. mac:{macAddress} address:{address} port:{port}");

		string data = $"{macAddress}|{address}:{port}";

		using SHA256 sha256 = SHA256.Create();
		byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
		return BitConverter.ToString(hash).Replace("-", "").ToLower();
	}

	public string GetSystemUID
	{ get => _serverInfo.Value.UID; }
}
