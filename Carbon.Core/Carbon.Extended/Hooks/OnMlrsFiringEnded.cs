using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( MLRS ), "EndFiring" )]
    public class OnMlrsFiringEnded
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnMlrsFiringEnded" );
        }
    }
}