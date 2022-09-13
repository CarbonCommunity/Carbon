using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "UpdateSpectateTarget" )]
    public class CanSpectateTarget
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanSpectateTarget" );
        }
    }
}