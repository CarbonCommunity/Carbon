using Carbon.Core.Extensions;
using Harmony;
using System;
using System.Reflection;

namespace Carbon.Core.Oxide.Hooks
{
    [HarmonyPatch ( typeof ( ServerMgr ), "UpdateServerInformation" )]
    public class ServerMgr_UpdateServerInformation
    {
        public static void Postfix ()
        {
            if ( CarbonCore.Instance == null || CarbonCore.Instance.Config == null ) return;

            try
            {
                if ( CarbonCore.Instance.Config.IsModded )
                {
                    ServerTagEx.SetRequiredTag ( "modded" );
                }
                else
                {
                    ServerTagEx.UnsetRequiredTag ( "modded" );
                }
            }
            catch ( Exception ex )
            {
                CarbonCore.Error ( $"Couldn't patch UpdateServerInformation.", ex );
            }
        }
    }
}
