using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "Server_ClearMapMarkers" )]
    public class OnMapMarkersCleared
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnMapMarkersCleared" );
        }
    }
}