using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseNetworkable ), "OnNetworkGroupEnter" )]
    public class OnNetworkGroupEntered
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnNetworkGroupEntered" );
        }
    }
}