using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "UpdateActiveItem" )]
    public class OnActiveItemChange
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnActiveItemChange" );
        }
    }
}