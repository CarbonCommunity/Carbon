using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "EnablePlayerCollider" )]
    public class OnPlayerColliderEnable
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnPlayerColliderEnable" );
        }
    }
}