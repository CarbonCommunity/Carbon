using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( DropUtil ), "DropItems" )]
    public class OnContainerDropItems
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnContainerDropItems" );
        }
    }
}