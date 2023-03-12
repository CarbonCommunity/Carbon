/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;

namespace Oxide.Ext.Discord.Entities.Messages
{
	// Token: 0x02000065 RID: 101
	public class MessageFileAttachment
	{
		// Token: 0x170000E3 RID: 227
		// (get) Token: 0x0600039C RID: 924 RVA: 0x00010A5A File Offset: 0x0000EC5A
		// (set) Token: 0x0600039D RID: 925 RVA: 0x00010A62 File Offset: 0x0000EC62
		public string FileName { get; set; }

		// Token: 0x170000E4 RID: 228
		// (get) Token: 0x0600039E RID: 926 RVA: 0x00010A6B File Offset: 0x0000EC6B
		// (set) Token: 0x0600039F RID: 927 RVA: 0x00010A73 File Offset: 0x0000EC73
		public byte[] Data { get; set; }

		// Token: 0x170000E5 RID: 229
		// (get) Token: 0x060003A0 RID: 928 RVA: 0x00010A7C File Offset: 0x0000EC7C
		// (set) Token: 0x060003A1 RID: 929 RVA: 0x00010A84 File Offset: 0x0000EC84
		public string ContentType { get; set; }

		// Token: 0x060003A2 RID: 930 RVA: 0x00010A8D File Offset: 0x0000EC8D
		public MessageFileAttachment()
		{
		}

		// Token: 0x060003A3 RID: 931 RVA: 0x00010A97 File Offset: 0x0000EC97
		public MessageFileAttachment(string fileName, byte[] data, string contentType)
		{
			this.FileName = fileName;
			this.Data = data;
			this.ContentType = contentType;
		}
	}
}
