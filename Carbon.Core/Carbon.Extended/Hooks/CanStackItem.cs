using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Item ), "CanStack" )]
    public class CanStackItem
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanStackItem" );
        }
    }
}