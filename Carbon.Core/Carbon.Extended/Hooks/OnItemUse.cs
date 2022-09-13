using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Item ), "UseItem" )]
    public class OnItemUse
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnItemUse" );
        }
    }
}