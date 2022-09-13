using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( HitchTrough ), "AttemptToHitch" )]
    public class OnHorseHitch
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnHorseHitch" );
        }
    }
}