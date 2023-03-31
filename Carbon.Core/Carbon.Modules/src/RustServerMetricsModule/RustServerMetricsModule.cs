using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Carbon.Base;
using Carbon.Components;
using Network;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Modules;
#pragma warning disable IDE0051

public partial class RustServerMetricsModule : CarbonModule<RustServerMetricsConfig, EmptyModuleData>
{
	public override string Name => "RustServerMetrics";
	public override Type Type => typeof(RustServerMetricsModule);
	public override bool ForceModded => true;
	public override bool Disabled => true;

	public override bool EnabledByDefault => false;

	internal static RustServerMetricsModule Singleton { get; set; }

	public override void Init()
	{
		base.Init();

		Singleton = this;
	}

	[ConsoleCommand("servermetrics.status")]
	private void PrintStatus(ConsoleSystem.Arg args)
	{
		var metrics = MetricsLogger.Instance;

		using var builder = new StringBody();
		builder.Add("[ServerMetrics]: Status");
		builder.Add("Overview");
		builder.Add("\tReady: ");
		builder.Add(metrics.Ready);
		builder.Add(null);
		builder.Add("Report Uploader:");
		builder.Add("\tRunning: ");
		builder.Add(metrics._reportUploader.IsRunning);
		builder.Add(null);
		builder.Add("\tIn Buffer: ");
		builder.Add(metrics._reportUploader.BufferSize);
		builder.Add(null);
		args.ReplyWith(builder.ToString());

	}

	public override void OnEnabled(bool initialized)
	{
		base.OnEnabled(initialized);

		MetricsLogger.Initialize();
	}
	public override void OnDisabled(bool initialized)
	{
		base.OnDisabled(initialized);

		MetricsLogger.Uninitialize();
	}

	#region Hooks

	private void OnPlayerConnected(BasePlayer player)
	{
		MetricsLogger.Instance?.OnPlayerInit(player);
	}
	private void OnPlayerDisconnected(BasePlayer player)
	{
		MetricsLogger.Instance?.OnPlayerDisconnected(player);
	}

	#endregion

	#region Custom Hooks

	private void IPostNetWriteSend() { }
	private void IPostNetWritePacketID() { }
	private void IOnPlayerPerformanceReport() { }
	private void IPostPerformanceTimer() { }

	#endregion

	public class MetricsLogger : SingletonComponent<MetricsLogger>
	{
		public bool Ready { get; private set; }

		internal RustServerMetricsConfig Configuration => Singleton.ConfigInstance;

		private Uri _baseUri;
		internal readonly HashSet<string> _requestedClientPerf = new HashSet<string>(1000);
		private readonly int _performanceReport_RequestId = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
		internal ReportUploader _reportUploader;
		private Message.Type _lastMessageType;
		private bool _firstReportGenerated;
		private static GameObject _go;
		private static MetricsLogger _logger;

		private static readonly Regex PLUGIN_NAME_REGEX = new Regex("_|[^\\w\\d]");
		private readonly StringBuilder _stringBuilder = new StringBuilder();
		private readonly Dictionary<ulong, Action> _playerStatsActions = new Dictionary<ulong, Action>();
		private readonly Dictionary<ulong, uint> _perfReportDelayCounter = new Dictionary<ulong, uint>();
		private readonly IReadOnlyDictionary<Message.Type, MetricsLogger.NetworkUpdateData> _networkUpdates = Enum.GetValues(typeof(Message.Type)).Cast<Message.Type>().Distinct<Message.Type>().ToDictionary((Message.Type x) => x, (Message.Type z) => new MetricsLogger.NetworkUpdateData(0, 0L));
		public readonly MetricsTimeStorage<MethodInfo> ServerInvokes = new MetricsTimeStorage<MethodInfo>("invoke_execution", new Action<StringBuilder, MethodInfo>(MetricsLogger.LogMethodInfo));
		public readonly MetricsTimeStorage<string> ServerRpcCalls = new MetricsTimeStorage<string>("rpc_calls", new Action<StringBuilder, string>(MetricsLogger.LogMethodName));
		public readonly MetricsTimeStorage<string> WorkQueueTimes = new MetricsTimeStorage<string>("work_queue", new Action<StringBuilder, string>(MetricsLogger.LogMethodName));
		public readonly MetricsTimeStorage<string> ServerUpdate = new MetricsTimeStorage<string>("server_update", new Action<StringBuilder, string>(MetricsLogger.LogMethodName));
		public readonly MetricsTimeStorage<string> TimeWarnings = new MetricsTimeStorage<string>("timewarnings", new Action<StringBuilder, string>(MetricsLogger.LogMethodName));
		public readonly MetricsTimeStorage<string> ServerConsoleCommands = new MetricsTimeStorage<string>("console_commands", delegate (StringBuilder builder, string command)
		{
			builder.Append(",command=\"");
			builder.Append(command);
		});


