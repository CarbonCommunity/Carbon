using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "IsHostile" )]
    public class CanEntityBeHostile [BasePlayer]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanEntityBeHostile [BasePlayer]" );
        }
    }
}