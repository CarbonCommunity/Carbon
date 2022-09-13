using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Item ), "Drop" )]
    public class OnItemDropped
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnItemDropped" );
        }
    }
}