		public Uri BaseUri
		{
			get
			{
				if (_baseUri != null)
				{
					return _baseUri;
				}
				Uri uri = new Uri(Configuration.databaseUrl);
				_baseUri = new Uri(uri, string.Concat(new string[]
				{
					"/write?db=",
					Configuration.databaseName,
					"&precision=ms&u=",
					Configuration.databaseUser,
					"&p=",
					Configuration.databasePassword
				}));
				return _baseUri;
			}
		}

		internal static void Initialize()
		{
			_go = new GameObject();
			_logger = _go.AddComponent<MetricsLogger>();
		}
		internal static void Uninitialize()
		{
			DestroyImmediate(_logger);
			Destroy(_go);
		}

		protected override void Awake()
		{
			base.Awake();
			_reportUploader = base.gameObject.AddComponent<ReportUploader>();
			if (ValidateConfiguration())
			{
				if (!Configuration.enabled)
				{
					Debug.LogWarning("[ServerMetrics]: Metrics gathering has been disabled in the configuration");
					return;
				}
				StartLoggingMetrics();
				Ready = true;
			}
		}

		public void StartLoggingMetrics()
		{
			base.InvokeRepeating(new Action(LogNetworkUpdates), UnityEngine.Random.Range(0.25f, 0.75f), 0.5f);
			base.InvokeRepeating(new Action(ServerInvokes.SerializeToStringBuilder), UnityEngine.Random.Range(0f, 1f), 1f);
			base.InvokeRepeating(new Action(ServerRpcCalls.SerializeToStringBuilder), UnityEngine.Random.Range(0f, 1f), 1f);
			base.InvokeRepeating(new Action(ServerConsoleCommands.SerializeToStringBuilder), UnityEngine.Random.Range(0f, 1f), 1f);
			base.InvokeRepeating(new Action(WorkQueueTimes.SerializeToStringBuilder), UnityEngine.Random.Range(0f, 1f), 1f);
			base.InvokeRepeating(new Action(ServerUpdate.SerializeToStringBuilder), UnityEngine.Random.Range(0f, 1f), 1f);
			base.InvokeRepeating(new Action(TimeWarnings.SerializeToStringBuilder), UnityEngine.Random.Range(0f, 1f), 1f);
		}

