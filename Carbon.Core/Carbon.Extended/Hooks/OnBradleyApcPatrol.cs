using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BradleyAPC ), "UpdateMovement_Patrol" )]
    public class OnBradleyApcPatrol
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnBradleyApcPatrol" );
        }
    }
}