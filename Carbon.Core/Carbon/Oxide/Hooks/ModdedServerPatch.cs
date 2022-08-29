using Harmony;
using System;
using System.Reflection;

namespace Carbon.Core.Oxide.Hooks
{
    [HarmonyPatch ( typeof ( ServerMgr ), "UpdateServerInformation" )]
    internal class ServerMgr_UpdateServerInformation
    {
        private static readonly PropertyInfo GameTags = AccessTools.TypeByName ( "Steamworks.SteamServer" ).GetProperty ( "GameTags", BindingFlags.Static | BindingFlags.Public );

        public static void Postfix ()
        {
            try
            {
                var gameTags = GameTags.GetValue ( null ) as string;

                if ( !gameTags.Contains ( ",modded" ) ) GameTags.SetValue ( null, $"{gameTags},modded" );
            }
            catch ( Exception ex )
            {
                CarbonCore.Error ( $"Couldn't patch UpdateServerInformation.", ex );
            }
        }
    }
}
