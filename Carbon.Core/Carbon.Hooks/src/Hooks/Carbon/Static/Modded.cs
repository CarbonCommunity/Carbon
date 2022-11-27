using System;
using Carbon.Extensions;

/*
 *
 * Copyright (c) 2022 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;

public partial class Category_Static
{
	public partial class Static_ServerMgr
	{
		/*
		[CarbonHook.AlwaysPatched, CarbonHook.Hidden]
		[CarbonHook("IServerInfoUpdate"), CarbonHook.Category(Hook.Category.Enum.Core)]
		[CarbonHook.Patch(typeof(ServerMgr), "UpdateServerInformation")]
		*/

		public class Static_ServerMgr_UpdateServerInformation_aaa38191cc9f4f6f911df9742d552a99
		{
			public static Metadata metadata = new Metadata("IServerInfoUpdate",
				typeof(ServerMgr), "UpdateServerInformation", new System.Type[] { });

			static Static_ServerMgr_UpdateServerInformation_aaa38191cc9f4f6f911df9742d552a99()
			{
				metadata.SetIdentifier("aaa38191cc9f4f6f911df9742d552a99");
				metadata.SetAlwaysPatch(true);
				metadata.SetHidden(true);
			}

			public static void Postfix()
			{
				if (Community.Runtime == null || Community.Runtime.Config == null) return;

				try
				{
					if (Community.Runtime.Config.CarbonTag)
					{
						ServerTagEx.SetRequiredTag("carbon");
					}
					else
					{
						ServerTagEx.UnsetRequiredTag("carbon");
					}

					if (Community.Runtime.Config.IsModded)
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
					Carbon.Logger.Error($"Couldn't patch UpdateServerInformation.", ex);
				}
			}
		}
	}
}