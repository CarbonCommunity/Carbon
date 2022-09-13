using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseFishingRod ), "Server_RequestCast" )]
    public class CanCastFishingRod
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "CanCastFishingRod" );
        }
    }
}