		internal void OnPlayerInit(BasePlayer player)
		{
			if (!Ready)
			{
				return;
			}
			if (!Configuration.gatherPlayerMetrics)
			{
				return;
			}
			Action action = delegate ()
			{
				GatherPlayerSecondStats(player);
			};
			Action existingAction;
			if (_playerStatsActions.TryGetValue(player.userID, out existingAction))
			{
				player.CancelInvoke(existingAction);
			}
			_playerStatsActions[player.userID] = action;
			player.InvokeRepeating(action, UnityEngine.Random.Range(0.5f, 1.5f), 1f);
		}
		internal void OnPlayerDisconnected(BasePlayer player)
		{
			if (!Ready)
			{
				return;
			}
			if (!Configuration.gatherPlayerMetrics)
			{
				return;
			}
			Action action;
			if (_playerStatsActions.TryGetValue(player.userID, out action))
			{
				player.CancelInvoke(action);
			}
			_playerStatsActions.Remove(player.userID);
		}
		internal void OnNetWritePacketID(Message.Type messageType)
		{
			if (!Ready)
			{
				return;
			}
			_lastMessageType = messageType;
		}
		internal void OnNetWriteSend(NetWrite write, SendInfo sendInfo)
		{
			if (!Ready)
			{
				return;
			}
			MetricsLogger.NetworkUpdateData data = _networkUpdates[_lastMessageType];
			if (sendInfo.connection != null)
			{
				data.count++;
				data.bytes += write.Position;
				return;
			}
			if (sendInfo.connections != null)
			{
				data.count += sendInfo.connections.Count;
				data.bytes += write.Position * (long)data.count;
			}
		}
		internal void OnOxidePluginMetrics(Dictionary<string, double> metrics)
		{
			if (!Ready)
			{
				return;
			}
			if (metrics.Count < 1)
			{
				return;
			}
			foreach (KeyValuePair<string, double> metric in metrics)
			{
				UploadPacket<KeyValuePair<string, double>>("oxide_plugins", metric, delegate (StringBuilder builder, KeyValuePair<string, double> report)
				{
					builder.Append(",plugin=\"");
					builder.Append(MetricsLogger.PLUGIN_NAME_REGEX.Replace(report.Key, string.Empty));
					builder.Append("\" hookTime=");
					builder.Append(report.Value);
				});
			}
		}
		internal bool OnClientPerformanceReport(ClientPerformanceReport clientPerformanceReport)
		{
			if (clientPerformanceReport.request_id != _performanceReport_RequestId)
			{
				return false;
			}
			UploadPacket<ClientPerformanceReport>("client_performance", clientPerformanceReport, delegate (StringBuilder builder, ClientPerformanceReport report)
			{
				builder.Append(",steamid=");
				builder.Append(report.user_id);
				builder.Append(" memory=");
				builder.Append(report.memory_system);
				builder.Append("i,fps=");
				builder.Append(report.fps);
			});
			_requestedClientPerf.Remove(clientPerformanceReport.user_id);
			return true;
		}
		private void GatherPlayerSecondStats(BasePlayer player)
		{
			if (!player.IsReceivingSnapshot)
			{
				uint perfReportCounter;
				_perfReportDelayCounter.TryGetValue(player.userID, out perfReportCounter);
				if (perfReportCounter < 4U)
				{
					_perfReportDelayCounter[player.userID] = perfReportCounter + 1U;
				}
				else
				{
					_perfReportDelayCounter[player.userID] = 0U;
					player.ClientRPCPlayer<string, int>(null, player, "GetPerformanceReport", "legacy", _performanceReport_RequestId);
				}
			}
			UploadPacket<BasePlayer>("connection_latency", player, delegate (StringBuilder builder, BasePlayer basePlayer)
			{
				string ip = basePlayer.net.connection.ipaddress;
				builder.Append(",steamid=");
				builder.Append(basePlayer.UserIDString);
				builder.Append(",ip=");
				builder.Append(ip.Substring(0, ip.LastIndexOf(':')));
				builder.Append(" ping=");
				builder.Append(Net.sv.GetAveragePing(basePlayer.net.connection));
				builder.Append("i,packet_loss=");
				builder.Append(Net.sv.GetStat(basePlayer.net.connection, BaseNetwork.StatTypeLong.PacketLossLastSecond));
				builder.Append("i ");
			});
		}
		private void LogNetworkUpdates()
		{
			if (_networkUpdates.Count < 1)
			{
				return;
			}
			string epochNow = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
			_stringBuilder.Clear();
			_stringBuilder.Append("network_updates,server=");
			_stringBuilder.Append(Configuration.serverTag);
			_stringBuilder.Append(" ");
			IEnumerator<KeyValuePair<Message.Type, MetricsLogger.NetworkUpdateData>> enumerator = _networkUpdates.GetEnumerator();
			if (enumerator.MoveNext())
			{
				KeyValuePair<Message.Type, MetricsLogger.NetworkUpdateData> networkUpdate = enumerator.Current;
				string key = networkUpdate.Key.ToString();
				MetricsLogger.NetworkUpdateData value = networkUpdate.Value;
				_stringBuilder.Append(key);
				_stringBuilder.Append("=");
				_stringBuilder.Append(value.count.ToString());
				_stringBuilder.Append("i");
				value.count = 0;
				_stringBuilder.Append(",");
				_stringBuilder.Append(key);
				_stringBuilder.Append("_bytes");
				_stringBuilder.Append("=");
				_stringBuilder.Append(value.bytes.ToString());
				_stringBuilder.Append("i");
				value.bytes = 0L;
				while (enumerator.MoveNext())
				{
					networkUpdate = enumerator.Current;
					key = networkUpdate.Key.ToString();
					value = networkUpdate.Value;
					_stringBuilder.Append(",");
					_stringBuilder.Append(key);
					_stringBuilder.Append("=");
					_stringBuilder.Append(value.count.ToString());
					_stringBuilder.Append("i");
					value.count = 0;
					_stringBuilder.Append(",");
					_stringBuilder.Append(key);
					_stringBuilder.Append("_bytes");
					_stringBuilder.Append("=");
					_stringBuilder.Append(value.bytes.ToString());
					_stringBuilder.Append("i");
					value.bytes = 0L;
				}
			}
			_stringBuilder.Append(" ");
			_stringBuilder.Append(epochNow);
			_reportUploader.AddToSendBuffer(_stringBuilder.ToString());
		}
		internal void OnPerformanceReportGenerated()
		{
			if (!Ready)
			{
				return;
			}
			if (!_firstReportGenerated)
			{
				_firstReportGenerated = true;
				return;
			}
			Performance.Tick current = Performance.current;
			string epochNow = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
			_stringBuilder.Clear();
			_stringBuilder.Append("framerate,server=");
			_stringBuilder.Append(Configuration.serverTag);
			_stringBuilder.Append(" instant=");
			_stringBuilder.Append(current.frameRate);
			_stringBuilder.Append(",average=");
			_stringBuilder.Append(current.frameRateAverage);
			_stringBuilder.Append(" ");
			_stringBuilder.Append(epochNow);
			_stringBuilder.Append("\n");
			_stringBuilder.Append("frametime,server=");
			_stringBuilder.Append(Configuration.serverTag);
			_stringBuilder.Append(" instant=");
			_stringBuilder.Append(current.frameTime);
			_stringBuilder.Append(",average=");
			_stringBuilder.Append(current.frameTimeAverage);
			_stringBuilder.Append(" ");
			_stringBuilder.Append(epochNow);
			_stringBuilder.Append("\n");
			_stringBuilder.Append("memory,server=");
			_stringBuilder.Append(Configuration.serverTag);
			_stringBuilder.Append(" used=");
			_stringBuilder.Append(current.memoryUsageSystem);
			_stringBuilder.Append("i,collections=");
			_stringBuilder.Append(current.memoryCollections);
			_stringBuilder.Append("i,allocations=");
			_stringBuilder.Append(current.memoryAllocations);
			_stringBuilder.Append("i,gc=");
			_stringBuilder.Append(current.gcTriggered);
			_stringBuilder.Append(" ");
			_stringBuilder.Append(epochNow);
			_stringBuilder.Append("\n");
			_stringBuilder.Append("tasks,server=");
			_stringBuilder.Append(Configuration.serverTag);
			_stringBuilder.Append(" load_balancer=");
			_stringBuilder.Append(current.loadBalancerTasks);
			_stringBuilder.Append("i,invoke_handler=");
			_stringBuilder.Append(current.invokeHandlerTasks);
			_stringBuilder.Append("i,workshop_skins_queue=");
			_stringBuilder.Append(current.workshopSkinsQueued);
			_stringBuilder.Append("i ");
			_stringBuilder.Append(epochNow);
			_stringBuilder.Append("\n");
			ulong bytesReceivedLastSecond = Net.sv.GetStat(null, BaseNetwork.StatTypeLong.BytesReceived_LastSecond);
			ulong bytesSentLastSecond = Net.sv.GetStat(null, BaseNetwork.StatTypeLong.BytesSent_LastSecond);
			ulong packetLossLastSecond = Net.sv.GetStat(null, BaseNetwork.StatTypeLong.PacketLossLastSecond);
			_stringBuilder.Append("network,server=");
			_stringBuilder.Append(Configuration.serverTag);
			_stringBuilder.Append(" bytes_received=");
			_stringBuilder.Append(bytesReceivedLastSecond);
			_stringBuilder.Append("i,bytes_sent=");
			_stringBuilder.Append(bytesSentLastSecond);
			_stringBuilder.Append("i,packet_loss=");
			_stringBuilder.Append(packetLossLastSecond);
			_stringBuilder.Append("i ");
			_stringBuilder.Append(epochNow);
			_stringBuilder.Append("\n");
			_stringBuilder.Append("players,server=");
			_stringBuilder.Append(Configuration.serverTag);
			_stringBuilder.Append(" count=");
			_stringBuilder.Append(BasePlayer.activePlayerList.Count);
			_stringBuilder.Append("i,joining=");
			_stringBuilder.Append(SingletonComponent<ServerMgr>.Instance.connectionQueue.Joining);
			_stringBuilder.Append("i,queued=");
			_stringBuilder.Append(SingletonComponent<ServerMgr>.Instance.connectionQueue.Queued);
			_stringBuilder.Append("i ");
			_stringBuilder.Append(epochNow);
			_stringBuilder.Append("\n");
			_stringBuilder.Append("entities,server=");
			_stringBuilder.Append(Configuration.serverTag);
			_stringBuilder.Append(" count=");
			_stringBuilder.Append(BaseNetworkable.serverEntities.Count);
			_stringBuilder.Append("i ");
			_stringBuilder.Append(epochNow);
			_reportUploader.AddToSendBuffer(_stringBuilder.ToString());
		}
		public void UploadPacket<T>(string ID, T data, Action<StringBuilder, T> serializer)
		{
			string epochNow = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
			_stringBuilder.Clear();
			_stringBuilder.Append(ID);
			_stringBuilder.Append(",server=");
			_stringBuilder.Append(Configuration.serverTag);
			serializer(_stringBuilder, data);
			_stringBuilder.Append(" ");
			_stringBuilder.Append(epochNow);
			AddToSendBuffer(_stringBuilder.ToString());
		}
		public void AddToSendBuffer(string toString)
		{
			_reportUploader.AddToSendBuffer(toString);
		}
		private static void LogMethodInfo(StringBuilder builder, MethodInfo info)
		{
			builder.Append(",behaviour=\"");
			Type declaringType = info.DeclaringType;
			builder.Append((declaringType != null) ? declaringType.Name : null);
			builder.Append("\",method=\"");
			builder.Append(info.Name);
		}

