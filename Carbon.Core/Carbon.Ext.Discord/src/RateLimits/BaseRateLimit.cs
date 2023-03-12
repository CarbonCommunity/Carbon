/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.Timers;
using Oxide.Ext.Discord.Helpers;

namespace Oxide.Ext.Discord.RateLimits
{
	// Token: 0x02000014 RID: 20
	public class BaseRateLimit
	{
		// Token: 0x060000EF RID: 239 RVA: 0x0000A608 File Offset: 0x00008808
		protected BaseRateLimit(int maxRequests, double interval)
		{
			this.MaxRequests = maxRequests;
			this.ResetInterval = interval;
			this._timer = new Timer(interval);
			this._timer.Elapsed += this.ResetRateLimit;
			this._timer.Start();
			this.LastReset = (double)Time.TimeSinceEpoch();
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x0000A674 File Offset: 0x00008874
		private void ResetRateLimit(object sender, ElapsedEventArgs e)
		{
			object syncRoot = this._syncRoot;
			lock (syncRoot)
			{
				this.NumRequests = 0;
				this.LastReset = (double)Time.TimeSinceEpoch();
			}
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x0000A6C8 File Offset: 0x000088C8
		public void FiredRequest()
		{
			object syncRoot = this._syncRoot;
			lock (syncRoot)
			{
				this.NumRequests++;
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060000F2 RID: 242 RVA: 0x0000A718 File Offset: 0x00008918
		public bool HasReachedRateLimit
		{
			get
			{
				return this.NumRequests >= this.MaxRequests;
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060000F3 RID: 243 RVA: 0x0000A72B File Offset: 0x0000892B
		public virtual double NextReset
		{
			get
			{
				return (double)Time.TimeSinceEpoch() + this.ResetInterval * 1000.0 - this.LastReset;
			}
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x0000A74C File Offset: 0x0000894C
		public void Shutdown()
		{
			bool flag = this._timer == null;
			if (!flag)
			{
				this._timer.Stop();
				this._timer.Dispose();
				this._timer = null;
			}
		}

		// Token: 0x040000C8 RID: 200
		protected int NumRequests;

		// Token: 0x040000C9 RID: 201
		protected double LastReset;

		// Token: 0x040000CA RID: 202
		protected readonly int MaxRequests;

		// Token: 0x040000CB RID: 203
		protected readonly double ResetInterval;

		// Token: 0x040000CC RID: 204
		private Timer _timer;

		// Token: 0x040000CD RID: 205
		private readonly object _syncRoot = new object();
	}
}
