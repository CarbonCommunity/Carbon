using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ItemContainer ), "Remove" )]
    public class OnItemRemovedFromContainer
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnItemRemovedFromContainer" );
        }
    }
}