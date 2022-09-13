using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( SleepingBag ), "ValidForPlayer" )]
    public class OnSleepingBagValidCheck
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnSleepingBagValidCheck" );
        }
    }
}