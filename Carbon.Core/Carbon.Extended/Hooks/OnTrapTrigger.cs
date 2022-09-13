using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Landmine ), "ObjectEntered" )]
    public class OnTrapTrigger
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnTrapTrigger" );
        }
    }
}