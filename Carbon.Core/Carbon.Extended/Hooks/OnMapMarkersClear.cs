using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "Server_ClearMapMarkers" )]
    public class OnMapMarkersClear
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnMapMarkersClear" );
        }
    }
}