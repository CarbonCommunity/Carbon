using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( SleepingBag ), "RPC_MakePublic" )]
    public class CanSetBedPublic
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "CanSetBedPublic" );
        }
    }
}