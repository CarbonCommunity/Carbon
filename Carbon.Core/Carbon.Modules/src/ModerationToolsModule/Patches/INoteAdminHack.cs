using API.Hooks;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Modules;

public partial class ModerationToolsModule
{
	[HookAttribute.Patch("INoteAdminHack", "INoteAdminHack", typeof(AntiHack), "NoteAdminHack", new System.Type[] { typeof(BasePlayer) })]
	[HookAttribute.Identifier("23685f4c3d6944d7ae75a794cd5e6144")]
	[HookAttribute.Options(HookFlags.Hidden)]

	public class AntiHack_INoteAdminHack_23685f4c3d6944d7ae75a794cd5e6144 : API.Hooks.Patch
	{
		public static bool Prefix(BasePlayer ply)
		{
			if (HookCaller.CallStaticHook(3335207430, ply) != null)
			{
				return false;
			}

			return true;
		}
	}
}
