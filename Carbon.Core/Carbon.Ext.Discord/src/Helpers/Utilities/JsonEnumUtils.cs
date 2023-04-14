using System;
using System.Linq;
using System.Reflection;
using Oxide.Plugins;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

namespace Oxide.Ext.Discord.Helpers.Utilities
{
	internal static class JsonEnumUtils
	{
		private static readonly Hash<Type, EnumData> EnumData = new Hash<Type, EnumData>();

		private static readonly object Lock = new object();

		internal static string ToEnumName(Type enumType, string enumText)
		{
			EnumData data = GetEnumData(enumType);
			if (enumText.IndexOf(",", StringComparison.Ordinal) == -1)
			{
				return data.NameToProperty[enumText];
			}

			return ParseEnumNameList(enumText, data.NameToProperty);
		}

		internal static string FromEnumName(Type enumType, string enumText)
		{
			EnumData data = GetEnumData(enumType);
			if (enumText.IndexOf(",", StringComparison.Ordinal) == -1)
			{
				return data.PropertyToName[enumText];
			}

			return ParseEnumNameList(enumText, data.PropertyToName);
		}

		private static string ParseEnumNameList(string enumText, Hash<string, string> lookup)
		{
			string[] enums = enumText.Split(',');
			for (int index = 0; index < enums.Length; index++)
			{
				enums[index] = lookup[enums[index].Trim()];
			}

			return string.Join(", ", enums);
		}

		private static EnumData GetEnumData(Type type)
		{
			EnumData data;
			lock (Lock)
			{
				data = EnumData[type];
			}

			if (data == null)
			{
				data = new EnumData(type);
				lock (Lock)
				{
					EnumData[type] = data;
				}
			}

			return data;
		}
	}

	internal class EnumData
	{
		public readonly Hash<string, string> NameToProperty = new Hash<string, string>();
		public readonly Hash<string, string> PropertyToName = new Hash<string, string>();

		public EnumData(Type type)
		{
			foreach (FieldInfo field in type.GetFields())
			{
				string name = field.Name;
				string propertyName = field.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), true)
										   .Cast<System.ComponentModel.DescriptionAttribute>()
										   .Select(f => f.Description).SingleOrDefault() ?? field.Name;
				Add(name, propertyName);
			}
		}

		private void Add(string name, string propertyName)
		{
			NameToProperty[name] = propertyName;
			PropertyToName[propertyName] = name;
		}
	}
}
