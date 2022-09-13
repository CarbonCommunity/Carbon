using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseMountable ), "RPC_WantsDismount" )]
    public class OnPlayerWantsDismount
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnPlayerWantsDismount" );
        }
    }
}