/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Oxide.Plugins;

namespace Oxide.Ext.Discord.Helpers.Utilities
{
	// Token: 0x0200002A RID: 42
	internal class EnumData
	{
		// Token: 0x06000179 RID: 377 RVA: 0x0000C7E8 File Offset: 0x0000A9E8
		public EnumData(Type type)
		{
			foreach (FieldInfo fieldInfo in type.GetFields())
			{
				string name = fieldInfo.Name;
				string propertyName = (from DescriptionAttribute f in fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true)
				select f.Description).SingleOrDefault<string>() ?? fieldInfo.Name;
				this.Add(name, propertyName);
			}
		}

		// Token: 0x0600017A RID: 378 RVA: 0x0000C88B File Offset: 0x0000AA8B
		private void Add(string name, string propertyName)
		{
			this.NameToProperty[name] = propertyName;
			this.PropertyToName[propertyName] = name;
		}

		// Token: 0x040000FD RID: 253
		public readonly Hash<string, string> NameToProperty = new Hash<string, string>();

		// Token: 0x040000FE RID: 254
		public readonly Hash<string, string> PropertyToName = new Hash<string, string>();
	}
}
