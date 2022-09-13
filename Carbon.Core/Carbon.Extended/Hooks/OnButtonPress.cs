using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PressButton ), "Press" )]
    public class OnButtonPress
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnButtonPress" );
        }
    }
}