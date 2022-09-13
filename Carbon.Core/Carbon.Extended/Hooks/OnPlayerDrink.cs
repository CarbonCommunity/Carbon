using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( LiquidContainer ), "SVDrink" )]
    public class OnPlayerDrink
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnPlayerDrink" );
        }
    }
}