		private static void LogMethodName(StringBuilder builder, string info)
		{
			builder.Append(",behaviour=\"");
			foreach (char cursor in info)
			{
				if (cursor == '.')
				{
					builder.Append("\",method=\"");
				}
				else
				{
					builder.Append(cursor);
				}
			}
		}

		private void StatusCommand(ConsoleSystem.Arg arg)
		{
			_stringBuilder.Clear();
			_stringBuilder.AppendLine("[ServerMetrics]: Status");
			_stringBuilder.AppendLine("Overview");
			_stringBuilder.Append("\tReady: ");
			_stringBuilder.Append(Ready);
			_stringBuilder.AppendLine();
			_stringBuilder.AppendLine("Report Uploader:");
			_stringBuilder.Append("\tRunning: ");
			_stringBuilder.Append(_reportUploader.IsRunning);
			_stringBuilder.AppendLine();
			_stringBuilder.Append("\tIn Buffer: ");
			_stringBuilder.Append(_reportUploader.BufferSize);
			_stringBuilder.AppendLine();
			arg.ReplyWith(_stringBuilder.ToString());
		}

		private bool ValidateConfiguration()
		{
			if (Configuration == null)
			{
				return false;
			}
			bool valid = true;
			if (Configuration.databaseUrl == "http://exampledb.com")
			{
				Debug.LogError("[ServerMetrics]: Default database url detected in configuration, loading aborted");
				valid = false;
			}
			if (Configuration.databaseName == "CHANGEME_rust_server_example")
			{
				Debug.LogError("[ServerMetrics]: Default database name detected in configuration, loading aborted");
				valid = false;
			}
			if (Configuration.serverTag == "CHANGEME-01")
			{
				Debug.LogError("[ServerMetrics]: Default server tag detected in configuration, loading aborted");
				valid = false;
			}
			return valid;
		}

