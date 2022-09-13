using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "Server_RemovePointOfInterest" )]
    public class OnMapMarkerRemove
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnMapMarkerRemove" );
        }
    }
}