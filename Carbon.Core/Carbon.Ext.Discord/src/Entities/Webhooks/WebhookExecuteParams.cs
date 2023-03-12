/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Oxide.Ext.Discord.Builders;

namespace Oxide.Ext.Discord.Entities.Webhooks
{
	// Token: 0x02000045 RID: 69
	public class WebhookExecuteParams
	{
		// Token: 0x17000042 RID: 66
		// (get) Token: 0x06000206 RID: 518 RVA: 0x0000E5CA File Offset: 0x0000C7CA
		// (set) Token: 0x06000207 RID: 519 RVA: 0x0000E5D2 File Offset: 0x0000C7D2
		public WebhookSendType SendType { get; set; } = WebhookSendType.Discord;

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x06000208 RID: 520 RVA: 0x0000E5DB File Offset: 0x0000C7DB
		// (set) Token: 0x06000209 RID: 521 RVA: 0x0000E5E3 File Offset: 0x0000C7E3
		public bool Wait { get; internal set; }

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x0600020A RID: 522 RVA: 0x0000E5EC File Offset: 0x0000C7EC
		// (set) Token: 0x0600020B RID: 523 RVA: 0x0000E5F4 File Offset: 0x0000C7F4
		public Snowflake? ThreadId { get; set; }

		// Token: 0x0600020C RID: 524 RVA: 0x0000E600 File Offset: 0x0000C800
		public string ToQueryString()
		{
			QueryStringBuilder queryStringBuilder = new QueryStringBuilder();
			bool wait = this.Wait;
			if (wait)
			{
				queryStringBuilder.Add("wait", "true");
			}
			bool flag = this.ThreadId != null;
			if (flag)
			{
				queryStringBuilder.Add("thread_id", this.ThreadId.Value.ToString());
			}
			return queryStringBuilder.ToString();
		}

		// Token: 0x0600020D RID: 525 RVA: 0x0000E67C File Offset: 0x0000C87C
		public string GetWebhookFormat()
		{
			string result;
			switch (this.SendType)
			{
			case WebhookSendType.Discord:
				result = string.Empty;
				break;
			case WebhookSendType.Slack:
				result = "/slack";
				break;
			case WebhookSendType.Github:
				result = "/github";
				break;
			default:
				throw new ArgumentOutOfRangeException("SendType", this.SendType, null);
			}
			return result;
		}
	}
}
