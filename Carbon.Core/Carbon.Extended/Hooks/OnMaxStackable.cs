using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Item ), "MaxStackable" )]
    public class OnMaxStackable
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnMaxStackable" );
        }
    }
}