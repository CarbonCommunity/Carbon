using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "Server_RemovePointOfInterest" )]
    public class OnMapMarkerRemove
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnMapMarkerRemove" );
        }
    }
}