using System;
using System.Linq;
using API.Abstracts;
using API.Hooks;
using Carbon.Base;
using Carbon.Extensions;

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
	public partial class Static_ServerMgr
	{
		[HookAttribute.Patch("IServerInfoUpdate", "IServerInfoUpdate", typeof(ServerMgr), "UpdateServerInformation", new System.Type[] { })]
		[HookAttribute.Identifier("aaa38191cc9f4f6f911df9742d552a99")]
		[HookAttribute.Options(HookFlags.Static | HookFlags.Hidden | HookFlags.IgnoreChecksum)]

		public class Static_ServerMgr_aaa38191cc9f4f6f911df9742d552a99 : Patch
		{
			public static bool ForceModded => CarbonAuto.Singleton.IsChanged() || Community.Runtime.ModuleProcessor.Modules.Any(x => x is BaseModule module && module.GetEnabled() && module.ForceModded);

			public static void Postfix()
			{
				if (Community.Runtime == null || Community.Runtime.Config == null) return;

				try
				{
					ServerTagEx.SetRequiredTag("carbon");

					if (Community.Runtime.Config.IsModded || ForceModded)
					{
						ServerTagEx.SetRequiredTag("modded");
					}
					else
					{
						ServerTagEx.UnsetRequiredTag("modded");
					}
				}
				catch (Exception ex)
				{
					Logger.Error($"Couldn't patch UpdateServerInformation.", ex);
				}
			}
		}
	}
}
