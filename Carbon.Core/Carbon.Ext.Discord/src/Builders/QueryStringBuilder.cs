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

namespace Oxide.Ext.Discord.Builders
{
	// Token: 0x0200012B RID: 299
	public class QueryStringBuilder
	{
		// Token: 0x06000AF5 RID: 2805 RVA: 0x000186D0 File Offset: 0x000168D0
		public void Add(string key, string value)
		{
			this._builder.Append(key);
			this._builder.Append('=');
			this._builder.Append(value);
			this._builder.Append('&');
		}

		// Token: 0x06000AF6 RID: 2806 RVA: 0x0001870C File Offset: 0x0001690C
		public void AddList<T>(string key, List<T> list, string separator)
		{
			this._builder.Append(key);
			this._builder.Append('=');
			for (int i = 0; i < list.Count; i++)
			{
				T t = list[i];
				this._builder.Append(t.ToString());
				bool flag = i + 1 != list.Count;
				if (flag)
				{
					this._builder.Append(separator);
				}
			}
			this._builder.Append('&');
		}

		// Token: 0x06000AF7 RID: 2807 RVA: 0x0001879C File Offset: 0x0001699C
		public override string ToString()
		{
			return this._builder.ToString();
		}

		// Token: 0x040006FE RID: 1790
		private readonly StringBuilder _builder = new StringBuilder("?");
	}
}
