/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Oxide.Plugins;

namespace Oxide.Ext.Discord.Helpers.Utilities
{
	// Token: 0x02000029 RID: 41
	internal static class JsonEnumUtils
	{
		// Token: 0x06000174 RID: 372 RVA: 0x0000C64C File Offset: 0x0000A84C
		internal static string ToEnumName(Type enumType, string enumText)
		{
			EnumData enumData = JsonEnumUtils.GetEnumData(enumType);
			bool flag = enumText.IndexOf(",", StringComparison.Ordinal) == -1;
			string result;
			if (flag)
			{
				result = enumData.NameToProperty[enumText];
			}
			else
			{
				result = JsonEnumUtils.ParseEnumNameList(enumText, enumData.NameToProperty);
			}
			return result;
		}

		// Token: 0x06000175 RID: 373 RVA: 0x0000C694 File Offset: 0x0000A894
		internal static string FromEnumName(Type enumType, string enumText)
		{
			EnumData enumData = JsonEnumUtils.GetEnumData(enumType);
			bool flag = enumText.IndexOf(",", StringComparison.Ordinal) == -1;
			string result;
			if (flag)
			{
				result = enumData.PropertyToName[enumText];
			}
			else
			{
				result = JsonEnumUtils.ParseEnumNameList(enumText, enumData.PropertyToName);
			}
			return result;
		}

		// Token: 0x06000176 RID: 374 RVA: 0x0000C6DC File Offset: 0x0000A8DC
		private static string ParseEnumNameList(string enumText, Hash<string, string> lookup)
		{
			string[] array = enumText.Split(new char[]
			{
				','
			});
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = lookup[array[i].Trim()];
			}
			return string.Join(", ", array);
		}

		// Token: 0x06000177 RID: 375 RVA: 0x0000C730 File Offset: 0x0000A930
		private static EnumData GetEnumData(Type type)
		{
			object @lock = JsonEnumUtils.Lock;
			EnumData enumData;
			lock (@lock)
			{
				enumData = JsonEnumUtils.EnumData[type];
			}
			bool flag2 = enumData == null;
			if (flag2)
			{
				enumData = new EnumData(type);
				object lock2 = JsonEnumUtils.Lock;
				lock (lock2)
				{
					JsonEnumUtils.EnumData[type] = enumData;
				}
			}
			return enumData;
		}

		// Token: 0x040000FB RID: 251
		private static readonly Hash<Type, EnumData> EnumData = new Hash<Type, EnumData>();

		// Token: 0x040000FC RID: 252
		private static readonly object Lock = new object();
	}
}
