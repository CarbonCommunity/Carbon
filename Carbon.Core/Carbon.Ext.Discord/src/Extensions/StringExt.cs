/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Oxide.Ext.Discord.Extensions
{
	// Token: 0x02000035 RID: 53
	public static class StringExt
	{
		// Token: 0x060001AF RID: 431 RVA: 0x0000D9C4 File Offset: 0x0000BBC4
		public static void ParseCommand(this string argStr, out string command, out string[] args)
		{
			List<string> list = new List<string>();
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			foreach (char c in argStr)
			{
				bool flag2 = c == '"';
				if (flag2)
				{
					bool flag3 = flag;
					if (flag3)
					{
						string text = stringBuilder.ToString().Trim();
						bool flag4 = !string.IsNullOrEmpty(text);
						if (flag4)
						{
							list.Add(text);
						}
						stringBuilder.Length = 0;
						flag = false;
					}
					else
					{
						flag = true;
					}
				}
				else
				{
					bool flag5 = char.IsWhiteSpace(c) && !flag;
					if (flag5)
					{
						string text2 = stringBuilder.ToString().Trim();
						bool flag6 = !string.IsNullOrEmpty(text2);
						if (flag6)
						{
							list.Add(text2);
						}
						stringBuilder.Length = 0;
					}
					else
					{
						stringBuilder.Append(c);
					}
				}
			}
			bool flag7 = stringBuilder.Length > 0;
			if (flag7)
			{
				string text3 = stringBuilder.ToString().Trim();
				bool flag8 = !string.IsNullOrEmpty(text3);
				if (flag8)
				{
					list.Add(text3);
				}
			}
			bool flag9 = list.Count == 0;
			if (flag9)
			{
				command = null;
				args = null;
			}
			else
			{
				command = list[0].ToLower();
				list.RemoveAt(0);
				args = list.ToArray();
			}
		}
	}
}
