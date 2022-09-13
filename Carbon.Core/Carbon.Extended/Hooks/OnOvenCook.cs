using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseOven ), "Cook" )]
    public class OnOvenCook
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnOvenCook" );
        }
    }
}