using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( GrowableEntity ), "RemoveDying" )]
    public class OnRemoveDying
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnRemoveDying" );
        }
    }
}