using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Landmine ), "ObjectEntered" )]
    public class OnTrapTrigger [Landmine]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnTrapTrigger [Landmine]" );
        }
    }
}