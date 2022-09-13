using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseMountable ), "RPC_WantsMount" )]
    public class OnPlayerWantsMount
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnPlayerWantsMount" );
        }
    }
}