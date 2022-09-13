using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( SprayCan ), "ChangeItemSkin" )]
    public class OnEntityReskin
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnEntityReskin" );
        }
    }
}