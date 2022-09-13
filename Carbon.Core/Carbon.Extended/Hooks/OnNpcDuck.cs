using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( HumanNPC ), "SetDucked" )]
    public class OnNpcDuck
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnNpcDuck" );
        }
    }
}