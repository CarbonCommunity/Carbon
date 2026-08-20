#if !TESTS_NO_WEBREQUEST
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Carbon.Extensions;
using Carbon.Test;
using Oxide.Core.Libraries;

namespace Carbon.Plugins;

public partial class Tests
{
	public class WebRequest
	{
		private const string HttpsGenerate204Url = "https://www.gstatic.com/generate_204";
		private const string HttpGenerate204Url = "http://www.gstatic.com/generate_204";
		private const string CustomUserAgent = "CarbonTests/1.0 (+plugin supplied)";
		private const string UnreachableBindIp = "203.0.113.1";

		[Integrations.Test.Assert]
		public void validate_library(Integrations.Test.Assert test)
		{
			test.IsNotNull(singleton.webrequest, "singleton.webrequest");
		}

		[Integrations.Test.Assert(Timeout = 20_000)]
		public async Task https_generate_204(Integrations.Test.Assert test)
		{
			var result = await ExecuteStringGet(HttpsGenerate204Url, test);

			test.IsTrue(result.CallbackCount == 1, "https callback invoked once");
			test.IsTrue(result.CallbackCode == 204, "https callback status code is 204");
			test.IsTrue(string.IsNullOrEmpty(result.CallbackBody), "https callback body is empty");
			test.IsTrue(result.CallbackThreadId == ThreadEx.MainThread.ManagedThreadId, "https callback is on main thread");
			test.IsTrue(result.Request.ResponseCode == 204, "https request response code is 204");
			test.IsNull(result.Request.ResponseError, "https request response error");
			test.IsTrue(result.Request.ResponseDuration.TotalMilliseconds >= 0, "https request has response duration");

			test.Complete();
		}

		[Integrations.Test.Assert(Timeout = 20_000)]
		public async Task http_generate_204(Integrations.Test.Assert test)
		{
			var result = await ExecuteStringGet(HttpGenerate204Url, test);

			test.IsTrue(result.CallbackCount == 1, "http callback invoked once");
			test.IsTrue(result.CallbackCode == 204, "http callback status code is 204");
			test.IsTrue(string.IsNullOrEmpty(result.CallbackBody), "http callback body is empty");
			test.IsTrue(result.CallbackThreadId == ThreadEx.MainThread.ManagedThreadId, "http callback is on main thread");
			test.IsTrue(result.Request.ResponseCode == 204, "http request response code is 204");
			test.IsNull(result.Request.ResponseError, "http request response error");

			test.Complete();
		}

		[Integrations.Test.Assert(Timeout = 20_000)]
		public async Task enqueue_async_invokes_callback_once(Integrations.Test.Assert test)
		{
			var callbackTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
			var mainThreadId = ThreadEx.MainThread.ManagedThreadId;

			var callbackCount = 0;
			var callbackCode = 0;
			var callbackThreadId = -1;
			string callbackBody = null;

			var request = await singleton.webrequest.EnqueueAsync(HttpsGenerate204Url, null, (code, body) =>
			{
				Interlocked.Increment(ref callbackCount);
				callbackCode = code;
				callbackBody = body;
				callbackThreadId = Thread.CurrentThread.ManagedThreadId;
				callbackTcs.TrySetResult(true);
			}, singleton, timeout: 15f);

			var callbackFinished = await Task.WhenAny(callbackTcs.Task, Task.Delay(8_000));

			test.IsTrue(callbackFinished == callbackTcs.Task, "EnqueueAsync callback invoked");
			test.IsTrue(callbackCount == 1, "EnqueueAsync callback invoked once");
			test.IsTrue(callbackCode == 204, "EnqueueAsync callback status code is 204");
			test.IsTrue(string.IsNullOrEmpty(callbackBody), "EnqueueAsync callback body is empty");
			test.IsTrue(callbackThreadId == mainThreadId, "EnqueueAsync callback is on main thread");
			test.IsTrue(request.ResponseCode == 204, "EnqueueAsync request response code is 204");
			test.IsNull(request.ResponseError, "EnqueueAsync request response error");

			test.Complete();
		}

