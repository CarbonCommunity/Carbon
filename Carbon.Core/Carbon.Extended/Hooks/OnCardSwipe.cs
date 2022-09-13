using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( CardReader ), "ServerCardSwiped" )]
    public class OnCardSwipe
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnCardSwipe" );
        }
    }
}