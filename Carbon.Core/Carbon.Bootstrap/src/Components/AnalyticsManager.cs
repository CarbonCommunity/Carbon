using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using API.Analytics;
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
	private static float _lastUpdate;
	private static float _lastEngagement;
	private const string MeasurementID = "G-M7ZBRYS3X7";
	private const string MeasurementSecret = "edBQH3_wRCWxZSzx5Y2IWA";


	public string Branch
	{ get => _branch.Value; }

	private static readonly Lazy<string> _branch = new(() =>
	{
		return _infoVersion.Value switch
		{
			string s when s.Contains("Debug") => "debug",
			string s when s.Contains("Staging") => "staging",
			string s when s.Contains("Release") => "release",
			_ => "Unknown"
		};
	});

	public string InformationalVersion
	{ get => _infoVersion.Value; }

	private static readonly Lazy<string> _infoVersion = new(() =>
	{
		return AccessTools.TypeByName("Carbon.Community")
			.Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
	});

	public string Platform
	{ get => _platform.Value; }

	private static readonly Lazy<string> _platform = new(() =>
	{
		return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) switch
		{
			true => "windows",
			false => "linux"
		};
	});

	public string Protocol
	{ get => _protocol.Value; }

	private static readonly Lazy<string> _protocol = new(() =>
	{
		return AccessTools.TypeByName("Carbon.Community")
			.Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
	});

	public string UserAgent
	{ get => _userAgent.Value; }

	private static readonly Lazy<string> _userAgent = new(() =>
	{
		return $"carbon/{_version.Value} ({_platform.Value}; x64; {_branch.Value};"
			+ $" +https://github.com/CarbonCommunity/Carbon.Core)";
	});

	public string Version
	{ get => _version.Value; }

	private static readonly Lazy<string> _version = new(() =>
	{
		return AccessTools.TypeByName("Carbon.Community")
			.Assembly.GetName().Version.ToString();
	});


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

	public void Awake()
	{
		SessionID = Util.GetRandomNumber(10);
		_lastEngagement = -1;
		_lastUpdate = 0;
	}

	private void Update()
	{
		_lastUpdate += UnityEngine.Time.deltaTime;
		if (_lastUpdate < 300) return;

		LogEvent("user_engagement");
		_lastUpdate = 0;
	}

	public void StartSession()
		=> LogEvent(_first ? "first_visit" : "user_engagement");

	public void LogEvent(string eventName)
		=> StartCoroutine(SendEvent(eventName));

	public void LogEvent(string eventName, IDictionary<string, object> parameters)
		=> StartCoroutine(SendMPEvent(eventName, parameters));

	public IEnumerator SendEvent(string eventName)
	{
		float delta = 1; bool newsession = true;
		if (_lastEngagement >= 0 && _lastEngagement <= 1800)
		{
			newsession = false;
			delta = (UnityEngine.Time.realtimeSinceStartup - _lastEngagement) * 1000;
		}
		_lastEngagement = UnityEngine.Time.realtimeSinceStartup;

		string url = "https://www.google-analytics.com/g/collect";
		string query = $"v=2&tid={MeasurementID}&cid={ClientID}&en={eventName}"
			+ $"{(newsession ? "&_ss=1" : string.Empty)}&seg=1&_et={Math.Round(delta)}";

#if DEBUG_VERBOSE
		query += "&_dbg=1";
#endif

		using UnityWebRequest request = new UnityWebRequest($"{url}?{query}", "POST");
		request.SetRequestHeader("User-Agent", UserAgent);
		request.SetRequestHeader("Content-Type", "application/json");
		request.uploadHandler = new UploadHandlerRaw((byte[])default);
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
		float delta = (UnityEngine.Time.realtimeSinceStartup - _lastEngagement) * 1000;
		_lastEngagement = UnityEngine.Time.realtimeSinceStartup;

		string url = "https://www.google-analytics.com/mp/collect";
		string query = $"api_secret={MeasurementSecret}&measurement_id={MeasurementID}";

		using UnityWebRequest request = new UnityWebRequest($"{url}?{query}", "POST");
		request.SetRequestHeader("User-Agent", UserAgent);
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