		[Integrations.Test.Assert(Timeout = 20_000)]
		public async Task enqueue_data_https_generate_204(Integrations.Test.Assert test)
		{
			var callbackTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
			var mainThreadId = ThreadEx.MainThread.ManagedThreadId;

			var callbackCount = 0;
			var callbackCode = 0;
			var callbackThreadId = -1;
			byte[] callbackData = null;

			var request = singleton.webrequest.EnqueueData(HttpsGenerate204Url, null, (code, data) =>
			{
				Interlocked.Increment(ref callbackCount);
				callbackCode = code;
				callbackData = data;
				callbackThreadId = Thread.CurrentThread.ManagedThreadId;
				callbackTcs.TrySetResult(true);
			}, singleton, timeout: 15f);

			var callbackFinished = await Task.WhenAny(callbackTcs.Task, Task.Delay(8_000));

			test.IsTrue(callbackFinished == callbackTcs.Task, "EnqueueData callback invoked");
			test.IsTrue(callbackCount == 1, "EnqueueData callback invoked once");
			test.IsTrue(callbackCode == 204, "EnqueueData callback status code is 204");
			test.IsTrue(callbackData != null && callbackData.Length == 0, "EnqueueData payload is empty");
			test.IsTrue(callbackThreadId == mainThreadId, "EnqueueData callback is on main thread");
			test.IsTrue(request.ResponseCode == 204, "EnqueueData request response code is 204");
			test.IsNull(request.ResponseError, "EnqueueData request response error");

			test.Complete();
		}

		[Integrations.Test.Assert(Timeout = 20_000)]
		public async Task enqueue_data_async_invokes_callback_once(Integrations.Test.Assert test)
		{
			var callbackTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
			var mainThreadId = ThreadEx.MainThread.ManagedThreadId;

			var callbackCount = 0;
			var callbackCode = 0;
			var callbackThreadId = -1;
			byte[] callbackData = null;

			var request = await singleton.webrequest.EnqueueDataAsync(HttpsGenerate204Url, null, (code, data) =>
			{
				Interlocked.Increment(ref callbackCount);
				callbackCode = code;
				callbackData = data;
				callbackThreadId = Thread.CurrentThread.ManagedThreadId;
				callbackTcs.TrySetResult(true);
			}, singleton, timeout: 15f);

			var callbackFinished = await Task.WhenAny(callbackTcs.Task, Task.Delay(8_000));

			test.IsTrue(callbackFinished == callbackTcs.Task, "EnqueueDataAsync callback invoked");
			test.IsTrue(callbackCount == 1, "EnqueueDataAsync callback invoked once");
			test.IsTrue(callbackCode == 204, "EnqueueDataAsync callback status code is 204");
			test.IsTrue(callbackData != null && callbackData.Length == 0, "EnqueueDataAsync payload is empty");
			test.IsTrue(callbackThreadId == mainThreadId, "EnqueueDataAsync callback is on main thread");
			test.IsTrue(request.ResponseCode == 204, "EnqueueDataAsync request response code is 204");
			test.IsNull(request.ResponseError, "EnqueueDataAsync request response error");

			test.Complete();
		}

		[Integrations.Test.Assert(Timeout = 20_000)]
		public async Task non_get_methods_invoke_callback_once(Integrations.Test.Assert test)
		{
			var methods = new[]
			{
				RequestMethod.POST,
				RequestMethod.PUT,
				RequestMethod.PATCH,
				RequestMethod.DELETE,
			};

			for (var i = 0; i < methods.Length; i++)
			{
				var method = methods[i];
				var result = await ExecuteStringRequest(HttpsGenerate204Url, method, "probe=1", test, 8_000);

				test.IsTrue(result.CallbackCount == 1, $"{method} callback invoked once");
				test.IsTrue(result.CallbackThreadId == ThreadEx.MainThread.ManagedThreadId,
					$"{method} callback is on main thread");
				test.IsTrue(result.Request.ResponseDuration.TotalMilliseconds >= 0,
					$"{method} request has response duration");
				test.IsTrue(result.Request.ResponseCode != 0 || result.Request.ResponseError != null,
					$"{method} produced status code or response error");
			}

			test.Complete();
		}

