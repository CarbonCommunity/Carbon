using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( WildlifeTrap ), "TrapWildlife" )]
    public class OnWildlifeTrap
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnWildlifeTrap" );
        }
    }
}