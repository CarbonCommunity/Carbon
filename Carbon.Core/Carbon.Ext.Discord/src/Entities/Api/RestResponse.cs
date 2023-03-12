/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Api
{
	// Token: 0x0200011D RID: 285
	public class RestResponse
	{
		// Token: 0x170003C4 RID: 964
		// (get) Token: 0x06000A8F RID: 2703 RVA: 0x00017DA1 File Offset: 0x00015FA1
		public string Data { get; }

		// Token: 0x06000A90 RID: 2704 RVA: 0x00017DA9 File Offset: 0x00015FA9
		public RestResponse(string data)
		{
			this.Data = data;
		}

		// Token: 0x06000A91 RID: 2705 RVA: 0x00017DBC File Offset: 0x00015FBC
		public T ParseData<T>()
		{
			return (!string.IsNullOrEmpty(this.Data)) ? JsonConvert.DeserializeObject<T>(this.Data) : default(T);
		}
	}
}
