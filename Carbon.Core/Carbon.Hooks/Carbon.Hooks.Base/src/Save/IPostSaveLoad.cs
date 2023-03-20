using System.Collections.Generic;
using System.Threading;
using API.Hooks;
using UnityEngine;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;
#pragma warning disable IDE0051

public partial class Category_Save
{
	public partial class Save_SaveRestore
	{
		[HookAttribute.Patch("IPostSaveLoad", "IPostSaveLoad", typeof(SaveRestore), "Load", new System.Type[] { typeof(string), typeof(bool) })]
		[HookAttribute.Identifier("184bb59290c64312a2d12bc3502fe6ca")]
		[HookAttribute.Options(HookFlags.Hidden)]

		public class Save_SaveRestore_Load_184bb59290c64312a2d12bc3502fe6ca : API.Hooks.Patch
		{
			public static void Postfix(ref bool __result)
			{
				var hook = HookCaller.CallStaticHook("IPostSaveLoad");

				if (hook is bool result)
				{
					__result = result;
				}
			}
		}
	}
}
