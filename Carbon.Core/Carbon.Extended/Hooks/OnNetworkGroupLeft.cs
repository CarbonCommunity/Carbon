using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseNetworkable ), "OnNetworkGroupLeave" )]
    public class OnNetworkGroupLeft
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnNetworkGroupLeft" );
        }
    }
}