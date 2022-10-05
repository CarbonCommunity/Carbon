///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using System.Collections.Generic;
using System.Net;
using Carbon;
using Carbon.Core;
using Oxide.Plugins;

namespace Oxide.Core.Libraries
{
	public enum RequestMethod
	{
		DELETE,
		GET,
		PATCH,
		POST,
		PUT
	}

	public class WebRequests
	{
		public WebRequests()
		{
			ServicePointManager.Expect100Continue = false;
			ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, error) => true;
			ServicePointManager.DefaultConnectionLimit = 200;
		}

		public WebRequest Enqueue(string url, string body, Action<int, string> callback, Plugin owner, RequestMethod method = RequestMethod.GET, Dictionary<string, string> headers = null, float timeout = 0f)
		{
			return new WebRequest(url, callback, owner)
			{
				Method = method.ToString(),
				RequestHeaders = headers,
				Timeout = timeout,
				Body = body
			}.Start();
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
			public Action<int, string> SuccessCallback { get; }
			public Action<int, string, Exception> ErrorCallback { get; }

			public float Timeout { get; set; }
			public string Method { get; set; }
			public string Url { get; }
			public string Body { get; set; }

			public int ResponseCode { get; protected set; } = 200;
			public string ResponseText { get; protected set; }
			public Exception ResponseError { get; protected set; }

			public Plugin Owner { get; protected set; }
			public Dictionary<string, string> RequestHeaders { get; set; }

			internal Uri _uri;
			internal WebClient _client;

			public WebRequest(string url, Action<int, string> callback, Plugin owner)
			{
				Url = url;
				SuccessCallback = callback;
				Owner = owner;
				_uri = new Uri(url);
			}

			public WebRequest Start()
			{
				_client = new WebClient();
				_client.Headers.Add("User-Agent", $"Carbon Mod (v{CarbonCore.Version}; https://github.com/Carbon-Modding/Carbon.Core");
				_client.Credentials = CredentialCache.DefaultCredentials;
				_client.Proxy = null;

				switch (Method)
				{
					case "GET":
						_client.DownloadStringCompleted += (object sender, DownloadStringCompletedEventArgs e) =>
						{
							ResponseText = e.Result;
							ResponseError = e.Error;

							if (e.Error != null)
							{
								ResponseCode = 400;

								OnComplete(true);
								return;
							}

							OnComplete(false);
						};

						_client.DownloadStringAsync(_uri);
						break;

					case "PUT":
					case "POST":
						_client.UploadStringCompleted += (object sender, UploadStringCompletedEventArgs e) =>
						{
							ResponseText = e.Result;
							ResponseError = e.Error;

							if (e.Error != null)
							{
								ResponseCode = 400;

								OnComplete(true);
								return;
							}

							OnComplete(false);
						};

						_client.UploadStringAsync(_uri, Body);
						break;
				}

				return this;
			}

			private void OnComplete(bool failure)
			{
				Owner?.TrackStart();

				try
				{
					if (failure) ErrorCallback(ResponseCode, ResponseText, ResponseError);
					else SuccessCallback(ResponseCode, ResponseText);
				}
				catch (Exception ex)
				{
					var text = "Web request callback raised an exception";

					if (Owner && Owner != null)
					{
						text += $" in '{Owner.Name} v{Owner.Version}' plugin";
					}

					Carbon.Logger.Error(text, ex);
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
		}
	}
}
