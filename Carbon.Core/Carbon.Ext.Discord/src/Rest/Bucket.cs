/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.Collections.Generic;
using System.Threading;
using Oxide.Ext.Discord.Helpers;
using Oxide.Ext.Discord.Logging;

namespace Oxide.Ext.Discord.Rest
{
	// Token: 0x0200000D RID: 13
	public class Bucket
	{
		// Token: 0x060000A6 RID: 166 RVA: 0x00008EAC File Offset: 0x000070AC
		public Bucket(RestHandler handler, string bucketId, ILogger logger)
		{
			this.Handler = handler;
			this.BucketId = bucketId;
			this._logger = logger;
			this._logger.Debug("New Bucket Created with id: " + bucketId);
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00008F03 File Offset: 0x00007103
		public void Close()
		{
			Thread thread = this._thread;
			if (thread != null)
			{
				thread.Abort();
			}
			this._thread = null;
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00008F1F File Offset: 0x0000711F
		public bool ShouldCleanup()
		{
			return (this._thread == null || !this._thread.IsAlive) && (double)Time.TimeSinceEpoch() > this.RateLimitReset;
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00008F48 File Offset: 0x00007148
		public void QueueRequest(Request request)
		{
			request.Bucket = this;
			object syncRoot = this._syncRoot;
			lock (syncRoot)
			{
				this._requests.Add(request);
			}
			bool flag2 = this._thread == null || !this._thread.IsAlive;
			if (flag2)
			{
				this._thread = new Thread(new ThreadStart(this.RunThread))
				{
					IsBackground = true
				};
				this._thread.Start();
			}
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00008FE8 File Offset: 0x000071E8
		public void DequeueRequest(Request request)
		{
			object syncRoot = this._syncRoot;
			lock (syncRoot)
			{
				this._requests.Remove(request);
			}
		}

		// Token: 0x060000AB RID: 171 RVA: 0x00009034 File Offset: 0x00007234
		private void RunThread()
		{
			try
			{
				while (this.RequestCount() > 0)
				{
					this.FireRequests();
				}
			}
			catch (ThreadAbortException)
			{
				this._logger.Debug("Bucket thread has been aborted.");
			}
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00009084 File Offset: 0x00007284
		private void FireRequests()
		{
			double num = (double)Time.TimeSinceEpoch();
			bool hasReachedRateLimit = this.Handler.RateLimit.HasReachedRateLimit;
			if (hasReachedRateLimit)
			{
				int millisecondsTimeout = (int)(this.Handler.RateLimit.NextReset * 1000.0) + 1;
				this._logger.Debug("Global Rate limit hit. Sleeping until Reset: " + millisecondsTimeout.ToString() + "ms");
				Thread.Sleep(millisecondsTimeout);
			}
			else
			{
				bool flag = this.RateLimitRemaining == 0 && this.RateLimitReset > num;
				if (flag)
				{
					int millisecondsTimeout2 = (int)((this.RateLimitReset - num) * 1000.0);
					this._logger.Debug("Bucket Rate limit hit. Sleeping until Reset: " + millisecondsTimeout2.ToString() + "ms");
					Thread.Sleep(millisecondsTimeout2);
				}
				else
				{
					bool flag2 = this.ErrorDelayUntil > num;
					if (flag2)
					{
						int millisecondsTimeout3 = (int)((this.ErrorDelayUntil - num) * 1000.0);
						this._logger.Debug("Web request error occured delaying next send until: " + millisecondsTimeout3.ToString() + "ms ");
						Thread.Sleep(millisecondsTimeout3);
					}
					else
					{
						for (int i = 0; i < this.RequestCount(); i++)
						{
							Request request = this.GetRequest(i);
							bool flag3 = request.HasTimedOut();
							if (flag3)
							{
								request.Close(false);
							}
						}
						bool flag4 = this.RequestCount() == 0;
						if (!flag4)
						{
							this.Handler.RateLimit.FiredRequest();
							this.GetRequest(0).Fire();
						}
					}
				}
			}
		}

		// Token: 0x060000AD RID: 173 RVA: 0x00009218 File Offset: 0x00007418
		private int RequestCount()
		{
			object syncRoot = this._syncRoot;
			int count;
			lock (syncRoot)
			{
				count = this._requests.Count;
			}
			return count;
		}

		// Token: 0x060000AE RID: 174 RVA: 0x00009264 File Offset: 0x00007464
		private Request GetRequest(int index)
		{
			object syncRoot = this._syncRoot;
			Request result;
			lock (syncRoot)
			{
				result = this._requests[index];
			}
			return result;
		}

		// Token: 0x04000093 RID: 147
		public readonly string BucketId;

		// Token: 0x04000094 RID: 148
		public int RateLimit;

		// Token: 0x04000095 RID: 149
		public int RateLimitRemaining;

		// Token: 0x04000096 RID: 150
		public double RateLimitReset;

		// Token: 0x04000097 RID: 151
		public double ErrorDelayUntil;

		// Token: 0x04000098 RID: 152
		public readonly RestHandler Handler;

		// Token: 0x04000099 RID: 153
		private readonly ILogger _logger;

		// Token: 0x0400009A RID: 154
		private readonly List<Request> _requests = new List<Request>();

		// Token: 0x0400009B RID: 155
		private readonly object _syncRoot = new object();

		// Token: 0x0400009C RID: 156
		private Thread _thread;
	}
}
