using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ConnectionQueue ), "Join" )]
    public class OnConnectionQueue
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnConnectionQueue" );
        }
    }
}