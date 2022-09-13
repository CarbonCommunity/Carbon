using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Mailbox ), "SubmitInputItems" )]
    public class OnItemSubmit
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnItemSubmit" );
        }
    }
}