using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Item ), "MoveToContainer" )]
    public class OnItemStacked
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnItemStacked" );
        }
    }
}