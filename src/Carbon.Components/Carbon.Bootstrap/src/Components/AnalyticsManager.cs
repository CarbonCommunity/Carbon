using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using API.Abstracts;
using API.Analytics;
using Carbon;
using Carbon.Components;
using Carbon.Pooling;
using Facepunch;
using HarmonyLib;
using Newtonsoft.Json;
using Utility;
using Logger = Utility.Logger;

namespace Components;
#pragma warning disable IDE0051

internal sealed class AnalyticsManager : CarbonBehaviour, IAnalyticsManager
{
	private int _sessions;
	private float _lastUpdate;
	private float _lastEngagement;
	private static string _location;

	private string MeasurementQuery;
	private const string MeasurementEntrypoint = "https://www.google-analytics.com/mp/collect";
	private const string MeasurementID = "G-M7ZBRYS3X7";
	private const string MeasurementSecret = "edBQH3_wRCWxZSzx5Y2IWA";

	public bool Enabled => enabled;

	public bool HasNewIdentifier
	{ get; private set; }

	public string Branch
	{ get => _branch.Value; }

	private static readonly Lazy<string> _branch = new(() =>
	{
		return _infoVersion.Value switch
		{
			string s when s.Contains("Debug") => "debug",
			string s when s.Contains("Release") => "release",
			string s when s.Contains("Minimal") => "minimal",
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

	public bool IsMinimalBuild =>
#if MINIMAL
		true;
#else
		false;
#endif

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
			+ $" +https://github.com/CarbonCommunity/Carbon)";
	});

	public string Version
	{ get => _version.Value; }

	private static readonly Lazy<string> _version = new(() =>
	{
		return AccessTools.TypeByName("Carbon.Community")
			.Assembly.GetName().Version.ToString();
	});

	public string ClientID
	{ get => _serverInfo.Value.UID; }

	private static readonly Lazy<Identity> _serverInfo = new(() =>
	{
		Identity info = default;

		try
		{
			_location = Path.Combine(Context.Game, "server", ConVar.Server.identity, "carbon.id");

			if (File.Exists(_location))
			{
				string raw = File.ReadAllText(_location);
				info = JsonConvert.DeserializeObject<Identity>(raw);
				if (!_serverInfo.Equals(default(Identity))) return info;
			}

			Carbon.Bootstrap.Analytics.HasNewIdentifier = true;
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

	public string SessionID
	{ get; private set; }

	public string SystemID
	{ get => UnityEngine.SystemInfo.deviceUniqueIdentifier; }

	public WebClient Client { get; set; }

	public Dictionary<string, object> Segments { get; set; }

	public void Awake()
	{
		HasNewIdentifier = false;
		_lastUpdate = 0;
		_lastEngagement = float.MinValue;
		SessionID = Util.GetRandomNumber(10);

		Config.Init();

		if (!Config.Singleton.Analytics.Enabled)
		{
			Logger.Warn("You have opted out from analytics data collection");
			enabled = false;
		}
		else
		{
			Logger.Warn("We use Google Analytics to collect basic data about Carbon such as Carbon version, platform, branch and plug-in count.");
			Logger.Warn("We have no access to any personal identifiable data such as steamids, server name, ip:port, title or description.");
			Logger.Warn("If you'd like to opt-out, disable it in the 'config.json' file.");
		}

		Segments = new Dictionary<string, object> {
			{ "branch", Branch },
			{ "platform", Platform },
		};

		MeasurementQuery = $"{MeasurementEntrypoint}?api_secret={MeasurementSecret}&measurement_id={MeasurementID}";
	}

	private void Update()
	{
		_lastUpdate += UnityEngine.Time.deltaTime;
		if (_lastUpdate < 300) return;

		_lastUpdate = 0;
		LogEvent("user_engagement");
	}

	public void SessionStart()
		=> LogEvent(HasNewIdentifier ? "first_visit" : "user_engagement");

	public void LogEvent(string eventName)
		=> SendEvent(eventName);

	public void LogEvents(string eventName)
		=> SendMPEvent(eventName);

	public void SendEvent(string eventName)
	{
		if (!enabled)
		{
			return;
		}

		float delta = Math.Min(Math.Max(UnityEngine.Time.realtimeSinceStartup - _lastEngagement, 0f), float.MaxValue);
		_lastEngagement = UnityEngine.Time.realtimeSinceStartup;

		const string url = "https://www.google-analytics.com/g/collect";
		string query = $"v=2&tid={MeasurementID}&cid={ClientID}&en={eventName}";

		if (delta >= 1800f)
		{
			if (delta == float.MaxValue)
			{
				delta = 0;
			}

			SessionID = Util.GetRandomNumber(10);
			query += $"&_ss=1";
			_sessions++;
		}

		query += $"&seg=1&_et={Math.Round(delta * 1000f)}&sid={SessionID}&sct={_sessions}";

#if DEBUG_VERBOSE
		query += "&_dbg=1";
#endif

		SendRequest($"{url}?{query}&_z={Util.GetRandomNumber(10)}");
	}

	private void SendMPEvent(string eventName)
	{
		if (!enabled)
		{
			return;
		}

		var delta = Math.Min(Math.Max( UnityEngine.Time.realtimeSinceStartup - _lastEngagement, 0f), 1800f);
		_lastEngagement = UnityEngine.Time.realtimeSinceStartup;

		var segment_cache = Pool.Get<List<Dictionary<string, object>>>();
		var user_properties = Pool.Get<Dictionary<string, object>>();
		var event_parameters = Pool.Get<Dictionary<string, object>>();
		var body = Pool.Get<Dictionary<string, object>>();
		var events = Pool.Get<List<Dictionary<string, object>>>();
		var events_entry = Pool.Get<Dictionary<string, object>>();

#if DEBUG_VERBOSE
		event_parameters["debug_mode"] = 1;
#endif
		event_parameters["session_id"] = SessionID;
		event_parameters["engagement_time_msec"] = Math.Round(delta * 1000f);

		body["client_id"] = ClientID;
		body["non_personalized_ads"] = true;

		if (Analytics.Metrics != null)
		{
			foreach (var metric in Analytics.Metrics)
				event_parameters.Add(metric.Key, metric.Value);
		}

		events_entry["name"] = eventName;
		events_entry["params"] = event_parameters;
		events.Add(events_entry);

		body.Add("events", value: events);

		if (Segments != null)
		{
			foreach (var segment in Segments)
			{
				var tempDictionary = Pool.Get<Dictionary<string, object>>();
				tempDictionary["value"] = segment.Value;

				user_properties[segment.Key] = tempDictionary;

				segment_cache.Add(tempDictionary);
			}
			body.Add("user_properties", user_properties);
		}

		SendRequest(MeasurementQuery, JsonConvert.SerializeObject(body));

		foreach (var cache in segment_cache)
		{
			var cacheInstance = cache;
			Pool.FreeUnmanaged(ref cacheInstance);
		}

		Pool.FreeUnmanaged(ref events);
		Pool.FreeUnmanaged(ref segment_cache);
		Pool.FreeUnmanaged(ref events_entry);
		Pool.FreeUnmanaged(ref user_properties);
		Pool.FreeUnmanaged(ref event_parameters);
		Pool.FreeUnmanaged(ref body);
	}

	private void SendRequest(string url, string body = null)
	{
		try
		{
			body ??= string.Empty;

			if (Client == null)
			{
				Client = new();
				Client.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);
				Client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
			}

			Client.UploadStringAsync(new Uri(url), "POST", body, url);
		}
		catch (System.Exception) { }
	}
}
