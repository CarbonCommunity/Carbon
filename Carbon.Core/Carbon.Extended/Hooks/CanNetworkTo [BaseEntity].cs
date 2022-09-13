using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseEntity ), "ShouldNetworkTo" )]
    public class CanNetworkTo [BaseEntity]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanNetworkTo [BaseEntity]" );
        }
    }
}