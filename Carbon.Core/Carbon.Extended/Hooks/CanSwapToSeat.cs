using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ModularCarSeat ), "CanSwapToThis" )]
    public class CanSwapToSeat
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanSwapToSeat" );
        }
    }
}