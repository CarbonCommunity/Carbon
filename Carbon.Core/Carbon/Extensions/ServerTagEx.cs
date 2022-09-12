using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carbon.Core.Extensions
{
    public static class ServerTagEx
    {
        public static bool SetRequiredTag ( string tag )
        {
            var tags = Steamworks.SteamServer.GameTags;

            if ( !tags.Contains ( $",{tag}" ) )
            {
                Steamworks.SteamServer.GameTags = $"{tags},{tag}";
                return true;
            }

            return false;
        }

        public static bool UnsetRequiredTag ( string tag )
        {
            var tags = Steamworks.SteamServer.GameTags;

            if ( tags.Contains ( $",{tag}" ) )
            {
                Steamworks.SteamServer.GameTags = tags.Replace ( $",{tag}", "" );
                return true;
            }

            return false;
        }
    }
}