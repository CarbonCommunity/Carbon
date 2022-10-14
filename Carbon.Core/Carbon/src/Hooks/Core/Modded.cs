///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using Carbon.Core;
using Carbon.Extensions;

namespace Carbon.Hooks
{
	[CarbonHook.AlwaysPatched, CarbonHook.Hidden]
	[CarbonHook("IServerInfoUpdate"), CarbonHook.Category(Hook.Category.Enum.Core)]
	[CarbonHook.Patch(typeof(ServerMgr), "UpdateServerInformation")]
	public class ServerMgr_UpdateServerInformation
	{
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
