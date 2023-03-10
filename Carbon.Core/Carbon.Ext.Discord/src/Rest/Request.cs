/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Carbon;
using Newtonsoft.Json;
using Oxide.Core;
using Oxide.Ext.Discord.Entities.Api;
using Oxide.Ext.Discord.Entities.Messages;
using Oxide.Ext.Discord.Helpers;
using Oxide.Ext.Discord.Interfaces;
using Oxide.Ext.Discord.Logging;
using Oxide.Ext.Discord.Rest.Multipart;

namespace Oxide.Ext.Discord.Rest
{
	// Token: 0x0200000F RID: 15
	public class Request
	{
		// Token: 0x17000010 RID: 16
		// (get) Token: 0x060000B6 RID: 182 RVA: 0x0000943D File Offset: 0x0000763D
		public RequestMethod Method { get; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x060000B7 RID: 183 RVA: 0x00009445 File Offset: 0x00007645
		public string Route { get; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x060000B8 RID: 184 RVA: 0x0000944D File Offset: 0x0000764D
		public string RequestUrl
		{
			get
			{
				return "https://discord.com/api/v9" + this.Route;
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x060000B9 RID: 185 RVA: 0x0000945F File Offset: 0x0000765F
		public object Data { get; }

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x060000BA RID: 186 RVA: 0x00009467 File Offset: 0x00007667
		// (set) Token: 0x060000BB RID: 187 RVA: 0x0000946F File Offset: 0x0000766F
		public byte[] Contents { get; set; }

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x060000BC RID: 188 RVA: 0x00009478 File Offset: 0x00007678
		// (set) Token: 0x060000BD RID: 189 RVA: 0x00009480 File Offset: 0x00007680
		internal List<IMultipartSection> MultipartSections { get; set; }

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x060000BE RID: 190 RVA: 0x00009489 File Offset: 0x00007689
		public bool MultipartRequest { get; }

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x060000BF RID: 191 RVA: 0x00009491 File Offset: 0x00007691
		// (set) Token: 0x060000C0 RID: 192 RVA: 0x00009499 File Offset: 0x00007699
		public string Boundary { get; set; }

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x060000C1 RID: 193 RVA: 0x000094A2 File Offset: 0x000076A2
		// (set) Token: 0x060000C2 RID: 194 RVA: 0x000094AA File Offset: 0x000076AA
		public RestResponse Response { get; private set; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x060000C3 RID: 195 RVA: 0x000094B3 File Offset: 0x000076B3
		public Action<RestResponse> Callback { get; }

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x060000C4 RID: 196 RVA: 0x000094BB File Offset: 0x000076BB
		public Action<RestError> OnError { get; }

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x060000C5 RID: 197 RVA: 0x000094C3 File Offset: 0x000076C3
		// (set) Token: 0x060000C6 RID: 198 RVA: 0x000094CB File Offset: 0x000076CB
		public DateTime? StartTime { get; private set; }

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x060000C7 RID: 199 RVA: 0x000094D4 File Offset: 0x000076D4
		// (set) Token: 0x060000C8 RID: 200 RVA: 0x000094DC File Offset: 0x000076DC
		public bool InProgress { get; set; }

		// Token: 0x060000C9 RID: 201 RVA: 0x000094E8 File Offset: 0x000076E8
		public Request(RequestMethod method, string route, object data, string authHeader, Action<RestResponse> callback, Action<RestError> onError, ILogger logger)
		{
			this.Method = method;
			this.Route = route;
			this.Data = data;
			this._authHeader = authHeader;
			this.Callback = callback;
			this.OnError = onError;
			this._logger = logger;
			IFileAttachments fileAttachments = this.Data as IFileAttachments;
			this.MultipartRequest = (fileAttachments != null && fileAttachments.FileAttachments != null && fileAttachments.FileAttachments.Count != 0);
		}

		// Token: 0x060000CA RID: 202 RVA: 0x00009560 File Offset: 0x00007760
		public void Fire()
		{
			this.InProgress = true;
			this.StartTime = new DateTime?(DateTime.UtcNow);
			HttpWebRequest httpWebRequest = this.CreateRequest();
			try
			{
				this.WriteRequestData(httpWebRequest);
				using (HttpWebResponse httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse)
				{
					bool flag = httpWebResponse != null;
					if (flag)
					{
						this.ParseResponse(httpWebResponse);
					}
				}
				this._success = true;
				Interface.Oxide.NextTick(delegate()
				{
					Action<RestResponse> callback = this.Callback;
					if (callback != null)
					{
						callback(this.Response);
					}
				});
				this.Close(true);
			}
			catch (WebException ex)
			{
				using (HttpWebResponse httpWebResponse2 = ex.Response as HttpWebResponse)
				{
					this._lastError = new RestError(ex, httpWebRequest.RequestUri, this.Method, this.Data);
					bool flag2 = httpWebResponse2 == null;
					if (flag2)
					{
						this.Bucket.ErrorDelayUntil = (double)(Time.TimeSinceEpoch() + 1);
						this.Close(false);
						this._logger.Exception(string.Format("A web request exception occured (internal error) [RETRY={0}/3].\nRequest URL: [{1}] {2}", this._retries.ToString(), httpWebRequest.Method, httpWebRequest.RequestUri), ex);
					}
					else
					{
						int statusCode = (int)httpWebResponse2.StatusCode;
						this._lastError.HttpStatusCode = statusCode;
						string text = this.ParseResponse(ex.Response);
						this._lastError.Message = text;
						bool flag3 = statusCode == 429;
						bool flag4 = flag3;
						if (flag4)
						{
							this._logger.Warning(string.Format("Discord rate limit reached. (Rate limit info: remaining: [{0}] Route: {1} Content-Type: {2} Remaining: {3} Limit: {4}, Reset In: {5}, Current Time: {6}", new object[]
							{
								httpWebRequest.Method,
								httpWebRequest.RequestUri,
								httpWebRequest.ContentType,
								this.Bucket.RateLimitRemaining.ToString(),
								this.Bucket.RateLimit.ToString(),
								this.Bucket.RateLimitReset.ToString(),
								Time.TimeSinceEpoch().ToString()
							}));
							this.Close(false);
						}
						else
						{
							DiscordApiError discordApiError = this.Response.ParseData<DiscordApiError>();
							this._lastError.DiscordError = discordApiError;
							bool flag5 = discordApiError != null && discordApiError.Code != 0;
							if (flag5)
							{
								this._logger.Error(string.Format("Discord API has returned error Discord Code: {0} Discord Error: {1} Request: [{2}] {3} (Response Code: {4}) Content-Type: {5}", new object[]
								{
									discordApiError.Code.ToString(),
									discordApiError.Message,
									httpWebRequest.Method,
									httpWebRequest.RequestUri,
									httpWebResponse2.StatusCode.ToString(),
									httpWebRequest.ContentType
								}) + string.Format("\nDiscord Errors: {0}", discordApiError.Errors) + "\nRequest Body:\n" + ((this.Contents != null) ? Encoding.UTF8.GetString(this.Contents) : "Contents is null"));
							}
							else
							{
								this._logger.Error(string.Format("An error occured whilst submitting a request: Exception Status: {0} Request: [{1}] {2} (Response Code: {3}): {4}", new object[]
								{
									ex.Status.ToString(),
									httpWebRequest.Method,
									httpWebRequest.RequestUri,
									httpWebResponse2.StatusCode.ToString(),
									text
								}));
							}
							this.Close(true);
						}
					}
				}
			}
			catch (Exception ex2)
			{
				this._logger.Exception(string.Format("An exception occured for request: [{0}] {1}", httpWebRequest.Method, httpWebRequest.RequestUri), ex2);
				this.Close(true);
			}
		}

		// Token: 0x060000CB RID: 203 RVA: 0x00009948 File Offset: 0x00007B48
		private HttpWebRequest CreateRequest()
		{
			this.SetRequestBody();
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(this.RequestUrl);
			httpWebRequest.Method = this.Method.ToString();
			httpWebRequest.UserAgent = "DiscordBot (https://github.com/CarbonCommunity/Carbon.Core, " + Community.InformationalVersion + ")";
			httpWebRequest.Timeout = 15000;
			httpWebRequest.ContentLength = 0L;
			httpWebRequest.Headers.Set("Authorization", this._authHeader);
			httpWebRequest.ContentType = (this.MultipartRequest ? ("multipart/form-data;boundary=\"" + this.Boundary + "\"") : "application/json");
			return httpWebRequest;
		}

		// Token: 0x060000CC RID: 204 RVA: 0x00009A00 File Offset: 0x00007C00
		private byte[] GetMultipartFormData()
		{
			StringBuilder sb = new StringBuilder();
			byte[] bytes = Encoding.UTF8.GetBytes(this.Boundary);
			List<byte> list = new List<byte>();
			foreach (IMultipartSection section in this.MultipartSections)
			{
				this.AddMultipartSection(sb, section, list, bytes);
			}
			list.AddRange(Request.NewLine);
			list.AddRange(Request.Separator);
			list.AddRange(bytes);
			list.AddRange(Request.Separator);
			list.AddRange(Request.NewLine);
			return list.ToArray();
		}

		// Token: 0x060000CD RID: 205 RVA: 0x00009AC0 File Offset: 0x00007CC0
		private void AddMultipartSection(StringBuilder sb, IMultipartSection section, List<byte> data, byte[] boundary)
		{
			sb.Length = 0;
			sb.Append("Content-Disposition: form-data; name=\"");
			sb.Append(section.SectionName);
			sb.Append("\"");
			bool flag = section.FileName != null;
			if (flag)
			{
				sb.Append("; filename=\"");
				sb.Append(section.FileName);
				sb.Append("\"");
			}
			bool flag2 = !string.IsNullOrEmpty(section.ContentType);
			if (flag2)
			{
				sb.AppendLine();
				sb.Append("Content-Type: ");
				sb.Append(section.ContentType);
			}
			sb.AppendLine();
			data.AddRange(Request.NewLine);
			data.AddRange(Request.Separator);
			data.AddRange(boundary);
			data.AddRange(Request.NewLine);
			data.AddRange(Encoding.UTF8.GetBytes(sb.ToString()));
			data.AddRange(Request.NewLine);
			data.AddRange(section.Data);
		}

		// Token: 0x060000CE RID: 206 RVA: 0x00009BC8 File Offset: 0x00007DC8
		private void SetRequestBody()
		{
			bool flag = this.Data == null || this.Contents != null;
			if (!flag)
			{
				bool multipartRequest = this.MultipartRequest;
				if (multipartRequest)
				{
					IFileAttachments fileAttachments = (IFileAttachments)this.Data;
					this.MultipartSections = new List<IMultipartSection>
					{
						new MultipartFormSection("payload_json", this.Data, "application/json")
					};
					for (int i = 0; i < fileAttachments.FileAttachments.Count; i++)
					{
						MessageFileAttachment messageFileAttachment = fileAttachments.FileAttachments[i];
						this.MultipartSections.Add(new MultipartFileSection("files[" + (i + 1).ToString() + "]", messageFileAttachment.FileName, messageFileAttachment.Data, messageFileAttachment.ContentType));
					}
					this.Boundary = Guid.NewGuid().ToString().Replace("-", "");
					this.Contents = this.GetMultipartFormData();
				}
				else
				{
					this.Contents = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this.Data, DiscordExtension.ExtensionSerializeSettings));
				}
			}
		}

		// Token: 0x060000CF RID: 207 RVA: 0x00009D00 File Offset: 0x00007F00
		public void Close(bool remove = true)
		{
			this._retries += 1;
			bool flag = remove || this._retries >= 3;
			if (flag)
			{
				bool flag2 = !this._success;
				if (flag2)
				{
					try
					{
						Interface.Oxide.NextTick(delegate()
						{
							Action<RestError> onError = this.OnError;
							if (onError != null)
							{
								onError(this._lastError);
							}
						});
					}
					catch (Exception ex)
					{
						this._logger.Exception("An exception occured during OnError callback for request: [" + this.Method.ToString() + "] " + this.RequestUrl, ex);
					}
				}
				this.Bucket.DequeueRequest(this);
			}
			else
			{
				this.InProgress = false;
				this.StartTime = null;
			}
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x00009DD4 File Offset: 0x00007FD4
		public bool HasTimedOut()
		{
			bool flag = !this.InProgress || this.StartTime == null;
			return !flag && (DateTime.UtcNow - this.StartTime.Value).TotalSeconds > 15.0;
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x00009E38 File Offset: 0x00008038
		private void WriteRequestData(WebRequest request)
		{
			bool flag = this.Contents == null || this.Contents.Length == 0;
			if (!flag)
			{
				request.ContentLength = (long)this.Contents.Length;
				using (Stream requestStream = request.GetRequestStream())
				{
					requestStream.Write(this.Contents, 0, this.Contents.Length);
				}
			}
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x00009EB0 File Offset: 0x000080B0
		private string ParseResponse(WebResponse response)
		{
			string result;
			using (Stream responseStream = response.GetResponseStream())
			{
				bool flag = responseStream == null;
				if (flag)
				{
					result = null;
				}
				else
				{
					using (StreamReader streamReader = new StreamReader(responseStream))
					{
						string text = streamReader.ReadToEnd().Trim();
						this.Response = new RestResponse(text);
						this.ParseHeaders(response.Headers, this.Response);
						result = text;
					}
				}
			}
			return result;
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x00009F44 File Offset: 0x00008144
		private void ParseHeaders(WebHeaderCollection headers, RestResponse response)
		{
			string text = headers.Get("Retry-After");
			string value = headers.Get("X-RateLimit-Global");
			int num = 0;
			bool flag2;
			bool flag = !string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(value) && int.TryParse(text, out num) && bool.TryParse(value, out flag2) && flag2;
			if (flag)
			{
				RateLimit rateLimit = response.ParseData<RateLimit>();
				bool global = rateLimit.Global;
				if (global)
				{
					this.Bucket.Handler.RateLimit.ReachedRateLimit((double)num);
				}
			}
			string text2 = headers.Get("X-RateLimit-Limit");
			string text3 = headers.Get("X-RateLimit-Remaining");
			string text4 = headers.Get("X-RateLimit-Reset-After");
			string text5 = headers.Get("X-RateLimit-Bucket");
			int rateLimit2 = 0;
			bool flag3 = !string.IsNullOrEmpty(text2) && int.TryParse(text2, out rateLimit2);
			if (flag3)
			{
				this.Bucket.RateLimit = rateLimit2;
			}
			int rateLimitRemaining = 0;
			bool flag4 = !string.IsNullOrEmpty(text3) && int.TryParse(text3, out rateLimitRemaining);
			if (flag4)
			{
				this.Bucket.RateLimitRemaining = rateLimitRemaining;
			}
			double num2 = (double)Time.TimeSinceEpoch();
			double num3 = 0;
			bool flag5 = !string.IsNullOrEmpty(text4) && double.TryParse(text4, out num3);
			if (flag5)
			{
				double num4 = num2 + num3;
				bool flag6 = num4 > this.Bucket.RateLimitReset;
				if (flag6)
				{
					this.Bucket.RateLimitReset = num4;
				}
			}
			this._logger.Debug(string.Concat(new string[]
			{
				"Method: ",
				this.Method.ToString(),
				" Route: ",
				this.Route,
				" Internal Bucket Id: ",
				this.Bucket.BucketId,
				" Limit: ",
				this.Bucket.RateLimit.ToString(),
				" Remaining: ",
				this.Bucket.RateLimitRemaining.ToString(),
				" Reset: ",
				this.Bucket.RateLimitReset.ToString(),
				" Time: ",
				Time.TimeSinceEpoch().ToString(),
				" Bucket: ",
				text5
			}));
		}

		// Token: 0x040000B1 RID: 177
		internal Bucket Bucket;

		// Token: 0x040000B2 RID: 178
		public const string UrlBase = "https://discord.com/api";

		// Token: 0x040000B3 RID: 179
		public const string ApiVersion = "v9";

		// Token: 0x040000B4 RID: 180
		private const int TimeoutDuration = 15;

		// Token: 0x040000B5 RID: 181
		private readonly string _authHeader;

		// Token: 0x040000B6 RID: 182
		private byte _retries;

		// Token: 0x040000B7 RID: 183
		private readonly ILogger _logger;

		// Token: 0x040000B8 RID: 184
		private RestError _lastError;

		// Token: 0x040000B9 RID: 185
		private bool _success;

		// Token: 0x040000BA RID: 186
		private static readonly byte[] NewLine = Encoding.UTF8.GetBytes("\r\n");

		// Token: 0x040000BB RID: 187
		private static readonly byte[] Separator = Encoding.UTF8.GetBytes("--");
	}
}
