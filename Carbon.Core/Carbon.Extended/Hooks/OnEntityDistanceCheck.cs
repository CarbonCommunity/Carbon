using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseEntity.RPC_Server.MaxDistance ), "Test" )]
    public class OnEntityDistanceCheck
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnEntityDistanceCheck" );
        }
    }
}