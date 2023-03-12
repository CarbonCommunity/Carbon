/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Api;

namespace Oxide.Ext.Discord.Entities.Gatway
{
	// Token: 0x020000C5 RID: 197
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	internal class Gateway
	{
		// Token: 0x1700026D RID: 621
		// (get) Token: 0x0600075D RID: 1885 RVA: 0x00015105 File Offset: 0x00013305
		// (set) Token: 0x0600075E RID: 1886 RVA: 0x0001510D File Offset: 0x0001330D
		[JsonProperty("url")]
		public string Url { get; private set; }

		// Token: 0x1700026E RID: 622
		// (get) Token: 0x0600075F RID: 1887 RVA: 0x00015116 File Offset: 0x00013316
		// (set) Token: 0x06000760 RID: 1888 RVA: 0x0001511D File Offset: 0x0001331D
		public static string WebsocketUrl { get; private set; }

		// Token: 0x06000761 RID: 1889 RVA: 0x00015125 File Offset: 0x00013325
		public static void GetGateway(BotClient client, Action<Gateway> callback)
		{
			client.Rest.DoRequest<Gateway>("/gateway", RequestMethod.GET, null, callback, null);
		}

		// Token: 0x06000762 RID: 1890 RVA: 0x00015140 File Offset: 0x00013340
		public static void UpdateGatewayUrl(BotClient client, Action callback)
		{
			Gateway.GetGateway(client, delegate(Gateway gateway)
			{
				Gateway.WebsocketUrl = gateway.Url + "/?" + GatewayConnect.ConnectionArgs;
				client.Logger.Debug("Gateway.UpdateGatewayUrl Updated Gateway Url: " + Gateway.WebsocketUrl);
				callback();
			});
		}
	}
}
