using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseEntity ), "SignalBroadcast" )]
    public class OnSignalBroadcast
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnSignalBroadcast" );
        }
    }
}