		[Integrations.Test.Assert(Timeout = 20_000)]
		public async Task invalid_host_surfaces_error_and_invokes_callback_once(Integrations.Test.Assert test)
		{
			var invalidUrl = $"https://{Guid.NewGuid():N}.invalid/generate_204";
			var result = await ExecuteStringRequest(invalidUrl, RequestMethod.GET, null, test, 12_000);

			test.IsTrue(result.CallbackCount == 1, "invalid host callback invoked once");
			test.IsTrue(result.CallbackThreadId == ThreadEx.MainThread.ManagedThreadId,
				"invalid host callback is on main thread");
			test.IsTrue(result.Request.ResponseError != null,
				"invalid host request has response error");
			test.IsTrue(result.CallbackCode == result.Request.ResponseCode,
				$"invalid host callback/request response code match ({result.CallbackCode}/{result.Request.ResponseCode})");

			test.Complete();
		}

		[Integrations.Test.Assert(Timeout = 20_000)]
		public async Task default_user_agent_is_carbons(Integrations.Test.Assert test)
		{
			using var server = new LoopbackServer();

			var result = await ExecuteStringRequest(server.Url, RequestMethod.GET, null, test);
			var userAgent = await server.GetHeader("User-Agent");

			test.IsTrue(result.CallbackCode == 204, $"default user agent request completed ({result.CallbackCode})");
			test.IsTrue(userAgent == Community.Runtime.Analytics.UserAgent,
				$"default user agent is Carbon's (got '{userAgent}')");

			test.Complete();
		}

		[Integrations.Test.Assert(Timeout = 20_000)]
		public async Task custom_user_agent_is_preserved(Integrations.Test.Assert test)
		{
			using var server = new LoopbackServer();

			var result = await ExecuteStringRequest(server.Url, RequestMethod.GET, null, test,
				headers: new Dictionary<string, string> { ["User-Agent"] = CustomUserAgent });
			var userAgent = await server.GetHeader("User-Agent");

			test.IsTrue(result.CallbackCode == 204, $"custom user agent request completed ({result.CallbackCode})");
			test.IsTrue(userAgent == CustomUserAgent, $"custom user agent reached the wire (got '{userAgent}')");

			test.Complete();
		}

		[Integrations.Test.Assert(Timeout = 20_000)]
		public async Task custom_user_agent_is_case_insensitive(Integrations.Test.Assert test)
		{
			using var server = new LoopbackServer();

			var result = await ExecuteStringRequest(server.Url, RequestMethod.GET, null, test,
				headers: new Dictionary<string, string> { ["user-agent"] = CustomUserAgent });
			var userAgent = await server.GetHeader("User-Agent");

			test.IsTrue(result.CallbackCode == 204, $"lowercase user agent request completed ({result.CallbackCode})");
			test.IsTrue(userAgent == CustomUserAgent, $"lowercase user agent reached the wire (got '{userAgent}')");

			test.Complete();
		}

		[Integrations.Test.Assert(Timeout = 20_000)]
		public async Task pooled_client_keeps_user_agent_across_requests(Integrations.Test.Assert test)
		{
			using var first = new LoopbackServer();
			using var second = new LoopbackServer();
			using var client = new WebRequests.WebRequest.Client();

			client.Headers["User-Agent"] = Community.Runtime.Analytics.UserAgent;

			await client.DownloadStringTaskAsync(new Uri(first.Url));
			await client.DownloadStringTaskAsync(new Uri(second.Url));

			var firstUserAgent = await first.GetHeader("User-Agent");
			var secondUserAgent = await second.GetHeader("User-Agent");

			test.IsTrue(firstUserAgent == Community.Runtime.Analytics.UserAgent,
				$"pooled client sent the user agent on its first request (got '{firstUserAgent}')");
			test.IsTrue(secondUserAgent == Community.Runtime.Analytics.UserAgent,
				$"pooled client sent the user agent on its second request (got '{secondUserAgent}')");

			test.Complete();
		}

