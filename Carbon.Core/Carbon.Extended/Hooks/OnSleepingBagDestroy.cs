using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( SleepingBag ), "DestroyBag" )]
    public class OnSleepingBagDestroy
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnSleepingBagDestroy" );
        }
    }
}