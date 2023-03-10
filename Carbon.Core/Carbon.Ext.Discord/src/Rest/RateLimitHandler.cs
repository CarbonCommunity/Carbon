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

namespace Oxide.Ext.Discord.Rest
{
	// Token: 0x0200000E RID: 14
	public class RateLimitHandler
	{
		// Token: 0x060000AF RID: 175 RVA: 0x000092B0 File Offset: 0x000074B0
		public RateLimitHandler()
		{
			this._timer = new Timer(60000000.0);
			this._timer.Elapsed += this.ResetGlobal;
			this._timer.Start();
			this._lastReset = (double)Time.TimeSinceEpoch();
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x00009314 File Offset: 0x00007514
		private void ResetGlobal(object sender, ElapsedEventArgs e)
		{
			object syncRoot = this._syncRoot;
			lock (syncRoot)
			{
				this._global = 0;
				this._lastReset = (double)Time.TimeSinceEpoch();
			}
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x00009368 File Offset: 0x00007568
		public void FiredRequest()
		{
			object syncRoot = this._syncRoot;
			lock (syncRoot)
			{
				this._global++;
			}
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x000093B8 File Offset: 0x000075B8
		public void ReachedRateLimit(double retryAfter)
		{
			this._global = 110;
			this._retryAfter = retryAfter;
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x060000B3 RID: 179 RVA: 0x000093CA File Offset: 0x000075CA
		public bool HasReachedRateLimit
		{
			get
			{
				return this._global >= 110;
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x060000B4 RID: 180 RVA: 0x000093D9 File Offset: 0x000075D9
		public double NextBucketReset
		{
			get
			{
				return Math.Max(this._lastReset + 60.0, (double)Time.TimeSinceEpoch() + this._retryAfter);
			}
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x00009400 File Offset: 0x00007600
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

		// Token: 0x0400009D RID: 157
		private int _global;

		// Token: 0x0400009E RID: 158
		private Timer _timer;

		// Token: 0x0400009F RID: 159
		private double _lastReset;

		// Token: 0x040000A0 RID: 160
		private double _retryAfter;

		// Token: 0x040000A1 RID: 161
		private const int MaxRequestsPerMinute = 110;

		// Token: 0x040000A2 RID: 162
		private const int ResetInterval = 60000;

		// Token: 0x040000A3 RID: 163
		private const int ResetIntervalSeconds = 60;

		// Token: 0x040000A4 RID: 164
		private readonly object _syncRoot = new object();
	}
}
