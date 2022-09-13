using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( EACServer ), "HandleClientUpdate" )]
    public class OnPlayerKicked
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnPlayerKicked" );
        }
    }
}