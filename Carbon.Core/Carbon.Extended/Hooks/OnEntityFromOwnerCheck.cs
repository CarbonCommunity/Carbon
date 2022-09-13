using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseEntity.RPC_Server.FromOwner ), "Test" )]
    public class OnEntityFromOwnerCheck
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnEntityFromOwnerCheck" );
        }
    }
}