		private class NetworkUpdateData
		{
			public int count;
			public long bytes;

			public NetworkUpdateData(int count, long bytes)
			{
				this.count = count;
				this.bytes = bytes;
			}
		}
	}
	internal class ReportUploader : MonoBehaviour
	{
		private const int _sendBufferCapacity = 100000;
		private readonly Action _notifySubsequentNetworkFailuresAction;
		private readonly Action _notifySubsequentHttpFailuresAction;
		private readonly List<string> _sendBuffer = new List<string>(100000);
		private readonly StringBuilder _payloadBuilder = new StringBuilder();
		private bool _isRunning;
		private ushort _attempt;
		private byte[] _data;
		private Uri _uri;
		private MetricsLogger _metricsLogger;
		private char[] charBuffer = new char[32768];
		private bool _throttleNetworkErrorMessages;
		private uint _accumulatedNetworkErrors;
		private bool _throttleHttpErrorMessages;
		private uint _accumulatedHttpErrors;

		public ushort BatchSize
		{
			get
			{
				ushort configVal = (ushort)((Singleton.ConfigInstance != null) ? Singleton.ConfigInstance.batchSize : 1000);
				if (configVal < 1000)
				{
					return 1000;
				}
				return configVal;
			}
		}

		public bool IsRunning
		{
			get
			{
				return _isRunning;
			}
		}

