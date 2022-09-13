using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "OnHealthChanged" )]
    public class OnPlayerHealthChange
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnPlayerHealthChange" );
        }
    }
}