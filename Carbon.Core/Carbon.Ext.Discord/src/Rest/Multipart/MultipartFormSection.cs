/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.Text;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Rest.Multipart
{
	// Token: 0x02000013 RID: 19
	internal class MultipartFormSection : IMultipartSection
	{
		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060000E8 RID: 232 RVA: 0x0000A59D File Offset: 0x0000879D
		public string FileName
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060000E9 RID: 233 RVA: 0x0000A5A0 File Offset: 0x000087A0
		public string ContentType { get; }

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060000EA RID: 234 RVA: 0x0000A5A8 File Offset: 0x000087A8
		public byte[] Data { get; }

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060000EB RID: 235 RVA: 0x0000A5B0 File Offset: 0x000087B0
		public string SectionName { get; }

		// Token: 0x060000EC RID: 236 RVA: 0x0000A5B8 File Offset: 0x000087B8
		internal MultipartFormSection(string sectionName, byte[] data, string contentType)
		{
			this.ContentType = contentType;
			this.Data = data;
			this.SectionName = sectionName;
		}

		// Token: 0x060000ED RID: 237 RVA: 0x0000A5D7 File Offset: 0x000087D7
		internal MultipartFormSection(string sectionName, string data, string contentType) : this(contentType, Encoding.UTF8.GetBytes(data), sectionName)
		{
		}

		// Token: 0x060000EE RID: 238 RVA: 0x0000A5EE File Offset: 0x000087EE
		internal MultipartFormSection(string sectionName, object data, string contentType) : this(contentType, JsonConvert.SerializeObject(data, DiscordExtension.ExtensionSerializeSettings), sectionName)
		{
		}
	}
}
