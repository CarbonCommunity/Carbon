using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( MLRS ), "SetUserTargetHitPos" )]
    public class OnMlrsTarget
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnMlrsTarget" );
        }
    }
}