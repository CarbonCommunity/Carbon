using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseEntity.RPC_Server.IsActiveItem ), "Test" )]
    public class OnEntityActiveCheck
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnEntityActiveCheck" );
        }
    }
}