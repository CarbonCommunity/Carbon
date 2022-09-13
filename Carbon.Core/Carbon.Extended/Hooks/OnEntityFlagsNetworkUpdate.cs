using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseEntity ), "SendNetworkUpdate_Flags" )]
    public class OnEntityFlagsNetworkUpdate
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnEntityFlagsNetworkUpdate" );
        }
    }
}