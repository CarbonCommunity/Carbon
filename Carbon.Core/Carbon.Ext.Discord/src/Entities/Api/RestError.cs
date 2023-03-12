/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.Net;

namespace Oxide.Ext.Discord.Entities.Api
{
	// Token: 0x0200011C RID: 284
	public class RestError
	{
		// Token: 0x170003BD RID: 957
		// (get) Token: 0x06000A84 RID: 2692 RVA: 0x00017D27 File Offset: 0x00015F27
		// (set) Token: 0x06000A85 RID: 2693 RVA: 0x00017D2F File Offset: 0x00015F2F
		public int HttpStatusCode { get; internal set; }

		// Token: 0x170003BE RID: 958
		// (get) Token: 0x06000A86 RID: 2694 RVA: 0x00017D38 File Offset: 0x00015F38
		public RequestMethod RequestMethod { get; }

		// Token: 0x170003BF RID: 959
		// (get) Token: 0x06000A87 RID: 2695 RVA: 0x00017D40 File Offset: 0x00015F40
		public WebException Exception { get; }

		// Token: 0x170003C0 RID: 960
		// (get) Token: 0x06000A88 RID: 2696 RVA: 0x00017D48 File Offset: 0x00015F48
		public Uri Url { get; }

		// Token: 0x170003C1 RID: 961
		// (get) Token: 0x06000A89 RID: 2697 RVA: 0x00017D50 File Offset: 0x00015F50
		public object Data { get; }

		// Token: 0x170003C2 RID: 962
		// (get) Token: 0x06000A8A RID: 2698 RVA: 0x00017D58 File Offset: 0x00015F58
		// (set) Token: 0x06000A8B RID: 2699 RVA: 0x00017D60 File Offset: 0x00015F60
		public DiscordApiError DiscordError { get; internal set; }

		// Token: 0x170003C3 RID: 963
		// (get) Token: 0x06000A8C RID: 2700 RVA: 0x00017D69 File Offset: 0x00015F69
		// (set) Token: 0x06000A8D RID: 2701 RVA: 0x00017D71 File Offset: 0x00015F71
		public string Message { get; internal set; }

		// Token: 0x06000A8E RID: 2702 RVA: 0x00017D7A File Offset: 0x00015F7A
		public RestError(WebException exception, Uri url, RequestMethod requestMethod, object data)
		{
			this.Exception = exception;
			this.Url = url;
			this.RequestMethod = requestMethod;
			this.Data = data;
		}
	}
}
