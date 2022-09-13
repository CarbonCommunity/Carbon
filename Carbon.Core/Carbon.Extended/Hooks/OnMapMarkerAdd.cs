using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "Server_AddMarker" )]
    public class OnMapMarkerAdd
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnMapMarkerAdd" );
        }
    }
}