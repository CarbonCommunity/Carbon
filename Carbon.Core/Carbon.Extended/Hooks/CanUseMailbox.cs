using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Mailbox ), "PlayerIsOwner" )]
    public class CanUseMailbox
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanUseMailbox" );
        }
    }
}