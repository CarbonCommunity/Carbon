using System.Net;
using System.Text;
using Logger = Carbon.Logger;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

#pragma warning disable CS4014

namespace Oxide.Core.Libraries;

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

	public WebRequest Enqueue(string url, string body, Action<int, string> callback, Plugin owner, RequestMethod method = RequestMethod.GET, Dictionary<string, string> headers = null, float timeout = 0f, Action<int, object, Exception> onException = null)
	{
		return new WebRequest(url, callback, owner)
		{
			Method = method.ToString(),
			RequestHeaders = headers,
			Timeout = timeout,
			Body = body,
			ErrorCallback = onException
		}.Start();
	}

	public async Task<WebRequest> EnqueueAsync(string url, string body, Action<int, string> callback, Plugin owner, RequestMethod method = RequestMethod.GET, Dictionary<string, string> headers = null, float timeout = 0f, Action<int, object, Exception> onException = null)
	{
		var request = new WebRequest(url, callback, owner)
		{
			Method = method.ToString(),
			RequestHeaders = headers,
			Timeout = timeout,
			Body = body,
			ErrorCallback = onException
		}.Start();

		var tcs = new TaskCompletionSource<bool>();

		Task.Run(() =>
		{
			while (request._client != null) { }

			tcs.SetResult(true);
		});

		await tcs.Task;
		tcs = null;

		return request;
	}

	public WebRequest EnqueueData(string url, string body, Action<int, byte[]> callback, Plugin owner, RequestMethod method = RequestMethod.GET, Dictionary<string, string> headers = null, float timeout = 0f, Action<int, object, Exception> onException = null)
	{
		return new WebRequest(url, callback, owner)
		{
			Method = method.ToString(),
			RequestHeaders = headers,
			Timeout = timeout,
			Body = body,
			ErrorCallback = onException
		}.Start();
	}

	public async Task<WebRequest> EnqueueDataAsync(string url, string body, Action<int, byte[]> callback, Plugin owner, RequestMethod method = RequestMethod.GET, Dictionary<string, string> headers = null, float timeout = 0f, Action<int, object, Exception> onException = null)
	{
		var request = new WebRequest(url, callback, owner)
		{
			Method = method.ToString(),
			RequestHeaders = headers,
			Timeout = timeout,
			Body = body,
			ErrorCallback = onException
		}.Start();

		var tcs = new TaskCompletionSource<bool>();

		Task.Run(() =>
		{
			while (request._client != null) { }

			tcs.SetResult(true);
		});

		await tcs.Task;
		tcs = null;

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
		public Action<int, string> SuccessCallback { get; set; }
		public Action<int, byte[]> SuccessDataCallback { get; set; }
		public Action<int, object, Exception> ErrorCallback { get; set; }

		public float Timeout { get; set; }
		public string Method { get; set; }
		public string Url { get; }
		public string Body { get; set; }

		public int ResponseCode { get; protected set; } = 200;
		public object ResponseObject { get; protected set; }
		public Exception ResponseError { get; protected set; }

		public Plugin Owner { get; protected set; }
		public Dictionary<string, string> RequestHeaders { get; set; }

		internal bool _data;
		internal Uri _uri;
		internal Client _client;

		public WebRequest(string url, Action<int, string> callback, Plugin owner)
		{
			Url = url;
			SuccessCallback = callback;
			Owner = owner;
			_uri = new Uri(url);
			_data = false;
		}

		public WebRequest(string url, Action<int, byte[]> callback, Plugin owner)
		{
			Url = url;
			SuccessDataCallback = callback;
			Owner = owner;
			_uri = new Uri(url);
			_data = true;
		}

		public WebRequest Start()
		{
			using (_client = new Client())
			{
				_client.Headers.Add("User-Agent", Community.Runtime.Analytics.UserAgent);
				_client.Credentials = CredentialCache.DefaultCredentials;
				_client.Proxy = null;

				switch (Method)
				{
					case "GET":
						if (_data)
						{
							_client.DownloadDataCompleted += (object sender, DownloadDataCompletedEventArgs e) =>
							{
								try
								{
									if (e == null)
									{
										OnComplete(true);
										return;
									}

									if (e.Error != null)
									{
										if (e.Error is WebException web) ResponseCode = (int)(web.Response as HttpWebResponse).StatusCode;
										ResponseError = e.Error;
										OnComplete(true);
										return;
									}

									ResponseCode = _client.StatusCode;
									ResponseObject = e.Result;

									OnComplete(false);
								}
								catch { }
							};
						}
						else
						{
							_client.DownloadStringCompleted += (object sender, DownloadStringCompletedEventArgs e) =>
							{
								try
								{
									if (e == null)
									{
										OnComplete(true);
										return;
									}

									if (e.Error != null)
									{
										if (e.Error is WebException web) ResponseCode = (int)(web.Response as HttpWebResponse).StatusCode;
										ResponseError = e.Error;
										OnComplete(true);
										return;
									}

									ResponseCode = _client.StatusCode;
									ResponseObject = e.Result;

									OnComplete(false);
								}
								catch { }
							};
						}

						try
						{
							if (RequestHeaders != null && RequestHeaders.Count > 0)
							{
								foreach (var header in RequestHeaders)
								{
									_client.Headers.Add(header.Key, header.Value);
								}
							}
							if (_data)
							{
								_client.DownloadDataAsync(_uri);
							}
							else
							{
								_client.DownloadStringAsync(_uri);
							}
						}
						catch (Exception ex) { ResponseError = ex; OnComplete(true); }
						break;

					case "PUT":
					case "PATCH":
					case "POST":
					case "DELETE":
						if (_data)
						{
							_client.UploadDataCompleted += (object sender, UploadDataCompletedEventArgs e) =>
							{
								try
								{
									if (e == null)
									{
										OnComplete(true);
										return;
									}

									if (e.Error != null)
									{
										if (e.Error is WebException web) ResponseCode = (int)(web.Response as HttpWebResponse).StatusCode;
										ResponseError = e.Error;
										OnComplete(true);
										return;
									}

									ResponseCode = _client.StatusCode;
									ResponseObject = e.Result;

									OnComplete(false);
								}
								catch { }
							};
						}
						else
						{
							_client.UploadStringCompleted += (object sender, UploadStringCompletedEventArgs e) =>
							{
								try
								{
									if (e == null)
									{
										OnComplete(true);
										return;
									}

									if (e.Error != null)
									{
										if (e.Error is WebException web) ResponseCode = (int)(web.Response as HttpWebResponse).StatusCode;
										ResponseError = e.Error;
										OnComplete(true);
										return;
									}

									ResponseCode = _client.StatusCode;
									ResponseObject = e.Result;

									OnComplete(false);
								}
								catch { }
							};
						}

						try
						{
							if (RequestHeaders != null && RequestHeaders.Count > 0)
							{
								foreach (var header in RequestHeaders)
								{
									_client.Headers.Add(header.Key, header.Value);
								}
							}
							else
							{
								_client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
							}

							if (_data)
							{
								_client.UploadDataAsync(_uri, Method, Encoding.Default.GetBytes(Body));
							}
							else
							{
								_client.UploadStringAsync(_uri, Method, Body);
							}
						}
						catch (Exception ex) { ResponseError = ex; OnComplete(true); }
						break;
				}
			}

			return this;
		}

		private void OnComplete(bool failure)
		{
			Owner?.TrackStart();

			try
			{
				if (failure)
				{
					ErrorCallback?.Invoke(ResponseCode, ResponseObject, ResponseError);
				}
				else
				{
					if (_data)
					{
						SuccessDataCallback?.Invoke(ResponseCode, ResponseObject as byte[]);
					}
					else
					{
						SuccessCallback?.Invoke(ResponseCode, ResponseObject?.ToString());
					}
				}
			}
			catch (Exception ex)
			{
				var text = "Web request callback raised an exception";

				if (Owner && Owner != null)
				{
					text += $" in '{Owner.Name} v{Owner.Version}' plugin";
				}

				try { Logger.Error(text, ex); } catch { Logger.Error($"{text}", ex); }
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

			protected override WebResponse GetWebResponse(System.Net.WebRequest request, IAsyncResult result)
			{
				var response = base.GetWebResponse(request, result);

				StatusCode = (int)(request.GetResponse() as HttpWebResponse).StatusCode;

				return response;
			}
			protected override WebResponse GetWebResponse(System.Net.WebRequest request)
			{
				var response = base.GetWebResponse(request);

				StatusCode = (int)(request.GetResponse() as HttpWebResponse).StatusCode;

				return response;
			}

			protected override System.Net.WebRequest GetWebRequest(Uri address)
			{
				if (!Community.IsConfigReady || string.IsNullOrEmpty(Community.Runtime.Config.WebRequestIp))
				{
					return base.GetWebRequest(address);
				}

				var request = base.GetWebRequest(address) as HttpWebRequest;

				request.ServicePoint.BindIPEndPointDelegate = (servicePoint, remoteEndPoint, retryCount) =>
				{
					return new IPEndPoint(IPAddress.Parse(Community.Runtime.Config.WebRequestIp), 0);
				};

				return request;
			}

			public new void Dispose()
			{
				base.Dispose();
			}
		}
	}
}
