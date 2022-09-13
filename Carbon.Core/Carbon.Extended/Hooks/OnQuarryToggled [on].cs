using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( EngineSwitch ), "StartEngine" )]
    public class OnQuarryToggled [on]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnQuarryToggled [on]" );
        }
    }
}