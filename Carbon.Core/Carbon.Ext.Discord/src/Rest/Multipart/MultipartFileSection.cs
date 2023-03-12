/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;

namespace Oxide.Ext.Discord.Rest.Multipart
{
	// Token: 0x02000012 RID: 18
	internal class MultipartFileSection : IMultipartSection
	{
		// Token: 0x17000021 RID: 33
		// (get) Token: 0x060000E3 RID: 227 RVA: 0x0000A556 File Offset: 0x00008756
		public string FileName { get; }

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x060000E4 RID: 228 RVA: 0x0000A55E File Offset: 0x0000875E
		public string ContentType { get; }

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x060000E5 RID: 229 RVA: 0x0000A566 File Offset: 0x00008766
		public byte[] Data { get; }

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060000E6 RID: 230 RVA: 0x0000A56E File Offset: 0x0000876E
		public string SectionName { get; }

		// Token: 0x060000E7 RID: 231 RVA: 0x0000A576 File Offset: 0x00008776
		internal MultipartFileSection(string sectionName, string fileName, byte[] data, string contentType)
		{
			this.FileName = fileName;
			this.ContentType = contentType;
			this.Data = data;
			this.SectionName = sectionName;
		}
	}
}
