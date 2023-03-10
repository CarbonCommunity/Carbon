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
	// Token: 0x02000011 RID: 17
	internal interface IMultipartSection
	{
		// Token: 0x1700001D RID: 29
		// (get) Token: 0x060000DF RID: 223
		string FileName { get; }

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x060000E0 RID: 224
		string ContentType { get; }

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x060000E1 RID: 225
		byte[] Data { get; }

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x060000E2 RID: 226
		string SectionName { get; }
	}
}
