using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseOven ), "StartCooking" )]
    public class OnOvenStart
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnOvenStart" );
        }
    }
}