using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ConnectionQueue ), "SendMessage" )]
    public class OnQueueMessage
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnQueueMessage" );
        }
    }
}