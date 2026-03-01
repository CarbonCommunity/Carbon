using System.Net;
using System.Text;
using Logger = Carbon.Logger;

namespace Oxide.Core.Libraries;

#pragma warning disable CS4014

public enum RequestMethod
{
	DELETE,
	GET,
	PATCH,
	POST,
	PUT
}

public class WebRequests : Library
{
	public WebRequests()
	{
		ServicePointManager.Expect100Continue = false;
		ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, error) => true;
		ServicePointManager.DefaultConnectionLimit = 200;
	}

	public WebRequest Enqueue(string url, string body, Action<int, string> callback, Plugin owner, RequestMethod method = RequestMethod.GET, Dictionary<string, string> headers = null, float timeout = 0f, DecompressionMethods decompressionMethod = DecompressionMethods.None)
	{
		return new WebRequest(url, callback, owner)
		{
			Method = method.ToString(),
			RequestHeaders = headers,
			Timeout = timeout,
			Body = body,
			DecompressionMethod = decompressionMethod
		}.Start();
	}
	public WebRequest EnqueueData(string url, string body, Action<int, byte[]> callback, Plugin owner, RequestMethod method = RequestMethod.GET, Dictionary<string, string> headers = null, float timeout = 0f, DecompressionMethods decompressionMethod = DecompressionMethods.None)
	{
		return new WebRequest(url, callback, owner)
		{
			Method = method.ToString(),
			RequestHeaders = headers,
			Timeout = timeout,
			Body = body,
			DecompressionMethod = decompressionMethod
		}.Start();
	}

	public async Task<WebRequest> EnqueueAsync(string url, string body, Action<int, string> callback, Plugin owner, RequestMethod method = RequestMethod.GET, Dictionary<string, string> headers = null, float timeout = 0f, DecompressionMethods decompressionMethod = DecompressionMethods.None)
	{
		var tcs = new TaskCompletionSource<bool>();

		WebRequest request = default;

		request = new WebRequest(url, (code, data) =>
		{
			try
			{
				callback?.Invoke(code, data);
			}
			catch (Exception ex)
			{
				Logger.Error($"Failed executing '{request.Method}' async webrequest [callback] ({request.Url})", ex);
			}

			tcs.SetResult(true);
		}, owner)
		{
			Method = method.ToString(),
			RequestHeaders = headers,
			Timeout = timeout,
			Body = body,
			DecompressionMethod = decompressionMethod
		}.Start();

		await tcs.Task;

		return request;
	}
	public async Task<WebRequest> EnqueueDataAsync(string url, string body, Action<int, byte[]> callback, Plugin owner, RequestMethod method = RequestMethod.GET, Dictionary<string, string> headers = null, float timeout = 0f, DecompressionMethods decompressionMethod = DecompressionMethods.None)
	{
		var tcs = new TaskCompletionSource<bool>();

		WebRequest request = default;

		request = new WebRequest(url, (code, data) =>
		{
			try
			{
				callback?.Invoke(code, data);
			}
			catch (Exception ex)
			{
				Logger.Error($"Failed executing '{request.Method}' async webrequest [callback] ({request.Url})", ex);
			}

			tcs.SetResult(true);
		}, owner)
		{
			Method = method.ToString(),
			RequestHeaders = headers,
			Timeout = timeout,
			Body = body,
			DecompressionMethod = decompressionMethod
		}.Start();

		await tcs.Task;

		return request;
	}

	[Obsolete("EnqueueGet is deprecated, use Enqueue instead")]
	public void EnqueueGet(string url, Action<int, string> callback, Plugin owner, Dictionary<string, string> headers = null, float timeout = 0f)
	{
		Enqueue(url, null, callback, owner, RequestMethod.GET, headers, timeout);
	}

	[Obsolete("EnqueuePost is deprecated, use Enqueue instead")]
	public void EnqueuePost(string url, string body, Action<int, string> callback, Plugin owner, Dictionary<string, string> headers = null, float timeout = 0f)
	{
		Enqueue(url, body, callback, owner, RequestMethod.POST, headers, timeout);
	}

	[Obsolete("EnqueuePut is deprecated, use Enqueue instead")]
	public void EnqueuePut(string url, string body, Action<int, string> callback, Plugin owner, Dictionary<string, string> headers = null, float timeout = 0f)
	{
		Enqueue(url, body, callback, owner, RequestMethod.PUT, headers, timeout);
	}

	public class WebRequest : IDisposable
	{
		public Action<int, string> Callback { get; set; }
		public Action<int, byte[]> DataCallback { get; set; }

		public float Timeout { get; set; }
		public string Method { get; set; }
		public string Url { get; }
		public string Body { get; set; }
		public DecompressionMethods DecompressionMethod { get; set; } = DecompressionMethods.GZip;

		public TimeSpan ResponseDuration { get; protected set; }
		public int ResponseCode { get; protected set; }
		public object ResponseObject { get; protected set; } = string.Empty;
		public Exception ResponseError { get; protected set; }

		public Plugin Owner { get; protected set; }
		public Dictionary<string, string> RequestHeaders { get; set; }

		internal DateTime _time;
		internal bool _data;
		internal Uri _uri;
		internal Client _client;

		public WebRequest(string url, Action<int, string> callback, Plugin owner)
		{
			Url = url;
			Callback = callback;
			Owner = owner;
			_uri = new Uri(url);
			_data = false;
		}
		public WebRequest(string url, Action<int, byte[]> callback, Plugin owner)
		{
			Url = url;
			DataCallback = callback;
			Owner = owner;
			_uri = new Uri(url);
			_data = true;
		}

		public WebRequest Start()
		{
			_client = new Client();
			_client.Headers["User-Agent"] = Community.Runtime.Analytics.UserAgent;

			if (Method != "GET")
			{
				_client.Headers["Content-Type"] = "application/x-www-form-urlencoded";
			}

			_client.Credentials = CredentialCache.DefaultCredentials;
			_client.Proxy = null;
			_client.Encoding = Encoding.UTF8;
			_client.AutomaticDecompression = DecompressionMethod;

			if (RequestHeaders != null && RequestHeaders.Count > 0)
			{
				foreach (var header in RequestHeaders)
				{
					_client.Headers[header.Key] = header.Value;
				}
			}

			switch (Method)
			{
				case "GET":
					_time = DateTime.Now;

					try
					{
						if (_data)
						{
							_client.DownloadDataCompleted += (_, e) =>
							{
								ResponseDuration = DateTime.Now - _time;
								ResponseCode = _client.StatusCode;

								try
								{
									if (e == null)
									{
										OnComplete();
										return;
									}

									if (e.Error != null)
									{
										ResponseError = e.Error;
										Logger.Error($"Failed executing '{Method}' webrequest [response] ({Url})", e.Error);
										OnComplete();
										return;
									}

									ResponseObject = e.Result;
									OnComplete();
								}
								catch (Exception ex)
								{
									Logger.Error($"Failed executing '{Method}' webrequest [internal] ({Url})", ex);
									OnComplete();
								}
							};
							_client.DownloadDataAsync(_uri);
						}
						else
						{
							_client.DownloadStringCompleted += (_, e) =>
							{
								ResponseDuration = DateTime.Now - _time;
								ResponseCode = _client.StatusCode;

								try
								{
									if (e == null)
									{
										OnComplete();
										return;
									}

									if (e.Error != null)
									{
										ResponseError = e.Error;
										Logger.Error($"Failed executing '{Method}' webrequest [response] ({Url})", e.Error);
										OnComplete();
										return;
									}

									ResponseObject = e.Result;
									OnComplete();
								}
								catch (Exception ex)
								{
									Logger.Error($"Failed executing '{Method}' webrequest [internal] ({Url})", ex);
									OnComplete();
								}
							};
							_client.DownloadStringAsync(_uri);
						}
					}
					catch (Exception ex)
					{
						Logger.Error($"Failed executing '{Method}' webrequest [internal] ({Url})", ex);
						ResponseCode = _client.StatusCode;
						ResponseError = ex;
						OnComplete();
					}

					break;

				case "PUT":
				case "PATCH":
				case "POST":
				case "DELETE":
					_time = DateTime.Now;

					try
					{
						if (_data)
						{
							_client.UploadDataCompleted += (_, e) =>
							{
								ResponseDuration = DateTime.Now - _time;
								ResponseCode = _client.StatusCode;

								try
								{
									if (e == null)
									{
										OnComplete();
										return;
									}

									if (e.Error != null)
									{
										ResponseError = e.Error;
										Logger.Error($"Failed executing '{Method}' webrequest [response] ({Url})", e.Error);
										OnComplete();
										return;
									}

									ResponseObject = e.Result;
									OnComplete();
								}
								catch (Exception ex)
								{
									Logger.Error($"Failed executing '{Method}' webrequest [internal] ({Url})", ex);
									OnComplete();
								}
							};
							_client.UploadDataAsync(_uri, Method, Encoding.Default.GetBytes(Body));
						}
						else
						{
							_client.UploadStringCompleted += (_, e) =>
							{
								ResponseDuration = DateTime.Now - _time;
								ResponseCode = _client.StatusCode;

								try
								{
									if (e == null)
									{
										OnComplete();
										return;
									}

									if (e.Error != null)
									{
										ResponseError = e.Error;
										Logger.Error($"Failed executing '{Method}' webrequest [response] ({Url})", e.Error);
										OnComplete();
										return;
									}

									ResponseObject = e.Result;
									OnComplete();
								}
								catch (Exception ex)
								{
									Logger.Error($"Failed executing '{Method}' webrequest [internal] ({Url})", ex);
									OnComplete();
								}
							};
							_client.UploadStringAsync(_uri, Method, string.IsNullOrEmpty(Body) ? string.Empty : Body);
						}
					}
					catch (Exception ex)
					{
						Logger.Error($"Failed executing '{Method}' webrequest [internal] ({Url})", ex);
						ResponseCode = _client.StatusCode;
						ResponseError = ex;
						OnComplete();
					}

					break;
			}

			return this;
		}

		private void OnComplete()
		{
			Owner?.TrackStart();

			var text = "Web request callback raised an exception";

			if (Owner && Owner != null)
			{
				text += $" in '{Owner.ToPrettyString()}' plugin";
			}

			try
			{
				if (_data)
				{
					DataCallback?.Invoke(ResponseCode, ResponseObject as byte[]);
				}
				else
				{
					Callback?.Invoke(ResponseCode, ResponseObject?.ToString());
				}
			}
			catch (Exception ex)
			{
				Logger.Error($"{text} [{ResponseCode}]", ex);
			}

			Owner?.TrackEnd();
			Dispose();
		}
		public void Dispose()
		{
			Owner = null;

			_uri = null;

			_client?.Dispose();
			_client = null;
		}

		public class Client : WebClient
		{
			public int StatusCode { get; private set; }
			public DecompressionMethods AutomaticDecompression { get; set; } = DecompressionMethods.GZip;

			public Client()
			{
				Encoding = Encoding.UTF8;
			}

			protected override WebResponse GetWebResponse(System.Net.WebRequest request, IAsyncResult result)
			{
				WebResponse response = null;

				try
				{
					response = base.GetWebResponse(request, result);

					if (response is HttpWebResponse httpResponse)
					{
						StatusCode = (int)httpResponse.StatusCode;
					}
				}
				catch (WebException exp)
				{
					response = exp.Response;

					if (response is HttpWebResponse httpResponse)
					{
						StatusCode = (int)httpResponse.StatusCode;
					}
				}

				return response;
			}

			protected override WebResponse GetWebResponse(System.Net.WebRequest request)
			{
				WebResponse response = null;

				try
				{
					response = base.GetWebResponse(request);

					if (response is HttpWebResponse httpResponse)
					{
						StatusCode = (int)httpResponse.StatusCode;
					}
				}
				catch (WebException exp)
				{
					response = exp.Response;

					if (response is HttpWebResponse httpResponse)
					{
						StatusCode = (int)httpResponse.StatusCode;
					}
				}

				return response;
			}

			protected override System.Net.WebRequest GetWebRequest(Uri address)
			{
				var request = base.GetWebRequest(address) as HttpWebRequest;

				request.UserAgent = Community.Runtime.Analytics.UserAgent;
				request.AutomaticDecompression = AutomaticDecompression;

				if (Community.IsConfigReady && !string.IsNullOrEmpty(Community.Runtime.Config.WebRequestIp))
				{
					request.ServicePoint.BindIPEndPointDelegate = (_, _, _) => new IPEndPoint(IPAddress.Parse(Community.Runtime.Config.WebRequestIp), 0);
				}

				return request;
			}

			public new void Dispose()
			{
				base.Dispose();
			}
		}
	}
}
