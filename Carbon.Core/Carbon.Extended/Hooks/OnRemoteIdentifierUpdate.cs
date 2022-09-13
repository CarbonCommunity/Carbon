using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PoweredRemoteControlEntity ), "UpdateIdentifier" )]
    public class OnRemoteIdentifierUpdate
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnRemoteIdentifierUpdate" );
        }
    }
}