using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using API.Analytics;
using HarmonyLib;
using Newtonsoft.Json;
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
	private bool _first;
	private float _lastUpdate;
	private float _lastEngagement;
	private static string _location;

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


			_location = Path.Combine(Context.Game, "server", identity, "carbon.id");

			if (File.Exists(_location))
			{
				string raw = File.ReadAllText(_location);
				info = JsonConvert.DeserializeObject<Identity>(raw);
				if (!_serverInfo.Equals(default(Identity))) return info;
			}

			info = new Identity { UID = $"{Guid.NewGuid()}" };
			Logger.Warn($"A new server identity was generated.");
			File.WriteAllText(_location, JsonConvert.SerializeObject(info, Formatting.Indented));
		}
		catch (Exception e)
		{
			Logger.Error("Unable to process server identity", e);
		}

		return info;
	});

	public void Awake()
	{
		_first = false;
		_lastUpdate = 0;
		_lastEngagement = -1;
		SessionID = Util.GetRandomNumber(10);
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
		=> SendEvent(eventName);

	public void LogEvent(string eventName, IDictionary<string, object> parameters)
		=> SendMPEvent(eventName, parameters);

	public void SendEvent(string eventName)
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

		SendRequest($"{url}?{query}");
	}

	private void SendMPEvent(string eventName, IDictionary<string, object> properties = null)
	{
		float delta = (UnityEngine.Time.realtimeSinceStartup - _lastEngagement) * 1000;
		_lastEngagement = UnityEngine.Time.realtimeSinceStartup;

		string url = "https://www.google-analytics.com/mp/collect";
		string query = $"api_secret={MeasurementSecret}&measurement_id={MeasurementID}";

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

		SendRequest($"{url}?{query}", JsonConvert.SerializeObject(body));
	}

	private void SendRequest(string url, string body = null)
	{
		try
		{
			body ??= string.Empty;

			using WebClient webClient = new WebClient();
			webClient.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);
			webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
			webClient.UploadStringCompleted += UploadStringCompleted;
			webClient.UploadStringAsync(new Uri(url), "POST", body, url);

#if DEBUG_VERBOSE
			Logger.Debug($"Request sent to Google Analytics");
			Logger.Debug($" > {url}");
#endif
		}
#if DEBUG_VERBOSE
		catch (System.Exception e)
		{
			Logger.Warn($"Failed to send request to Google Analytics ({e.Message})");
			Logger.Debug($" > {url}");
		}
#else
		catch (System.Exception) { }
#endif
	}

	private void UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
	{
		WebClient webClient = (WebClient)sender;
		string url = (string)e.UserState;

		try
		{
			if (e.Error != null) throw new Exception(e.Error.Message);
			if (e.Cancelled) throw new Exception("Job was cancelled");
		}
#if DEBUG_VERBOSE
		catch (System.Exception ex)
		{
			Logger.Warn($"Failed to send request to Google Analytics ({ex.Message})");
			Logger.Debug($" > {url}");
		}
#else
		catch (System.Exception) { }
#endif
		finally
		{
			webClient.Dispose();
		}
	}
}