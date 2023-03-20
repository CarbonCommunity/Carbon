using System;
using System.Linq;
using System.Reflection;
using API.Hooks;
using Carbon.Base;
using Carbon.Extensions;
using Carbon.Modules;
using UnityEngine;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;
#pragma warning disable IDE0051

public partial class Category_Static
{
	public partial class Static_Debug
	{
		[HookAttribute.Patch("ICleanDebugLog", "ICleanDebugLog", typeof(Debug), "Log", new System.Type[] { typeof(object) })]
		[HookAttribute.Identifier("e1c940a317fe4608ad4add1510107c09")]
		[HookAttribute.Options(HookFlags.Static | HookFlags.Hidden | HookFlags.IgnoreChecksum)]

		public class Static_Debug_Log_e1c940a317fe4608ad4add1510107c09 : API.Hooks.Patch
		{
			internal static string[] _filter = new string[]
			{
				"AIInformationZone performing complete refresh, please wait...",
			};

			public static bool Prefix(object message)
			{
				return !(message is string log && _filter.Any(x => log.StartsWith(x)));
			}
		}
	}
}