		[Integrations.Test.Assert(Timeout = 20_000)]
		public async Task timeout_aborts_a_stalled_request(Integrations.Test.Assert test)
		{
			using var server = new LoopbackServer(respond: false);

			var started = DateTime.UtcNow;
			var result = await ExecuteStringRequest(server.Url, RequestMethod.GET, null, test, 12_000, timeout: 2f);
			var elapsed = (DateTime.UtcNow - started).TotalMilliseconds;

			test.IsTrue(result.CallbackCount == 1, "stalled request callback invoked once");
			test.IsTrue(result.Request.ResponseError != null, "stalled request surfaced a response error");
			test.IsTrue(elapsed >= 1_000, $"stalled request waited for the timeout ({elapsed:0}ms)");
			test.IsTrue(elapsed < 10_000, $"stalled request gave up on the timeout ({elapsed:0}ms)");

			test.Complete();
		}

		[Integrations.Test.Assert(Timeout = 20_000)]
		public async Task loopback_request_ignores_webrequest_ip(Integrations.Test.Assert test)
		{
			if (!Community.IsConfigReady)
			{
				test.Warn("carbon config is not ready, skipping web request ip check");
				test.Complete();
				return;
			}

			using var server = new LoopbackServer();

			var previousIp = Community.Runtime.Config.WebRequestIp;
			Community.Runtime.Config.WebRequestIp = UnreachableBindIp;

			try
			{
				var result = await ExecuteStringRequest(server.Url, RequestMethod.GET, null, test);

				test.IsTrue(result.CallbackCount == 1, "loopback callback invoked once");
				test.IsTrue(result.CallbackCode == 204,
					$"loopback request completed while web request ip is set ({result.CallbackCode})");
				test.IsNull(result.Request.ResponseError, "loopback request has no response error");
			}
			finally
			{
				Community.Runtime.Config.WebRequestIp = previousIp;
			}

			test.Complete();
		}

		[Integrations.Test.Assert(Timeout = 30_000)]
		public async Task enqueue_burst_callbacks_once_and_main_thread(Integrations.Test.Assert test)
		{
			const int requestCount = 48;

			using var server = new LoopbackServer();

			AssertBurst(test, "burst", requestCount, await ExecuteBurst(server.Url, requestCount, 12_000));

			test.Complete();
		}

		[Integrations.Test.Assert(Timeout = 15_000)]
		public async Task enqueue_burst_under_threadpool_pressure(Integrations.Test.Assert test)
		{
			const int requestCount = 16;

			using var server = new LoopbackServer();

			var workerCount = Math.Clamp(Environment.ProcessorCount * 2, 8, 32);
			var startTarget = Math.Min(workerCount, 4);
			var release = new ManualResetEventSlim(false);
			var workers = new Task[workerCount];
			var started = 0;
			var faults = 0;

			for (var i = 0; i < workerCount; i++)
			{
				workers[i] = Task.Run(() =>
				{
					try
					{
						Interlocked.Increment(ref started);
						release.Wait(1_500);
					}
					catch
					{
						Interlocked.Increment(ref faults);
					}
				});
			}

			try
			{
				var ready = Task.Run(async () =>
				{
					while (Volatile.Read(ref started) < startTarget)
					{
						await Task.Delay(10);
					}
				});

				await Task.WhenAny(ready, Task.Delay(600));

				if (!ready.IsCompleted)
				{
					test.Warn($"threadpool pressure did not reach its startup target ({started}/{startTarget})");
				}

				AssertBurst(test, "pressure burst", requestCount, await ExecuteBurst(server.Url, requestCount, 6_000));

				test.IsTrue(faults == 0, $"threadpool pressure workers faulted ({faults})");

				test.Complete();
			}
			finally
			{
				release.Set();
				await Task.WhenAny(Task.WhenAll(workers), Task.Delay(2_000));
				release.Dispose();
			}
		}

