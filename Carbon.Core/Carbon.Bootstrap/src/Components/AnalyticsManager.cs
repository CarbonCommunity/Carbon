using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using API.Contracts;
using API.Structs;
using HarmonyLib;
using Newtonsoft.Json;
using UnityEngine.Networking;
using Utility;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Components;
#pragma warning disable IDE0051

internal sealed class AnalyticsManager : UnityEngine.MonoBehaviour, IAnalyticsManager
{
	private static bool _first;
	private static string _location;
	private static float _engagement;
	private const string MeasurementID = "G-M7ZBRYS3X7";
	private const string MeasurementSecret = "edBQH3_wRCWxZSzx5Y2IWA";
	public string Platform
	{ get; private set; }

	public string Branch
	{ get; private set; }

	public string Version
	{ get; private set; }

	public string InformationalVersion
	{ get; private set; }

	public string SessionID
	{ get; private set; }

	public string ClientID
	{ get => _serverInfo.Value.UID; }

	private static readonly Lazy<Identity> _serverInfo = new(() =>
	{
		Identity info;

		try
		{
			string identity = (string)AccessTools.TypeByName("ConVar.Server")
				?.GetField("identity", BindingFlags.Public | BindingFlags.Static)?.GetValue(null);

			_first = false;
			_location = Path.Combine(Context.Game, "server", identity, "carbon.id");

			if (File.Exists(_location))
			{
				string raw = File.ReadAllText(_location);
				info = JsonConvert.DeserializeObject<Identity>(raw);
				if (!_serverInfo.Equals(default(Identity))) return info;
			}

			_first = true;
			info = new Identity { UID = $"{Guid.NewGuid()}" };
			Logger.Warn($"A new server identity was generated.");
			File.WriteAllText(_location, JsonConvert.SerializeObject(info, Formatting.Indented));
		}
		catch (Exception e)
		{
			_first = false;
			Logger.Error("Unable to process server identity", e);
		}

		return info;
	});

	public AnalyticsManager()
	{
		InformationalVersion = Assembly.GetExecutingAssembly()
			.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
		Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

		Platform = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) switch
		{
			true => "windows",
			false => "linux"
		};

		Branch = InformationalVersion switch
		{
			string s when s.Contains("Debug") => "debug",
			string s when s.Contains("Staging") => "staging",
			string s when s.Contains("Release") => "release",
			_ => "Unknown"
		};

		SessionID = Util.GetRandomNumber(10);
		_engagement = -1;
	}

	public void StartSession()
		=> LogEvent(_first ? "first_visit" : "user_engagement");

	public void Keepalive()
		=> LogEvent("user_engagement");

	public void LogEvent(string eventName)
		=> StartCoroutine(SendEvent(eventName));

	public void LogEvent(string eventName, IDictionary<string, object> parameters)
		=> StartCoroutine(SendMPEvent(eventName, parameters));

	public IEnumerator SendEvent(string eventName)
	{
		float delta = 1; bool newsession = true;
		if (_engagement >= 0 && _engagement <= 1800)
		{
			newsession = false;
			delta = (UnityEngine.Time.realtimeSinceStartup - _engagement) * 1000;
		}
		_engagement = UnityEngine.Time.realtimeSinceStartup;

		string url = "https://www.google-analytics.com/g/collect";
		string query = $"v=2&tid={MeasurementID}&cid={ClientID}&en={eventName}"
			+ $"{(newsession ? "&_ss=1" : string.Empty)}&seg=1&_et={Math.Round(delta)}";

#if DEBUG_VERBOSE
		query += "&_dbg=1";
#endif

		using UnityWebRequest request = new UnityWebRequest($"{url}?{query}", "POST");
		request.SetRequestHeader("User-Agent", $"carbon/server ({Platform}; x64; {Branch}) carbon/{Version}");
		request.SetRequestHeader("Content-Type", "application/json");
		request.uploadHandler = new UploadHandlerRaw(default);
		request.downloadHandler = new DownloadHandlerBuffer();

		yield return request.SendWebRequest();

#if DEBUG_VERBOSE
		if (request.isNetworkError || request.isHttpError)
		{
			Logger.Warn($"Failed to event '{eventName}' to Google Analytics: {request.error}");
			Logger.Debug($" > {url}?{query}");
		}
		else
		{
			Logger.Debug($"Sent event '{eventName}' to Google Analytics ({request.responseCode})");
			Logger.Debug($" > {url}?{query}");
		}
#endif
	}

	private IEnumerator SendMPEvent(string eventName, IDictionary<string, object> properties = null)
	{
		float delta = (UnityEngine.Time.realtimeSinceStartup - _engagement) * 1000;
		_engagement = UnityEngine.Time.realtimeSinceStartup;

		string url = "https://www.google-analytics.com/mp/collect";
		string query = $"api_secret={MeasurementSecret}&measurement_id={MeasurementID}";

		using UnityWebRequest request = new UnityWebRequest($"{url}?{query}", "POST");
		request.SetRequestHeader("User-Agent", $"carbon/server ({Platform}; x64; {Branch}) carbon/{Version}");
		request.SetRequestHeader("Content-Type", "application/json");
		request.downloadHandler = new DownloadHandlerBuffer();

		Dictionary<string, object> parameters = new() {
#if DEBUG_VERBOSE
			{ "debug_mode", 1 },
#endif
			{ "session_id", SessionID },
			{ "engagement_time_msec", Math.Round(delta) },
		};

		Dictionary<string, object> body = new Dictionary<string, object>
		{
			{ "client_id", ClientID },
			{ "non_personalized_ads", true },
		};

		body.Add("events", value: new List<Dictionary<string, object>> {
			new Dictionary<string, object> {
				{ "name", eventName },
				{ "params", parameters }
			}
		});

		if (properties != null)
		{
			Dictionary<string, object> user_properties = new();

			foreach (var property in properties)
			{
				user_properties.Add(property.Key, new Dictionary<string, object> {
					{"value", property.Value}
				});
			}
			body.Add("user_properties", user_properties);
		}

		string json = JsonConvert.SerializeObject(body);
		request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
		yield return request.SendWebRequest();

#if DEBUG_VERBOSE
		if (request.isNetworkError || request.isHttpError)
		{
			Logger.Warn($"Failed to event '{eventName}' to Google Analytics: {request.error}");
			Logger.Debug($" > {url}?{query}");
		}

		else
		{
			Logger.Debug($"Sent event '{eventName}' to Google Analytics ({request.responseCode})");
			Logger.Debug($" > {url}?{query}");
		}
#endif
	}
}