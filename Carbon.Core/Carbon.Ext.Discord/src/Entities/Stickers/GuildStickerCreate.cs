using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Messages;
using Oxide.Ext.Discord.Interfaces;

namespace Oxide.Ext.Discord.Entities.Stickers
{
	// Token: 0x02000058 RID: 88
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class GuildStickerCreate : IFileAttachments
	{
		// Token: 0x17000098 RID: 152
		// (get) Token: 0x060002D2 RID: 722 RVA: 0x0000F6C8 File Offset: 0x0000D8C8
		// (set) Token: 0x060002D3 RID: 723 RVA: 0x0000F6D0 File Offset: 0x0000D8D0
		[JsonProperty("name")]
		public string Name { get; set; }

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x060002D4 RID: 724 RVA: 0x0000F6D9 File Offset: 0x0000D8D9
		// (set) Token: 0x060002D5 RID: 725 RVA: 0x0000F6E1 File Offset: 0x0000D8E1
		[JsonProperty("description")]
		public string Description { get; set; }

		// Token: 0x1700009A RID: 154
		// (get) Token: 0x060002D6 RID: 726 RVA: 0x0000F6EA File Offset: 0x0000D8EA
		// (set) Token: 0x060002D7 RID: 727 RVA: 0x0000F6F2 File Offset: 0x0000D8F2
		[JsonProperty("tags")]
		public string Tags { get; set; }

		// Token: 0x1700009B RID: 155
		// (get) Token: 0x060002D8 RID: 728 RVA: 0x0000F6FB File Offset: 0x0000D8FB
		// (set) Token: 0x060002D9 RID: 729 RVA: 0x0000F703 File Offset: 0x0000D903
		public List<MessageFileAttachment> FileAttachments { get; set; }

		// Token: 0x060002DA RID: 730 RVA: 0x0000F70C File Offset: 0x0000D90C
		public void AddSticker(string fileName, string contentType, byte[] data)
		{
			bool flag = this.FileAttachments.Count != 0;
			if (flag)
			{
				throw new Exception("Can only add one sticker at a time");
			}
			bool flag2 = data.Length > 512000;
			if (flag2)
			{
				throw new Exception("Data cannot be larger than 500KB");
			}
			string text = fileName.Substring(fileName.LastIndexOf('.') + 1);
			string text2 = text.ToLower();
			string text3 = text2;
			if (text3 != null)
			{
				if (text3 == "png" || text3 == "apng" || text3 == "json")
				{
					this.FileAttachments.Add(new MessageFileAttachment(fileName, data, contentType));
					return;
				}
			}
			throw new Exception("Sticker can only be of type png, apng, or lottie json");
		}
	}
}
