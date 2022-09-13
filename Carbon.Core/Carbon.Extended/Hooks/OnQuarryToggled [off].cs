using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( EngineSwitch ), "StopEngine" )]
    public class OnQuarryToggled [off]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnQuarryToggled [off]" );
        }
    }
}