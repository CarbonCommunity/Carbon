/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.Text;
using Oxide.Ext.Discord.Entities.Api;
using Oxide.Ext.Discord.Extensions;
using Oxide.Ext.Discord.Logging;
using Oxide.Ext.Discord.RateLimits;
using Oxide.Plugins;

namespace Oxide.Ext.Discord.Rest
{
	// Token: 0x02000010 RID: 16
	public class RestHandler
	{
		// Token: 0x060000D7 RID: 215 RVA: 0x0000A1E0 File Offset: 0x000083E0
		public RestHandler(BotClient client, ILogger logger)
		{
			this._authorization = "Bot " + client.Settings.ApiToken;
			this._logger = logger;
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x0000A238 File Offset: 0x00008438
		public void DoRequest(string url, RequestMethod method, object data, Action callback, Action<RestError> error)
		{
			this.CreateRequest(method, url, data, delegate(RestResponse response)
			{
				Action callback2 = callback;
				if (callback2 != null)
				{
					callback2();
				}
			}, error);
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x0000A26C File Offset: 0x0000846C
		public void DoRequest<T>(string url, RequestMethod method, object data, Action<T> callback, Action<RestError> error)
		{
			this.CreateRequest(method, url, data, delegate(RestResponse response)
			{
				Action<T> callback2 = callback;
				if (callback2 != null)
				{
					callback2(response.ParseData<T>());
				}
			}, error);
		}

		// Token: 0x060000DA RID: 218 RVA: 0x0000A2A0 File Offset: 0x000084A0
		private void CreateRequest(RequestMethod method, string url, object data, Action<RestResponse> callback, Action<RestError> error)
		{
			Request request = new Request(method, url, data, this._authorization, callback, error, this._logger);
			this.QueueRequest(request, this._logger);
			this.CleanupExpired();
		}

		// Token: 0x060000DB RID: 219 RVA: 0x0000A2DC File Offset: 0x000084DC
		public void CleanupExpired()
		{
			object bucketSyncObject = this._bucketSyncObject;
			lock (bucketSyncObject)
			{
				this.Buckets.RemoveAll((Bucket b) => b.ShouldCleanup());
			}
		}

		// Token: 0x060000DC RID: 220 RVA: 0x0000A348 File Offset: 0x00008548
		public void QueueRequest(Request request, ILogger logger)
		{
			string bucketId = RestHandler.GetBucketId(request.Route);
			object bucketSyncObject = this._bucketSyncObject;
			lock (bucketSyncObject)
			{
				Bucket bucket = this.Buckets[bucketId];
				bool flag2 = bucket == null;
				if (flag2)
				{
					bucket = new Bucket(this, bucketId, logger);
					this.Buckets[bucketId] = bucket;
				}
				bucket.QueueRequest(request);
			}
		}

		// Token: 0x060000DD RID: 221 RVA: 0x0000A3CC File Offset: 0x000085CC
		public void Shutdown()
		{
			object bucketSyncObject = this._bucketSyncObject;
			lock (bucketSyncObject)
			{
				foreach (Bucket bucket in this.Buckets.Values)
				{
					bucket.Close();
				}
				this.Buckets.Clear();
			}
			this.RateLimit.Shutdown();
		}

		// Token: 0x060000DE RID: 222 RVA: 0x0000A468 File Offset: 0x00008668
		private static string GetBucketId(string route)
		{
			string[] array = route.Split(new char[]
			{
				'/'
			});
			StringBuilder stringBuilder = new StringBuilder(array[0]);
			stringBuilder.Append('/');
			string text = array[0];
			int i = 1;
			while (i < array.Length)
			{
				string text2 = array[i];
				string text3 = text;
				string text4 = text3;
				if (text4 == null)
				{
					goto IL_87;
				}
				if (text4 == "reactions")
				{
					return stringBuilder.ToString();
				}
				if (!(text4 == "guilds") && !(text4 == "channels") && !(text4 == "webhooks"))
				{
					goto IL_87;
				}
				goto IL_AA;
				IL_C2:
				i++;
				continue;
				IL_87:
				ulong num;
				bool flag = ulong.TryParse(text2, out num);
				if (flag)
				{
					stringBuilder.Append("id/");
					text = text2;
					goto IL_C2;
				}
				IL_AA:
				stringBuilder.Append(text = text2);
				stringBuilder.Append("/");
				goto IL_C2;
			}
			return stringBuilder.ToString();
		}

		// Token: 0x040000BC RID: 188
		public readonly RestRateLimit RateLimit = new RestRateLimit();

		// Token: 0x040000BD RID: 189
		public readonly Hash<string, Bucket> Buckets = new Hash<string, Bucket>();

		// Token: 0x040000BE RID: 190
		private readonly string _authorization;

		// Token: 0x040000BF RID: 191
		private readonly ILogger _logger;

		// Token: 0x040000C0 RID: 192
		private readonly object _bucketSyncObject = new object();
	}
}
