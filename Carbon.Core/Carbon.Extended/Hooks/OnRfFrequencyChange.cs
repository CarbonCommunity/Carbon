using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PagerEntity ), "ServerSetFrequency" )]
    public class OnRfFrequencyChange
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnRfFrequencyChange" );
        }
    }
}