		private static async Task<StringGetResult> ExecuteStringGet(string url, Integrations.Test.Assert test)
		{
			return await ExecuteStringRequest(url, RequestMethod.GET, null, test);
		}

		private static async Task<StringGetResult> ExecuteStringRequest(
			string url, RequestMethod method, string body, Integrations.Test.Assert test, int callbackTimeoutMs = 8_000,
			Dictionary<string, string> headers = null, float timeout = 15f
		)
		{
			var callbackTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

			var callbackCount = 0;
			var callbackCode = 0;
			var callbackThreadId = -1;
			string callbackBody = null;

			var request = singleton.webrequest.Enqueue(url, body, (code, callbackBodyValue) =>
			{
				Interlocked.Increment(ref callbackCount);
				callbackCode = code;
				callbackBody = callbackBodyValue;
				callbackThreadId = Thread.CurrentThread.ManagedThreadId;
				callbackTcs.TrySetResult(true);
			}, singleton, method, headers, timeout);

			var callbackFinished = await Task.WhenAny(callbackTcs.Task, Task.Delay(callbackTimeoutMs));
			test.IsTrue(callbackFinished == callbackTcs.Task, $"callback invoked ({method} {url})");

			return new StringGetResult
			{
				Request = request,
				CallbackCount = callbackCount,
				CallbackCode = callbackCode,
				CallbackBody = callbackBody,
				CallbackThreadId = callbackThreadId,
			};
		}

		private struct StringGetResult
		{
			public WebRequests.WebRequest Request;
			public int CallbackCount;
			public int CallbackCode;
			public string CallbackBody;
			public int CallbackThreadId;
		}

		private static async Task<BurstResult> ExecuteBurst(string url, int requestCount, int callbackTimeoutMs)
		{
			var allCallbacksTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
			var mainThreadId = ThreadEx.MainThread.ManagedThreadId;

			var callbackCounts = new int[requestCount];
			var callbackCodes = new int[requestCount];
			var callbackThreadIds = new int[requestCount];
			var callbackBodies = new string[requestCount];
			var completed = 0;

			for (var i = 0; i < requestCount; i++)
			{
				var requestIndex = i;

				callbackCodes[requestIndex] = -1;
				callbackThreadIds[requestIndex] = -1;

				singleton.webrequest.Enqueue(url, null, (code, body) =>
				{
					Interlocked.Increment(ref callbackCounts[requestIndex]);
					callbackCodes[requestIndex] = code;
					callbackThreadIds[requestIndex] = Thread.CurrentThread.ManagedThreadId;
					callbackBodies[requestIndex] = body;

					if (Interlocked.Increment(ref completed) == requestCount)
					{
						allCallbacksTcs.TrySetResult(true);
					}
				}, singleton, timeout: 15f);
			}

			var callbacksFinished = await Task.WhenAny(allCallbacksTcs.Task, Task.Delay(callbackTimeoutMs));
			var result = new BurstResult
			{
				AllCompleted = callbacksFinished == allCallbacksTcs.Task,
				Completed = completed,
			};

			for (var i = 0; i < requestCount; i++)
			{
				if (callbackCounts[i] == 0)
				{
					result.Missing++;
				}
				else if (callbackCounts[i] > 1)
				{
					result.Duplicates += callbackCounts[i] - 1;
				}

				if (callbackCodes[i] != 204)
				{
					result.WrongCodes++;
				}

				if (callbackThreadIds[i] != mainThreadId)
				{
					result.WrongThreads++;
				}

				if (!string.IsNullOrEmpty(callbackBodies[i]))
				{
					result.NonEmptyBodies++;
				}
			}

			return result;
		}

