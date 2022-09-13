using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseFishingRod ), "CatchProcessBudgeted" )]
    public class OnFishCatch
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnFishCatch" );
        }
    }
}