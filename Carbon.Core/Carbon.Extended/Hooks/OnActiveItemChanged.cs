using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "UpdateActiveItem" )]
    public class OnActiveItemChanged
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnActiveItemChanged" );
        }
    }
}