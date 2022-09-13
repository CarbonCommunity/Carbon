using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( TreeEntity ), "DidHitMarker" )]
    public class OnTreeMarkerHit
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnTreeMarkerHit" );
        }
    }
}