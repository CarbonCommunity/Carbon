using System;
using System.Linq;
using API.Abstracts;
using API.Hooks;
using Carbon.Base;
using Carbon.Extensions;
using Steamworks;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Static
{
	public partial class Static_ServerMgr
	{
		[HookAttribute.Patch("IServerInfoUpdate", "IServerInfoUpdate", typeof(ServerMgr), "UpdateServerInformation", new System.Type[] { })]
		[HookAttribute.Options(HookFlags.Static | HookFlags.Hidden | HookFlags.IgnoreChecksum)]

		public class IServerInfoUpdate : Patch
		{
			public static bool ForceModded =>
#if !MINIMAL
				CarbonAuto.Singleton.IsForceModded() ||
#endif
			    Community.Runtime.ModuleProcessor.Modules.Any(x => x is BaseModule module && module.IsEnabled() && module.ForceModded);

			public static void Postfix()
			{
				if (!SteamServer.IsValid || Community.Runtime == null || Community.Runtime.Config == null || Community.Runtime.Core == null) return;

				try
				{
					ServerTagEx.SetRequiredTag("^y", true);

					if (Community.Runtime.Config.IsModded || ForceModded)
					{
						ServerTagEx.SetRequiredTag("^z", true);
					}
					else
					{
						ServerTagEx.UnsetRequiredTag("^z", true);
					}

					#if !MINIMAL

					if (!string.IsNullOrEmpty(Community.Runtime.Core.CustomMapName) && !Community.Runtime.Core.CustomMapName.Equals("-1"))
					{
						SteamServer.MapName = Community.Runtime.Core.CustomMapName;
					}

					#endif
				}
				catch (Exception ex)
				{
					Logger.Error($"Couldn't patch UpdateServerInformation.", ex);
				}
			}
		}
	}
}
