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

		private static async Task<StringGetResult> ExecuteStringGet(string url, Integrations.Test.Assert test)
		{
			var callbackTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

			var callbackCount = 0;
			var callbackCode = 0;
			var callbackThreadId = -1;
			string callbackBody = null;

			var request = singleton.webrequest.Enqueue(url, null, (code, body) =>
			{
				Interlocked.Increment(ref callbackCount);
				callbackCode = code;
				callbackBody = body;
				callbackThreadId = Thread.CurrentThread.ManagedThreadId;
				callbackTcs.TrySetResult(true);
			}, singleton, timeout: 15f);

			var callbackFinished = await Task.WhenAny(callbackTcs.Task, Task.Delay(8_000));
			test.IsTrue(callbackFinished == callbackTcs.Task, $"callback invoked ({url})");

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
