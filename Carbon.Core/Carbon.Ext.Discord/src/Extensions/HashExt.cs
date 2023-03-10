/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.Collections.Generic;
using Oxide.Plugins;

namespace Oxide.Ext.Discord.Extensions
{
	// Token: 0x02000033 RID: 51
	internal static class HashExt
	{
		// Token: 0x060001A4 RID: 420 RVA: 0x0000D6D8 File Offset: 0x0000B8D8
		internal static void RemoveAll<TKey, TValue>(this Hash<TKey, TValue> hash, Func<TValue, bool> predicate)
		{
			bool flag = hash == null;
			if (!flag)
			{
				List<TKey> list = new List<TKey>();
				foreach (KeyValuePair<TKey, TValue> keyValuePair in hash)
				{
					bool flag2 = predicate(keyValuePair.Value);
					if (flag2)
					{
						list.Add(keyValuePair.Key);
					}
				}
				foreach (TKey tkey in list)
				{
					hash.Remove(tkey);
				}
			}
		}

		// Token: 0x060001A5 RID: 421 RVA: 0x0000D79C File Offset: 0x0000B99C
		public static Hash<TKey, TValue> Copy<TKey, TValue>(this Hash<TKey, TValue> hash)
		{
			Hash<TKey, TValue> hash2 = new Hash<TKey, TValue>();
			foreach (KeyValuePair<TKey, TValue> keyValuePair in hash)
			{
				hash2[keyValuePair.Key] = keyValuePair.Value;
			}
			return hash2;
		}
	}
}
