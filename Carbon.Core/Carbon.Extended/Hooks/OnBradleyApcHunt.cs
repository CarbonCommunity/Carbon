using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BradleyAPC ), "UpdateMovement_Hunt" )]
    public class OnBradleyApcHunt
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnBradleyApcHunt" );
        }
    }
}