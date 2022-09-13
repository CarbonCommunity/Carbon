using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( HitchTrough ), "Unhitch" )]
    public class OnHorseUnhitch
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnHorseUnhitch" );
        }
    }
}