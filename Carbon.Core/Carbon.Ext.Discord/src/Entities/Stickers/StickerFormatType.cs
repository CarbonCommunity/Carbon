/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.ComponentModel;

namespace Oxide.Ext.Discord.Entities.Stickers
{
	// Token: 0x02000059 RID: 89
	public enum StickerFormatType
	{
		// Token: 0x0400019F RID: 415
		[System.ComponentModel.Description("PNG")]
		Png = 1,
		// Token: 0x040001A0 RID: 416
		[System.ComponentModel.Description("APNG")]
		Apng,
		// Token: 0x040001A1 RID: 417
		[System.ComponentModel.Description("LOTTIE")]
		Lottie
	}
}
