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
	[HookAttribute.Patch("IServerEventToasts", "IServerEventToasts", typeof(BasePlayer), "ShowToast", new System.Type[] { typeof(GameTip.Styles), typeof(Translate.Phrase), typeof(string[]) })]
	[HookAttribute.Identifier("83fa548863164d4eab4856c48b8dae97")]
	[HookAttribute.Options(HookFlags.Hidden)]

	public class BasePlayer_ShowToast_83fa548863164d4eab4856c48b8dae97 : API.Hooks.Patch
	{
		public static bool Prefix(GameTip.Styles style, Translate.Phrase phrase, string[] arguments)
		{
			if (HookCaller.CallStaticHook("IServerEventToasts", style) != null)
			{
				return false;
			}

			return true;
		}
	}
}