		private static void AssertBurst(Integrations.Test.Assert test, string label, int requestCount, BurstResult result)
		{
			test.IsTrue(result.AllCompleted, $"{label} callbacks completed ({result.Completed}/{requestCount})");
			test.IsTrue(result.Missing == 0, $"{label} has no missing callbacks ({result.Missing})");
			test.IsTrue(result.Duplicates == 0, $"{label} has no duplicate callbacks ({result.Duplicates})");
			test.IsTrue(result.WrongCodes == 0, $"{label} callback status codes are 204 ({result.WrongCodes} wrong)");
			test.IsTrue(result.WrongThreads == 0, $"{label} callbacks are on main thread ({result.WrongThreads} wrong)");
			test.IsTrue(result.NonEmptyBodies == 0, $"{label} callback bodies are empty ({result.NonEmptyBodies} non-empty)");
		}

		private struct BurstResult
		{
			public bool AllCompleted;
			public int Completed;
			public int Missing;
			public int Duplicates;
			public int WrongCodes;
			public int WrongThreads;
			public int NonEmptyBodies;
		}

		private sealed class LoopbackServer : IDisposable
		{
			private const int CaptureTimeoutMs = 8_000;

			private static readonly byte[] NoContentResponse =
				Encoding.ASCII.GetBytes("HTTP/1.1 204 No Content\r\nConnection: close\r\n\r\n");

			private readonly TcpListener _listener;
			private readonly bool _respond;
			private readonly List<TcpClient> _clients = new List<TcpClient>();
			private readonly TaskCompletionSource<string> _firstHead =
				new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);

			private bool _disposed;

			public string Url { get; }

			public LoopbackServer(bool respond = true)
			{
				_respond = respond;
				_listener = new TcpListener(IPAddress.Loopback, 0);
				_listener.Start();

				Url = $"http://127.0.0.1:{((IPEndPoint)_listener.LocalEndpoint).Port}/probe";

				_ = Task.Run(Accept);
			}

			public async Task<string> GetHeader(string name)
			{
				var captured = await Task.WhenAny(_firstHead.Task, Task.Delay(CaptureTimeoutMs));
				var head = captured == _firstHead.Task ? _firstHead.Task.Result : null;

				if (string.IsNullOrEmpty(head))
				{
					return null;
				}

				var prefix = $"{name}:";
				var lines = head.Split('\n');

				for (var i = 0; i < lines.Length; i++)
				{
					var line = lines[i].Trim();

					if (line.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
					{
						return line.Substring(prefix.Length).Trim();
					}
				}

				return string.Empty;
			}

			private async Task Accept()
			{
				while (true)
				{
					TcpClient client;

					try
					{
						client = await _listener.AcceptTcpClientAsync();
					}
					catch
					{
						_firstHead.TrySetResult(null);
						return;
					}

					lock (_clients)
					{
						if (_disposed)
						{
							client.Close();
							return;
						}

						_clients.Add(client);
					}

					_ = Task.Run(() => Serve(client));
				}
			}

			private async Task Serve(TcpClient client)
			{
				try
				{
					var stream = client.GetStream();
					var reader = new StreamReader(stream, Encoding.ASCII, false, 256, true);
					var head = new StringBuilder();

					string line;

					while (!string.IsNullOrEmpty(line = await reader.ReadLineAsync()))
					{
						head.AppendLine(line);
					}

					_firstHead.TrySetResult(head.ToString());

					if (_respond)
					{
						await stream.WriteAsync(NoContentResponse, 0, NoContentResponse.Length);
						client.Close();
					}
				}
				catch
				{
					_firstHead.TrySetResult(null);
				}
			}

			public void Dispose()
			{
				lock (_clients)
				{
					if (_disposed)
					{
						return;
					}

					_disposed = true;

					for (var i = 0; i < _clients.Count; i++)
					{
						_clients[i].Close();
					}

					_clients.Clear();
				}

				_firstHead.TrySetResult(null);
				_listener.Stop();
			}
		}
	}
}
#endif