		public int BufferSize
		{
			get
			{
				return _sendBuffer.Count;
			}
		}

		public ReportUploader()
		{
			_notifySubsequentNetworkFailuresAction = new Action(NotifySubsequentNetworkFailures);
			_notifySubsequentHttpFailuresAction = new Action(NotifySubsequentHttpFailures);
		}

		private void Awake()
		{
			_metricsLogger = base.GetComponent<MetricsLogger>();
			if (_metricsLogger == null)
			{
				Debug.LogError("[ServerMetrics] ReportUploader failed to find the MetricsLogger component");
				UnityEngine.Object.Destroy(this);
			}
		}

		public void AddToSendBuffer(string payload)
		{
			if (_sendBuffer.Count == 100000)
			{
				_sendBuffer.RemoveAt(0);
			}
			_sendBuffer.Add(payload);
			if (!_isRunning)
			{
				base.StartCoroutine(SendBufferLoop());
			}
		}

		private IEnumerator SendBufferLoop()
		{
			_isRunning = true;
			yield return null;
			while (_sendBuffer.Count > 0 && _isRunning)
			{
				int amountToTake = Mathf.Min(_sendBuffer.Count, (int)BatchSize);
				for (int i = 0; i < amountToTake; i++)
				{
					_payloadBuilder.Append(_sendBuffer[i]);
					_payloadBuilder.Append("\n");
				}
				_sendBuffer.RemoveRange(0, amountToTake);
				_attempt = 0;
				if (_payloadBuilder.Length > charBuffer.Length)
				{
					charBuffer = new char[_payloadBuilder.Length + 1024];
				}
				_payloadBuilder.CopyTo(0, charBuffer, 0, _payloadBuilder.Length);
				_data = Encoding.UTF8.GetBytes(charBuffer, 0, _payloadBuilder.Length);
				_uri = _metricsLogger.BaseUri;
				_payloadBuilder.Clear();
				yield return SendRequest();
			}
			_isRunning = false;
			yield break;
		}

		private IEnumerator SendRequest()
		{
			UnityWebRequest request = new UnityWebRequest(_uri, "POST")
			{
				uploadHandler = new UploadHandlerRaw(_data),
				downloadHandler = new DownloadHandlerBuffer(),
				timeout = 15,
				useHttpContinue = true,
				redirectLimit = 5
			};
			yield return request.SendWebRequest();
			if (request.isNetworkError)
			{
				if (_attempt >= 2)
				{
					if (_throttleNetworkErrorMessages)
					{
						_accumulatedNetworkErrors += 1U;
					}
					else
					{
						Debug.LogError("Two consecutive network failures occurred while submitting a batch of metrics");
						InvokeHandler.Invoke(this, _notifySubsequentNetworkFailuresAction, 5f);
						_throttleNetworkErrorMessages = true;
					}
					yield break;
				}
				_attempt += 1;
				yield return SendRequest();
				yield break;
			}
			else
			{
				if (request.isHttpError)
				{
					if (_throttleHttpErrorMessages)
					{
						_accumulatedHttpErrors += 1U;
					}
					else
					{
						Debug.LogError("A HTTP error occurred while submitting batch of metrics: " + request.error);
						var configuration = Singleton.ConfigInstance;
						if (configuration != null && configuration.debugLogging)
						{
							Debug.LogError(request.downloadHandler.text);
						}
						InvokeHandler.Invoke(this, _notifySubsequentHttpFailuresAction, 5f);
						_throttleHttpErrorMessages = true;
					}
					yield break;
				}
				yield break;
			}
		}

