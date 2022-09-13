using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( AdventCalendar ), "AwardGift" )]
    public class OnAdventGiftAward
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnAdventGiftAward" );
        }
    }
}