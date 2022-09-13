using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BearTrap ), "ObjectEntered" )]
    public class OnTrapTrigger [BearTrap]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnTrapTrigger [BearTrap]" );
        }
    }
}