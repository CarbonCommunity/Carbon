#define DEBUG_VERBOSE
#pragma warning disable IDE0051

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using API.Contracts;
using API.Structs;
using HarmonyLib;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using Utility;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Components;


internal sealed class AnalyticsManager : MonoBehaviour, IAnalyticsManager
{
	private static bool _first;
	private static string _location;
	private const string MeasurementID = "G-M7ZBRYS3X7";
	private const string MeasurementSecret = "edBQH3_wRCWxZSzx5Y2IWA";
	public string ClientID { get => _serverInfo.Value.UID; }

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

			_first = true;
			info = new Identity { UID = $"{Guid.NewGuid()}" };
			Utility.Logger.Warn($"A new server identity was generated.");
			File.WriteAllText(_location, JsonConvert.SerializeObject(info, Formatting.Indented));
		}
		catch (Exception e)
		{
			Utility.Logger.Error("Unable to process server identity", e);
		}

		return info;
	});

	public void StartSession()
		=> LogEvent((_first) ? "first_visit" : "session_start", null);

	public void LogEvent(string eventName, IDictionary<string, object> parameters)
		=> StartCoroutine(SendEvent(eventName, parameters));

	private IEnumerator SendEvent(string eventName, IDictionary<string, object> parameters = null)
	{
		string url = "https://www.google-analytics.com/mp/collect";
		string query = $"api_secret={MeasurementSecret}&measurement_id={MeasurementID}";

		Dictionary<string, object> eventData = new()
		{
			{ "client_id", ClientID },
			{ "non_personalized_ads", true },
			{ "timestamp_micros", GetUnixTimestampMicros() }
		};

		eventData.Add("events", value: new List<Dictionary<string, object>> {
			new Dictionary<string, object> {
				{ "name", eventName },
				{ "params", parameters }
			}
		});

		string json = JsonConvert.SerializeObject(eventData);
		byte[] postData = Encoding.UTF8.GetBytes(json);

		using UnityWebRequest request = new UnityWebRequest($"{url}?{query}", "POST");
		request.SetRequestHeader("Content-Type", "application/json");
		request.uploadHandler = new UploadHandlerRaw(postData);
		request.downloadHandler = new DownloadHandlerBuffer();

		yield return request.SendWebRequest();

		if (request.isNetworkError || request.isHttpError)
		{
			Utility.Logger.Warn($"Failed to send event '{eventName}' to Google Analytics: {request.error}");
		}
#if DEBUG_VERBOSE
		else
		{
			Utility.Logger.Debug($"Sent event '{eventName}' to Google Analytics ({request.responseCode})");
			if (request.downloadHandler.text != string.Empty)
				Utility.Logger.Debug($" > {request.downloadHandler.text}");
		}
#endif
	}

	private long GetUnixTimestampMicros()
	{
		DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		long timestampMicros = (long)(DateTime.UtcNow - epochStart).TotalMilliseconds * 1000;
		return timestampMicros;
	}
}