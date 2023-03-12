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

public partial class Category_Save
{
	public partial class Save_SaveRestore
	{
		[HookAttribute.Patch("IPostSaveSave", "IPostSaveSave", typeof(SaveRestore), "Save", new System.Type[] { typeof(string), typeof(bool) })]
		[HookAttribute.Identifier("184bb59290c64312a2d12bc3502fe6ca")]
		[HookAttribute.Options(HookFlags.Hidden)]

		public class Save_SaveRestore_Save_184bb59290c64312a2d12bc3502fe6ca : API.Hooks.Patch
		{
			public static void Postfix()
			{
				HookCaller.CallStaticHook("IPostSaveSave");
			}
		}
	}
}
