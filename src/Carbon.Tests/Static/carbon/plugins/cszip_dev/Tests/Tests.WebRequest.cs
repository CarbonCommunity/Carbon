using System;
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

		[Integrations.Test.Assert(Timeout = 30_000)]
		public async Task enqueue_burst_callbacks_once_and_main_thread(Integrations.Test.Assert test)
		{
			const int requestCount = 48;

			var allCallbacksTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
			var mainThreadId = ThreadEx.MainThread.ManagedThreadId;

			var completed = 0;
			var callbackCounts = new int[requestCount];
			var callbackCodes = new int[requestCount];
			var callbackThreadIds = new int[requestCount];
			var callbackBodies = new string[requestCount];

			for (var i = 0; i < requestCount; i++)
			{
				callbackCodes[i] = -1;
				callbackThreadIds[i] = -1;
				var requestIndex = i;

				singleton.webrequest.Enqueue(HttpsGenerate204Url, null, (code, body) =>
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

			var callbacksFinished = await Task.WhenAny(allCallbacksTcs.Task, Task.Delay(12_000));

			test.IsTrue(callbacksFinished == allCallbacksTcs.Task,
				$"burst callbacks completed ({completed}/{requestCount})");

			var missingCallbacks = 0;
			var duplicateCallbacks = 0;
			var wrongStatusCodes = 0;
			var wrongThreadCallbacks = 0;
			var nonEmptyBodies = 0;

			for (var i = 0; i < requestCount; i++)
			{
				if (callbackCounts[i] == 0)
				{
					missingCallbacks++;
				}
				else if (callbackCounts[i] > 1)
				{
					duplicateCallbacks += callbackCounts[i] - 1;
				}

				if (callbackCodes[i] != 204)
				{
					wrongStatusCodes++;
				}

				if (callbackThreadIds[i] != mainThreadId)
				{
					wrongThreadCallbacks++;
				}

				if (!string.IsNullOrEmpty(callbackBodies[i]))
				{
					nonEmptyBodies++;
				}
			}

			test.IsTrue(missingCallbacks == 0, $"burst has no missing callbacks ({missingCallbacks})");
			test.IsTrue(duplicateCallbacks == 0, $"burst has no duplicate callbacks ({duplicateCallbacks})");
			test.IsTrue(wrongStatusCodes == 0, $"burst callback status codes are 204 ({wrongStatusCodes} wrong)");
			test.IsTrue(wrongThreadCallbacks == 0, $"burst callbacks are on main thread ({wrongThreadCallbacks} wrong)");
			test.IsTrue(nonEmptyBodies == 0, $"burst callback bodies are empty ({nonEmptyBodies} non-empty)");

			test.Complete();
		}

		[Integrations.Test.Assert(Timeout = 15_000)]
		public async Task enqueue_burst_under_threadpool_pressure(Integrations.Test.Assert test)
		{
			const int requestCount = 16;

			ThreadPool.GetMaxThreads(out var maxWorkerThreads, out _);
			ThreadPool.GetAvailableThreads(out var availableBeforeWorkers, out _);

			var pressureWorkers = Math.Clamp(Environment.ProcessorCount * 2, 8, 32);
			var pressureStartTarget = Math.Min(pressureWorkers, 4);

			var pressureStarted = 0;
			var pressureFaults = 0;
			var pressureTasks = new Task[pressureWorkers];
			var pressureRelease = new ManualResetEventSlim(false);

			for (var i = 0; i < pressureWorkers; i++)
			{
				pressureTasks[i] = Task.Run(() =>
				{
					try
					{
						Interlocked.Increment(ref pressureStarted);
						pressureRelease.Wait(1_500);
					}
					catch
					{
						Interlocked.Increment(ref pressureFaults);
					}
				});
			}

			try
			{
				var pressureReadyTask = Task.Run(async () =>
				{
					while (Volatile.Read(ref pressureStarted) < pressureStartTarget)
					{
						await Task.Delay(10);
					}
				});
				await Task.WhenAny(pressureReadyTask, Task.Delay(600));

				ThreadPool.GetAvailableThreads(out var availableDuringWorkers, out _);

				if (pressureReadyTask.IsCompleted)
				{
					test.Log($"threadpool pressure workers reached startup target ({pressureStarted}/{pressureStartTarget})");
				}
				else
				{
					test.Warn($"threadpool pressure workers did not reach startup target in time ({pressureStarted}/{pressureStartTarget})");
				}

				if (availableDuringWorkers < availableBeforeWorkers)
				{
					test.Log($"threadpool pressure reduced available workers ({availableBeforeWorkers} -> {availableDuringWorkers})");
				}
				else
				{
					test.Warn($"threadpool pressure did not reduce available workers ({availableBeforeWorkers} -> {availableDuringWorkers})");
				}

				var allCallbacksTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
				var mainThreadId = ThreadEx.MainThread.ManagedThreadId;

				var completed = 0;
				var callbackCounts = new int[requestCount];
				var callbackCodes = new int[requestCount];
				var callbackThreadIds = new int[requestCount];
				var callbackBodies = new string[requestCount];

				for (var i = 0; i < requestCount; i++)
				{
					callbackCodes[i] = -1;
					callbackThreadIds[i] = -1;
					var requestIndex = i;

					singleton.webrequest.Enqueue(HttpsGenerate204Url, null, (code, body) =>
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

				var callbacksFinished = await Task.WhenAny(allCallbacksTcs.Task, Task.Delay(6_000));

				test.IsTrue(callbacksFinished == allCallbacksTcs.Task,
					$"pressure burst callbacks completed ({completed}/{requestCount})");

				var missingCallbacks = 0;
				var duplicateCallbacks = 0;
				var wrongStatusCodes = 0;
				var wrongThreadCallbacks = 0;
				var nonEmptyBodies = 0;

				for (var i = 0; i < requestCount; i++)
				{
					if (callbackCounts[i] == 0)
					{
						missingCallbacks++;
					}
					else if (callbackCounts[i] > 1)
					{
						duplicateCallbacks += callbackCounts[i] - 1;
					}

					if (callbackCodes[i] != 204)
					{
						wrongStatusCodes++;
					}

					if (callbackThreadIds[i] != mainThreadId)
					{
						wrongThreadCallbacks++;
					}

					if (!string.IsNullOrEmpty(callbackBodies[i]))
					{
						nonEmptyBodies++;
					}
				}

				test.IsTrue(missingCallbacks == 0, $"pressure burst has no missing callbacks ({missingCallbacks})");
				test.IsTrue(duplicateCallbacks == 0, $"pressure burst has no duplicate callbacks ({duplicateCallbacks})");
				test.IsTrue(wrongStatusCodes == 0, $"pressure burst callback status codes are 204 ({wrongStatusCodes} wrong)");
				test.IsTrue(wrongThreadCallbacks == 0, $"pressure burst callbacks are on main thread ({wrongThreadCallbacks} wrong)");
				test.IsTrue(nonEmptyBodies == 0, $"pressure burst callback bodies are empty ({nonEmptyBodies} non-empty)");

				test.IsTrue(pressureFaults == 0, $"threadpool pressure workers faulted ({pressureFaults})");

				test.Complete();
			}
			finally
			{
				pressureRelease.Set();
				await Task.WhenAny(Task.WhenAll(pressureTasks), Task.Delay(2_000));
				pressureRelease.Dispose();
			}
		}

		private static async Task<StringGetResult> ExecuteStringGet(string url, Integrations.Test.Assert test)
		{
			return await ExecuteStringRequest(url, RequestMethod.GET, null, test);
		}

		private static async Task<StringGetResult> ExecuteStringRequest(
			string url, RequestMethod method, string body, Integrations.Test.Assert test, int callbackTimeoutMs = 8_000
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
			}, singleton, method, timeout: 15f);

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
	}
}
