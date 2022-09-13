using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( EACServer ), "HandleClientUpdate" )]
    public class OnPlayerKicked [eac]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnPlayerKicked [eac]" );
        }
    }
}