		private void NotifySubsequentNetworkFailures()
		{
			_throttleNetworkErrorMessages = false;
			if (_accumulatedNetworkErrors == 0U)
			{
				return;
			}
			Debug.LogError(string.Format("{0} subsequent network errors occurred in the last 5 seconds", _accumulatedNetworkErrors));
			_accumulatedNetworkErrors = 0U;
		}

		private void NotifySubsequentHttpFailures()
		{
			_throttleHttpErrorMessages = false;
			if (_accumulatedHttpErrors == 0U)
			{
				return;
			}
			Debug.LogError(string.Format("{0} subsequent HTTP errors occurred in the last 5 seconds", _accumulatedHttpErrors));
			_accumulatedHttpErrors = 0U;
		}

		private void OnDestroy()
		{
			Stop();
		}

		public void Stop()
		{
			_isRunning = false;
			base.StopAllCoroutines();
		}
	}
	internal class DelayedHarmonyPatchAttribute : Attribute
	{
	}
	public static class Helpers
	{
	}
	public class MetricsTimeStorage<TKey>
	{
		private readonly string _metricKey;
		private readonly Action<StringBuilder, TKey> _stringBuilderSerializer;
		private Dictionary<TKey, double> dict = new Dictionary<TKey, double>();
		private readonly StringBuilder sb = new StringBuilder();

		public MetricsTimeStorage(string metricKey, Action<StringBuilder, TKey> stringBuilderSerializer)
		{
			_metricKey = metricKey;
			_stringBuilderSerializer = stringBuilderSerializer;
		}

		public void LogTime(TKey key, double milliseconds)
		{
			if (!SingletonComponent<MetricsLogger>.Instance.Ready)
			{
				return;
			}
			double currentDuration;
			if (!dict.TryGetValue(key, out currentDuration))
			{
				dict.Add(key, milliseconds);
				return;
			}
			dict[key] = currentDuration + milliseconds;
		}

		public void SerializeToStringBuilder()
		{
			if (!SingletonComponent<MetricsLogger>.Instance.Ready)
			{
				return;
			}
			foreach (KeyValuePair<TKey, double> item in dict)
			{
				string epochNow = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
				sb.Clear();
				sb.Append(_metricKey);
				sb.Append(",server=");
				sb.Append(Singleton.ConfigInstance.serverTag);
				_stringBuilderSerializer(sb, item.Key);
				sb.Append("\" duration=");
				sb.Append((float)item.Value);
				sb.Append(" ");
				sb.Append(epochNow);
				SingletonComponent<MetricsLogger>.Instance.AddToSendBuffer(sb.ToString());
			}
			dict.Clear();
		}
	}
}

public class RustServerMetricsConfig
{
	public const string DEFAULT_INFLUX_DB_URL = "http://exampledb.com";
	public const string DEFAULT_INFLUX_DB_NAME = "CHANGEME_rust_server_example";
	public const string DEFAULT_INFLUX_DB_USER = "admin";
	public const string DEFAULT_INFLUX_DB_PASSWORD = "adminadmin";
	public const string DEFAULT_SERVER_TAG = "CHANGEME-01";

	[JsonProperty(PropertyName = "Enabled")]
	public bool enabled;

	[JsonProperty(PropertyName = "Influx Database Url")]
	public string databaseUrl = "http://exampledb.com";

	[JsonProperty(PropertyName = "Influx Database Name")]
	public string databaseName = "CHANGEME_rust_server_example";

	[JsonProperty(PropertyName = "Influx Database User")]
	public string databaseUser = "admin";

	[JsonProperty(PropertyName = "Influx Database Password")]
	public string databasePassword = "adminadmin";

	[JsonProperty(PropertyName = "Server Tag")]
	public string serverTag = "CHANGEME-01";

	[JsonProperty(PropertyName = "Debug Logging")]
	public bool debugLogging;

	[JsonProperty(PropertyName = "Amount of metrics to submit in each request")]
	public ushort batchSize = 1000;

	[JsonProperty(PropertyName = "Gather Player Averages (Client FPS, Client Latency, Player FPS, Player Memory, Player Latency, Player Packet Loss)")]
	public bool gatherPlayerMetrics = true;
}
