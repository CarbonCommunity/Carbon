using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( FlameThrower ), "FlameTick" )]
    public class OnFlameThrowerBurn
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnFlameThrowerBurn" );
        }
    }
}